namespace Regulae.Rql.Tests.Messages
{
    using System;
    using FluentAssertions;
    using Regulae.Rql;
    using Regulae.Rql.Messages;
    using Xunit;

    public class MessageContainerTests
    {
        [Fact]
        public void Ctor_InitializesEmptyState()
        {
            // Act
            var sut = new MessageContainer();

            // Assert
            sut.ErrorsCount.Should().Be(0);
            sut.WarningsCount.Should().Be(0);
            sut.Messages.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public void Error_AddsErrorMessageAndIncrementsErrorsCount()
        {
            // Arrange
            var sut = new MessageContainer();
            var begin = RqlSourcePosition.From(1, 2);
            var end = RqlSourcePosition.From(1, 5);

            // Act
            sut.Error("Some error", begin, end);

            // Assert
            sut.ErrorsCount.Should().Be(1);
            sut.WarningsCount.Should().Be(0);
            sut.Messages.Should().HaveCount(1);
            var message = sut.Messages[0];
            message.Text.Should().Be("Some error");
            message.Severity.Should().Be(MessageSeverity.Error);
            message.BeginPosition.Should().Be(begin);
            message.EndPosition.Should().Be(end);
        }

        [Fact]
        public void Warning_AddsWarningMessageAndIncrementsWarningsCount()
        {
            // Arrange
            var sut = new MessageContainer();
            var begin = RqlSourcePosition.From(2, 1);
            var end = RqlSourcePosition.From(2, 3);

            // Act
            sut.Warning("Be careful", begin, end);

            // Assert
            sut.WarningsCount.Should().Be(1);
            sut.ErrorsCount.Should().Be(0);
            sut.Messages.Should().HaveCount(1);
            var message = sut.Messages[0];
            message.Text.Should().Be("Be careful");
            message.Severity.Should().Be(MessageSeverity.Warning);
            message.BeginPosition.Should().Be(begin);
            message.EndPosition.Should().Be(end);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Error_WithNullOrWhitespaceMessage_ThrowsArgumentNullException(string? invalidMessage)
        {
            // Arrange
            var sut = new MessageContainer();
            var begin = RqlSourcePosition.From(1, 1);
            var end = RqlSourcePosition.From(1, 1);

            // Act
            Action act = () => sut.Error(invalidMessage, begin, end);

            // Assert
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("message");
        }

        [Fact]
        public void Dispose_WhenCalled_NullsInternalMessages_AndAccessingMessagesThrows()
        {
            // Arrange
            var sut = new MessageContainer();
            sut.Error("initial", RqlSourcePosition.From(1, 1), RqlSourcePosition.From(1, 2));

            // Act
            Action dispose = () => sut.Dispose();

            // Assert dispose does not throw
            dispose.Should().NotThrow();

            // Accessing Messages after dispose should throw because internal list is nulled
            Action access = () => { var _ = sut.Messages; };

            access.Should().Throw<NullReferenceException>();

            // Calling Dispose again should be safe (idempotent)
            Action disposeAgain = () => sut.Dispose();
            disposeAgain.Should().NotThrow();
        }
    }
}