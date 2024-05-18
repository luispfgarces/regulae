namespace Regulae.Providers.InMemory
{
    using Regulae;
    using Regulae.Providers.InMemory.DataModel;

    internal interface IRuleFactory
    {
        Rule CreateRule(RuleDataModel ruleDataModel);

        RuleDataModel CreateRule(Rule rule);
    }
}