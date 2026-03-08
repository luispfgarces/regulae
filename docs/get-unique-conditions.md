This page focus on how to get the condition types associated with rules of a specific ruleset using Regulae.

## The `GetUniqueConditionsTypes` Method

Get the unique condition types associated with rules of a specific ruleset and period.

```csharp
GetUniqueConditionTypesAsync(string ruleset, DateTime dateBegin, DateTime dateEnd)
```
A set of rules is requested to rules data source and all conditions are evaluated against them to provide a set of matches. All rules matching supplied conditions are returned.

```csharp
GetConditionTypes(IEnumerable<Rule<TContentType, TConditionType>> matchedRules)
```
If no conditions are found an empty list is returned