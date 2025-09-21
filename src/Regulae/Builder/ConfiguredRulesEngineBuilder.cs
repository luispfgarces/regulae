namespace Regulae.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Regulae;
    using Regulae.Builder.Validation;
    using Regulae.Cache;
    using Regulae.Core;
    using Regulae.Evaluation;
    using Regulae.Evaluation.Compiled;
    using Regulae.Evaluation.Compiled.ConditionBuilders;
    using Regulae.Evaluation.Interpreted;
    using Regulae.Evaluation.Interpreted.ValueEvaluation;
    using Regulae.Evaluation.Interpreted.ValueEvaluation.Dispatchers;
    using Regulae.Management;
    using Regulae.Source;
    using Regulae.Validation;

    internal sealed class ConfiguredRulesEngineBuilder : IConfiguredRulesEngineBuilder
    {
        private readonly IRulesDataSource rulesDataSource;
        private readonly RulesEngineOptionsBuilder rulesEngineOptionsBuilder;

        public ConfiguredRulesEngineBuilder(IRulesDataSource rulesDataSource)
        {
            this.rulesDataSource = rulesDataSource;
            this.rulesEngineOptionsBuilder = new RulesEngineOptionsBuilder();
        }

        public IRulesEngine Build()
        {
            var rulesEngineOptions = this.rulesEngineOptionsBuilder.Build();
            var rulesSourceMiddlewares = new List<IRulesSourceMiddleware>();
            var dataTypesConfigurationProvider = new DataTypesConfigurationProvider(rulesEngineOptions);
            var multiplicityEvaluator = new MultiplicityEvaluator();
            var conditionsTreeAnalyzer = new ConditionsTreeAnalyzer();

            IConditionsEvalEngine conditionsEvalEngine;
            switch (rulesEngineOptions.EvaluationStrategy)
            {
                case EvaluationStrategies.Compiled:
                    var conditionExpressionBuilderProvider = new ConditionExpressionBuilderProvider();
                    var valueConditionNodeCompilerProvider = new ValueConditionNodeExpressionBuilderProvider(conditionExpressionBuilderProvider);
                    var ruleConditionsExpressionBuilder = new RuleConditionsExpressionBuilder(valueConditionNodeCompilerProvider, dataTypesConfigurationProvider, rulesEngineOptions);
                    conditionsEvalEngine = new CompiledConditionsEvalEngine(conditionsTreeAnalyzer, rulesEngineOptions);
                    var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware(ruleConditionsExpressionBuilder, this.rulesDataSource);
                    rulesSourceMiddlewares.Add(compilationRulesSourceMiddleware);
                    break;

                default:
                    var operatorEvalStrategyFactory = new OperatorEvalStrategyFactory();
                    var conditionEvalDispatchProvider = new ConditionEvalDispatchProvider(operatorEvalStrategyFactory, multiplicityEvaluator, dataTypesConfigurationProvider);
                    conditionsEvalEngine = new InterpretedConditionsEvalEngine(conditionEvalDispatchProvider, conditionsTreeAnalyzer, rulesEngineOptions);
                    break;
            }

            if (rulesEngineOptions.Cache is not null)
            {
                var cacheRulesSourceMiddleware = new CacheRulesSourceMiddleware(rulesEngineOptions.Cache);
                rulesSourceMiddlewares.Add(cacheRulesSourceMiddleware);
            }

            var ruleConditionsExtractor = new RuleConditionsExtractor();

            var orderedMiddlewares = rulesSourceMiddlewares
                .Reverse<IRulesSourceMiddleware>();
            var rulesSource = new RulesSource(this.rulesDataSource, orderedMiddlewares);
            var ruleSanitizer = new RuleSanitizer(rulesSource, dataTypesConfigurationProvider);
            var conditionsConverter = new ConditionsConverter(rulesSource);
            var validationProvider = ValidationProvider.New()
                .MapValidatorFor(new SearchArgsValidator<string, string>())
                .MapValidatorFor(new RuleValidator(rulesSource, rulesEngineOptions))
                .MapValidatorFor(new RuleAddPriorityOptionValidator(rulesSource));
            var addRuleController = new AddRuleController(ruleSanitizer, rulesSource, validationProvider);
            var updateRuleController = new UpdateRuleController(rulesSource, validationProvider);
            var rulesEngineArgs = new RulesEngineArgs
            {
                AddRuleController = addRuleController,
                ConditionsConverter = conditionsConverter,
                ConditionsEvalEngine = conditionsEvalEngine,
                RuleConditionsExtractor = ruleConditionsExtractor,
                RulesEngineOptions = rulesEngineOptions,
                RulesSource = rulesSource,
                ValidatorProvider = validationProvider,
                UpdateRuleController = updateRuleController,
            };

            return new RulesEngine(rulesEngineArgs);
        }

        public IConfiguredRulesEngineBuilder Configure(Action<IRulesEngineConfiguration> configurationAction)
        {
            configurationAction(this.rulesEngineOptionsBuilder);
            return this;
        }
    }
}