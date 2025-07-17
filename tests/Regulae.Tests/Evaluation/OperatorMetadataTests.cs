namespace Regulae.Tests.Evaluation
{
    using FluentAssertions;
    using Regulae;
    using Regulae.Evaluation;
    using Xunit;

    public class OperatorMetadataTests
    {
        [Fact]
        public void HasSupportForOneMultiplicityAtLeft_WhenHasAtLeastOneToAnyMultiplicity_ReturnsTrue()
        {
            // Arrange
            var operatorMetadata = new OperatorMetadata(Operators.NotStartsWith, Multiplicities.OneToMany);

            // Act
            var result = operatorMetadata.HasSupportForOneMultiplicityAtLeft;

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void HasSupportForOneMultiplicityAtLeft_WhenHasNoneOneToAnyMultiplicity_ReturnsFalse()
        {
            // Arrange
            var operatorMetadata = new OperatorMetadata(Operators.LesserThanOrEqual, Multiplicities.ManyToOne);

            // Act
            var result = operatorMetadata.HasSupportForOneMultiplicityAtLeft;

            // Assert
            result.Should().BeFalse();
        }
    }
}