namespace Regulae.WebUI.Sample.Engine
{
    using System.Collections.Generic;
    using Regulae.WebUI.Sample.Enums;

    internal interface IRuleSpecificationsProvider
    {
        RulesetNames[] Rulesets { get; }

        IEnumerable<RuleSpecification> GetRulesSpecifications();
    }
}