namespace Regulae.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using FluentValidation;
    using FluentValidation.Results;
    using Moq;
    using Regulae;
    using Regulae.ConditionNodes;
    using Regulae.Core;
    using Regulae.Evaluation;
    using Regulae.Management;
    using Regulae.Source;
    using Regulae.Tests.TestStubs;
    using Regulae.Validation;
    using Xunit;

    public class RulesEngineTests
    {
        private readonly IConditionsEvalEngine conditionsEvalEngineMock;
        private readonly IRuleConditionsExtractor ruleConditionsExtractorMock;
        private readonly IRuleSanitizer ruleSanitizerMock;
        private readonly IRulesSource rulesSourceMock;
        private readonly IValidatorProvider validatorProviderMock;

        public RulesEngineTests()
        {
            this.ruleSanitizerMock = Mock.Of<IRuleSanitizer>();
            this.rulesSourceMock = Mock.Of<IRulesSource>();
            this.ruleConditionsExtractorMock = Mock.Of<IRuleConditionsExtractor>();
            this.conditionsEvalEngineMock = Mock.Of<IConditionsEvalEngine>();
            this.validatorProviderMock = Mock.Of<IValidatorProvider>();
        }

        [Fact]
        public async Task ActivateRuleAsync_GivenEmptyRuleDataSource_ActivatesRuleSuccessfully()
        {
            // Arrange
            var ruleset = nameof(RulesetNames.Type1);

            var testRule = new Rule("Update test rule", ruleset, new DateTime(2018, 01, 01), new DateTime(2019, 01, 01), new ObjectContentContainer(new object()))
            {
                Priority = 3,
                Active = false,
                RootCondition = new ValueConditionNode(ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA"),
            };

            Mock.Get(rulesSourceMock)
                .Setup(s => s.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()))
                .ReturnsAsync(new List<Rule> { testRule });

            var validatorProviderMock = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            var sut = this.CreateRulesEngine(rulesEngineOptions);

            // Act
            var actual = await sut.ActivateRuleAsync(testRule);

            // Assert
            actual.IsSuccess.Should().BeTrue();
            actual.Errors.Should().BeEmpty();

            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()), Times.Once());
            Mock.Get(conditionsEvalEngineMock).VerifyNoOtherCalls();
        }

        [Fact]
        public async Task AddRuleAsync_GivenEmptyRuleDataSourceAndExistentRuleset_AddsRuleSuccessfully()
        {
            // Arrange
            var ruleset = new Ruleset(RulesetNames.Type1.ToString(), DateTime.UtcNow);

            var testRule = new Rule("Test rule", ruleset.Name, new DateTime(2018, 01, 01), new DateTime(2019, 01, 01), new ObjectContentContainer(new object()))
            {
                Priority = 3,
                RootCondition = new ValueConditionNode(ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA"),
            };

            Mock.Get(rulesSourceMock)
                .Setup(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>()))
                .ReturnsAsync(new Dictionary<string, Ruleset> { { ruleset.Name, ruleset } });
            Mock.Get(rulesSourceMock)
                .Setup(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()))
                .ReturnsAsync(new List<Rule>());

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            Mock.Get(this.ruleSanitizerMock)
                .Setup(x => x.SanitizeAsync(It.IsAny<Rule>()))
                .ReturnsAsync(OperationResult.Success());

            rulesEngineOptions.PriorityCriteria = PriorityCriterias.LargestNumber;
            var sut = this.CreateRulesEngine(rulesEngineOptions);

            // Act
            var actual = await sut.AddRuleAsync(testRule, RuleAddPriorityOption.AtLargestNumber);

            // Assert
            actual.IsSuccess.Should().BeTrue();
            actual.Errors.Should().BeEmpty();

            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()), Times.Once());
            Mock.Get(conditionsEvalEngineMock).VerifyNoOtherCalls();
        }

        [Fact]
        public async Task AddRuleAsync_GivenEmptyRuleDataSourceAndNonExistentRulesetAndAutoCreateRulesetDisabled_DoesNotAddRuleAndReportsError()
        {
            // Arrange
            var ruleset = new Ruleset(RulesetNames.Type1.ToString(), DateTime.UtcNow);

            var testRule = new Rule("Test rule", ruleset.Name, new DateTime(2018, 01, 01), new DateTime(2019, 01, 01), new ObjectContentContainer(new object()))
            {
                Priority = 3,
                RootCondition = new ValueConditionNode(ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA"),
            };

            Mock.Get(rulesSourceMock)
                .Setup(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>()))
                .ReturnsAsync(new Dictionary<string, Ruleset>());

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            rulesEngineOptions.PriorityCriteria = PriorityCriterias.LargestNumber;
            var sut = this.CreateRulesEngine(rulesEngineOptions);

            // Act
            var actual = await sut.AddRuleAsync(testRule, RuleAddPriorityOption.AtLargestNumber);

            // Assert
            actual.IsSuccess.Should().BeFalse();
            actual.Errors.Should().HaveCount(1);

            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()), Times.Never());
            Mock.Get(conditionsEvalEngineMock).VerifyNoOtherCalls();
        }

        [Fact]
        public async Task AddRuleAsync_GivenEmptyRuleDataSourceAndNonExistentRulesetAndAutoCreateRulesetEnabled_CreatesRulesetAndAddsRuleSuccessfully()
        {
            // Arrange
            var ruleset = new Ruleset(RulesetNames.Type1.ToString(), DateTime.UtcNow);

            var testRule = new Rule("Test rule", ruleset.Name, new DateTime(2018, 01, 01), new DateTime(2019, 01, 01), new ObjectContentContainer(new object()))
            {
                Priority = 3,
                RootCondition = new ValueConditionNode(ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA"),
            };

            Mock.Get(this.rulesSourceMock)
                .Setup(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>()))
                .ReturnsAsync(new Dictionary<string, Ruleset>());
            Mock.Get(this.rulesSourceMock)
                .Setup(x => x.CreateRulesetAsync(It.IsAny<CreateRulesetArgs>()))
                .Returns(new ValueTask());
            Mock.Get(this.rulesSourceMock)
                .Setup(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()))
                .ReturnsAsync(new List<Rule>());
            Mock.Get(this.ruleSanitizerMock)
                .Setup(x => x.SanitizeAsync(It.IsAny<Rule>()))
                .ReturnsAsync(OperationResult.Success());

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            rulesEngineOptions.PriorityCriteria = PriorityCriterias.LargestNumber;
            rulesEngineOptions.AutoCreateRulesets = true;
            var sut = this.CreateRulesEngine(rulesEngineOptions);

            // Act
            var actual = await sut.AddRuleAsync(testRule, RuleAddPriorityOption.AtLargestNumber);

            // Assert
            actual.IsSuccess.Should().BeTrue();
            actual.Errors.Should().BeEmpty();

            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>()), Times.Once());
            Mock.Get(rulesSourceMock).Verify(x => x.CreateRulesetAsync(It.IsAny<CreateRulesetArgs>()), Times.Once());
            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()), Times.Once());
            Mock.Get(conditionsEvalEngineMock).VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CreateRulesetAsync_GivenExistentRulesetName_DoesNotAddRulesetToRulesSource()
        {
            // Arrange
            var ruleset = RulesetNames.Type1.ToString();

            Mock.Get(this.rulesSourceMock)
                .Setup(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>()))
                .ReturnsAsync(new Dictionary<string, Ruleset> { { nameof(RulesetNames.Type1), new Ruleset(nameof(RulesetNames.Type1), DateTime.UtcNow) } });

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            rulesEngineOptions.PriorityCriteria = PriorityCriterias.LargestNumber;
            var sut = this.CreateRulesEngine(rulesEngineOptions);

            // Act
            var operationResult = await sut.CreateRulesetAsync(ruleset);

            // Assert
            operationResult.Should().NotBeNull();
            operationResult.IsSuccess.Should().BeFalse();
            operationResult.Errors.Should().NotBeNull()
                .And.HaveCount(1);

            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>()), Times.Once());
            Mock.Get(rulesSourceMock).Verify(x => x.CreateRulesetAsync(It.Is<CreateRulesetArgs>(x => string.Equals(x.Name, ruleset))), Times.Never());
            Mock.Get(conditionsEvalEngineMock).VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CreateRulesetAsync_GivenNonExistentRulesetName_AddsRulesetToRulesSource()
        {
            // Arrange
            var ruleset = RulesetNames.Type1.ToString();

            Mock.Get(this.rulesSourceMock)
                .Setup(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>()))
                .ReturnsAsync(new Dictionary<string, Ruleset>());
            Mock.Get(rulesSourceMock)
                .Setup(x => x.CreateRulesetAsync(It.Is<CreateRulesetArgs>(x => string.Equals(x.Name, ruleset))))
                .Returns(new ValueTask());

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            rulesEngineOptions.PriorityCriteria = PriorityCriterias.LargestNumber;
            var sut = this.CreateRulesEngine(rulesEngineOptions);

            // Act
            var operationResult = await sut.CreateRulesetAsync(ruleset);

            // Assert
            operationResult.Should().NotBeNull();
            operationResult.IsSuccess.Should().BeTrue();
            operationResult.Errors.Should().NotBeNull()
                .And.BeEmpty();

            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>()), Times.Once());
            Mock.Get(rulesSourceMock).Verify(x => x.CreateRulesetAsync(It.Is<CreateRulesetArgs>(x => string.Equals(x.Name, ruleset))), Times.Once());
            Mock.Get(conditionsEvalEngineMock).VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeactivateRuleAsync_GivenEmptyRuleDataSource_DeactivatesRuleSuccessfully()
        {
            // Arrange
            var ruleset = nameof(RulesetNames.Type1);

            var testRule = new Rule("Update test rule", ruleset, new DateTime(2018, 01, 01), new DateTime(2019, 01, 01), new ObjectContentContainer(new object()))
            {
                Priority = 3,
                RootCondition = new ValueConditionNode(ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA"),
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            Mock.Get(rulesSourceMock).Setup(s => s.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()))
                .ReturnsAsync(new List<Rule> { testRule });

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            var sut = this.CreateRulesEngine(rulesEngineOptions);

            // Act
            var actual = await sut.DeactivateRuleAsync(testRule);

            // Assert
            actual.IsSuccess.Should().BeTrue();
            actual.Errors.Should().BeEmpty();

            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()), Times.Once());
            Mock.Get(conditionsEvalEngineMock).Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, Operand>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.Never());
        }

        [Fact]
        public async Task GetRulesetsAsync_NoConditionsGiven_ReturnsRulesetsFromRulesSource()
        {
            // Arrange
            Mock.Get(this.rulesSourceMock)
                .Setup(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>()))
                .ReturnsAsync(new Dictionary<string, Ruleset>
                {
                    { nameof(RulesetNames.Type1), new Ruleset(nameof(RulesetNames.Type1), DateTime.UtcNow) },
                    { nameof(RulesetNames.Type2), new Ruleset(nameof(RulesetNames.Type2), DateTime.UtcNow) },
                });
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            rulesEngineOptions.PriorityCriteria = PriorityCriterias.LargestNumber;
            var sut = this.CreateRulesEngine(rulesEngineOptions);

            // Act
            var rulesets = await sut.GetRulesetsAsync();

            // Assert
            rulesets.Should().NotBeNull()
                .And.HaveCount(2)
                .And.ContainKey(nameof(RulesetNames.Type1))
                .And.ContainKey(nameof(RulesetNames.Type2));

            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>()), Times.Once());
            Mock.Get(conditionsEvalEngineMock).VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetUniqueConditionsAsync_GivenThereAreRulesInDataSource_ReturnsAllRequiredConditions()
        {
            // Arrange

            var dateBegin = new DateTime(2018, 01, 01);
            var dateEnd = new DateTime(2019, 01, 01);

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            var expectedConditions = new List<string> { ConditionNames.IsoCountryCode.ToString() };

            Mock.Get(ruleConditionsExtractorMock)
                .Setup(x => x.GetConditions(It.IsAny<IReadOnlyCollection<Rule>>()))
                .Returns(expectedConditions);

            this.SetupMockForConditionsEvalEngine(
                (rootConditionNode, _, _) => rootConditionNode is ValueConditionNode stringConditionNode && stringConditionNode.RightOperand.ToString() == "USA",
                evaluationOptions);

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            var sut = this.CreateRulesEngine(rulesEngineOptions);

            // Act
            var actual = await sut.GetUniqueConditionsAsync(RulesetNames.Type1.ToString(), dateBegin, dateEnd);

            // Assert
            actual.Should().NotBeNull();
            actual.ToList().Count.Should().Be(1);
            actual.Should().BeEquivalentTo(expectedConditions);
        }

        [Fact]
        public async Task MatchManyAsync_GivenRulesetDateAndConditions_FetchesRulesForDayEvalsAndReturnsAllMatches()
        {
            // Arrange
            var matchDateTime = new DateTime(2018, 07, 01, 18, 19, 30);
            var ruleset = RulesetNames.Type1.ToString();
            var conditions = new Dictionary<string, object>
            {
                { ConditionNames.IsoCountryCode.ToString(), "USA" },
                { ConditionNames.IsoCurrency.ToString(), "USD" },
            };

            var expected1 = new Rule("Expected rule 1", ruleset, new DateTime(2018, 01, 01), new DateTime(2019, 01, 01), new ObjectContentContainer(new object()))
            {
                Priority = 3,
                RootCondition = new ValueConditionNode(ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA"),
            };

            var expected2 = new Rule("Expected rule 2", ruleset, new DateTime(2010, 01, 01), new DateTime(2021, 01, 01), new ObjectContentContainer(new object()))
            {
                Priority = 200,
                RootCondition = new ValueConditionNode(ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA"),
            };

            var notExpected = new Rule("Not expected rule", ruleset, new DateTime(2018, 01, 01), new DateTime(2019, 01, 01), new ObjectContentContainer(new object()))
            {
                Priority = 1, // Topmost rule, should be the one that wins if options are set to topmost wins.
                RootCondition = new ValueConditionNode(ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "CHE"),
            };

            var rules = new[]
            {
                expected1,
                expected2,
                notExpected
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };
            this.SetupMockForRulesDataSource(rules);

            this.SetupMockForConditionsEvalEngine((rootConditionNode, _, _) =>
            {
                return rootConditionNode is ValueConditionNode stringConditionNode && stringConditionNode.RightOperand.Value.ToString() == "USA";
            }, evaluationOptions);

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            var sut = this.CreateRulesEngine(rulesEngineOptions);

            // Act
            var actual = await sut.MatchManyAsync(ruleset, matchDateTime, conditions);

            // Assert
            actual.Should().Contain(expected1)
                .And.Contain(expected2)
                .And.NotContain(notExpected);
            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesAsync(It.IsAny<GetRulesArgs>()), Times.Once());
            Mock.Get(conditionsEvalEngineMock).Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, Operand>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.AtLeastOnce());
        }

        [Fact]
        public async Task MatchOneAsync_GivenRulesetDateAndConditions_FetchesRulesForDayEvalsAndReturnsTheBottommostPriorityOne()
        {
            // Arrange
            var matchDateTime = new DateTime(2018, 07, 01, 18, 19, 30);
            var ruleset = RulesetNames.Type1.ToString();
            var conditions = new Dictionary<string, object>
            {
                { ConditionNames.IsoCountryCode.ToString(), "USA" },
                { ConditionNames.IsoCurrency.ToString(), "USD" },
            };

            var other = new Rule("Expected rule", ruleset, new DateTime(2010, 01, 01), new DateTime(2021, 01, 01), new ObjectContentContainer(new object()))
            {
                Priority = 3,
                RootCondition = new ValueConditionNode(ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA"),
            };

            var expected = new Rule("Expected rule", ruleset, new DateTime(2010, 01, 01), new DateTime(2021, 01, 01), new ObjectContentContainer(new object()))
            {
                Priority = 200,
                RootCondition = new ValueConditionNode(ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA"),
            };

            var rules = new[]
            {
                other,
                expected
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            this.SetupMockForRulesDataSource(rules);

            this.SetupMockForConditionsEvalEngine(true, evaluationOptions);

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            rulesEngineOptions.PriorityCriteria = PriorityCriterias.LargestNumber;
            var sut = this.CreateRulesEngine(rulesEngineOptions);

            // Act
            var actual = await sut.MatchOneAsync(ruleset, matchDateTime, conditions);

            // Assert
            actual.Should().BeSameAs(expected);
            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesAsync(It.IsAny<GetRulesArgs>()), Times.Once());
            Mock.Get(conditionsEvalEngineMock).Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, Operand>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.AtLeastOnce());
        }

        [Fact]
        public async Task MatchOneAsync_GivenRulesetDateAndConditions_FetchesRulesForDayEvalsAndReturnsTheTopmostPriorityOne()
        {
            // Arrange
            var matchDateTime = new DateTime(2018, 07, 01, 18, 19, 30);
            var ruleset = RulesetNames.Type1.ToString();
            var conditions = new Dictionary<string, object>
            {
                { ConditionNames.IsoCountryCode.ToString(), "USA" },
                { ConditionNames.IsoCurrency.ToString(), "USD" },
            };

            var expected = new Rule("Expected rule", ruleset, new DateTime(2018, 01, 01), new DateTime(2019, 01, 01), new ObjectContentContainer(new object()))
            {
                Priority = 3,
                RootCondition = new ValueConditionNode(ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA"),
            };

            var other = new Rule("Expected rule", ruleset, new DateTime(2010, 01, 01), new DateTime(2021, 01, 01), new ObjectContentContainer(new object()))
            {
                Priority = 200,
                RootCondition = new ValueConditionNode(ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA"),
            };

            var rules = new[]
            {
                expected,
                other
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            this.SetupMockForRulesDataSource(rules);

            this.SetupMockForConditionsEvalEngine(true, evaluationOptions);

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            var sut = this.CreateRulesEngine(rulesEngineOptions);

            // Act
            var actual = await sut.MatchOneAsync(ruleset, matchDateTime, conditions);

            // Assert
            actual.Should().BeSameAs(expected);
            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesAsync(It.IsAny<GetRulesArgs>()), Times.Once());
            Mock.Get(conditionsEvalEngineMock).Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, Operand>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.AtLeastOnce());
        }

        [Fact]
        public async Task MatchOneAsync_GivenRulesetDateAndConditions_FetchesRulesForDayFailsEvalsAndReturnsNull()
        {
            // Arrange
            var matchDateTime = new DateTime(2018, 07, 01, 18, 19, 30);
            var ruleset = RulesetNames.Type1.ToString();
            var conditions = new Dictionary<string, object>
            {
                { ConditionNames.IsoCountryCode.ToString(), "USA" },
                { ConditionNames.IsoCurrency.ToString(), "USD" },
            };

            var rules = new[]
            {
                new Rule("Expected rule", ruleset, new DateTime(2018, 01, 01), new DateTime(2019, 01, 01), new ObjectContentContainer(new object()))
                {
                    Priority = 3,
                    RootCondition = new ValueConditionNode(ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA"),
                },
                new Rule("Expected rule", ruleset, new DateTime(2010, 01, 01), new DateTime(2021, 01, 01), new ObjectContentContainer(new object()))
                {
                    Priority = 200,
                    RootCondition = new ValueConditionNode(ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA"),
                }
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            this.SetupMockForRulesDataSource(rules);

            this.SetupMockForConditionsEvalEngine(false, evaluationOptions);

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            var sut = this.CreateRulesEngine(rulesEngineOptions);

            // Act
            var actual = await sut.MatchOneAsync(ruleset, matchDateTime, conditions);

            // Assert
            actual.Should().BeNull();
            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesAsync(It.IsAny<GetRulesArgs>()), Times.Once());
            Mock.Get(conditionsEvalEngineMock).Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, Operand>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.AtLeastOnce());
        }

        [Fact]
        public async Task UpdateRuleAsync_GivenEmptyRuleDataSource_UpdatesRuleSuccesfully()
        {
            // Arrange
            var ruleset = nameof(RulesetNames.Type1);

            var testRule = new Rule("Update test rule", ruleset, new DateTime(2018, 01, 01), new DateTime(2019, 01, 01), new ObjectContentContainer(new object()))
            {
                Priority = 3,
                RootCondition = new ValueConditionNode(ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA"),
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            Mock.Get(rulesSourceMock).Setup(s => s.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()))
                .ReturnsAsync(new List<Rule> { testRule });

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            var sut = this.CreateRulesEngine(rulesEngineOptions);

            testRule.DateEnd = new DateTime(2019, 01, 02);
            testRule.Priority = 1;

            // Act
            var actual = await sut.UpdateRuleAsync(testRule);

            // Assert
            actual.IsSuccess.Should().BeTrue();
            actual.Errors.Should().BeEmpty();

            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()), Times.Once());
            Mock.Get(conditionsEvalEngineMock).Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, Operand>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.Never());
        }

        [Fact]
        public async Task UpdateRuleAsync_GivenRuleWithInvalidDateEnd_UpdatesRuleFailure()
        {
            // Arrange
            var testRule = new Rule("Update test rule", nameof(RulesetNames.Type1), new DateTime(2018, 01, 01), new DateTime(2019, 01, 01), new ObjectContentContainer(new object()))
            {
                Priority = 3,
                RootCondition = new ValueConditionNode(ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA"),
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            Mock.Get(rulesSourceMock).Setup(s => s.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()))
                .ReturnsAsync(new List<Rule> { testRule });

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            var sut = this.CreateRulesEngine(rulesEngineOptions);

            testRule.DateEnd = testRule.DateBegin.AddYears(-2);
            testRule.Priority = 1;

            // Act
            var actual = await sut.UpdateRuleAsync(testRule);

            // Assert
            actual.IsSuccess.Should().BeFalse();
            actual.Errors.Should().NotBeEmpty();

            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()), Times.Once());
            Mock.Get(conditionsEvalEngineMock).Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, Operand>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.Never());
        }

        [Theory]
        [InlineData(nameof(RulesEngine.ActivateRuleAsync), "rule", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine.AddRuleAsync), "rule", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine.AddRuleAsync), "ruleAddPriorityOption", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine.CreateRulesetAsync), "ruleset", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine.DeactivateRuleAsync), "rule", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine.GetUniqueConditionsAsync), "ruleset", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine.MatchManyAsync), "ruleset", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine.MatchOneAsync), "ruleset", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine.SearchAsync), "searchArgs", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine.SearchAsync), "searchArgs", typeof(ArgumentException))]
        [InlineData(nameof(RulesEngine.UpdateRuleAsync), "rule", typeof(ArgumentNullException))]
        public async Task VerifyParameters_GivenNullParameter_ThrowsArgumentNullException(string methodName, string parameterName, Type exceptionType)
        {
            // Arrange
            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            var validator = Mock.Of<IValidator<SearchArgs<string, string>>>();
            Mock.Get(validator)
                .Setup(x => x.ValidateAsync(It.IsAny<SearchArgs<string, string>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Prop1", "Sample error message") }));
            Mock.Get(this.validatorProviderMock)
                .Setup(x => x.GetValidatorFor<SearchArgs<string, string>>())
                .Returns(validator);
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            var sut = this.CreateRulesEngine(rulesEngineOptions);

            // Act
            var actual = await Assert.ThrowsAsync(exceptionType, async () =>
            {
                switch (methodName)
                {
                    case nameof(RulesEngine.ActivateRuleAsync):
                        _ = await sut.ActivateRuleAsync(null);
                        break;

                    case nameof(RulesEngine.AddRuleAsync):
                        switch (parameterName)
                        {
                            case "rule":
                                _ = await sut.AddRuleAsync(null, RuleAddPriorityOption.AtSmallestNumber);
                                break;

                            case "ruleAddPriorityOption":
                                _ = await sut.AddRuleAsync(CreateTestStubRule(), null);
                                break;

                            default:
                                Assert.Fail("Test scenario not supported, please review test implementation to support it.");
                                break;
                        }
                        break;

                    case nameof(RulesEngine.CreateRulesetAsync):
                        await sut.CreateRulesetAsync(null);
                        break;

                    case nameof(RulesEngine.DeactivateRuleAsync):
                        _ = await sut.DeactivateRuleAsync(null);
                        break;

                    case nameof(RulesEngine.GetUniqueConditionsAsync):
                        _ = await sut.GetUniqueConditionsAsync(null, DateTime.MinValue, DateTime.MaxValue);
                        break;

                    case nameof(RulesEngine.MatchManyAsync):
                        _ = await sut.MatchManyAsync(null, DateTime.UtcNow, new Dictionary<string, object>());
                        break;

                    case nameof(RulesEngine.MatchOneAsync):
                        _ = await sut.MatchOneAsync(null, DateTime.UtcNow, new Dictionary<string, object>());
                        break;

                    case nameof(RulesEngine.SearchAsync):
                        switch (exceptionType.Name)
                        {
                            case nameof(ArgumentNullException):
                                _ = await sut.SearchAsync(null);
                                break;

                            case nameof(ArgumentException):
                                _ = await sut.SearchAsync(new SearchArgs<string, string>("test", DateTime.MinValue, DateTime.MaxValue));
                                break;

                            default:
                                Assert.Fail("Test scenario not supported, please review test implementation to support it.");
                                break;
                        }
                        break;

                    case nameof(RulesEngine.UpdateRuleAsync):
                        _ = await sut.UpdateRuleAsync(null);
                        break;

                    default:
                        Assert.Fail("Test scenario not supported, please review test implementation to support it.");
                        break;
                }
            });

            // Assert
            actual.Should().NotBeNull()
                .And.BeOfType(exceptionType);
            if (actual is ArgumentException argumentException)
            {
                argumentException.Message.Should().Contain(parameterName);
                argumentException.ParamName.Should().Be(parameterName);
            }
        }

        private static Rule CreateTestStubRule()
            => Rule.Create("Test stub")
                .InRuleset("Test ruleset")
                .SetContent(new object())
                .Since(DateTime.Parse("2024-08-17", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal))
                .Build()
                .Rule;

        private RulesEngine CreateRulesEngine(RulesEngineOptions rulesEngineOptions)
        {
            var rulesEngineArgs = new RulesEngineArgs
            {
                ConditionsConverter = Mock.Of<IConditionsConverter>(),
                ConditionsEvalEngine = this.conditionsEvalEngineMock,
                RulesSource = this.rulesSourceMock,
                ValidatorProvider = this.validatorProviderMock,
                RulesEngineOptions = rulesEngineOptions,
                RuleConditionsExtractor = this.ruleConditionsExtractorMock,
                RuleSanitizer = this.ruleSanitizerMock,
            };

            var sut = new RulesEngine(rulesEngineArgs);
            return sut;
        }

        private void SetupMockForConditionsEvalEngine(Func<IConditionNode, IDictionary<string, object>, EvaluationOptions, bool> evalFunc, EvaluationOptions evaluationOptions)
        {
            Mock.Get(this.conditionsEvalEngineMock)
                .Setup(x => x.Eval(
                    It.IsAny<IConditionNode>(),
                    It.IsAny<IDictionary<string, Operand>>(),
                    It.Is<EvaluationOptions>(eo => eo == evaluationOptions)))
                .Returns(evalFunc);
        }

        private void SetupMockForConditionsEvalEngine(bool result, EvaluationOptions evaluationOptions)
        {
            Mock.Get(this.conditionsEvalEngineMock)
                .Setup(x => x.Eval(
                    It.IsAny<IConditionNode>(),
                    It.IsAny<IDictionary<string, Operand>>(),
                    It.Is<EvaluationOptions>(eo => eo == evaluationOptions)))
                .Returns(result);
        }

        private void SetupMockForRulesDataSource(IReadOnlyCollection<Rule> rules)
        {
            Mock.Get(this.rulesSourceMock)
                .Setup(x => x.GetRulesAsync(It.IsAny<GetRulesArgs>()))
                .ReturnsAsync(rules);
        }
    }
}