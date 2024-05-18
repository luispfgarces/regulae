namespace Regulae.Providers.MongoDb
{
    using Regulae;
    using Regulae.Providers.MongoDb.DataModel;

    internal interface IRuleFactory
    {
        Rule CreateRule(RuleDataModel ruleDataModel);

        RuleDataModel CreateRule(Rule rule);
    }
}