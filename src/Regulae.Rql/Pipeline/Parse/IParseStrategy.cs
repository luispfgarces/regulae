namespace Regulae.Rql.Pipeline.Parse
{
    internal interface IParseStrategy<out TParseOutput>
    {
        TParseOutput Parse(ParseContext parseContext);
    }
}