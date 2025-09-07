namespace Regulae.Builder.Validation
{
    using FluentValidation.Results;
    using Regulae;
    using Regulae.ConditionNodes;
    using Regulae.Generic;
    using Regulae.Generic.ConditionNodes;

    internal static class ConditionNodeValidationExtensions
    {
        public static void PerformValidation<TCondition, TValidationContext>(this IConditionNode<TCondition> conditionNode, GenericConditionNodeValidationArgs<TCondition, TValidationContext> conditionNodeValidationArgs)
            where TCondition : notnull
        {
            ValidationResult validationResult;
            switch (conditionNode)
            {
                case ComposedConditionNode<TCondition> composedConditionNode:
                    validationResult = conditionNodeValidationArgs.ComposedConditionNodeValidator.Validate(composedConditionNode);
                    break;

                case null:
                    return;

                default:
                    validationResult = conditionNodeValidationArgs.ValueConditionNodeValidator.Validate((ValueConditionNode<TCondition>)conditionNode);
                    break;
            }

            if (!validationResult.IsValid)
            {
                foreach (var validationFailure in validationResult.Errors)
                {
                    conditionNodeValidationArgs.ValidationContext.AddFailure(validationFailure);
                }
            }
        }

        public static void PerformValidation<TValidationContext>(this IConditionNode conditionNode, ConditionNodeValidationArgs<TValidationContext> conditionNodeValidationArgs)
        {
            ValidationResult validationResult;
            switch (conditionNode)
            {
                case ComposedConditionNode composedConditionNode:
                    validationResult = conditionNodeValidationArgs.ComposedConditionNodeValidator.Validate(composedConditionNode);
                    break;

                case null:
                    return;

                default:
                    validationResult = conditionNodeValidationArgs.ValueConditionNodeValidator.Validate((ValueConditionNode)conditionNode);
                    break;
            }

            if (!validationResult.IsValid)
            {
                foreach (var validationFailure in validationResult.Errors)
                {
                    conditionNodeValidationArgs.ValidationContext.AddFailure(validationFailure);
                }
            }
        }
    }
}