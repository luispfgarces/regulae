namespace Regulae.Rql.Tests.Pipeline.Interpret
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae.Rql;
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Pipeline.Interpret;
    using Regulae.Rql.Runtime;
    using Regulae.Rql.Tokens;
    using Xunit;

    public partial class InterpreterTests
    {
        public static IEnumerable<object?[]> ValidCasesLiteralExpression => new[]
        {
            new object?[] { LiteralType.Bool, null, NewRqlNothing() },
            new object?[] { LiteralType.Bool, true, NewRqlBool(true) },
            new object?[] { LiteralType.Decimal, null, NewRqlNothing() },
            new object?[] { LiteralType.Decimal, 10.5m, NewRqlDecimal(10.5m) },
            new object?[] { LiteralType.Integer, null, NewRqlNothing() },
            new object?[] { LiteralType.Integer, 1, NewRqlInteger(1) },
            new object?[] { LiteralType.String, null, NewRqlNothing() },
            new object?[] { LiteralType.String, "test", NewRqlString("test") },
            new object?[] { LiteralType.DateTime, null, NewRqlNothing() },
            new object?[] { LiteralType.DateTime, new DateTime(2024, 1, 1), NewRqlDate(new DateTime(2024, 1, 1)) },
            new object?[] { LiteralType.Undefined, null, NewRqlNothing() },
            new object?[] { (LiteralType)(-1), null, NewRqlNothing() },
        };

        [Fact]
        public async Task VisitLiteralExpression_GivenLiteralExpressionWithUnsupportedLiteralType_ThrowsNotSupportedException()
        {
            // Arrange
            var literalToken = NewToken("dummy", "dummy", TokenType.IDENTIFIER);
            var literalExpression = LiteralExpression.Create((LiteralType)(-1), literalToken, "test");

            var runtime = Mock.Of<IRuntime>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await Assert.ThrowsAsync<NotSupportedException>(async () => await interpreter.VisitLiteralExpression(literalExpression));

            // Assert
            actual.Message.Should().Be("Literal with type '-1' is not supported.");
        }

        [Theory]
        [MemberData(nameof(ValidCasesLiteralExpression))]
        public async Task VisitLiteralExpression_GivenValidLiteralExpression_ReturnsRuntimeValue(object literalType, object? runtimeValue, object expected)
        {
            // Arrange
            var literalToken = NewToken("dummy", expected, TokenType.IDENTIFIER);
            var literalExpression = LiteralExpression.Create((LiteralType)literalType, literalToken, runtimeValue);

            var runtime = Mock.Of<IRuntime>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitLiteralExpression(literalExpression);

            // Assert
            actual.Should().NotBeNull().And.BeEquivalentTo(expected);
            actual.RuntimeValue.Should().Be(runtimeValue);
        }
    }
}