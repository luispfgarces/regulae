namespace Regulae.Evaluation.Compiled.ExpressionBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal sealed class ExpressionConfiguration
    {
        public required string ExpressionName { get; set; }

        public required IReadOnlyDictionary<string, ParameterExpression> Parameters { get; set; }

        public required object? ReturnDefaultValue { get; set; }

        public required LabelTarget ReturnLabelTarget { get; set; }

        public required Type ReturnType { get; set; }
    }
}