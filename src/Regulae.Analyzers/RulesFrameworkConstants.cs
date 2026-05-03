namespace Regulae.Analyzers
{
    internal static class RulesFrameworkConstants
    {
        public const string AddCondition = "AddCondition";
        public const string And = "And";
        public const string AsComposed = "AsComposed";
        public const string AsValued = "AsValued";
        public const string AtBottom = "AtBottom";
        public const string AtTop = "AtTop";
        public const string Build = "Build";
        public const string ByPriorityNumber = "ByPriorityNumber";
        public const string ByRuleName = "ByRuleName";
        public const string NewRule = "NewRule";
        public const string OfDataType = "OfDataType";
        public const string Or = "Or";
        public const string SetOperand = "SetOperand";
        public const string Value = "Value";
        public const string WithActive = "WithActive";
        public const string WithComparisonOperator = "WithComparisonOperator";
        public const string WithCondition = "WithCondition";
        public const string WithContent = "WithContent";
        public const string WithDate = "WithDate";
        public const string WithDateBegin = "WithDateBegin";
        public const string WithDatesInterval = "WithDatesInterval";
        public const string WithName = "WithName";
        public const string WithLogicalOperator = "WithLogicalOperator";

        public static readonly string[] AsComposedMinimumRequiredMethodPrefixes =
        [
            AsComposed,
            WithLogicalOperator,
            AddCondition,
            Build,
        ];

        public static readonly string[] AsValuedMinimumRequiredMethodPrefixes =
        [
            AsValued,
            OfDataType,
            WithComparisonOperator,
            SetOperand,
            Build,
        ];

        public static readonly string[] FluentConditionBuilderComposedConditionMethods =
        [
            And,
            Or,
        ];

        public static readonly string[] FluentConditionBuilderSupportedMethods =
        [
            And,
            Or,
            Value,
        ];

        public static readonly string[] RuleBuilderMinimumRequiredMethodPrefixes =
        [
            NewRule,
            WithName,
            WithDate,
            WithContent,
            Build,
        ];

        public static readonly string[] RuleBuilderAllSupportedMethods =
        [
            NewRule,
            WithName,
            WithDateBegin,
            WithDatesInterval,
            WithContent,
            WithActive,
            WithCondition,
            Build,
        ];

        public static readonly string[] RuleBuilderTypes =
        [
            "Rules.Framework.Builder.IRuleBuilder<TContentType, TConditionType>",
            "Rules.Framework.RuleBuilder",
        ];

        public static readonly string[] ConditionBuilderTypes =
        [
            "Rules.Framework.Builder.IRootConditionNodeBuilder<TConditionType>",
            "Rules.Framework.Builder.IFluentComposedConditionNodeBuilder<TConditionType>",
            "Rules.Framework.Builder.IConditionNodeBuilder<TConditionType>",
            "Rules.Framework.Builder.IComposedConditionNodeBuilder<TConditionType>",
            "Rules.Framework.Builder.IValueConditionNodeBuilder<TConditionType>",
            "Rules.Framework.Builder.IValueConditionNodeBuilder<TConditionType, TDataType>",
        ];

        public static readonly string[] SupportedWithConditionParameterTypesForArityOne =
        [
            "System.Func<Rules.Framework.Builder.IRootConditionNodeBuilder<TConditionType>, Rules.Framework.Core.IConditionNode<TConditionType>>",
            "System.Func<Rules.Framework.Builder.IConditionNodeBuilder<TConditionType>, Rules.Framework.Core.IConditionNode<TConditionType>>",
        ];
    }
}
