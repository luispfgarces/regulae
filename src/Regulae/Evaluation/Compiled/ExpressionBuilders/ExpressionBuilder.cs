namespace Regulae.Evaluation.Compiled.ExpressionBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Regulae.Evaluation.Compiled.ExpressionBuilders.StateMachine;

    internal class ExpressionBuilder : IExpressionParametersBuilder, IExpressionReturnBuilder, IExpressionImplementationBuilder, IConfiguredExpressionBuilder
    {
        private readonly IExpressionBuilderFactory factory;

        public ExpressionBuilder(string name, IExpressionBuilderFactory factory)
        {
            this.Name = name;
            this.factory = factory;
        }

        public static IExpressionBuilderFactory ExpressionBuilderFactory { get; set; } = new DefaultExpressionBuilderFactory();

        public IEnumerable<Expression>? Expressions { get; private set; }

        public IReadOnlyDictionary<string, LabelTarget>? LabelTargets { get; private set; }

        public string Name { get; }

        public IReadOnlyDictionary<string, ParameterExpression>? Parameters { get; private set; }

        public LabelTarget? ReturnLabelTarget { get; private set; }

        public Type? ReturnType { get; private set; }

        public object? ReturnDefaultValue { get; private set; }

        public IReadOnlyDictionary<string, ParameterExpression>? Variables { get; private set; }

        public static IExpressionParametersBuilder NewExpression(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("A non-null, empty, or white-space expression name must be provided.", nameof(name));
            }

            return ExpressionBuilderFactory.CreateExpressionBuilder(name);
        }

        public ExpressionResult Build()
        {
            var variableExpressionsCopy = new List<ParameterExpression>(this.Variables!.Values);
            var bodyBlockExpressionsCopy = new List<Expression>(this.Expressions!)
            {
                Expression.Label(
                    this.ReturnLabelTarget!,
                    Expression.Constant(this.ReturnDefaultValue)),
            };

            var implementationExpression = Expression.Block(variables: variableExpressionsCopy, expressions: bodyBlockExpressionsCopy);

            return new ExpressionResult(
                this.Name,
                implementationExpression,
                this.Parameters!.Values,
                this.ReturnType!);
        }

        public IExpressionImplementationBuilder HavingReturn(Type type, object defaultValue)
        {
            ArgumentNullException.ThrowIfNull(type);

            this.ReturnType = type;
            this.ReturnDefaultValue = defaultValue;
            this.ReturnLabelTarget
                = Expression.Label(type, $"{this.Name}_ReturnLabel");

            return this;
        }

        public IExpressionImplementationBuilder HavingReturn<T>(object defaultValue)
            => this.HavingReturn(typeof(T), defaultValue);

        public IConfiguredExpressionBuilder SetImplementation(
            Action<IExpressionBlockBuilder> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            var expressionConfiguration = new ExpressionConfiguration
            {
                ExpressionName = this.Name!,
                ReturnType = this.ReturnType!,
                ReturnDefaultValue = this.ReturnDefaultValue,
                ReturnLabelTarget = this.ReturnLabelTarget!,
                Parameters = this.Parameters!,
            };

            var implementationExpressionBuilder = this.factory.CreateExpressionBlockBuilder(
                scopeName: string.Empty,
                parent: null!,
                expressionConfiguration: expressionConfiguration);
            builder(implementationExpressionBuilder);

            this.LabelTargets = implementationExpressionBuilder.LabelTargets;
            this.Variables = implementationExpressionBuilder.Variables;
            this.Expressions = implementationExpressionBuilder.Expressions;

            return this;
        }

        public IExpressionReturnBuilder WithoutParameters()
        {
            this.Parameters = new Dictionary<string, ParameterExpression>(StringComparer.Ordinal);

            return this;
        }

        public IExpressionReturnBuilder WithParameters(
            Action<IExpressionParametersConfiguration> parametersConfigurationAction)
        {
            ArgumentNullException.ThrowIfNull(parametersConfigurationAction);

            var expressionBuilderParametersConfiguration = this.factory.CreateExpressionParametersConfiguration();
            parametersConfigurationAction(expressionBuilderParametersConfiguration);

            this.Parameters = expressionBuilderParametersConfiguration.Parameters;

            return this;
        }
    }
}