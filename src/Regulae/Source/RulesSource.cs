namespace Regulae.Source
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Regulae;

    internal sealed class RulesSource : IRulesSource
    {
        private readonly AddRuleDelegate addRuleDelegate;
        private readonly CreateConditionDelegate createConditionDelegate;
        private readonly CreateRulesetDelegate createRulesetDelegate;
        private readonly GetConditionsDelegate getConditionsDelegate;
        private readonly GetRulesDelegate getRulesDelegate;
        private readonly GetRulesetsDelegate getRulesetsDelegate;
        private readonly GetRulesFilteredDelegate getRulesFilteredDelegate;
        private readonly UpdateRuleDelegate updateRuleDelegate;

        public RulesSource(
            IRulesDataSource rulesDataSource,
            IEnumerable<IRulesSourceMiddleware> middlewares)
        {
            var middlewaresLinkedList = new LinkedList<IRulesSourceMiddleware>(middlewares);
            this.addRuleDelegate = CreateAddRulePipelineDelegate(rulesDataSource, middlewaresLinkedList);
            this.createConditionDelegate = CreateCreateConditionPipelineDelegate(rulesDataSource, middlewaresLinkedList);
            this.createRulesetDelegate = CreateCreateRulesetPipelineDelegate(rulesDataSource, middlewaresLinkedList);
            this.getConditionsDelegate = CreateGetConditionsPipelineDelegate(rulesDataSource, middlewaresLinkedList);
            this.getRulesetsDelegate = CreateGetRulesetsPipelineDelegate(rulesDataSource, middlewaresLinkedList);
            this.getRulesDelegate = CreateGetRulesPipelineDelegate(rulesDataSource, middlewaresLinkedList);
            this.getRulesFilteredDelegate = CreateGetRulesFilteredPipelineDelegate(rulesDataSource, middlewaresLinkedList);
            this.updateRuleDelegate = CreateUpdateRulePipelineDelegate(rulesDataSource, middlewaresLinkedList);
        }

        public ValueTask AddRuleAsync(AddRuleArgs args)
        {
            return this.addRuleDelegate(args);
        }

        public ValueTask CreateConditionAsync(CreateConditionArgs args)
        {
            return this.createConditionDelegate(args);
        }

        public ValueTask CreateRulesetAsync(CreateRulesetArgs args)
        {
            return this.createRulesetDelegate(args);
        }

        public ValueTask<IReadOnlyDictionary<string, Condition>> GetConditionsAsync(GetConditionsArgs args)
        {
            return this.getConditionsDelegate(args);
        }

        public ValueTask<IReadOnlyCollection<Rule>> GetRulesAsync(GetRulesArgs args)
        {
            return this.getRulesDelegate(args);
        }

        public ValueTask<IReadOnlyDictionary<string, Ruleset>> GetRulesetsAsync(GetRulesetsArgs args)
        {
            return this.getRulesetsDelegate(args);
        }

        public ValueTask<IReadOnlyCollection<Rule>> GetRulesFilteredAsync(GetRulesFilteredArgs args)
        {
            return this.getRulesFilteredDelegate(args);
        }

        public ValueTask UpdateRuleAsync(UpdateRuleArgs args)
        {
            return this.updateRuleDelegate(args);
        }

        private static AddRuleDelegate CreateAddRulePipelineDelegate(
            IRulesDataSource rulesDataSource,
            LinkedList<IRulesSourceMiddleware> middlewares)
        {
            AddRuleDelegate action = (args) => rulesDataSource.AddRuleAsync(args.Rule);

            if (middlewares.Count > 0)
            {
                var middlewareNode = middlewares.Last;

                while (middlewareNode is { })
                {
                    var middleware = middlewareNode.Value;
                    var immutableAction = action;
                    action = (args) => middleware.HandleAddRuleAsync(args, immutableAction);

                    // Get previous middleware node.
                    middlewareNode = middlewareNode.Previous;
                }
            }

            return action;
        }

        private static CreateConditionDelegate CreateCreateConditionPipelineDelegate(
            IRulesDataSource rulesDataSource,
            LinkedList<IRulesSourceMiddleware> middlewares)
        {
            CreateConditionDelegate action = (args) => rulesDataSource.CreateConditionAsync(args.Name, args.DataType);

            if (middlewares.Count > 0)
            {
                var middlewareNode = middlewares.Last;

                while (middlewareNode is { })
                {
                    var middleware = middlewareNode.Value;
                    var immutableAction = action;
                    action = (args) => middleware.HandleCreateConditionAsync(args, immutableAction);

                    // Get previous middleware node.
                    middlewareNode = middlewareNode.Previous;
                }
            }

            return action;
        }

        private static CreateRulesetDelegate CreateCreateRulesetPipelineDelegate(
            IRulesDataSource rulesDataSource,
            LinkedList<IRulesSourceMiddleware> middlewares)
        {
            CreateRulesetDelegate action = (args) => rulesDataSource.CreateRulesetAsync(args.Name);

            if (middlewares.Count > 0)
            {
                var middlewareNode = middlewares.Last;

                while (middlewareNode is { })
                {
                    var middleware = middlewareNode.Value;
                    var immutableAction = action;
                    action = (args) => middleware.HandleCreateRulesetAsync(args, immutableAction);

                    // Get previous middleware node.
                    middlewareNode = middlewareNode.Previous;
                }
            }

            return action;
        }

        private static GetConditionsDelegate CreateGetConditionsPipelineDelegate(
            IRulesDataSource rulesDataSource,
            LinkedList<IRulesSourceMiddleware> middlewares)
        {
            GetConditionsDelegate action = (_) => rulesDataSource.GetConditionsAsync();

            if (middlewares.Count > 0)
            {
                var middlewareNode = middlewares.Last;

                while (middlewareNode is { })
                {
                    var middleware = middlewareNode.Value;
                    var immutableAction = action;
                    action = (args) => middleware.HandleGetConditionsAsync(args, immutableAction);

                    // Get previous middleware node.
                    middlewareNode = middlewareNode.Previous;
                }
            }

            return action;
        }

        private static GetRulesetsDelegate CreateGetRulesetsPipelineDelegate(
            IRulesDataSource rulesDataSource,
            LinkedList<IRulesSourceMiddleware> middlewares)
        {
            GetRulesetsDelegate action = (_) => rulesDataSource.GetRulesetsAsync();

            if (middlewares.Count > 0)
            {
                var middlewareNode = middlewares.Last;

                while (middlewareNode is { })
                {
                    var middleware = middlewareNode.Value;
                    var immutableAction = action;
                    action = (args) => middleware.HandleGetRulesetsAsync(args, immutableAction);

                    // Get previous middleware node.
                    middlewareNode = middlewareNode.Previous;
                }
            }

            return action;
        }

        private static GetRulesFilteredDelegate CreateGetRulesFilteredPipelineDelegate(
            IRulesDataSource rulesDataSource,
            LinkedList<IRulesSourceMiddleware> middlewares)
        {
            GetRulesFilteredDelegate action =
                (args) =>
                {
                    RulesFilterArgs rulesFilterArgs = new()
                    {
                        Ruleset = args.Ruleset,
                        Name = args.Name,
                        Priority = args.Priority,
                    };

                    return rulesDataSource.GetRulesByAsync(rulesFilterArgs);
                };

            if (middlewares.Count > 0)
            {
                var middlewareNode = middlewares.Last;

                while (middlewareNode is { })
                {
                    var middleware = middlewareNode.Value;
                    var immutableAction = action;
                    action = (args) => middleware.HandleGetRulesFilteredAsync(args, immutableAction);

                    // Get previous middleware node.
                    middlewareNode = middlewareNode.Previous;
                }
            }

            return action;
        }

        private static GetRulesDelegate CreateGetRulesPipelineDelegate(
            IRulesDataSource rulesDataSource,
            LinkedList<IRulesSourceMiddleware> middlewares)
        {
            GetRulesDelegate action =
                (args)
                    => rulesDataSource.GetRulesAsync(args.Ruleset, args.DateBegin, args.DateEnd);

            if (middlewares.Count > 0)
            {
                var middlewareNode = middlewares.Last;

                while (middlewareNode is { })
                {
                    var middleware = middlewareNode.Value;
                    var immutableAction = action;
                    action = (args) => middleware.HandleGetRulesAsync(args, immutableAction);

                    // Get previous middleware node.
                    middlewareNode = middlewareNode.Previous;
                }
            }

            return action;
        }

        private static UpdateRuleDelegate CreateUpdateRulePipelineDelegate(
            IRulesDataSource rulesDataSource,
            LinkedList<IRulesSourceMiddleware> middlewares)
        {
            UpdateRuleDelegate action =
                (args) => rulesDataSource.UpdateRuleAsync(args.Rule);

            if (middlewares.Count > 0)
            {
                var middlewareNode = middlewares.Last;

                while (middlewareNode is { })
                {
                    var middleware = middlewareNode.Value;
                    var immutableAction = action;
                    action = (args) => middleware.HandleUpdateRuleAsync(args, immutableAction);

                    // Get previous middleware node.
                    middlewareNode = middlewareNode.Previous;
                }
            }

            return action;
        }
    }
}