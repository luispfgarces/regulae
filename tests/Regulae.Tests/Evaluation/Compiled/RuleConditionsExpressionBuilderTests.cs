namespace Regulae.Tests.Evaluation.Compiled
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using DiffPlex.DiffBuilder;
    using ExpressionDebugger;
    using FluentAssertions;
    using Moq;
    using Regulae;
    using Regulae.ConditionNodes;
    using Regulae.Evaluation;
    using Regulae.Evaluation.Compiled;
    using Regulae.Evaluation.Compiled.ExpressionBuilders;
    using Regulae.Tests.Evaluation;
    using Regulae.Tests.TestStubs;
    using Xunit;

    public partial class RuleConditionsExpressionBuilderTests
    {
        public static IEnumerable<object[]> AndComposedConditionNodeScenarios => new[]
        {
            new object[]
            {
                "Scenario 1 - MissingConditionsBehavior = 'Discard', MatchMode = 'Exact', and only contains condition for 'NumberOfSales'",
                new Dictionary<string, Operand>
                {
                    { ConditionNames.NumberOfSales.ToString(), 500 },
                },
                MatchModes.Exact,
                MissingConditionBehaviors.Discard,
                false
            },
            new object[]
            {
                "Scenario 2 - MissingConditionsBehavior = 'Discard', MatchMode = 'Exact', and both needed conditions",
                new Dictionary<string, Operand>
                {
                    { ConditionNames.NumberOfSales.ToString(), 500 },
                    { ConditionNames.IsoCountryCode.ToString(), "PT" },
                },
                MatchModes.Exact,
                MissingConditionBehaviors.Discard,
                true
            },
            new object[]
            {
                "Scenario 3 - MissingConditionsBehavior = 'UseDataTypeDefault', MatchMode = 'Exact', and both needed conditions",
                new Dictionary<string, Operand>
                {
                    { ConditionNames.NumberOfSales.ToString(), 500 },
                    { ConditionNames.IsoCountryCode.ToString(), "PT" },
                },
                MatchModes.Exact,
                MissingConditionBehaviors.UseDataTypeDefault,
                true
            },
            new object[]
            {
                "Scenario 4 - MissingConditionsBehavior = 'UseDataTypeDefault', MatchMode = 'Search', and both needed conditions",
                new Dictionary<string, Operand>
                {
                    { ConditionNames.NumberOfSales.ToString(), 500 },
                    { ConditionNames.IsoCountryCode.ToString(), "PT" },
                },
                MatchModes.Search,
                MissingConditionBehaviors.UseDataTypeDefault,
                true
            },
            new object[]
            {
                "Scenario 5 - MissingConditionsBehavior = 'UseDataTypeDefault', MatchMode = 'Search', and only contains condition for 'NumberOfSales'",
                new Dictionary<string, Operand>
                {
                    { ConditionNames.NumberOfSales.ToString(), 500 },
                },
                MatchModes.Search,
                MissingConditionBehaviors.UseDataTypeDefault,
                true
            }
        };

        public static IEnumerable<object[]> OrComposedConditionNodeScenarios => new[]
        {
            new object[]
            {
                "Scenario 1 - MissingConditionsBehavior = 'Discard', MatchMode = 'Exact', and only contains condition for 'NumberOfSales'",
                new Dictionary<string, Operand>
                {
                    { ConditionNames.NumberOfSales.ToString(), 500 },
                },
                MatchModes.Exact,
                MissingConditionBehaviors.Discard,
                true
            },
            new object[]
            {
                "Scenario 2 - MissingConditionsBehavior = 'Discard', MatchMode = 'Exact', and both needed conditions",
                new Dictionary<string, Operand>
                {
                    { ConditionNames.NumberOfSales.ToString(), 500 },
                    { ConditionNames.IsoCountryCode.ToString(), "PT" },
                },
                MatchModes.Exact,
                MissingConditionBehaviors.Discard,
                true
            },
            new object[]
            {
                "Scenario 3 - MissingConditionsBehavior = 'UseDataTypeDefault', MatchMode = 'Exact', and both needed conditions",
                new Dictionary<string, Operand>
                {
                    { ConditionNames.NumberOfSales.ToString(), 500 },
                    { ConditionNames.IsoCountryCode.ToString(), "PT" },
                },
                MatchModes.Exact,
                MissingConditionBehaviors.UseDataTypeDefault,
                true
            },
            new object[]
            {
                "Scenario 4 - MissingConditionsBehavior = 'UseDataTypeDefault', MatchMode = 'Search', and both needed conditions",
                new Dictionary<string, Operand>
                {
                    { ConditionNames.NumberOfSales.ToString(), 500 },
                    { ConditionNames.IsoCountryCode.ToString(), "PT" },
                },
                MatchModes.Search,
                MissingConditionBehaviors.UseDataTypeDefault,
                true
            },
            new object[]
            {
                "Scenario 5 - MissingConditionsBehavior = 'UseDataTypeDefault', MatchMode = 'Search', and only contains condition for 'NumberOfSales'",
                new Dictionary<string, Operand>
                {
                    { ConditionNames.NumberOfSales.ToString(), 500 },
                },
                MatchModes.Search,
                MissingConditionBehaviors.UseDataTypeDefault,
                true
            }
        };

        [Theory]
        [MemberData(nameof(AndComposedConditionNodeScenarios))]
        public void BuildExpression_GivenAndComposedConditionNodeWith2ChildValueConditionNodes_BuildsLambdaExpression(
            string scenarioName,
            object evaluationContext,
            string matchModeName,
            string missingConditionBehaviorName,
            bool expectedResult)
        {
            // Arrange
            string expectedScript;
            var matchMode = Enum.Parse<MatchModes>(matchModeName);
            var missingConditionBehavior = Enum.Parse<MissingConditionBehaviors>(missingConditionBehaviorName);
            var resourceName = matchMode switch
            {
                MatchModes.Exact when missingConditionBehavior == MissingConditionBehaviors.UseDataTypeDefault =>
                    "Regulae.Tests.Evaluation.Compiled.RuleConditionsExpressionBuilderTests.GoldenFile.And.ExactUseDataTypeDefault.csx",
                MatchModes.Exact when missingConditionBehavior == MissingConditionBehaviors.Discard =>
                    "Regulae.Tests.Evaluation.Compiled.RuleConditionsExpressionBuilderTests.GoldenFile.And.ExactDiscard.csx",
                MatchModes.Search =>
                    "Regulae.Tests.Evaluation.Compiled.RuleConditionsExpressionBuilderTests.GoldenFile.And.Search.csx",
                _ => ""
            };
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (var streamReader = new StreamReader(stream))
            {
                expectedScript = streamReader.ReadToEnd();
            }
            var valueConditionNode1
                = new ValueConditionNode(ConditionNames.NumberOfSales.ToString(), Operators.Equal, 100);
            var valueConditionNode2
                = new ValueConditionNode(ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "GB");

            var composedConditionNode
                = new ComposedConditionNode(LogicalOperators.And, new[] { valueConditionNode1, valueConditionNode2 });

            var valueConditionNodeExpressionBuilder = Mock.Of<IValueConditionNodeExpressionBuilder>();
            Mock.Get(valueConditionNodeExpressionBuilder)
                .Setup(x => x.Build(It.IsAny<IExpressionBlockBuilder>(), It.IsAny<BuildValueConditionNodeExpressionArgs>()))
                .Callback<IExpressionBlockBuilder, BuildValueConditionNodeExpressionArgs>(
                (builder, args) =>
                {
                    builder.Assign(args.ResultVariableExpression, builder.Constant(true));
                    builder.AddExpression(builder.Empty());
                });

            var valueConditionNodeExpressionBuilderProvider = Mock.Of<IValueConditionNodeExpressionBuilderProvider>();
            Mock.Get(valueConditionNodeExpressionBuilderProvider)
                .Setup(x => x.GetExpressionBuilder(It.Is(Multiplicities.OneToOne, EqualityComparer<Multiplicities>.Default)))
                .Returns(valueConditionNodeExpressionBuilder);

            var dataTypeConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypeConfigurationProvider)
                .Setup(x => x.GetDataTypeConfiguration(DataTypes.String))
                .Returns(DataTypeConfiguration.Create(DataTypes.String, typeof(string), null));
            Mock.Get(dataTypeConfigurationProvider)
                .Setup(x => x.GetDataTypeConfiguration(DataTypes.Integer))
                .Returns(DataTypeConfiguration.Create(DataTypes.Integer, typeof(int), 0));

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            rulesEngineOptions.MissingConditionBehavior = missingConditionBehavior;

            var conditionsTreeCompiler = new RuleConditionsExpressionBuilder(
                valueConditionNodeExpressionBuilderProvider,
                dataTypeConfigurationProvider,
                rulesEngineOptions);

            // Act
            var expression = conditionsTreeCompiler.BuildExpression(composedConditionNode, matchMode);

            // Assert
            expression.Should().NotBeNull();
            var actualScript = expression.ToScript();
            var diffResult = SideBySideDiffBuilder.Diff(expectedScript, actualScript, ignoreWhiteSpace: true);
            diffResult.NewText.HasDifferences.Should().BeFalse();

            Func<IDictionary<string, Operand>, bool> compiledLambdaExpression = null;
            FluentActions.Invoking(() => compiledLambdaExpression = expression.Compile())
                .Should()
                .NotThrow("expression should be compilable");

            foreach (var scenario in AndComposedConditionNodeScenarios)
            {
                bool? result = null;
                FluentActions.Invoking(() => result = compiledLambdaExpression.Invoke((IDictionary<string, Operand>)evaluationContext))
                    .Should()
                    .NotThrow($"compiled expression should be executable under scenario: {scenarioName}");

                result.Should().Be(expectedResult);
            }
        }

        [Theory]
        [MemberData(nameof(OrComposedConditionNodeScenarios))]
        public void BuildExpression_GivenOrComposedConditionNodeWith2ChildValueConditionNodes_BuildsLambdaExpression(
            string scenarioName,
            object evaluationContext,
            string matchModeName,
            string missingConditionBehaviorName,
            bool expectedResult)
        {
            // Arrange
            string expectedScript;
            var matchMode = Enum.Parse<MatchModes>(matchModeName);
            var missingConditionBehavior = Enum.Parse<MissingConditionBehaviors>(missingConditionBehaviorName);
            var resourceName = matchMode switch
            {
                MatchModes.Exact when missingConditionBehavior == MissingConditionBehaviors.UseDataTypeDefault =>
                    "Regulae.Tests.Evaluation.Compiled.RuleConditionsExpressionBuilderTests.GoldenFile.Or.ExactUseDataTypeDefault.csx",
                MatchModes.Exact when missingConditionBehavior == MissingConditionBehaviors.Discard =>
                    "Regulae.Tests.Evaluation.Compiled.RuleConditionsExpressionBuilderTests.GoldenFile.Or.ExactDiscard.csx",
                MatchModes.Search =>
                    "Regulae.Tests.Evaluation.Compiled.RuleConditionsExpressionBuilderTests.GoldenFile.Or.Search.csx",
                _ => ""
            };
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (var streamReader = new StreamReader(stream))
            {
                expectedScript = streamReader.ReadToEnd();
            }
            var valueConditionNode1
                = new ValueConditionNode(ConditionNames.NumberOfSales.ToString(), Operators.Equal, 100);
            var valueConditionNode2
                = new ValueConditionNode(ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "GB");

            var composedConditionNode
                = new ComposedConditionNode(LogicalOperators.Or, new[] { valueConditionNode1, valueConditionNode2 });

            var valueConditionNodeExpressionBuilder = Mock.Of<IValueConditionNodeExpressionBuilder>();
            Mock.Get(valueConditionNodeExpressionBuilder)
                .Setup(x => x.Build(It.IsAny<IExpressionBlockBuilder>(), It.IsAny<BuildValueConditionNodeExpressionArgs>()))
                .Callback<IExpressionBlockBuilder, BuildValueConditionNodeExpressionArgs>(
                (builder, args) =>
                {
                    builder.Assign(args.ResultVariableExpression, builder.Constant(true));
                    builder.AddExpression(builder.Empty());
                });

            var valueConditionNodeExpressionBuilderProvider = Mock.Of<IValueConditionNodeExpressionBuilderProvider>();
            Mock.Get(valueConditionNodeExpressionBuilderProvider)
                .Setup(x => x.GetExpressionBuilder(It.Is(Multiplicities.OneToOne, EqualityComparer<Multiplicities>.Default)))
                .Returns(valueConditionNodeExpressionBuilder);

            var dataTypeConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypeConfigurationProvider)
                .Setup(x => x.GetDataTypeConfiguration(DataTypes.String))
                .Returns(DataTypeConfiguration.Create(DataTypes.String, typeof(string), null));
            Mock.Get(dataTypeConfigurationProvider)
                .Setup(x => x.GetDataTypeConfiguration(DataTypes.Integer))
                .Returns(DataTypeConfiguration.Create(DataTypes.Integer, typeof(int), 0));

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            rulesEngineOptions.MissingConditionBehavior = missingConditionBehavior;

            var conditionsTreeCompiler = new RuleConditionsExpressionBuilder(
                valueConditionNodeExpressionBuilderProvider,
                dataTypeConfigurationProvider,
                rulesEngineOptions);

            // Act
            var expression = conditionsTreeCompiler.BuildExpression(composedConditionNode, matchMode);

            // Assert
            expression.Should().NotBeNull();
            var actualScript = expression.ToScript();
            var diffResult = SideBySideDiffBuilder.Diff(expectedScript, actualScript, ignoreWhiteSpace: true);
            diffResult.NewText.HasDifferences.Should().BeFalse();

            Func<IDictionary<string, Operand>, bool> compiledLambdaExpression = null;
            FluentActions.Invoking(() => compiledLambdaExpression = expression.Compile())
                .Should()
                .NotThrow("expression should be compilable");

            foreach (var scenario in OrComposedConditionNodeScenarios)
            {
                bool? result = null;
                FluentActions.Invoking(() => result = compiledLambdaExpression.Invoke((IDictionary<string, Operand>)evaluationContext))
                    .Should()
                    .NotThrow($"compiled expression should be executable under scenario: {scenarioName}");

                result.Should().Be(expectedResult);
            }
        }

        [Fact]
        public void BuildExpression_GivenUnknownConditionNode_ThrowsNotSupportedException()
        {
            // Arrange
            var stubConditionNode = new StubConditionNode();

            var valueConditionNodeExpressionBuilderProvider = Mock.Of<IValueConditionNodeExpressionBuilderProvider>();
            var dataTypeConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var ruleConditionsExpressionBuilder = new RuleConditionsExpressionBuilder(
                valueConditionNodeExpressionBuilderProvider,
                dataTypeConfigurationProvider,
                rulesEngineOptions);

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => ruleConditionsExpressionBuilder.BuildExpression(stubConditionNode, MatchModes.Exact));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain(nameof(StubConditionNode));
        }

        [Fact]
        public void BuildExpression_GivenUnsupportedLogicalOperatorForComposedConditionNode_ThrowsNotSupportedException()
        {
            // Arrange
            var composedConditionNode = new ComposedConditionNode(LogicalOperators.Eval, Enumerable.Empty<IConditionNode>());

            var valueConditionNodeExpressionBuilderProvider = Mock.Of<IValueConditionNodeExpressionBuilderProvider>();
            var dataTypeConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var ruleConditionsExpressionBuilder = new RuleConditionsExpressionBuilder(
                valueConditionNodeExpressionBuilderProvider,
                dataTypeConfigurationProvider,
                rulesEngineOptions);

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => ruleConditionsExpressionBuilder.BuildExpression(composedConditionNode, MatchModes.Exact));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain(nameof(LogicalOperators.Eval));
        }

        [Fact]
        public void BuildExpression_GivenXorComposedConditionNode_EvaluatesExclusiveOr()
        {
            // Arrange
            var valueConditionNode1 = new ValueConditionNode(ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "A");
            var valueConditionNode2 = new ValueConditionNode(ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "B");
            var composed = new ComposedConditionNode(LogicalOperators.Xor, new[] { valueConditionNode1, valueConditionNode2 });

            // Create a provider that returns a builder which assigns sequential values for each child
            var seq = new Queue<bool>(new[] { true, false });
            var builderMock = new Mock<IValueConditionNodeExpressionBuilder>();
            builderMock.Setup(b => b.Build(It.IsAny<IExpressionBlockBuilder>(), It.IsAny<BuildValueConditionNodeExpressionArgs>()))
                .Callback<IExpressionBlockBuilder, BuildValueConditionNodeExpressionArgs>((blk, args) =>
                {
                    var val = seq.Dequeue();
                    blk.Assign(args.ResultVariableExpression, blk.Constant(val));
                    blk.AddExpression(blk.Empty());
                });

            var providerMock = new Mock<IValueConditionNodeExpressionBuilderProvider>();
            providerMock.Setup(p => p.GetExpressionBuilder(It.IsAny<Multiplicities>())).Returns(builderMock.Object);

            var dataTypeProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypeProvider).Setup(d => d.GetDataTypeConfiguration(DataTypes.String))
                .Returns(DataTypeConfiguration.Create(DataTypes.String, typeof(string), string.Empty));

            var options = RulesEngineOptions.NewWithDefaults();

            var sut = new RuleConditionsExpressionBuilder(providerMock.Object, dataTypeProvider, options);

            // Act
            var expr = sut.BuildExpression(composed, MatchModes.Exact);

            // Assert
            expr.Should().NotBeNull();
            var fn = expr.Compile();

            // Provide dummy operands dictionary (builders ignore operands since they set constants)
            var context = new Dictionary<string, Operand>
            {
                { ConditionNames.IsoCountryCode.ToString(), "X" }
            };

            // Seq returns true (first child true, second false) -> XOR true
            fn(context).Should().BeTrue();
        }

        [Fact]
        public void BuildExpression_ManyMultiplicities_SelectsCorrectMultiplicityBranch()
        {
            // Arrange: operator Contains supports OneToOne and ManyToOne
            var conditionName = ConditionNames.NumberOfSales.ToString();
            var valueConditionNode = new ValueConditionNode(conditionName, Operators.Contains, 5);

            var providerMock = new Mock<IValueConditionNodeExpressionBuilderProvider>();
            var builderOne = new Mock<IValueConditionNodeExpressionBuilder>();
            var builderMany = new Mock<IValueConditionNodeExpressionBuilder>();

            // builderOne sets result to false
            builderOne.Setup(b => b.Build(It.IsAny<IExpressionBlockBuilder>(), It.IsAny<BuildValueConditionNodeExpressionArgs>()))
                .Callback<IExpressionBlockBuilder, BuildValueConditionNodeExpressionArgs>((blk, args) =>
                {
                    blk.Assign(args.ResultVariableExpression, blk.Constant(false));
                    blk.AddExpression(blk.Empty());
                });

            // builderMany sets result to true
            builderMany.Setup(b => b.Build(It.IsAny<IExpressionBlockBuilder>(), It.IsAny<BuildValueConditionNodeExpressionArgs>()))
                .Callback<IExpressionBlockBuilder, BuildValueConditionNodeExpressionArgs>((blk, args) =>
                {
                    blk.Assign(args.ResultVariableExpression, blk.Constant(true));
                    blk.AddExpression(blk.Empty());
                });

            providerMock.Setup(p => p.GetExpressionBuilder(Multiplicities.OneToOne)).Returns(builderOne.Object);
            providerMock.Setup(p => p.GetExpressionBuilder(Multiplicities.ManyToOne)).Returns(builderMany.Object);

            var dataTypeProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypeProvider).Setup(d => d.GetDataTypeConfiguration(DataTypes.Integer))
                .Returns(DataTypeConfiguration.Create(DataTypes.Integer, typeof(int), 0));

            var options = RulesEngineOptions.NewWithDefaults();
            var sut = new RuleConditionsExpressionBuilder(providerMock.Object, dataTypeProvider, options);

            // Act
            var expr = sut.BuildExpression(valueConditionNode, MatchModes.Exact);
            var fn = expr.Compile();

            // Assert:
            // Case 1: left operand single (One), right operand single -> OneToOne -> builderOne returns false
            var ctxOne = new Dictionary<string, Operand>
            {
                { conditionName, 5 }
            };
            fn(ctxOne).Should().BeFalse();

            // Case 2: left operand many (array), right operand single -> ManyToOne -> builderMany returns true
            var ctxMany = new Dictionary<string, Operand>
            {
                { conditionName, new[] { 1, 2, 3 } }
            };
            fn(ctxMany).Should().BeTrue();
        }
    }
}