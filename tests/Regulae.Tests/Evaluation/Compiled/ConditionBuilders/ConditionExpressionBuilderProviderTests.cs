namespace Regulae.Tests.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Regulae;
    using Regulae.Evaluation;
    using Regulae.Evaluation.Compiled.ConditionBuilders;
    using Xunit;

    public class ConditionExpressionBuilderProviderTests
    {
        public static IEnumerable<object[]> Scenarios => OperatorsMetadata.All
            .SelectMany(c => c.SupportedMultiplicities.Select(m => new object[] { c.Operator, m }))
            .ToList();

        [Fact]
        public void GetConditionExpressionBuilderFor_GivenOperatorAndNotSupportedMultiplicity_ThrowsNotSupportedException()
        {
            // Arrange
            var @operator = Operators.In;
            var multiplicity = Multiplicities.OneToOne;

            var conditionExpressionBuilderProvider = new ConditionExpressionBuilderProvider();

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => conditionExpressionBuilderProvider.GetConditionExpressionBuilderFor(@operator, multiplicity));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain(@operator.ToString()).And.Contain(multiplicity.ToString());
        }

        [Theory]
        [MemberData(nameof(Scenarios))]
        public void GetConditionExpressionBuilderFor_GivenOperatorAndSupportedMultiplicity_ReturnsConditionExpressionBuilder(Operators @operator, object multiplicity)
        {
            // Arrange
            var conditionExpressionBuilderProvider = new ConditionExpressionBuilderProvider();

            // Act
            var conditionExpressionBuilder = conditionExpressionBuilderProvider.GetConditionExpressionBuilderFor(@operator, (Multiplicities)multiplicity);

            // Assert
            conditionExpressionBuilder.Should().NotBeNull();
        }
    }
}