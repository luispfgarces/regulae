namespace Regulae.Rql.Pipeline.Interpret
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Regulae.Rql;

    [ExcludeFromCodeCoverage]
    internal class InterpreterException : Exception
    {
        public InterpreterException(
            string message,
            string rql,
            RqlSourcePosition beginPosition,
            RqlSourcePosition endPosition)
            : base(message)
        {
            this.Rql = rql;
            this.BeginPosition = beginPosition;
            this.EndPosition = endPosition;
        }

        public RqlSourcePosition BeginPosition { get; }

        public RqlSourcePosition EndPosition { get; }

        public string Rql { get; }
    }
}