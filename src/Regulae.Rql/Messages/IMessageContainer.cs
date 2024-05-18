namespace Regulae.Rql.Messages
{
    using System;
    using Regulae.Rql;

    internal interface IMessageContainer : IDisposable
    {
        void Error(string message, RqlSourcePosition beginPosition, RqlSourcePosition endPosition);

        void Warning(string message, RqlSourcePosition beginPosition, RqlSourcePosition endPosition);
    }
}