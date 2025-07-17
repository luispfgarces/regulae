namespace Regulae.WebUI.Sample.Engine
{
    using System.Collections.Generic;
    using Regulae.WebUI.Sample.Enums;

    internal interface IRuleSpecificationsProvider
    {
        (ConditionNames Condition, DataTypes DataType)[] Conditions { get; }

        RulesetNames[] Rulesets { get; }

        IEnumerable<RuleSpecification> GetRulesSpecifications();
    }
}