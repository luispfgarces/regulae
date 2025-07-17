namespace Regulae.Management
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Regulae.ConditionNodes;
    using Regulae.Evaluation;
    using Regulae.Source;

    internal sealed class RuleSanitizer : IRuleSanitizer
    {
        private readonly IDataTypesConfigurationProvider dataTypesConfigurationProvider;
        private readonly IRulesSource rulesSource;

        public RuleSanitizer(
            IRulesSource rulesSource,
            IDataTypesConfigurationProvider dataTypesConfigurationProvider)
        {
            this.rulesSource = rulesSource;
            this.dataTypesConfigurationProvider = dataTypesConfigurationProvider;
        }

        public async ValueTask<OperationResult> SanitizeAsync(Rule rule)
        {
            if (rule.RootCondition is not null)
            {
                var errors = new List<string>();
                var conditions = await this.rulesSource.GetConditionsAsync(new GetConditionsArgs()).ConfigureAwait(false);
                SanitizeCondition(rule.RootCondition, conditions, errors);

                if (errors.Count > 0)
                {
                    return OperationResult.Failure(errors);
                }
            }

            return OperationResult.Success();
        }

        private void SanitizeCondition(IConditionNode conditionNode, IReadOnlyDictionary<string, Condition> conditions, List<string> errors)
        {
            if (conditionNode is ComposedConditionNode composedCondition)
            {
                foreach (var childCondition in composedCondition.ChildConditionNodes)
                {
                    SanitizeCondition(childCondition, conditions, errors);
                }
            }
            else
            {
                var valueCondition = (ValueConditionNode)conditionNode;
                if (!conditions.TryGetValue(valueCondition.Condition, out var condition))
                {
                    throw new ArgumentOutOfRangeException(nameof(conditions), $"The given condition with name '{valueCondition.Condition}' does not exist. " +
                        "Please create the condition before using it to evaluate rules.");
                }

                if (valueCondition.RightOperand.DataType != condition.DataType)
                {
                    object converted;
                    if (valueCondition.RightOperand.Cardinality == Cardinalities.One)
                    {
                        var dataTypeConfiguration = this.dataTypesConfigurationProvider.GetDataTypeConfiguration(condition.DataType);

                        try
                        {
                            converted = Convert.ChangeType(valueCondition.RightOperand.Value!
                                ?? dataTypeConfiguration.OneCardinality.Default, dataTypeConfiguration.OneCardinality.Type, CultureInfo.InvariantCulture);

                            valueCondition.RightOperand = new Operand(
                                converted,
                                condition.DataType,
                                Cardinalities.One);
                        }
                        catch (InvalidCastException)
                        {
                            errors.Add($"Condition '{condition.Name}' value '{valueCondition.RightOperand.Value!}' is not convertible to {dataTypeConfiguration.OneCardinality.Type.Name}.");
                        }
                    }
                    else
                    {
                        if (valueCondition.RightOperand.Value is IEnumerable enumerable)
                        {
                            converted = enumerable.Cast<object>();
                            valueCondition.RightOperand = new Operand(
                                converted,
                                condition.DataType,
                                Cardinalities.One);
                        }

                        errors.Add($"Condition '{condition.Name}' value must be of type {nameof(IEnumerable)}.");
                    }
                }
            }
        }
    }
}