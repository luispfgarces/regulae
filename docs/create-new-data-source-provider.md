In case you need to support other databases, you'll have to implement your own data source provider logic. You should implement `#!csharp IRulesDataSource` interface and use it on rules engine builder.

```csharp
public interface IRulesDataSource
{
    Task AddRuleAsync(Rule rule);

    Task<IEnumerable<Rule>> GetRulesAsync(string ruleset, DateTime dateBegin, DateTime dateEnd);

    Task<IEnumerable<Rule>> GetRulesByAsync(RulesFilterArgs rulesFilterArgs);

    Task UpdateRuleAsync(Rule rule);
}
```

You'll have to map each condition type defined by you to a data type on your rules data source. The ones available are:

- Integer
- Decimal
- String
- Boolean

For your guidance, check Mongo DB implementation: [MongoDbProviderRulesDataSource.cs](src/Regulae.Providers.MongoDb/MongoDbProviderRulesDataSource.cs)