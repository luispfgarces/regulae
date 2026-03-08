# Getting started

The following guide demonstrates the setup and usage logic for a simple ruleset which returns the right body mass index calculation formula for the scenario provided to the rules engine.

## Setting up the rules engine

Setting up a basic rules engine is fairly simple and it only requires a few lines of code. The most basic usage of Regulae's rules engine can be set up using the in-memory data source provider. Set up a rules engine instance as follows:

```csharp
var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
    .SetInMemoryDataSource() // You can use other data source implementations, and even create your own data source.
    .Build();
```

Before creating rules and adding them to the rules engine, you must take into consideration:

* the *ruleset*, which categorizes and groups related rules which should be evaluated together - in this case, we'll use `BodyMassIndexFormula` as ruleset name.
* the *conditions* to be used on the rules to constrain their applicability and to be provided to the rules engine for evaluation against those rules. In our case, we'll be using the conditions `MassUnitOfMeasure` and `HeightUnitOfMeasure`.

Those are necessary definitions in order to be able to create a rule and adding it to the rules engine - validation is enforced when adding a rule, and a failure will be returned if the ruleset or conditions have not been previously created.

Create the ruleset `#!csharp "BodyMassIndexFormula"`:

```csharp
var createRulesetResult = await rulesEngine.CreateRulesetAsync("BodyMassIndexFormula");
if (createRulesetResult.IsSuccess)
{
    // Ruleset created successfully
}
else
{
    var errors = createRulesetResult.Errors;
    // Handle errors
}
```

And create the conditions `#!csharp "MassUnitOfMeasure"` and `#!csharp "HeightUnitOfMeasure"` as well:

```csharp
var createConditionResult = await rulesEngine.CreateConditionAsync("MassUnitOfMeasure", ConditionTypes.String);
if (createConditionResult.IsSuccess)
{
    // Condition created successfully
}
else
{
    var errors = createConditionResult.Errors;
    // Handle errors
}

createConditionResult = await rulesEngine.CreateConditionAsync("HeightUnitOfMeasure", ConditionTypes.String);
if (createConditionResult.IsSuccess)
{
    // Ruleset created successfuly
}
else
{
    var errors = createConditionResult.Errors;
    // Handle errors
}
```

!!! tip "Auto-create rulesets"
    The rules engine builder options provides the setting `AutoCreateRulesets` which allows the automatic ruleset creation when adding a new rule, and bypasses the need to manually create a ruleset. This can be accomplished by setting it to true by invoking `#!csharp EnableAutoCreateRulesets()`.

    ```csharp
    var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
        .SetInMemoryDataSource()
        .Configure(opt => opt.EnableAutoCreateRulesets())
        .Build();
    ```

    This setting will allow you to add whose ruleset does not exist, by instructing the rules engine to automatically create the rule's provided ruleset.

## Defining rules

We'll add 2 new rules to the rules engine now, one with the formula for calculating the body mass index on the metric system, and the other with the formula for calculating the body mass index on the imperial system.

Create the rule for the metric system:

```csharp
var bodyMassIndexFormulaMetricRule = Rule.Create("Body mass formula for metric system")
    .InRuleset("BodyMassIndexFormula")
    .Since(DateTime.Parse("2018-01-01Z"))
    .ApplyWhen(x => x
        .Value("MassUnitOfMeasure", Operators.Equal, "kilograms")
        .Value("HeightUnitOfMeasure", Operators.Equal, "meters"))
    .SetContent("weight / height ^ 2")
    .Build()
    .Rule;
```

And create the rule for the imperial system:

```csharp
var bodyMassIndexFormulaImperialRule = Rule.Create("Body mass formula for imperial system")
    .InRuleset("BodyMassIndexFormula")
    .Since(DateTime.Parse("2018-01-01Z"))
    .ApplyWhen(x => x
        .Value("MassUnitOfMeasure", Operators.Equal, "pounds")
        .Value("HeightUnitOfMeasure", Operators.Equal, "inches"))
    .SetContent("703 * (weight / height ^ 2)")
    .Build()
    .Rule;
```

At last, add the rules to the rules engine.

```csharp
var addRuleResult = await rulesEngine.AddRuleAsync(bodyMassIndexFormulaMetricRule, RuleAddPriorityOption.AtSmallestNumber);
if (addRuleResult.IsSuccess)
{
    // Rule added successfully
}
else
{
    var errors = addRuleResult.Errors;
    // Handle errors
}

addRuleResult = await rulesEngine.AddRuleAsync(bodyMassIndexFormulaImperialRule, RuleAddPriorityOption.AtSmallestNumber);
if (addRuleResult.IsSuccess)
{
    // Rule added successfully
}
else
{
    var errors = addRuleResult.Errors;
    // Handle errors
}
```

!!! tip "Full documentation on adding rules"
    Please refer to [add rules](add-rules.md) documentation to explore the rule builder API and perceive the possibilities to configure a rule.

## Evaluating rules



Set the conditions to supply to the engine.

```csharp
var conditions = new Dictionary<string, object>
{
    { "MassUnitOfMeasure", "pounds" },
    { "HeightUnitOfMeasure", "inches" },
};
```

Set the date at which you want the rules set to evalutated.

```csharp
var matchDate = new DateTime(2019, 1, 1);
```

Set the ruleset you want to evaluate.

```csharp
var ruleset = "BodyMassIndexFormula";
```

Evaluate rule match and print the result to console.

```csharp
var ruleMatch = await rulesEngine.MatchOneAsync(ruleset, matchDate, conditions);
if (ruleMatch is not null)
{
    Console.WriteLine($"Formula for body mass index calculation: {ruleMatch.ContentContainer.GetContentAs<string>()}");
}
```
