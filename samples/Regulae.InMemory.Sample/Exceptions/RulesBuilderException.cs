namespace Regulae.InMemory.Sample.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [Serializable]
    [ExcludeFromCodeCoverage]
    public class RulesBuilderException : Exception
    {
        public RulesBuilderException(string message, IEnumerable<string> ruleEngineErrors) : base(message)
        {
            this.RuleEngineErrors = ruleEngineErrors;
        }

        public IEnumerable<string> RuleEngineErrors { get; }
    }
}