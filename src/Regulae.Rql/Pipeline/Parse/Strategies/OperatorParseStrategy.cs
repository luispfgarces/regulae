namespace Regulae.Rql.Pipeline.Parse.Strategies
{
    using System;
    using System.Collections.Generic;
    using Regulae.Rql.Ast.Segments;
    using Regulae.Rql.Pipeline.Parse;
    using Regulae.Rql.Tokens;

    internal class OperatorParseStrategy : ParseStrategyBase<Segment>, ISegmentParseStrategy
    {
        public OperatorParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Segment Parse(ParseContext parseContext)
        {
            var currentToken = parseContext.GetCurrentToken();
            var operatorTokens = new List<Token>(2)
            {
                currentToken,
            };

            switch (currentToken.Type)
            {
                case TokenType.AND:
                case TokenType.ASSIGN:
                case TokenType.EQUAL:
                case TokenType.GREATER_THAN:
                case TokenType.GREATER_THAN_OR_EQUAL:
                case TokenType.IN:
                case TokenType.LESS_THAN:
                case TokenType.LESS_THAN_OR_EQUAL:
                case TokenType.MINUS:
                case TokenType.NOT_EQUAL:
                case TokenType.OR:
                case TokenType.PLUS:
                case TokenType.SLASH:
                case TokenType.STAR:
                case TokenType.XOR:
                    break;

                case TokenType.NOT:
                    if (!parseContext.MoveNextIfNextToken(TokenType.IN))
                    {
                        parseContext.EnterPanicMode("Expected token 'in'.", parseContext.GetNextToken());
                        break;
                    }

                    operatorTokens.Add(parseContext.GetCurrentToken());
                    break;

                default:
                    throw new InvalidOperationException("Unable to handle operator expression.");
            }

            return new OperatorSegment(operatorTokens.ToArray());
        }
    }
}