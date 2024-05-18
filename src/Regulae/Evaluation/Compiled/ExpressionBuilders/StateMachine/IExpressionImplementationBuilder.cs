namespace Regulae.Evaluation.Compiled.ExpressionBuilders.StateMachine
{
    using System;
    using Regulae.Evaluation.Compiled.ExpressionBuilders;

    internal interface IExpressionImplementationBuilder
    {
        IConfiguredExpressionBuilder SetImplementation(Action<IExpressionBlockBuilder> builder);
    }
}