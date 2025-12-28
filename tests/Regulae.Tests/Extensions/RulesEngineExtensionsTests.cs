namespace Regulae.Tests.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae;
    using Regulae.Extensions;
    using Regulae.Generic;
    using Regulae.Source;
    using Regulae.Tests.TestStubs;
    using Xunit;

    public class RulesEngineExtensionsTests
    {
        [Fact]
        public void MakeGeneric_NoOptions_ReturnsGenericRulesEngine()
        {
            // Arrange
            var rulesEngineArgs = new RulesEngineArgs();
            var rulesEngine = new RulesEngine(rulesEngineArgs);

            // Act
            var genericEngine = rulesEngine.MakeGeneric<RulesetNames, ConditionNames>();

            // Assert
            genericEngine.Should().NotBeNull();
            genericEngine.GetType().Should().Be(typeof(RulesEngine<RulesetNames, ConditionNames>));
        }

        [Fact]
        public void MakeGeneric_GivenAutoCreateConditionsOption_ReturnsGenericRulesEngineWithConditionsCreated()
        {
            // Arrange
            var options = RulesEngineOptions.NewWithDefaults();
            var rulesSource = Mock.Of<IRulesSource>();
            var createdConditions = new List<(string, DataTypes)>();
            Mock.Get(rulesSource)
                .Setup(rs => rs.CreateConditionAsync(It.IsAny<CreateConditionArgs>()))
                .Returns<CreateConditionArgs>(args =>
                {
                    createdConditions.Add((args.Name, args.DataType));
                    return ValueTask.CompletedTask;
                });
            var rulesEngineArgs = new RulesEngineArgs
            {
                RulesEngineOptions = options,
                RulesSource = rulesSource,
            };
            var rulesEngine = new RulesEngine(rulesEngineArgs);

            // Act
            var genericEngine = rulesEngine.MakeGeneric<RulesetNames, AnnotatedConditionNames>(opt => opt.AutoCreateConditions = true);

            // Assert
            genericEngine.Should().NotBeNull();
            genericEngine.GetType().Should().Be(typeof(RulesEngine<RulesetNames, AnnotatedConditionNames>));
            createdConditions.Should().Contain(
            [
                (nameof(AnnotatedConditionNames.Condition1), DataTypes.String),
                (nameof(AnnotatedConditionNames.Condition2), DataTypes.Integer),
            ]);
        }

        [Fact]
        public void MakeGeneric_GivenAutoCreateConditionsOptionWithConditionTypeNotAnnotatedWithDataType_ThrowsInvalidOperationException()
        {
            // Arrange
            var options = RulesEngineOptions.NewWithDefaults();
            var rulesSource = Mock.Of<IRulesSource>();
            var rulesEngineArgs = new RulesEngineArgs
            {
                RulesEngineOptions = options,
                RulesSource = rulesSource,
            };
            var rulesEngine = new RulesEngine(rulesEngineArgs);

            // Act
            var act = () => rulesEngine.MakeGeneric<RulesetNames, ConditionNames>(opt => opt.AutoCreateConditions = true);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("The condition value 'IsoCountryCode' does not declare attribute [DataType] which is required to auto create conditions. " +
                    "Please declare the attribute with the desired data type.");
        }

        private enum AnnotatedConditionNames
        {
            [DataType(DataTypes.String)]
            Condition1,

            [DataType(DataTypes.Integer)]
            Condition2,
        }
    }
}