namespace Regulae.Rql.Tests.Tokens
{
    using System;
    using FluentAssertions;
    using Regulae.Rql;
    using Regulae.Rql.Tokens;
    using Xunit;

    public class TokenTests
    {
        [Fact]
        public void Create_WithValidArguments_SetsProperties()
        {
            // Arrange
            var begin = RqlSourcePosition.Empty;
            var end = RqlSourcePosition.Empty;
            var lexeme = "abc";
            var literal = "lit";

            // Act
            var token = Token.Create(lexeme, false, literal, begin, end, 3u, TokenType.IDENTIFIER);

            // Assert
            token.Lexeme.Should().Be(lexeme);
            token.IsEscaped.Should().BeFalse();
            token.Literal.Should().Be(literal);
            token.BeginPosition.Should().Be(begin);
            token.EndPosition.Should().Be(end);
            token.Length.Should().Be(3u);
            token.Type.Should().Be(TokenType.IDENTIFIER);
            token.Next.Should().BeNull();
            token.Previous.Should().BeNull();
        }

        [Fact]
        public void Create_NullLexeme_ThrowsArgumentNullException()
        {
            // Arrange / Act
            var act = () => Token.Create(null!, false, null!, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 0u, TokenType.None);

            // Assert
            var ex = Assert.Throws<ArgumentNullException>(act);
            ex.ParamName.Should().Be("lexeme");
        }

        [Fact]
        public void UnescapedLexeme_WhenEscaped_ReturnsWithoutLeadingChar()
        {
            // Arrange
            var token = Token.Create("#name", true, "name", RqlSourcePosition.Empty, RqlSourcePosition.Empty, 5u, TokenType.IDENTIFIER);

            // Act
            var unescaped = token.UnescapedLexeme;

            // Assert
            unescaped.Should().Be("name");
        }

        [Fact]
        public void UnescapedLexeme_WhenNotEscaped_ReturnsLexeme()
        {
            // Arrange
            var token = Token.Create("name", false, "name", RqlSourcePosition.Empty, RqlSourcePosition.Empty, 4u, TokenType.IDENTIFIER);

            // Act
            var unescaped = token.UnescapedLexeme;

            // Assert
            unescaped.Should().Be("name");
        }

        [Fact]
        public void ToString_ContainsKeyParts()
        {
            // Arrange
            var token = Token.Create("x", false, 123, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1u, TokenType.INT);

            // Act
            var str = token.ToString();

            // Assert
            str.Should().Contain(token.Type.ToString());
            str.Should().Contain(token.Lexeme);
            str.Should().Contain(token.Literal.ToString());
        }

        [Fact]
        public void None_IsSingletonWithExpectedValues()
        {
            // Act
            var none = Token.None;

            // Assert
            none.Type.Should().Be(TokenType.None);
            none.Length.Should().Be(0u);
            // Lexeme was created as null for Token.None in implementation; ensure accessing it does not throw
            ((object)none.Lexeme).Should().BeNull();
        }

        [Fact]
        public void NextAndPrevious_AreSettable()
        {
            // Arrange
            var t1 = Token.Create("a", false, null!, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1u, TokenType.IDENTIFIER);
            var t2 = Token.Create("b", false, null!, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1u, TokenType.IDENTIFIER);

            // Act
            t1.Next = t2;
            t2.Previous = t1;

            // Assert
            t1.Next.Should().BeSameAs(t2);
            t2.Previous.Should().BeSameAs(t1);
        }
    }
}