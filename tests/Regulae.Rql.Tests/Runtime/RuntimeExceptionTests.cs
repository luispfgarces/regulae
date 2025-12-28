namespace Regulae.Rql.Tests.Runtime
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Regulae.Rql.Runtime;
    using Xunit;

    public class RuntimeExceptionTests
    {
        [Fact]
        public void Ctor_WithSingleError_SetsMessageAndErrors()
        {
            // Arrange
            var error = "Single error";

            // Act
            var ex = new RuntimeException(error);

            // Assert
            ex.Should().BeOfType<RuntimeException>();
            ex.Message.Should().Be(error);
            ex.Errors.Should().ContainSingle().Which.Should().Be(error);
        }

        [Fact]
        public void Ctor_WithMultipleErrors_AggregatesMessageAndExposesErrors()
        {
            // Arrange
            var errors = new[] { "Error A", "Error B", "Error C" };
            var expectedMessage = string.Join(Environment.NewLine, errors);

            // Act
            var ex = new RuntimeException((IEnumerable<string>)errors);

            // Assert
            ex.Should().BeOfType<RuntimeException>();
            ex.Message.Should().Be(expectedMessage);
            ex.Errors.Should().BeEquivalentTo(errors, options => options.WithStrictOrdering());
        }
    }
}