# Strong Typing and Type Safety

Regulae leverages .NET's strong typing capabilities to provide compile-time safety and improved developer experience when managing and evaluating rules programmatically. By using generic types for rulesets and conditions, Regulae ensures that rule definitions are type-checked, reducing runtime errors and enhancing code maintainability.

## Generic APIs Overview

Regulae's core APIs are designed with generics to enforce type safety:

- **`#!csharp IRulesEngine<TRuleset, TCondition>`**: The main engine interface, parameterized by ruleset and condition types.
- **`#!csharp Rule<TRuleset, TCondition>`**: Represents a strongly-typed rule with typed ruleset and conditions.
- **`#!csharp RuleBuilder<TRuleset, TCondition>`**: Fluent API for building rules with type-safe condition definitions.

These generics restrict `TRuleset` and `TCondition` to enums or strings, preventing invalid types at compile time.

## Advantages of Strong Typing

- **Compile-Time Validation**: Catch type mismatches during development, not at runtime.
- **IntelliSense Support**: IDEs provide auto-completion for enum values, reducing typos.
- **Refactoring Safety**: Renaming enums automatically updates all references.
- **Domain Clarity**: Enums make business logic explicit and self-documenting.
- **Error Prevention**: Eliminates string-based condition names that could lead to runtime failures.

!!! warning "Persistent data sources"
    Consider the impact on persistent data sources - e.g. MongoDB - before refactoring ruleset names or condition names.

## Examples

Define enums for rulesets and conditions:

```csharp
public enum DiscountRulesets
{
    ProductDiscounts,
    CustomerDiscounts
}

public enum DiscountConditions
{
    Age,
    LoyaltyLevel,
    PurchaseAmount
}
```

Use them in the rules engine:

```csharp
// Configure the engine
var rulesEngine = RulesEngineBuilder
    .CreateRulesEngine()
    .WithInMemoryDataSource()
    .Build();

// Obtain generic engine with strong types
var genericRulesEngine = rulesEngine.MakeGeneric<DiscountRulesets, DiscountConditions>();

// Build a rule with type-safe conditions
var rule = Rule.Create<DiscountRulesets, DiscountConditions>("Senior Discount")
    .InRuleset(DiscountRulesets.CustomerDiscounts)  // Enum, not string
    .Since(DateTime.Now)
    .SetContent("10% off")
    .ApplyWhen(c => c.Value(DiscountConditions.Age, Operators.GreaterThan, 65))  // Enum, not string
    .Build();

// Add and evaluate
await genericRulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.HighPriority);

var matchedRule = await genericRulesEngine.MatchOneAsync(
    DiscountRulesets.CustomerDiscounts,
    DateTime.Now,
    new Dictionary<DiscountConditions, object> { { DiscountConditions.Age, 70 } });
```

Without strong typing, you'd use strings like `"CustomerDiscounts"` and `"Age"`, risking typos and losing IDE support.

This approach makes rule management more robust and developer-friendly, aligning with .NET best practices for type safety.