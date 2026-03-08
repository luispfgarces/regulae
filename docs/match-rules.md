
# Match rules

This page focus on how to match rules using Rules.Framework.

The Rules Engine exposes two methods to match rules: `MatchOneAsync` and `MatchManyAsync`.

## MatchOneAsync

The `MatchOneAsync` method provides a single rule match (if any) to the given `ruleset` at the specified `matchDateTime` and satisfying the supplied `conditions`.

If there's more than one match, a rule is selected based on the priority criteria and value: topmost selects the lowest priority number and bottommost selects highest priority.

```csharp
var conditions = new Dictionary<string, object>
{
    { "Condition1", "Value1" },
    { "Condition2", 123 }
};

Rule matchedRule = await rulesEngine.MatchOneAsync("my-ruleset", DateTime.UtcNow, conditions);
```

## MatchManyAsync

The `MatchManyAsync` method provides all rule matches (if any) to the given `ruleset` at the specified `matchDateTime` and satisfying the supplied `conditions`.

```csharp
var conditions = new Dictionary<string, object>
{
    { "Condition1", "Value1" },
    { "Condition2", 123 }
};

IReadOnlyCollection<Rule> matchedRules = await rulesEngine.MatchManyAsync("my-ruleset", DateTime.UtcNow, conditions);
```

## Differences

The main difference between `MatchOneAsync` and `MatchManyAsync` is that `MatchOneAsync` returns only one rule, selected by priority if multiple rules match, while `MatchManyAsync` returns all rules that match the given conditions.
