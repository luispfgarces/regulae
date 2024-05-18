namespace Regulae.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Regulae;
    using Regulae.Evaluation;
    using Regulae.Evaluation.Compiled;
    using Regulae.Evaluation.Compiled.ConditionBuilders;
    using Regulae.Evaluation.Interpreted;
    using Regulae.Evaluation.Interpreted.ValueEvaluation;
    using Regulae.Evaluation.Interpreted.ValueEvaluation.Dispatchers;
    using Regulae.Source;
    using Regulae.Validation;

    internal sealed class ConfiguredRulesEngineBuilder : IConfiguredRulesEngineBuilder
    {
        private readonly IRulesDataSource rulesDataSource;
        private readonly RulesEngineOptions rulesEngineOptions;

        public ConfiguredRulesEngineBuilder(IRulesDataSource rulesDataSource)
        {
            this.rulesDataSource = rulesDataSource;
            this.rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
        }

        public IRulesEngine Build()
        {
            var rulesSourceMiddlewares = new List<IRulesSourceMiddleware>();
            var dataTypesConfigurationProvider = new DataTypesConfigurationProvider(this.rulesEngineOptions);
            var multiplicityEvaluator = new MultiplicityEvaluator();
            var conditionsTreeAnalyzer = new ConditionsTreeAnalyzer();

            IConditionsEvalEngine conditionsEvalEngine;

            if (this.rulesEngineOptions.EnableCompilation)
            {
                // Use specific conditions eval engine to use compiled parts of conditions tree.
                var conditionExpressionBuilderProvider = new ConditionExpressionBuilderProvider();
                var valueConditionNodeCompilerProvider = new ValueConditionNodeExpressionBuilderProvider(conditionExpressionBuilderProvider);
                var ruleConditionsExpressionBuilder = new RuleConditionsExpressionBuilder(valueConditionNodeCompilerProvider, dataTypesConfigurationProvider);
                conditionsEvalEngine = new CompiledConditionsEvalEngine(conditionsTreeAnalyzer, this.rulesEngineOptions);

                // Add conditions compiler middleware to ensure compilation occurs before rules
                // engine uses the rules, while also ensuring that the compilation result is kept on
                // data source (avoiding future re-compilation).
                var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware(ruleConditionsExpressionBuilder, this.rulesDataSource);
                rulesSourceMiddlewares.Add(compilationRulesSourceMiddleware);
            }
            else
            {
                // Use interpreted conditions eval engine that runs throught all conditions and
                // interprets them at each evaluation.
                var operatorEvalStrategyFactory = new OperatorEvalStrategyFactory();
                var conditionEvalDispatchProvider = new ConditionEvalDispatchProvider(operatorEvalStrategyFactory, multiplicityEvaluator, dataTypesConfigurationProvider);
                var deferredEval = new DeferredEval(conditionEvalDispatchProvider, this.rulesEngineOptions);
                conditionsEvalEngine = new InterpretedConditionsEvalEngine(deferredEval, conditionsTreeAnalyzer);
            }

            var ruleConditionsExtractor = new RuleConditionsExtractor();

            var validationProvider = ValidationProvider.New()
                .MapValidatorFor(new SearchArgsValidator<string, string>());

            var orderedMiddlewares = rulesSourceMiddlewares
                .Reverse<IRulesSourceMiddleware>();
            var rulesSource = new RulesSource(this.rulesDataSource, orderedMiddlewares);

            return new RulesEngine(conditionsEvalEngine, rulesSource, validationProvider, this.rulesEngineOptions, ruleConditionsExtractor);
        }

        public IConfiguredRulesEngineBuilder Configure(Action<RulesEngineOptions> configurationAction)
        {
            configurationAction.Invoke(this.rulesEngineOptions);
            RulesEngineOptionsValidator.Validate(this.rulesEngineOptions);

            return this;
        }
    }
}