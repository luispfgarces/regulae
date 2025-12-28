namespace Regulae.Rql.IntegrationTests.AssistCheck
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Regulae.IntegrationTests.Common.Scenarios;
    using Regulae.IntegrationTests.Common.Scenarios.Scenario8;
    using Regulae.Providers.InMemory;
    using Xunit;
    using Xunit.Abstractions;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    public class AssistCheckTests
    {
        private static readonly string[] checksFiles =
        [
            "Regulae.Rql.IntegrationTests.AssistCheck.CheckFiles.MatchExpressionChecks.yaml",
            "Regulae.Rql.IntegrationTests.AssistCheck.CheckFiles.SearchExpressionChecks.yaml",
        ];

        private readonly IRqlEngine rqlEngine;
        private readonly ITestOutputHelper testOutputHelper;

        public AssistCheckTests(ITestOutputHelper testOutputHelper)
        {
            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetInMemoryDataSource()
                .Build();

            ScenarioLoader.LoadScenarioAsync(rulesEngine, new Scenario8Data()).GetAwaiter().GetResult();

            this.rqlEngine = rulesEngine.GetRqlEngine();
            this.testOutputHelper = testOutputHelper;
        }

        public static IEnumerable<object[]> GetTestCases()
        {
            foreach (var checksFile in checksFiles)
            {
                var checksStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(checksFile);
                using var checksStreamReader = new StreamReader(checksStream!);
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                var checks = deserializer.Deserialize<AssistChecks>(checksStreamReader);

                if (checks.Checks is null || checks.Checks.Length == 0)
                {
                    throw new InvalidOperationException($"No checks found in the file '{checksFile}'.");
                }

                foreach (var checkLine in checks.Checks)
                {
                    yield return new object[] { checkLine.Rql!, checkLine.Line, checkLine.Column, checkLine.ExpectedAssistSuggestions! ?? [], };
                }
            }

            yield break;
        }

        [Theory]
        [MemberData(nameof(GetTestCases))]
        public async Task CheckRqlGrammar(string rqlSource, uint line, uint column, IEnumerable<string> expectedAssistSuggestions)
        {
            // Arrange
            var testOutputMessage = new StringBuilder()
                .Append("RQL: ")
                .AppendLine(rqlSource);

            if (expectedAssistSuggestions.Any())
            {
                testOutputMessage.AppendLine()
                    .AppendLine("Expected assist suggestions:");
                foreach (var assistSuggestion in expectedAssistSuggestions)
                {
                    testOutputMessage.Append("  - ")
                        .AppendLine(assistSuggestion);
                }
            }

            this.testOutputHelper.WriteLine(testOutputMessage.ToString());

            // Act
            var assistSuggestions = await this.rqlEngine.ProvideAssistSuggestionsAsync(
                rqlSource,
                RqlSourcePosition.From(line, column));

            testOutputMessage.Clear()
                .Append("Outcome: ");

            if (assistSuggestions.Any())
            {
                testOutputMessage.AppendLine();
                foreach (var assistSuggestion in assistSuggestions)
                {
                    testOutputMessage.Append("  - ")
                        .AppendLine(assistSuggestion.Lexeme);
                }
            }
            else
            {
                testOutputMessage.AppendLine("No assist suggestions returned.");
            }

            this.testOutputHelper.WriteLine(testOutputMessage.ToString());

            // Assert
            if (expectedAssistSuggestions.Any())
            {
                assistSuggestions.Select(a => a.Lexeme).Should().Contain(expectedAssistSuggestions);
            }
            else
            {
                assistSuggestions.Select(a => a.Lexeme).Should().BeEmpty();
            }
        }
    }
}
