namespace Regulae.IntegrationTests.Common.Scenarios
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using Newtonsoft.Json;
    using Regulae;
    using Regulae.Builder.Generic.RulesBuilder;
    using Regulae.Generic;
    using Regulae.IntegrationTests.Common.Scenarios.DataSource;

    internal class RulesFromJsonFile
    {
        private static readonly RulesFromJsonFile instance = new();

        public static RulesFromJsonFile Load => instance;

        public IEnumerable<Rule<TRuleset, TCondition>> FromJsonFile<TRuleset, TCondition>(string filePath, Type contentRuntimeType, bool serializedContent = true)
            where TRuleset : notnull, new()
            where TCondition : notnull
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var streamReader = new StreamReader(fileStream);
            var contents = streamReader.ReadToEnd();
            var ruleDataModels = JsonConvert.DeserializeObject<IEnumerable<RuleDataModel>>(contents)!;
            var addedRules = new List<Rule<TRuleset, TCondition>>();

            foreach (var ruleDataModel in ruleDataModels)
            {
                if (ruleDataModel.Name is null)
                {
                    throw new InvalidDataException("Rule name is not defined.");
                }

                if (ruleDataModel.Ruleset is null)
                {
                    throw new InvalidDataException($"Ruleset is not defined for rule '{ruleDataModel.Name}'.");
                }

                if (ruleDataModel.Content is null)
                {
                    throw new InvalidDataException($"Content is not defined for rule '{ruleDataModel.Name}'.");
                }

                var ruleset = GetRuleset<TRuleset>(ruleDataModel.Ruleset);
                object content;
                if (serializedContent)
                {
                    content = JsonConvert.DeserializeObject(ruleDataModel.Content, contentRuntimeType)!;
                }
                else
                {
                    content = Parse(ruleDataModel.Content, contentRuntimeType);
                }

                var ruleBuilder = Rule.Create<TRuleset, TCondition>(ruleDataModel.Name)
                    .InRuleset(ruleset)
                    .SetContent(content)
                    .Since(ruleDataModel.DateBegin)
                    .Until(ruleDataModel.DateEnd);

                if (ruleDataModel.RootCondition is { })
                {
                    ruleBuilder.ApplyWhen(b => this.ConvertConditionNode(b, ruleDataModel.RootCondition, ruleDataModel.Name));
                }
                var ruleBuilderResult = ruleBuilder.Build();

                if (ruleBuilderResult.IsSuccess)
                {
                    var rule = ruleBuilderResult.Rule;
                    addedRules.Add(rule);
                }
                else
                {
                    throw new InvalidRuleException($"Loaded invalid rule from file. Rule name: {ruleDataModel.Name}");
                }
            }

            return addedRules;
        }

        private static IFluentConditionNodeBuilder<TCondition> CreateValueConditionNode<TCondition>(IFluentConditionNodeBuilder<TCondition> conditionNodeBuilder, ConditionNodeDataModel conditionNodeDataModel, string ruleName)
        {
            if (conditionNodeDataModel.DataType is null)
            {
                throw new InvalidDataException($"Data type is not defined for value condition node on rule '{ruleName}'.");
            }

            if (conditionNodeDataModel.Condition is null)
            {
                throw new InvalidDataException($"Condition is not defined for value condition node on rule '{ruleName}'.");
            }

            if (conditionNodeDataModel.Operator is null)
            {
                throw new InvalidDataException($"Operator is not defined for value condition node on rule '{ruleName}'.");
            }

            var dataType = Parse<DataTypes>(conditionNodeDataModel.DataType);
            var condition = Parse<TCondition>(conditionNodeDataModel.Condition);
            var @operator = Parse<Operators>(conditionNodeDataModel.Operator);

            return dataType switch
            {
                DataTypes.Integer => conditionNodeBuilder.Value(
                    condition,
                    @operator,
                    Convert.ToInt32(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture)),
                DataTypes.Decimal => conditionNodeBuilder.Value(
                    condition,
                    @operator,
                    Convert.ToDecimal(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture)),
                DataTypes.String => conditionNodeBuilder.Value(
                    condition,
                    @operator,
                    conditionNodeDataModel.Operand),
                DataTypes.Boolean => conditionNodeBuilder.Value(
                    condition,
                    @operator,
                    Convert.ToBoolean(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture)),
                _ => throw new NotSupportedException($"Unsupported data type: {dataType}."),
            };
        }

        private static IConditionNode CreateValueConditionNode<TCondition>(IRootConditionNodeBuilder<TCondition> conditionNodeBuilder, ConditionNodeDataModel conditionNodeDataModel, string ruleName)
        {
            if (conditionNodeDataModel.DataType is null)
            {
                throw new InvalidDataException($"Data type is not defined for value condition node on rule '{ruleName}'.");
            }

            if (conditionNodeDataModel.Condition is null)
            {
                throw new InvalidDataException($"Condition is not defined for value condition node on rule '{ruleName}'.");
            }

            if (conditionNodeDataModel.Operator is null)
            {
                throw new InvalidDataException($"Operator is not defined for value condition node on rule '{ruleName}'.");
            }

            var dataType = Parse<DataTypes>(conditionNodeDataModel.DataType);
            var condition = Parse<TCondition>(conditionNodeDataModel.Condition);
            var @operator = Parse<Operators>(conditionNodeDataModel.Operator);

            return dataType switch
            {
                DataTypes.Integer => conditionNodeBuilder.Value(
                    condition,
                    @operator,
                    Convert.ToInt32(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture)),
                DataTypes.Decimal => conditionNodeBuilder.Value(
                    condition,
                    @operator,
                    Convert.ToDecimal(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture)),
                DataTypes.String => conditionNodeBuilder.Value(
                    condition,
                    @operator,
                    conditionNodeDataModel.Operand),
                DataTypes.Boolean => conditionNodeBuilder.Value(
                    condition,
                    @operator,
                    Convert.ToBoolean(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture)),
                _ => throw new NotSupportedException($"Unsupported data type: {dataType}."),
            };
        }

        private static TRuleset GetRuleset<TRuleset>(string ruleset) where TRuleset : new()
            => Parse<TRuleset>(ruleset);

        private static T Parse<T>(string value)
            => (T)Parse(value, typeof(T));

        private static object Parse(string value, Type type)
            => type.IsEnum ? Enum.Parse(type, value) : Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

        private IConditionNode ConvertConditionNode<TCondition>(IRootConditionNodeBuilder<TCondition> conditionNodeBuilder, ConditionNodeDataModel conditionNodeDataModel, string ruleName)
        {
            if (conditionNodeDataModel.LogicalOperator is null)
            {
                throw new InvalidDataException($"Logical operator is not defined for condition node on rule '{ruleName}'.");
            }

            var logicalOperator = Parse<LogicalOperators>(conditionNodeDataModel.LogicalOperator);

            return logicalOperator switch
            {
                LogicalOperators.And => conditionNodeBuilder.And(b => HandleChildConditionNodes(b, conditionNodeDataModel, ruleName)),
                LogicalOperators.Or => conditionNodeBuilder.Or(b => HandleChildConditionNodes(b, conditionNodeDataModel, ruleName)),
                LogicalOperators.Eval => CreateValueConditionNode(conditionNodeBuilder, conditionNodeDataModel, ruleName),
                _ => throw new NotSupportedException($"The logical operator '{logicalOperator}' is not supported."),
            };
        }

        private IFluentConditionNodeBuilder<TCondition> ConvertConditionNode<TCondition>(IFluentConditionNodeBuilder<TCondition> conditionNodeBuilder, ConditionNodeDataModel conditionNodeDataModel, string ruleName)
        {
            if (conditionNodeDataModel.LogicalOperator is null)
            {
                throw new InvalidDataException($"Logical operator is not defined for condition node on rule '{ruleName}'.");
            }

            var logicalOperator = Parse<LogicalOperators>(conditionNodeDataModel.LogicalOperator);

            return logicalOperator switch
            {
                LogicalOperators.And => conditionNodeBuilder.And(b => HandleChildConditionNodes(b, conditionNodeDataModel, ruleName)),
                LogicalOperators.Or => conditionNodeBuilder.Or(b => HandleChildConditionNodes(b, conditionNodeDataModel, ruleName)),
                LogicalOperators.Eval => CreateValueConditionNode(conditionNodeBuilder, conditionNodeDataModel, ruleName),
                _ => throw new NotSupportedException($"The logical operator '{logicalOperator}' is not supported."),
            };
        }

        private IFluentConditionNodeBuilder<TCondition> HandleChildConditionNodes<TCondition>(IFluentConditionNodeBuilder<TCondition> conditionNodeBuilder, ConditionNodeDataModel conditionNodeDataModel, string ruleName)
        {
            if (conditionNodeDataModel.ChildConditionNodes is null || !conditionNodeDataModel.ChildConditionNodes.Any())
            {
                throw new InvalidDataException($"Child condition nodes are not defined for logical operator node on rule '{ruleName}'.");
            }

            foreach (var child in conditionNodeDataModel.ChildConditionNodes)
            {
                this.ConvertConditionNode(conditionNodeBuilder, child, ruleName);
            }

            return conditionNodeBuilder;
        }
    }
}