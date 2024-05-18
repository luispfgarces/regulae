namespace Regulae.InMemory.Sample.Engine
{
    using System.Collections.Generic;
    using Regulae.InMemory.Sample.Enums;

    internal interface IRuleSpecificationsProvider
    {
        RulesetNames[] Rulesets { get; }

        IEnumerable<RuleSpecification> GetRulesSpecifications();
    }
}