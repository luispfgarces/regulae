
# Activate/Deactivate rules

This page focus on how to activate or deactivate existent rules using Rules.Framework.

Rules activation/deactivation allows you to:

- Activate a rule - a rule will be active and will be considered for evaluation.
- Deactivate a rule - a rule will be inactive and will not be considered for evaluation.

Assuming you have a existent rule instance and a built Rules Engine, you can activate or deactivate by simply:

```csharp
// to activate a rule
RuleOperationResult ruleOperationResult = await rulesEngine.ActivateRuleAsync(rule);

// to deactivate a rule
RuleOperationResult ruleOperationResult = await rulesEngine.DeactivateRuleAsync(rule);
```

Be sure to check on the returned `ruleOperationResult` if the operation was a success, as it may contain any errors occurred while activating/deactivating the rule if operation was a failure.
