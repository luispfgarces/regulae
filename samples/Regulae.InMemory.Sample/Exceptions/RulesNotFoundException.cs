namespace Regulae.InMemory.Sample.Exceptions
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [Serializable]
    [ExcludeFromCodeCoverage]
    public class RulesNotFoundException : Exception
    {
        public RulesNotFoundException(string message) : base(message)
        {
        }
    }
}