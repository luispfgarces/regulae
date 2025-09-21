namespace Regulae
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using Regulae.Core;
    using Regulae.Evaluation;
    using Regulae.Management;
    using Regulae.Source;
    using Regulae.Validation;

    /// <summary>
    /// Exposes rules engine logic to provide rule matches to requests.
    /// </summary>
    public class RulesEngine : IRulesEngine
    {
        private readonly IAddRuleController addRuleController;
        private readonly IConditionsConverter conditionsConverter;
        private readonly IConditionsEvalEngine conditionsEvalEngine;
        private readonly IRuleConditionsExtractor ruleConditionsExtractor;
        private readonly IRulesSource rulesSource;
        private readonly IValidatorProvider validatorProvider;
        private readonly IUpdateRuleController updateRuleController;

        internal RulesEngine(
            RulesEngineArgs rulesEngineArgs)
        {
            this.addRuleController = rulesEngineArgs.AddRuleController;
            this.conditionsConverter = rulesEngineArgs.ConditionsConverter;
            this.conditionsEvalEngine = rulesEngineArgs.ConditionsEvalEngine;
            this.rulesSource = rulesEngineArgs.RulesSource;
            this.validatorProvider = rulesEngineArgs.ValidatorProvider;
            this.Options = rulesEngineArgs.RulesEngineOptions;
            this.ruleConditionsExtractor = rulesEngineArgs.RuleConditionsExtractor;
            this.updateRuleController = rulesEngineArgs.UpdateRuleController;
        }

        /// <inheritdoc/>
        public IRulesEngineOptions Options { get; }

        /// <inheritdoc/>
        public Task<OperationResult> ActivateRuleAsync(Rule rule)
        {
            ArgumentNullException.ThrowIfNull(rule);

            rule.Active = true;

            return this.UpdateRuleInternalAsync(rule).AsTask();
        }

        /// <inheritdoc/>
        public Task<OperationResult> AddRuleAsync(Rule rule, RuleAddPriorityOption ruleAddPriorityOption)
        {
            ArgumentNullException.ThrowIfNull(rule);
            ArgumentNullException.ThrowIfNull(ruleAddPriorityOption);

            return this.AddRuleInternalAsync(rule, ruleAddPriorityOption).AsTask();
        }

        /// <inheritdoc/>
        public async Task<OperationResult> CreateConditionAsync(string name, DataTypes dataType)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Operation.Failure(OperationError.Create("R000", "A condition must have a non-null, blank, or whitespace name."));
            }

            var args = new CreateConditionArgs
            {
                DataType = dataType,
                Name = name,
            };

            await this.rulesSource.CreateConditionAsync(args).ConfigureAwait(false);
            return Operation.Success();
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
                return Operation.Failure(OperationError.Create("R000", $"The ruleset '{ruleset}' already exists."));
            }

            return await this.CreateRulesetInternalAsync(ruleset).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<OperationResult> DeactivateRuleAsync(Rule rule)
        {
            ArgumentNullException.ThrowIfNull(rule);

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
                ? this.EvalOneTraverse(orderedRules, evaluationOptions, conditionsAsOperands, active: true)
                : this.EvalOneReverse(orderedRules, evaluationOptions, conditionsAsOperands, active: true);
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyCollection<Rule>> SearchAsync(SearchArgs<string, string> searchArgs)
        {
            ArgumentNullException.ThrowIfNull(searchArgs);

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
            ArgumentNullException.ThrowIfNull(rule);

            return this.UpdateRuleInternalAsync(rule).AsTask();
        }

        private async ValueTask<OperationResult> AddRuleInternalAsync(Rule rule, RuleAddPriorityOption ruleAddPriorityOption)
        {
            var validateOperationResult = await this.addRuleController.ValidateAddRuleAsync(rule, ruleAddPriorityOption).ConfigureAwait(false);
            if (validateOperationResult.Errors.Count == 1 && string.Equals(validateOperationResult.Errors[0].Code, Constants.ErrorCodes.R0006, StringComparison.Ordinal) && this.Options.AutoCreateRulesets)
            {
                var createRulesetOperationResult = await this.CreateRulesetInternalAsync(rule.Ruleset).ConfigureAwait(false);
                if (createRulesetOperationResult.IsSuccess)
                {
                    validateOperationResult.Errors.Clear();
                }
                else
                {
                    foreach (var error in createRulesetOperationResult.Errors)
                    {
                        validateOperationResult.Errors.Add(error);
                    }
                }
            }
            if (validateOperationResult.IsSuccess)
            {
                return await this.addRuleController.AddRuleAsync(rule, ruleAddPriorityOption).ConfigureAwait(false);
            }

            return validateOperationResult;
        }

        private async ValueTask<OperationResult> CreateRulesetInternalAsync(string ruleset)
        {
            var createRulesetArgs = new CreateRulesetArgs { Name = ruleset };
            await this.rulesSource.CreateRulesetAsync(createRulesetArgs).ConfigureAwait(false);
            return Operation.Success();
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
            var validationResult = await this.updateRuleController.ValidateUpdateRuleAsync(rule).ConfigureAwait(false);
            if (validationResult.IsSuccess)
            {
                return await this.updateRuleController.UpdateRuleAsync(rule).ConfigureAwait(false);
            }

            return validationResult;

        }
    }
}