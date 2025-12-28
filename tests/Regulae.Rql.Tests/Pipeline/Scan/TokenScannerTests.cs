namespace Regulae.Rql.Tests.Pipeline.Scan
{
    using System;
    using FluentAssertions;
    using Regulae.Rql.Pipeline.Scan;
    using Regulae.Rql.Tokens;
    using Xunit;

    public class TokenScannerTests
    {
        [Fact]
        public void ScanTokens_NullSource_ThrowsArgumentNullException()
        {
            // Arrange
            var scanner = new TokenScanner();

            // Act
            var act = () => scanner.ScanTokens(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void ScanTokens_EmptyOrWhitespace_ReturnsSuccessAndNoTokens(string source)
        {
            // Arrange
            var scanner = new TokenScanner();

            // Act
            var result = scanner.ScanTokens(source);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Tokens.Should().BeEmpty();
            result.Messages.Should().BeEmpty();
        }

        [Fact]
        public void ScanTokens_Identifier_ReturnsIdentifierToken()
        {
            // Arrange
            var scanner = new TokenScanner();
            var source = "abc";

            // Act
            var result = scanner.ScanTokens(source);

            // Assert
            result.Success.Should().BeTrue();
            result.Tokens.Should().HaveCount(1);
            var token = result.Tokens[0];
            token.Type.Should().Be(TokenType.IDENTIFIER);
            token.Lexeme.Should().Be("abc");
            token.Literal.Should().Be("abc");
            token.Next.Should().Be(Token.None);
        }

        [Fact]
        public void ScanTokens_KeywordBoolTrue_ReturnsBoolTokenWithLiteralTrue()
        {
            // Arrange
            var scanner = new TokenScanner();
            var source = "TRUE";

            // Act
            var result = scanner.ScanTokens(source);

            // Assert
            result.Success.Should().BeTrue();
            result.Tokens.Should().HaveCount(1);
            var token = result.Tokens[0];
            token.Type.Should().Be(TokenType.BOOL);
            token.Lexeme.Should().Be("TRUE");
            token.Literal.Should().Be(true);
        }

        [Fact]
        public void ScanTokens_Keyword_SEARCH_ReturnsKeywordToken()
        {
            // Arrange
            var scanner = new TokenScanner();
            var source = "SEARCH";

            // Act
            var result = scanner.ScanTokens(source);

            // Assert
            result.Success.Should().BeTrue();
            result.Tokens.Should().HaveCount(1);
            var token = result.Tokens[0];
            token.Type.Should().Be(TokenType.SEARCH);
            token.Lexeme.Should().Be("SEARCH");
        }

        [Fact]
        public void ScanTokens_Placeholder_ReturnsPlaceholderWithLiteral()
        {
            // Arrange
            var scanner = new TokenScanner();
            var source = "@name";

            // Act
            var result = scanner.ScanTokens(source);

            // Assert
            result.Success.Should().BeTrue();
            result.Tokens.Should().HaveCount(1);
            var token = result.Tokens[0];
            token.Type.Should().Be(TokenType.PLACEHOLDER);
            token.Lexeme.Should().Be("@name");
            token.Literal.Should().Be("name");
        }

        [Fact]
        public void ScanTokens_StringWithEscapedQuote_ReturnsStringWithUnescapedLiteral()
        {
            // Arrange
            var scanner = new TokenScanner();
            var source = "\"a\\\"b\"";

            // Act
            var result = scanner.ScanTokens(source);

            // Assert
            result.Success.Should().BeTrue();
            result.Tokens.Should().HaveCount(1);
            var token = result.Tokens[0];
            token.Type.Should().Be(TokenType.STRING);
            token.Lexeme.Should().Be("\"a\\\"b\"");
            token.Literal.Should().Be("a\"b");
        }

        [Fact]
        public void ScanTokens_DateLiteral_ReturnsDateTokenWithValueString()
        {
            // Arrange
            var scanner = new TokenScanner();
            var source = "$2024-01-01T00:00:00Z$";

            // Act
            var result = scanner.ScanTokens(source);

            // Assert
            result.Success.Should().BeTrue();
            result.Tokens.Should().HaveCount(1);
            var token = result.Tokens[0];
            token.Type.Should().Be(TokenType.DATE);
            token.Lexeme.Should().Be("$2024-01-01T00:00:00Z$");
            token.Literal.Should().Be("2024-01-01T00:00:00Z");
        }

        [Fact]
        public void ScanTokens_InvalidDate_ReturnsError()
        {
            // Arrange
            var scanner = new TokenScanner();
            var source = "$not-a-date$";

            // Act
            var result = scanner.ScanTokens(source);

            // Assert
            result.Success.Should().BeFalse();
            result.Messages.Should().Contain(m => m.Text.Contains("Invalid date"));
        }

        [Fact]
        public void ScanTokens_InvalidNumber_ReturnsError()
        {
            // Arrange
            var scanner = new TokenScanner();
            var source = "1abc";

            // Act
            var result = scanner.ScanTokens(source);

            // Assert
            result.Success.Should().BeFalse();
            result.Messages.Should().NotBeNull();
            result.Messages.Should().Contain(m => m.Text.Contains("Invalid number"));
        }

        [Fact]
        public void ScanTokens_MultipleTokens_AreLinkedByNextPrevious()
        {
            // Arrange
            var scanner = new TokenScanner();
            var source = "a @p";

            // Act
            var result = scanner.ScanTokens(source);

            // Assert
            result.Success.Should().BeTrue();
            result.Tokens.Should().HaveCount(2);

            var first = result.Tokens[0];
            var second = result.Tokens[1];

            first.Next.Should().Be(second);
            second.Previous.Should().Be(first);
            second.Next.Should().Be(Token.None);
        }

        [Fact]
        public void ScanTokens_EscapedIdentifier_IsMarkedAsEscapedAndUnescapedLexemeAvailable()
        {
            // Arrange
            var scanner = new TokenScanner();
            var source = "#abc";

            // Act
            var result = scanner.ScanTokens(source);

            // Assert
            result.Success.Should().BeTrue();
            result.Tokens.Should().HaveCount(1);
            var token = result.Tokens[0];
            token.Type.Should().Be(TokenType.IDENTIFIER);
            token.Lexeme.Should().Be("#abc");
            token.IsEscaped.Should().BeTrue();
            token.UnescapedLexeme.Should().Be("abc");
        }

        [Theory]
        [InlineData("(", TokenType.BRACKET_LEFT)]
        [InlineData(")", TokenType.BRACKET_RIGHT)]
        [InlineData("{", TokenType.BRACE_LEFT)]
        [InlineData("}", TokenType.BRACE_RIGHT)]
        [InlineData(";", TokenType.SEMICOLON)]
        [InlineData(",", TokenType.COMMA)]
        [InlineData(".", TokenType.DOT)]
        [InlineData("+", TokenType.PLUS)]
        [InlineData("-", TokenType.MINUS)]
        [InlineData("[", TokenType.STRAIGHT_BRACKET_LEFT)]
        [InlineData("]", TokenType.STRAIGHT_BRACKET_RIGHT)]
        [InlineData("/", TokenType.SLASH)]
        [InlineData("*", TokenType.STAR)]
        [InlineData("=", TokenType.ASSIGN)]
        [InlineData("==", TokenType.EQUAL)]
        [InlineData("!=", TokenType.NOT_EQUAL)]
        [InlineData(">", TokenType.GREATER_THAN)]
        [InlineData(">=", TokenType.GREATER_THAN_OR_EQUAL)]
        [InlineData("<", TokenType.LESS_THAN)]
        [InlineData("<=", TokenType.LESS_THAN_OR_EQUAL)]
        [InlineData("<>", TokenType.NOT_EQUAL)]
        public void ScanTokens_SingleOrMultiCharSymbols_AreRecognized(string source, object expected)
        {
            // Arrange
            var scanner = new TokenScanner();

            // Act
            var result = scanner.ScanTokens(source);

            // Assert
            result.Success.Should().BeTrue();
            result.Tokens.Should().HaveCount(1);
            result.Tokens[0].Type.Should().Be((TokenType)expected);
        }

        [Fact]
        public void ScanTokens_DecimalNumber_IsParsedAsDecimal()
        {
            // Arrange
            var scanner = new TokenScanner();
            var source = "12.34";

            // Act
            var result = scanner.ScanTokens(source);

            // Assert
            result.Success.Should().BeTrue();
            result.Tokens.Should().HaveCount(1);
            var token = result.Tokens[0];
            token.Type.Should().Be(TokenType.DECIMAL);
            token.Lexeme.Should().Be("12.34");
            token.Literal.Should().BeOfType<decimal>().Which.Should().Be(12.34m);
        }

        [Fact]
        public void ScanTokens_Integer_IsParsedAsInt()
        {
            // Arrange
            var scanner = new TokenScanner();
            var source = "42";

            // Act
            var result = scanner.ScanTokens(source);

            // Assert
            result.Success.Should().BeTrue();
            result.Tokens.Should().HaveCount(1);
            var token = result.Tokens[0];
            token.Type.Should().Be(TokenType.INT);
            token.Lexeme.Should().Be("42");
            token.Literal.Should().BeOfType<int>().Which.Should().Be(42);
        }

        [Fact]
        public void ScanTokens_UnterminatedString_ProducesErrorMessage()
        {
            // Arrange
            var scanner = new TokenScanner();
            var source = "\"abc";

            // Act
            var result = scanner.ScanTokens(source);

            // Assert
            result.Success.Should().BeFalse();
            result.Messages.Should().Contain(m => m.Text.Contains("Unterminated string"));
        }

        [Fact]
        public void ScanTokens_UnterminatedDate_ProducesErrorMessage()
        {
            // Arrange
            var scanner = new TokenScanner();
            var source = "$2024-01-01";

            // Act
            var result = scanner.ScanTokens(source);

            // Assert
            result.Success.Should().BeFalse();
            result.Messages.Should().Contain(m => m.Text.Contains("Unterminated date"));
        }

        [Fact]
        public void ScanTokens_EscapeAtEnd_ProducesExpectedCharError()
        {
            // Arrange
            var scanner = new TokenScanner();
            var source = "#";

            // Act
            var result = scanner.ScanTokens(source);

            // Assert
            result.Success.Should().BeFalse();
            result.Messages.Should().Contain(m => m.Text.Contains("Expected char after"));
        }

        [Fact]
        public void ScanTokens_ExclamationFollowedByUnexpectedChar_ProducesExpectedEqualError()
        {
            // Arrange
            var scanner = new TokenScanner();
            var source = "!>";

            // Act
            var result = scanner.ScanTokens(source);

            // Assert
            result.Success.Should().BeFalse();
            result.Messages.Should().Contain(m => m.Text.Contains("Expected '=' after '!'"));
        }

        [Fact]
        public void ScanTokens_InvalidChar_ProducesInvalidCharError()
        {
            // Arrange
            var scanner = new TokenScanner();
            var source = "€";

            // Act
            var result = scanner.ScanTokens(source);

            // Assert
            result.Success.Should().BeFalse();
            result.Messages.Should().Contain(m => m.Text.Contains("Invalid char"));
        }
    }
}