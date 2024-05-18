namespace Regulae.Tests.Validation
{
    using System;
    using FluentAssertions;
    using FluentValidation;
    using Moq;
    using Regulae.Validation;
    using Xunit;

    public class ValidationProviderTests
    {
        [Fact]
        public void GetValidatorFor_GivenMappedType_ReturnsValidator()
        {
            // Arrange
            var expectedValidator = Mock.Of<IValidator<object>>();

            ValidationProvider validationProvider = ValidationProvider.New()
                .MapValidatorFor(expectedValidator);

            // Act
            IValidator actualValidator = validationProvider.GetValidatorFor<object>();

            // Assert
            actualValidator.Should().NotBeNull();
            actualValidator.Should().BeSameAs(expectedValidator);
        }

        [Fact]
        public void GetValidatorFor_GivenUnmappedType_ThrowsNotSupportedException()
        {
            // Arrange
            ValidationProvider validationProvider = ValidationProvider.New();

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => validationProvider.GetValidatorFor<object>());

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain(typeof(object).Name);
        }
    }
}