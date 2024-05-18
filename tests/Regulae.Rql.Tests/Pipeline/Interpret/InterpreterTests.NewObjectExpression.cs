namespace Regulae.Rql.Tests.Pipeline.Interpret
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae.Rql;
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Pipeline.Interpret;
    using Regulae.Rql.Runtime;
    using Regulae.Rql.Runtime.Types;
    using Xunit;

    public partial class InterpreterTests
    {
        [Fact]
        public async Task VisitNewObjectExpression_GivenValidNewObjectExpressionWithPropertiesInitializer_ReturnsObjectWithPropertiesFilled()
        {
            // Arrange
            var objectToken = NewToken("object", null, Regulae.Rql.Tokens.TokenType.OBJECT);
            var assignementToken = NewToken("=", null, Regulae.Rql.Tokens.TokenType.ASSIGN);
            var values = new[]
            {
                new AssignmentExpression(
                    CreateMockedExpression(NewRqlString("Name")),
                    assignementToken,
                    CreateMockedExpression(NewRqlString("Roger"))),
                new AssignmentExpression(
                    CreateMockedExpression(NewRqlString("Age")),
                    assignementToken,
                    CreateMockedExpression(NewRqlInteger(25))),
            };

            var newObjectExpression = new NewObjectExpression(objectToken, values);

            var runtime = Mock.Of<IRuntime>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitNewObjectExpression(newObjectExpression);

            // Assert
            actual.Should().NotBeNull()
                .And.BeOfType<RqlObject>();
            var objProperties = (IDictionary<string, object>)actual.RuntimeValue;
            objProperties.Should().NotBeNullOrEmpty()
                .And.Contain("Name", "Roger")
                .And.Contain("Age", 25);
        }
    }
}