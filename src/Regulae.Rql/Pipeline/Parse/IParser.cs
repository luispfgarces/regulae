namespace Regulae.Rql.Pipeline.Parse
{
    using System.Collections.Generic;
    using Regulae.Rql.Tokens;

    internal interface IParser
    {
        ParseResult Parse(IReadOnlyList<Token> tokens);
    }
}