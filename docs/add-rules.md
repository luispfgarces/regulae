# Creating a rule

## The rule builder API

The rule builder API is currently the only way to build new rules. It has a fluent API in order to make rule building an easy and comprehensive development experience.

To start building a rule, begin by invoking `#!csharp Create(string name)` and providing a `name` for the new rule:

```csharp
Rule.Create("Citizen classification for adult")
```

A rule's name is its unique identifier in the scope of a ruleset, so you must pay attention to create your rules without repeating names considering a particular ruleset.

## Define a ruleset

The ruleset defines to which set of rules the new rule will belong to and be evaluated together with. You can set the new rule's `ruleset` by invoking `#!csharp InRuleset(string ruleset)`:

```csharp
Rule.Create("Citizen classification for adult")
    .InRuleset("Citizen classification")
```

## Set dates interval

After defining a name and a ruleset, you should assign the rule a dates interval to define from when to when the rule is applicable. Although the begin date is mandatory, the end date is not, so you can combine `#!csharp Since(DateTime dateBegin)` and `#!csharp Until(DateTime dateEnd)` methods at your choosing to represent your scenario:

```csharp
Rule.Create("Citizen classification for adult")
    .InRuleset("Citizen classification")
    .Since(DateTime.Parse("2025-01-01Z"))
```

or

```csharp
Rule.Create("Citizen classification for adult")
    .InRuleset("Citizen classification")
    .Since(DateTime.Parse("2025-01-01Z"))
    .Until(DateTime.Parse("2026-01-01Z"))
```

If you don't specify `#!csharp Until(DateTime dateEnd)`, the date end will be setted to `#!csharp null` (it means rule applies to *ad eternum*).

You also have other other shorthand alternatives to define both date begin and date end:

- `#!csharp Since(string dateBegin)` - shorthand that handles the date begin parsing from string.
- `#!csharp SinceUtc(int year, int month, int day)` - shorthand that defines the date begin from `year`, `month`, and `day` parameters, assuming hour, minute, and second as 0 and UTC timezone.
- `#!csharp SinceUtc(int year, int month, int day, int hour, int minute, int second)` - shorthand that defines the date begin from `year`, `month`, `day`, `hour`, `minute`, and `second` parameters, assuming UTC timezone.
- `#!csharp Until(string dateEnd)` - shorthand that handles the date end parsing from string.
- `#!csharp UntilUtc(int year, int month, int day)` - shorthand that defines the date end from `year`, `month`, and `day` parameters, assuming hour, minute, and second as 0 and UTC timezone.
- `#!csharp UntilUtc(int year, int month, int day, int hour, int minute, int second)` - shorthand that defines the date end from `year`, `month`, `day`, `hour`, `minute`, and `second` parameters, assuming UTC timezone.

You should see the implementation of those shorthands before using them, to properly assess if those fit your implementation needs.

## Set content

Next, you can set the rule content using the `#!csharp SetContent(object content)` method:

```csharp
Rule.Create("Citizen classification for adult")
    .InRuleset("Citizen classification")
    .Since(DateTime.Parse("2025-01-01Z"))
    .Until(DateTime.Parse("2026-01-01Z"))
    .SetContent("adult")
```

This method allows you to set what value the rule is actually keeping so that when you match it, you can retrieve its' value.

There is also the method overload `#!csharp SetContent(object content, IContentSerializationProvider contentSerializationProvider)`, however you should only use it if you are implementing a new data source for rules - a new `#!csharp IRulesDataSource<TContentType, TConditionType>` implementation.

## Set the rule's applicability conditions

You can define a rule's applicability conditions using the optional method `#!csharp ApplyWhen(...)`, which you can use to set additional conditions to restrict at your discretion how rule should be matched when you try to evaluate it using the Rules Engine.

### Value condition

A value condition allows you to define conditions evaluated against values supplied to the Rules Engine when matching or searching rules. This type of condition considers the value supplied to the Rules Engine as a left hand operand, defines a operator to apply on the condition, and a right hand operand. Consider the example:

> Age > 18

The simplest way to define a value condition is by using the `#!csharp ApplyWhen<T>(string condition, Operators condOperator, T rightOperand)` method overload:

```csharp
Rule.Create("Citizen classification for adult")
    .InRuleset("Citizen classification")
    .Since(DateTime.Parse("2025-01-01Z"))
    .Until(DateTime.Parse("2026-01-01Z"))
    .SetContent("adult")
    .ApplyWhen("Age", Operators.GreaterThanOrEqual, 18)
```

You can also alternatively use the fluent condition builder to specify the value condition, using the `#!csharp ApplyWhen(Func<IRootConditionNodeBuilder, IConditionNode> conditionFunc)` method overload and using the condition builder method `#!csharp Value<T>(string condition, Operators condOperator, T operand)`.

```csharp
Rule.Create("Citizen classification for adult")
    .InRuleset("Citizen classification")
    .Since(DateTime.Parse("2025-01-01Z"))
    .Until(DateTime.Parse("2026-01-01Z"))
    .SetContent("adult")
    .ApplyWhen(x => x
        .Value("Age", Operators.GreaterThanOrEqual, 18))
```

Value conditions support the following operators:

- CaseInsensitiveEndsWith
- CaseInsensitiveStartsWith
- Contains
- EndsWith
- Equal
- GreaterThan
- GreaterThanOrEqual
- In
- LesserThan
- LesserThanOrEqual
- NotContains
- NotEndsWith
- NotEqual
- NotIn
- NotStartsWith
- StartsWith

Also, value conditions can be defined with a limited set of data types, which are:
- int
- decimal
- boolean
- string

Not all comparison operators are supported for all data types, some combinations of them are not valid. This will be validated by rule builder and a validation error will be returned if a invalid combination is attempted. Check the following grid to see combinations are valid or not:

|Operator/Data Type|int|decimal|bool|string|
|-|-|-|-|-|
|Equal|:white_check_mark:|:white_check_mark:|:white_check_mark:|:white_check_mark:|
|NotEqual|:white_check_mark:|:white_check_mark:|:white_check_mark:|:white_check_mark:|
|GreaterThan|:white_check_mark:|:white_check_mark:|:x:|:x:|
|GreaterThanOrEqual|:white_check_mark:|:white_check_mark:|:x:|:x:|
|LesserThan|:white_check_mark:|:white_check_mark:|:x:|:x:|
|LesserThanOrEqual|:white_check_mark:|:white_check_mark:|:x:|:x:|
|Contains|:x:|:x:|:x:|:white_check_mark:|
|In|:white_check_mark:|:white_check_mark:|:x:|:white_check_mark:|
|StartsWith|:x:|:x:|:x:|:white_check_mark:|
|EndsWith|:x:|:x:|:x:|:white_check_mark:|
|CaseInsensitiveStartsWith|:x:|:x:|:x:|:white_check_mark:|
|CaseInsensitiveEndsWith|:x:|:x:|:x:|:white_check_mark:|
|NotStartsWith|:x:|:x:|:x:|:white_check_mark:|
|NotEndsWith|:x:|:x:|:x:|:white_check_mark:|

You should also take into consideration the multiplicity of operands when setting value conditions on a rule. Theoretically, all multiplicity combinations are supported (one to one, one to many, many to one, many to many), but that also relies on a per-operator implementation for each multiplicity. Please refer to following combinations to see which multiplicities are supported or not:

|Operator/Multiplicity|One to One (1 - 1)|One to Many (1 - *)|Many to One (\* - 1)|Many to Many (\* - \*)|
|-|-|-|-|-|
|Equal|:white_check_mark:|:x:|:x:|:x:|
|NotEqual|:white_check_mark:|:x:|:x:|:x:|
|GreaterThan|:white_check_mark:|:x:|:x:|:x:|
|GreaterThanOrEqual|:white_check_mark:|:x:|:x:|:x:|
|LesserThan|:white_check_mark:|:x:|:x:|:x:|
|LesserThanOrEqual|:white_check_mark:|:x:|:x:|:x:|
|Contains|:white_check_mark:|:x:|:x:|:x:|
|In|:x:|:white_check_mark:|:x:|:x:|
|StartsWith|:white_check_mark:|:x:|:x:|:x:|
|EndsWith|:white_check_mark:|:x:|:x:|:x:|
|CaseInsensitiveStartsWith|:white_check_mark:|:x:|:x:|:x:|
|CaseInsensitiveEndsWith|:white_check_mark:|:x:|:x:|:x:|
|NotStartsWith|:white_check_mark:|:x:|:x:|:x:|
|NotEndsWith|:white_check_mark:|:x:|:x:|:x:|

### Composed condition nodes

A composed condition allows you to make compositions (in tree) of both composed conditions and valued condition, also specifying a logical operator to apply between them (And/Or/Xor). This type of condition actualy lets you build complex decision trees tailored to your needs. Taking for instance a little more complex example:

> Age > 18 **And** Gender = 'male'

It would be captured in the following way:

```csharp
Rule.Create("Citizen classification for a male adult")
    .InRuleset("Citizen classification")
    .Since(DateTime.Parse("2025-01-01Z"))
    .Until(DateTime.Parse("2026-01-01Z"))
    .SetContent("male adult")
    .ApplyWhen(x => x
        .And(y => y
            .Value("Age", Operators.GreaterThanOrEqual, 18)
            .Value("Gender", Operators.Equal, "male")))
```

To build a composed condition, you have to methods for each logical operator supported. Supported logical operators are:

- `#!csharp And(Func<IFluentConditionNodeBuilder, IFluentConditionNodeBuilder> conditionFunc)`
- `#!csharp Or(Func<IFluentConditionNodeBuilder, IFluentConditionNodeBuilder> conditionFunc)`
- `#!csharp Xor(Func<IFluentConditionNodeBuilder, IFluentConditionNodeBuilder> conditionFunc)`

On the method parameter, you can set a lambda expression adding as much conditions as you like to build the composed condition, keeping in mind that the logical operator will apply to all equally.

## Build

By last, call `#!csharp Build()` on your rule to finalize building it. Be aware that this method returns `#!csharp RuleBuilderResult`, which may be a success or failure result. If result is success, it will contain the built rule. In case it is a failure, it will contain the errors occurred that led to fail to build the rule.

```csharp
var ruleResult = Rule.Create("Citizen classification for a male adult")
    .InRuleset("Citizen classification")
    .Since(DateTime.Parse("2025-01-01Z"))
    .Until(DateTime.Parse("2026-01-01Z"))
    .SetContent("male adult")
    .ApplyWhen(x => x
        .And(y => y
            .Value("Age", Operators.GreaterThanOrEqual, 18)
            .Value("Gender", Operators.Equal, "male")))
    .Build();

if (ruleResult.IsSuccess)
{
    var rule = ruleResult.Rule;
    // Do something with the built rule
}
else
{
    var errors = ruleResult.Errors;
    // Do something with the errors
}
```

# Adding a rule

The rule engine API exposes the method `#!csharp AddRuleAsync(Rule rule, RuleAddPriorityOption ruleAddPriorityOption)` to add rules to the rules engine. It takes as parameters the rule to add and a `#!csharp RuleAddPriorityOption` that defines the priority configuration to set how the rule will be added considering the existent rules. When adding a new rule, it may be added to a ruleset with pre-existent rules, so the rules engine needs to be told how to handle the new rule priority considering the existent rules' priorities. This is what `#!csharp RuleAddPriorityOption` is for, it defines the priority configuration to add the new rule to the ruleset.

The parameter `ruleAddPriorityOption` can be configured with one of the following options:

- Add the rule with the smallest priority number - 1.

    ```csharp
    RuleAddPriorityOption.AtSmallestNumber
    ```

- Add the rule with the largest priority number - we mean the number next to the largest priority number already existent.

    ```csharp
    RuleAddPriorityOption.AtLargestNumber
    ```

- Add the rule assuming another rule's priority, identified by the `ruleName` given by parameter, which causes that rule and subsequent rules with larger priority to be pushed to a larger priority value by 1.

    ```csharp
    RuleAddPriorityOption.AtRuleName(string ruleName)
    ```

    If a rule name that does not exist is specified, a failure will be returned as result of the adding operation.

- Add the rule assuming a specific priority number, which causes any rule existing on specified priority number and subsequent with larger priority to be pushed to a larger priority value by 1.

    ```csharp
    RuleAddPriorityOption.AtNumber(int number)
    ```

    If a priority number lower than existent rules is specified, Rules Engine will fix it to be the same as the smallest existent priority number (e.g. if -2 is specified, and the existent rule with the smallest priority value is 1, then Rules Engine will fix the value to 1). On the other hand, if a priority number higher than existent rules plus 1 is specified, Rules Engine will fix it to be the same as the largest existent priority plus 1 (e.g. if 30 is specified and the largest existent rule priority is 20, the Rules Engine will fix the value to 21).

When addind a rule to the rules engine, take into consideration that the rules engine will ensure that (per each ruleset):

- Rule name is unique
- Priority value is a unique
- Priority values are continguous

The method `#!csharp AddRuleAsync(Rule rule, RuleAddPriorityOption ruleAddPriorityOption)` ensures those conditions are respected when adding a rule, and thus ensuring rules evaluation works as supposed when requesting rule matches. So, considering a already built rule and Rules Engine, adding a rule is simple as:

```csharp
var ruleResult = Rule.Create("Citizen classification for a male adult")
    .InRuleset("Citizen classification")
    .Since(DateTime.Parse("2025-01-01Z"))
    .Until(DateTime.Parse("2026-01-01Z"))
    .SetContent("male adult")
    .ApplyWhen(x => x
        .And(y => y
            .Value("Age", Operators.GreaterThanOrEqual, 18)
            .Value("Gender", Operators.Equal, "male")))
    .Build();

if (ruleResult.IsSuccess)
{
    var rule = ruleResult.Rule;
    RuleOperationResult ruleOperationResult = await rulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.AtSmallestNumber);

    if (ruleOperationResult.IsSuccess)
    {
        // Rule added successfully, do something if needed
    }
    else
    {
        var errors = ruleOperationResult.Errors;
        // Do something with the errors occurred while adding the rule
    }
}
```

Be sure to check on the returned `ruleOperationResult` if the operation was a success, as it may contain any errors occurred while adding rule if operation was a failure.
