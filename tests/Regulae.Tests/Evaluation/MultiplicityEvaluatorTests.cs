namespace Regulae.Tests.Evaluation
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Regulae;
    using Regulae.Evaluation;
    using Xunit;

    public class MultiplicityEvaluatorTests
    {
        public static IEnumerable<object[]> SuccessCombinations => new[]
        {
            new object[] { Cardinalities.One, Cardinalities.One, Multiplicities.OneToOne },
            new object[] { Cardinalities.One, Cardinalities.Many, Multiplicities.OneToMany },
            new object[] { Cardinalities.Many, Cardinalities.Many, Multiplicities.ManyToMany },
            new object[] { Cardinalities.Many, Cardinalities.One, Multiplicities.ManyToOne },
        };

        [Theory]
        [MemberData(nameof(SuccessCombinations))]
        public void EvaluateMultiplicity_GivenLeftOperandOperatorAndRightOperand_ReturnsMultiplicity(
            object leftOperandCardinality,
            object rightOperandCardinality,
            object expectedMultiplicity)
        {
            // Arrange
            var multiplicityEvaluator = new MultiplicityEvaluator();

            // Act
            var multiplicity = multiplicityEvaluator.EvaluateMultiplicity((Cardinalities)leftOperandCardinality, (Cardinalities)rightOperandCardinality);

            // Assert
            multiplicity.Should().Be((Multiplicities)expectedMultiplicity);
        }
    }
}