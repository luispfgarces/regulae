namespace Regulae.Evaluation.Compiled
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Regulae;
    using Regulae.Core;
    using Regulae.Source;

    internal sealed class CompilationRulesSourceMiddleware : IRulesSourceMiddleware
    {
        private readonly IRuleConditionsExpressionBuilder ruleConditionsExpressionBuilder;
        private readonly IRulesDataSource rulesDataSource;

        public CompilationRulesSourceMiddleware(
            IRuleConditionsExpressionBuilder ruleConditionsExpressionBuilder,
            IRulesDataSource rulesDataSource)
        {
            this.ruleConditionsExpressionBuilder = ruleConditionsExpressionBuilder;
            this.rulesDataSource = rulesDataSource;
        }

        public async ValueTask HandleAddRuleAsync(
            AddRuleArgs args,
            AddRuleDelegate next)
        {
            this.TryCompile(args.Rule);

            await next(args).ConfigureAwait(false);
        }

        public ValueTask HandleCreateConditionAsync(CreateConditionArgs args, CreateConditionDelegate next) => next(args);

        public ValueTask HandleCreateRulesetAsync(CreateRulesetArgs args, CreateRulesetDelegate next) => next(args);

        public ValueTask<IReadOnlyDictionary<string, Condition>> HandleGetConditionsAsync(GetConditionsArgs args, GetConditionsDelegate next) => next(args);

        public async ValueTask<IReadOnlyCollection<Rule>> HandleGetRulesAsync(
            GetRulesArgs args,
            GetRulesDelegate next)
        {
            var rules = await next(args).ConfigureAwait(false);

            foreach (var rule in rules)
            {
                var compiled = this.TryCompile(rule);
                if (compiled)
                {
                    // Commit compilation result to data source, so that next time rule is loaded,
                    // it won't go through the compilation process again.
                    await this.rulesDataSource.UpdateRuleAsync(rule).ConfigureAwait(false);
                }
            }

            return rules;
        }

        public ValueTask<IReadOnlyDictionary<string, Ruleset>> HandleGetRulesetsAsync(GetRulesetsArgs args, GetRulesetsDelegate next) => next(args);

        public async ValueTask<IReadOnlyCollection<Rule>> HandleGetRulesFilteredAsync(
            GetRulesFilteredArgs args,
            GetRulesFilteredDelegate next)
        {
            var rules = await next(args).ConfigureAwait(false);

            foreach (var rule in rules)
            {
                var compiled = this.TryCompile(rule);
                if (compiled)
                {
                    // Commit compilation result to data source, so that next time rule is loaded,
                    // it won't go through the compilation process again.
                    await this.rulesDataSource.UpdateRuleAsync(rule).ConfigureAwait(false);
                }
            }

            return rules;
        }

        public async ValueTask HandleUpdateRuleAsync(
            UpdateRuleArgs args,
            UpdateRuleDelegate next)
        {
            this.TryCompile(args.Rule);

            await next(args).ConfigureAwait(false);
        }

        private bool TryCompile(Rule rule)
        {
            var conditionNode = rule.RootCondition;

            if (conditionNode is { } && (!conditionNode.Properties.TryGetValue(ConditionNodeProperties.CompilationProperties.IsCompiledKey, out var compiledFlag) || !(bool)compiledFlag))
            {
                var matchExpression = this.ruleConditionsExpressionBuilder.BuildExpression(conditionNode, MatchModes.Exact);
                var compiledMatchExpression = matchExpression.Compile();
                conditionNode.Properties[ConditionNodeProperties.CompilationProperties.CompiledMatchDelegateKey] = compiledMatchExpression;
                var searchExpression = this.ruleConditionsExpressionBuilder.BuildExpression(conditionNode, MatchModes.Search);
                var compiledSearchExpression = searchExpression.Compile();
                conditionNode.Properties[ConditionNodeProperties.CompilationProperties.CompiledSearchDelegateKey] = compiledSearchExpression;
                conditionNode.Properties[ConditionNodeProperties.CompilationProperties.IsCompiledKey] = true;
                return true;
            }

            return false;
        }
    }
}