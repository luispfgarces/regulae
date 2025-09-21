namespace Regulae.Cache
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Regulae.Source;

    internal sealed class CacheRulesSourceMiddleware : IRulesSourceMiddleware
    {
        private const string ConditionsAllCacheKey = "Regulae:Conditions:All";
        private const string RulesAllCacheKey = "Regulae:Rules:All";
        private const string RulesCacheKeyPrefix = "Regulae:Rules:{0}";
        private const string RulesetsAllCacheKey = "Regulae:Rulesets:All";
        private readonly ICache cache;

        public CacheRulesSourceMiddleware(ICache cache)
        {
            this.cache = cache;
        }

        public async ValueTask HandleAddRuleAsync(AddRuleArgs args, AddRuleDelegate next)
        {
            await next(args).ConfigureAwait(false);

            this.cache.Evict(RulesAllCacheKey);
            this.cache.EvictMany(GetRulesByRulesetCachekeyPrefix(args.Rule.Ruleset));
        }

        public async ValueTask HandleCreateConditionAsync(CreateConditionArgs args, CreateConditionDelegate next)
        {
            await next(args).ConfigureAwait(false);

            this.cache.Evict(ConditionsAllCacheKey);
        }

        public async ValueTask HandleCreateRulesetAsync(CreateRulesetArgs args, CreateRulesetDelegate next)
        {
            await next(args).ConfigureAwait(false);

            this.cache.Evict(RulesetsAllCacheKey);
        }

        public async ValueTask<IReadOnlyDictionary<string, Condition>> HandleGetConditionsAsync(GetConditionsArgs args, GetConditionsDelegate next)
        {
            if (this.cache.TryGet(ConditionsAllCacheKey, out var cachedConditions))
            {
                return (IReadOnlyDictionary<string, Condition>)cachedConditions;
            }

            var conditions = await next(args).ConfigureAwait(false);
            this.cache.Set(ConditionsAllCacheKey, conditions);
            return conditions;
        }

        public async ValueTask<IReadOnlyCollection<Rule>> HandleGetRulesAsync(GetRulesArgs args, GetRulesDelegate next)
        {
            var dateBegin = args.DateBegin;
            var dateEnd = args.DateEnd;
            var cacheKey = string.Concat(
                RulesCacheKeyPrefix, args.Ruleset, "_",
                dateBegin.Year, dateBegin.Month, dateBegin.Day, "_",
                dateEnd.Year, dateEnd.Month, dateEnd.Day);

            if (this.cache.TryGet(cacheKey, out var cacheValue))
            {
                return CloneCachedRules((IReadOnlyCollection<Rule>)cacheValue);
            }

            var rules = await next(args).ConfigureAwait(false);
            this.cache.Set(cacheKey, rules);
            return rules;
        }

        public async ValueTask<IReadOnlyDictionary<string, Ruleset>> HandleGetRulesetsAsync(GetRulesetsArgs args, GetRulesetsDelegate next)
        {
            if (this.cache.TryGet(RulesetsAllCacheKey, out var cachedRulesets))
            {
                return (IReadOnlyDictionary<string, Ruleset>)cachedRulesets;
            }

            var rulesets = await next(args).ConfigureAwait(false);
            this.cache.Set(RulesetsAllCacheKey, rulesets);
            return rulesets;
        }

        public ValueTask<IReadOnlyCollection<Rule>> HandleGetRulesFilteredAsync(GetRulesFilteredArgs args, GetRulesFilteredDelegate next)
            => next(args);

        public async ValueTask HandleUpdateRuleAsync(UpdateRuleArgs args, UpdateRuleDelegate next)
        {
            await next(args).ConfigureAwait(false);

            this.cache.Evict(RulesAllCacheKey);
            this.cache.EvictMany(GetRulesByRulesetCachekeyPrefix(args.Rule.Ruleset));
        }

        private static Rule[] CloneCachedRules(IReadOnlyCollection<Rule> rulesFromCache)
        {
            var cachedRules = new Rule[rulesFromCache.Count];
            var i = 0;
            foreach (var rule in rulesFromCache)
            {
                cachedRules[i++] = rule.Clone();
            }

            return cachedRules;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetRulesByRulesetCachekeyPrefix(string ruleset)
            => string.Concat("Regulae:Rules:", ruleset);
    }
}