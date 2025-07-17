namespace Regulae
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using Regulae.Builder.Validation;
    using Regulae.Core;
    using Regulae.Evaluation;
    using Regulae.Extensions;
    using Regulae.Management;
    using Regulae.Source;
    using Regulae.Validation;

    /// <summary>
    /// Exposes rules engine logic to provide rule matches to requests.
    /// </summary>
    public class RulesEngine : IRulesEngine
    {
        private readonly IConditionsConverter conditionsConverter;
        private readonly IConditionsEvalEngine conditionsEvalEngine;
        private readonly IRuleConditionsExtractor ruleConditionsExtractor;
        private readonly IRuleSanitizer ruleSanitizer;
        private readonly IRulesSource rulesSource;
        private readonly RuleValidator ruleValidator = RuleValidator.Instance;
        private readonly IValidatorProvider validatorProvider;

        internal RulesEngine(
            RulesEngineArgs rulesEngineArgs)
        {
            this.conditionsConverter = rulesEngineArgs.ConditionsConverter;
            this.conditionsEvalEngine = rulesEngineArgs.ConditionsEvalEngine;
            this.rulesSource = rulesEngineArgs.RulesSource;
            this.validatorProvider = rulesEngineArgs.ValidatorProvider;
            this.Options = rulesEngineArgs.RulesEngineOptions;
            this.ruleConditionsExtractor = rulesEngineArgs.RuleConditionsExtractor;
            this.ruleSanitizer = rulesEngineArgs.RuleSanitizer;
        }

        /// <inheritdoc/>
        public IRulesEngineOptions Options { get; }

        /// <inheritdoc/>
        public Task<OperationResult> ActivateRuleAsync(Rule rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            rule.Active = true;

            return this.UpdateRuleInternalAsync(rule).AsTask();
        }

        /// <inheritdoc/>
        public Task<OperationResult> AddRuleAsync(Rule rule, RuleAddPriorityOption ruleAddPriorityOption)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            if (ruleAddPriorityOption is null)
            {
                throw new ArgumentNullException(nameof(ruleAddPriorityOption));
            }

            return this.AddRuleInternalAsync(rule, ruleAddPriorityOption).AsTask();
        }

        /// <inheritdoc/>
        public async Task<OperationResult> CreateConditionAsync(string name, DataTypes dataType)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return OperationResult.Failure("A condition must have a non-null, blank, or whitespace name.");
            }

            var args = new CreateConditionArgs
            {
                DataType = dataType,
                Name = name,
            };

            await this.rulesSource.CreateConditionAsync(args).ConfigureAwait(false);
            return OperationResult.Success();
        }

        /// <inheritdoc/>
        public async Task<OperationResult> CreateRulesetAsync(string ruleset)
        {
            if (string.IsNullOrWhiteSpace(ruleset))
            {
                throw new ArgumentNullException(nameof(ruleset));
            }

            var getRulesetArgs = new GetRulesetsArgs();
            var existentRulesets = await this.rulesSource.GetRulesetsAsync(getRulesetArgs).ConfigureAwait(false);
            if (existentRulesets.ContainsKey(ruleset))
            {
                return OperationResult.Failure($"The ruleset '{ruleset}' already exists.");
            }

            return await this.CreateRulesetInternalAsync(ruleset).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<OperationResult> DeactivateRuleAsync(Rule rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            rule.Active = false;

            return this.UpdateRuleInternalAsync(rule).AsTask();
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyDictionary<string, Condition>> GetConditionsAsync()
        {
            return await this.rulesSource.GetConditionsAsync(new GetConditionsArgs()).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyDictionary<string, Ruleset>> GetRulesetsAsync()
        {
            return this.rulesSource.GetRulesetsAsync(new GetRulesetsArgs()).AsTask();
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyCollection<string>> GetUniqueConditionsAsync(string ruleset, DateTime dateBegin, DateTime dateEnd)
        {
            if (string.IsNullOrWhiteSpace(ruleset))
            {
                throw new ArgumentNullException(nameof(ruleset));
            }

            var getRulesArgs = new GetRulesArgs
            {
                DateBegin = dateBegin,
                DateEnd = dateEnd,
                Ruleset = ruleset,
            };

            var rules = await this.rulesSource.GetRulesAsync(getRulesArgs).ConfigureAwait(false);

            return this.ruleConditionsExtractor.GetConditions(rules);
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyCollection<Rule>> MatchManyAsync(
            string ruleset,
            DateTime matchDateTime,
            IDictionary<string, object> conditions)
        {
            if (string.IsNullOrWhiteSpace(ruleset))
            {
                throw new ArgumentNullException(nameof(ruleset));
            }

            var evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = false,
                MatchMode = MatchModes.Exact,
            };

            var getRulesArgs = new GetRulesArgs
            {
                DateBegin = matchDateTime,
                DateEnd = matchDateTime,
                Ruleset = ruleset,
            };

            var orderedRules = await this.GetRulesAsync(getRulesArgs).ConfigureAwait(false);
            var conditionsAsOperands = await this.conditionsConverter.ConvertConditionsAsync(conditions).ConfigureAwait(false);
            return this.EvalAll(orderedRules, evaluationOptions, conditionsAsOperands, active: true);
        }

        /// <inheritdoc/>
        public async Task<Rule> MatchOneAsync(
            string ruleset,
            DateTime matchDateTime,
            IDictionary<string, object> conditions)
        {
            if (string.IsNullOrWhiteSpace(ruleset))
            {
                throw new ArgumentNullException(nameof(ruleset));
            }

            var evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = false,
                MatchMode = MatchModes.Exact,
            };

            var getRulesArgs = new GetRulesArgs
            {
                DateBegin = matchDateTime,
                DateEnd = matchDateTime,
                Ruleset = ruleset,
            };

            var orderedRules = await this.GetRulesAsync(getRulesArgs).ConfigureAwait(false);
            var conditionsAsOperands = await this.conditionsConverter.ConvertConditionsAsync(conditions).ConfigureAwait(false);

            return this.Options.PriorityCriteria == PriorityCriterias.SmallestNumber
                ? EvalOneTraverse(orderedRules, evaluationOptions, conditionsAsOperands, active: true)
                : EvalOneReverse(orderedRules, evaluationOptions, conditionsAsOperands, active: true);
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyCollection<Rule>> SearchAsync(SearchArgs<string, string> searchArgs)
        {
            if (searchArgs is null)
            {
                throw new ArgumentNullException(nameof(searchArgs));
            }

            var validator = this.validatorProvider.GetValidatorFor<SearchArgs<string, string>>();
            var validationResult = await validator.ValidateAsync(searchArgs).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                var stringBuilder = new StringBuilder()
                    .AppendFormat(CultureInfo.InvariantCulture, "Specified '{0}' with invalid search values:", nameof(searchArgs))
                    .AppendLine();

                foreach (var validationFailure in validationResult.Errors)
                {
                    stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "> {0}", validationFailure.ErrorMessage)
                        .AppendLine();
                }

                throw new ArgumentException(stringBuilder.ToString(), nameof(searchArgs));
            }

            var evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = searchArgs.ExcludeRulesWithoutSearchConditions,
                MatchMode = MatchModes.Search,
            };

            var getRulesArgs = new GetRulesArgs
            {
                DateBegin = searchArgs.DateBegin,
                DateEnd = searchArgs.DateEnd,
                Ruleset = searchArgs.Ruleset,
            };

            var orderedRules = await this.GetRulesAsync(getRulesArgs).ConfigureAwait(false);
            var conditionsAsOperands = await this.conditionsConverter.ConvertConditionsAsync(searchArgs.Conditions).ConfigureAwait(false);
            return this.EvalAll(orderedRules, evaluationOptions, conditionsAsOperands, searchArgs.Active.GetValueOrDefault(defaultValue: true));
        }

        /// <inheritdoc/>
        public Task<OperationResult> UpdateRuleAsync(Rule rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            return this.UpdateRuleInternalAsync(rule).AsTask();
        }

        private async ValueTask<OperationResult> AddRuleInternalAsync(Rule rule, RuleAddPriorityOption ruleAddPriorityOption)
        {
            var errors = new List<string>();
            var rulesets = await this.rulesSource.GetRulesetsAsync(new GetRulesetsArgs()).ConfigureAwait(false);

            if (!rulesets.ContainsKey(rule.Ruleset))
            {
                if (!this.Options.AutoCreateRulesets)
                {
                    errors.Add($"Specified ruleset '{rule.Ruleset}' does not exist. " +
                        $"Please create the ruleset first or set the rules engine option '{nameof(this.Options.AutoCreateRulesets)}' to true.");
                    return OperationResult.Failure(errors);
                }

                await this.CreateRulesetInternalAsync(rule.Ruleset).ConfigureAwait(false);
            }

            var rulesFilterArgs = new GetRulesFilteredArgs
            {
                Ruleset = rule.Ruleset,
            };

            var existentRules = await this.rulesSource.GetRulesFilteredAsync(rulesFilterArgs).ConfigureAwait(false);

            if (ruleAddPriorityOption.PriorityOption == PriorityOptions.AtRuleName
                && !existentRules.Any(r => string.Equals(r.Name, ruleAddPriorityOption.AtRuleNameOptionValue, StringComparison.OrdinalIgnoreCase)))
            {
                errors.Add($"Rule name '{ruleAddPriorityOption.AtRuleNameOptionValue}' specified for priority placement does not exist.");
            }

            if (existentRules.Any(r => string.Equals(r.Name, rule.Name, StringComparison.OrdinalIgnoreCase)))
            {
                errors.Add($"A rule with name '{rule.Name}' already exists.");
            }

            var ruleSanitizeResult = await this.ruleSanitizer.SanitizeAsync(rule).ConfigureAwait(false);
            if (!ruleSanitizeResult.IsSuccess)
            {
                errors.AddRange(ruleSanitizeResult.Errors);
            }

            if (errors.Any())
            {
                return OperationResult.Failure(errors);
            }

            switch (ruleAddPriorityOption.PriorityOption)
            {
                case PriorityOptions.AtSmallestNumber:
                    await this.AddRuleInternalAtTopAsync(rule, existentRules).ConfigureAwait(false);

                    break;

                case PriorityOptions.AtLargestNumber:

                    await this.AddRuleInternalAtBottomAsync(rule, existentRules).ConfigureAwait(false);

                    break;

                case PriorityOptions.AtNumber:
                    await this.AddRuleInternalAtPriorityNumberAsync(rule, ruleAddPriorityOption, existentRules).ConfigureAwait(false);

                    break;

                case PriorityOptions.AtRuleName:
                    await this.AddRuleInternalAtRuleNameAsync(rule, ruleAddPriorityOption, existentRules).ConfigureAwait(false);

                    break;

                default:
                    throw new NotSupportedException($"The placement option '{ruleAddPriorityOption.PriorityOption}' is not supported.");
            }

            return OperationResult.Success();
        }

        private ValueTask AddRuleInternalAtBottomAsync(Rule rule, IReadOnlyCollection<Rule> existentRules)
        {
            rule.Priority = !existentRules.Any() ? 1 : existentRules.Max(r => r.Priority) + 1;

            return ManagementOperations.Manage(existentRules)
                .UsingSource(this.rulesSource)
                .AddRule(rule)
                .ExecuteOperationsAsync();
        }

        private ValueTask AddRuleInternalAtPriorityNumberAsync(Rule rule, RuleAddPriorityOption ruleAddPriorityOption, IReadOnlyCollection<Rule> existentRules)
        {
            var priorityMin = existentRules.MinOrDefault(r => r.Priority);
            var priorityMax = existentRules.MaxOrDefault(r => r.Priority);

            var rulePriority = ruleAddPriorityOption.AtNumberOptionValue;
            rulePriority = Math.Min(rulePriority, priorityMax + 1);
            rulePriority = Math.Max(rulePriority, priorityMin);

            rule.Priority = rulePriority;

            return ManagementOperations.Manage(existentRules)
                .UsingSource(this.rulesSource)
                .FilterFromThresholdPriorityToBottom(rulePriority)
                .IncreasePriority()
                .UpdateRules()
                .AddRule(rule)
                .ExecuteOperationsAsync();
        }

        private ValueTask AddRuleInternalAtRuleNameAsync(Rule rule, RuleAddPriorityOption ruleAddPriorityOption, IReadOnlyCollection<Rule> existentRules)
        {
            var firstPriorityToIncrement = existentRules
                                    .FirstOrDefault(r => string.Equals(r.Name, ruleAddPriorityOption.AtRuleNameOptionValue, StringComparison.OrdinalIgnoreCase))
                                    .Priority;
            rule.Priority = firstPriorityToIncrement;

            return ManagementOperations.Manage(existentRules)
                .UsingSource(this.rulesSource)
                .FilterFromThresholdPriorityToBottom(firstPriorityToIncrement)
                .IncreasePriority()
                .UpdateRules()
                .AddRule(rule)
                .ExecuteOperationsAsync();
        }

        private ValueTask AddRuleInternalAtTopAsync(Rule rule, IReadOnlyCollection<Rule> existentRules)
        {
            rule.Priority = 1;

            return ManagementOperations.Manage(existentRules)
                .UsingSource(this.rulesSource)
                .IncreasePriority()
                .UpdateRules()
                .AddRule(rule)
                .ExecuteOperationsAsync();
        }

        private async ValueTask<OperationResult> CreateRulesetInternalAsync(string ruleset)
        {
            var createRulesetArgs = new CreateRulesetArgs { Name = ruleset };
            await this.rulesSource.CreateRulesetAsync(createRulesetArgs).ConfigureAwait(false);
            return OperationResult.Success();
        }

        private List<Rule> EvalAll(
            IReadOnlyCollection<Rule> orderedRules,
            EvaluationOptions evaluationOptions,
            IDictionary<string, Operand> conditionsAsDictionary,
            bool active)
        {
            // Begins evaluation at the first element of the given list as parameter. Returns all
            // rules that match. Assumes given list is ordered.
            var matchedRules = new List<Rule>(orderedRules.Count);
            foreach (var rule in orderedRules)
            {
                if (this.EvalRule(rule, evaluationOptions, conditionsAsDictionary, active))
                {
                    matchedRules.Add(rule);
                }
            }

            return matchedRules;
        }

        private Rule EvalOneReverse(
            IReadOnlyCollection<Rule> rules,
            EvaluationOptions evaluationOptions,
            IDictionary<string, Operand> conditions,
            bool active)
        {
            var orderedRules = rules.OrderByDescending(r => r.Priority);
            foreach (var rule in orderedRules)
            {
                if (this.EvalRule(rule, evaluationOptions, conditions, active))
                {
                    return rule;
                }
            }

            return null!;
        }

        private Rule EvalOneTraverse(
            IReadOnlyCollection<Rule> rules,
            EvaluationOptions evaluationOptions,
            IDictionary<string, Operand> conditions,
            bool active)
        {
            //var orderedRules = rules.OrderBy(r => r.Priority);
            foreach (var rule in rules)
            {
                if (this.EvalRule(rule, evaluationOptions, conditions, active))
                {
                    return rule;
                }
            }

            return null!;
        }

        private bool EvalRule(
            Rule rule,
            EvaluationOptions evaluationOptions,
            IDictionary<string, Operand> conditions,
            bool active)
        {
            var rootCondition = rule.RootCondition;
            return rule.Active == active && (rootCondition == null || this.conditionsEvalEngine.Eval(rootCondition, conditions, evaluationOptions));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ValueTask<IReadOnlyCollection<Rule>> GetRulesAsync(GetRulesArgs getRulesArgs)
        {
            return this.rulesSource.GetRulesAsync(getRulesArgs);
        }

        private async ValueTask<OperationResult> UpdateRuleInternalAsync(Rule rule)
        {
            var rulesFilterArgs = new GetRulesFilteredArgs
            {
                Ruleset = rule.Ruleset,
            };

            var existentRules = await this.rulesSource.GetRulesFilteredAsync(rulesFilterArgs).ConfigureAwait(false);

            var existentRule = existentRules.FirstOrDefault(r => string.Equals(r.Name, rule.Name, StringComparison.OrdinalIgnoreCase));
            if (existentRule is null)
            {
                return OperationResult.Failure($"Rule with name '{rule.Name}' does not exist.");
            }

            var validationResult = this.ruleValidator.Validate(rule);

            if (!validationResult.IsValid)
            {
                return OperationResult.Failure(validationResult.Errors.Select(ve => ve.ErrorMessage));
            }

            var topPriorityThreshold = Math.Min(rule.Priority, existentRule.Priority);
            var bottomPriorityThreshold = Math.Max(rule.Priority, existentRule.Priority);

            switch (rule.Priority)
            {
                case int p when p > existentRule.Priority:
                    await ManagementOperations.Manage(existentRules)
                        .UsingSource(this.rulesSource)
                        .FilterPrioritiesRange(topPriorityThreshold, bottomPriorityThreshold)
                        .DecreasePriority()
                        .SetRuleForUpdate(rule)
                        .UpdateRules()
                        .ExecuteOperationsAsync()
                        .ConfigureAwait(false);
                    break;

                case int p when p < existentRule.Priority:
                    await ManagementOperations.Manage(existentRules)
                        .UsingSource(this.rulesSource)
                        .FilterPrioritiesRange(topPriorityThreshold, bottomPriorityThreshold)
                        .IncreasePriority()
                        .SetRuleForUpdate(rule)
                        .UpdateRules()
                        .ExecuteOperationsAsync()
                        .ConfigureAwait(false);
                    break;

                default:
                    await ManagementOperations.Manage(existentRules)
                        .UsingSource(this.rulesSource)
                        .SetRuleForUpdate(rule)
                        .UpdateRules()
                        .ExecuteOperationsAsync()
                        .ConfigureAwait(false);

                    break;
            }

            return OperationResult.Success();
        }
    }
}