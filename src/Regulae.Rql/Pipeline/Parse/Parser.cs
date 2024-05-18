namespace Regulae.Rql.Pipeline.Parse
{
    using System.Collections.Generic;
    using Regulae.Rql.Ast.Statements;
    using Regulae.Rql.Messages;
    using Regulae.Rql.Pipeline.Parse.Strategies;
    using Regulae.Rql.Tokens;

    internal class Parser : IParser
    {
        private readonly IParseStrategyProvider parseStrategyProvider;

        public Parser(IParseStrategyProvider parseStrategyProvider)
        {
            this.parseStrategyProvider = parseStrategyProvider;
        }

        public ParseResult Parse(IReadOnlyList<Token> tokens)
        {
            var parseContext = new ParseContext(tokens);
            var statements = new List<Statement>();

            using var messageContainer = new MessageContainer();
            while (parseContext.MoveNext())
            {
                var statement = this.parseStrategyProvider.GetStatementParseStrategy<DeclarationParseStrategy>().Parse(parseContext);
                if (parseContext.PanicMode)
                {
                    var panicModeInfo = parseContext.PanicModeInfo;
                    messageContainer.Error(
                        panicModeInfo.Message,
                        panicModeInfo.CauseToken.BeginPosition,
                        panicModeInfo.CauseToken.EndPosition);
                    parseContext.ExitPanicMode();
                }

                statements.Add(statement);
            }

            var messages = messageContainer.Messages;
            if (messageContainer.ErrorsCount > 0)
            {
                return ParseResult.CreateError(statements, messages);
            }

            return ParseResult.CreateSuccess(statements, messages);
        }
    }
}