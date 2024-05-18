namespace Regulae.Evaluation.Compiled.ExpressionBuilders.StateMachine
{
    using System;
    using Regulae.Evaluation.Compiled.ExpressionBuilders;

    internal interface IExpressionParametersBuilder
    {
        IExpressionReturnBuilder WithoutParameters();

        IExpressionReturnBuilder WithParameters(Action<IExpressionParametersConfiguration> parametersConfigurationAction);
    }
}