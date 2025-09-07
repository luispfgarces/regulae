namespace Regulae.Evaluation.Compiled.ExpressionBuilders
{
    using System;
    using Regulae.Evaluation.Compiled.ExpressionBuilders.StateMachine;

    internal class DefaultExpressionBuilderFactory : IExpressionBuilderFactory
    {
        public IExpressionBlockBuilder CreateExpressionBlockBuilder(
            string scopeName,
            IExpressionBlockBuilder parent,
            ExpressionConfiguration expressionConfiguration)
        {
            ArgumentNullException.ThrowIfNull(expressionConfiguration);

            return new ExpressionBlockBuilder(scopeName, parent, this, expressionConfiguration);
        }

        public IExpressionParametersBuilder CreateExpressionBuilder(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return new ExpressionBuilder(name, this);
        }

        public IExpressionParametersConfiguration CreateExpressionParametersConfiguration()
        {
            return new ExpressionParametersConfiguration();
        }

        public IExpressionSwitchBuilder CreateExpressionSwitchBuilder(
            IExpressionBlockBuilder parent)
        {
            if (parent is null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            return new ExpressionSwitchBuilder(parent);
        }
    }
}