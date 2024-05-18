namespace Regulae.Rql
{
    using Regulae.Rql.Ast;

    internal interface IReverseRqlBuilder
    {
        string BuildRql(IAstElement astElement);
    }
}