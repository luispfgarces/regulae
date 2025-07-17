namespace Regulae.Tests.Source
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Regulae;
    using Regulae.Source;

    internal class StubRulesSourceMiddleware : IRulesSourceMiddleware
    {
        private readonly List<string> middlewareMessages;

        public StubRulesSourceMiddleware(string name, List<string> middlewareMessages)
        {
            this.Name = name;
            this.middlewareMessages = middlewareMessages;
        }

        public int AddRuleCalls { get; private set; }
        public int CreateConditionCalls { get; private set; }
        public int CreateRulesetCalls { get; private set; }
        public int GetConditionsCalls { get; private set; }
        public int GetRulesCalls { get; private set; }
        public int GetRulesetsCalls { get; private set; }
        public int GetRulesFilteredCalls { get; private set; }
        public string Name { get; }
        public int UpdateRulesCalls { get; private set; }

        public async ValueTask HandleAddRuleAsync(
            AddRuleArgs args,
            AddRuleDelegate next)
        {
            this.AddRuleCalls++;
            this.middlewareMessages.Add($"Enter {this.Name}.");
            await next.Invoke(args).ConfigureAwait(false);
            this.middlewareMessages.Add($"Exit {this.Name}.");
        }

        public async ValueTask HandleCreateConditionAsync(CreateConditionArgs args, CreateConditionDelegate next)
        {
            this.CreateConditionCalls++;
            this.middlewareMessages.Add($"Enter {this.Name}.");
            await next.Invoke(args).ConfigureAwait(false);
            this.middlewareMessages.Add($"Exit {this.Name}.");
        }

        public async ValueTask HandleCreateRulesetAsync(
            CreateRulesetArgs args,
            CreateRulesetDelegate next)
        {
            this.CreateRulesetCalls++;
            this.middlewareMessages.Add($"Enter {this.Name}.");
            await next.Invoke(args).ConfigureAwait(false);
            this.middlewareMessages.Add($"Exit {this.Name}.");
        }

        public async ValueTask<IReadOnlyDictionary<string, Condition>> HandleGetConditionsAsync(GetConditionsArgs args, GetConditionsDelegate next)
        {
            this.GetConditionsCalls++;
            this.middlewareMessages.Add($"Enter {this.Name}.");
            var conditions = await next.Invoke(args).ConfigureAwait(false);
            this.middlewareMessages.Add($"Exit {this.Name}.");
            return conditions;
        }

        public async ValueTask<IReadOnlyCollection<Rule>> HandleGetRulesAsync(
            GetRulesArgs args,
            GetRulesDelegate next)
        {
            this.GetRulesCalls++;
            this.middlewareMessages.Add($"Enter {this.Name}.");
            var rules = await next.Invoke(args).ConfigureAwait(false);
            this.middlewareMessages.Add($"Exit {this.Name}.");
            return rules;
        }

        public async ValueTask<IReadOnlyDictionary<string, Ruleset>> HandleGetRulesetsAsync(GetRulesetsArgs args, GetRulesetsDelegate next)
        {
            this.GetRulesetsCalls++;
            this.middlewareMessages.Add($"Enter {this.Name}.");
            var rulesets = await next.Invoke(args).ConfigureAwait(false);
            this.middlewareMessages.Add($"Exit {this.Name}.");
            return rulesets;
        }

        public async ValueTask<IReadOnlyCollection<Rule>> HandleGetRulesFilteredAsync(
            GetRulesFilteredArgs args,
            GetRulesFilteredDelegate next)
        {
            this.GetRulesFilteredCalls++;
            this.middlewareMessages.Add($"Enter {this.Name}.");
            var rules = await next.Invoke(args).ConfigureAwait(false);
            this.middlewareMessages.Add($"Exit {this.Name}.");
            return rules;
        }

        public async ValueTask HandleUpdateRuleAsync(
            UpdateRuleArgs args,
            UpdateRuleDelegate next)
        {
            this.UpdateRulesCalls++;
            this.middlewareMessages.Add($"Enter {this.Name}.");
            await next.Invoke(args).ConfigureAwait(false);
            this.middlewareMessages.Add($"Exit {this.Name}.");
        }
    }
}