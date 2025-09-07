namespace Regulae
{
    using System;
    using Regulae.Builder.Generic.RulesBuilder;
    using Regulae.Builder.RulesBuilder;

    /// <summary>
    /// Defines a rule.
    /// </summary>
    public class Rule
    {
        internal Rule(
            string name,
            string ruleset,
            DateTime dateBegin,
            DateTime? dateEnd,
            IContentContainer contentContainer)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(ruleset);
            this.Name = name;
            this.Ruleset = ruleset;
            this.DateBegin = dateBegin;
            this.DateEnd = dateEnd;
            this.ContentContainer = contentContainer ?? throw new ArgumentNullException(nameof(contentContainer));
        }

        /// <summary>
        /// Gets and sets the if the rules is active.
        /// </summary>
        public bool Active { get; internal set; } = true;

        /// <summary>
        /// Gets the content container which contains the rule content.
        /// </summary>
        public IContentContainer ContentContainer { get; internal set; }

        /// <summary>
        /// Gets the date from which the rule begins being applicable.
        /// </summary>
        public DateTime DateBegin { get; internal set; }

        /// <summary>
        /// Gets and sets the date from which the rule ceases to be applicable.
        /// </summary>
        public DateTime? DateEnd { get; set; }

        /// <summary>
        /// Gets the rule name.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets and sets the rule priority compared to other rules (preferably it is unique).
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets the rule root condition. This property is null when rule has no conditions.
        /// </summary>
        public IConditionNode? RootCondition { get; internal set; }

        /// <summary>
        /// Gets the ruleset to which the rule belongs to.
        /// </summary>
        public string Ruleset { get; internal set; }

        /// <summary>
        /// Creates a new rule with generic ruleset type and condition type.
        /// </summary>
        /// <typeparam name="TRuleset">The type of the ruleset.</typeparam>
        /// <typeparam name="TCondition">The type of the conditions.</typeparam>
        /// <returns></returns>
        public static IRuleConfigureRuleset<TRuleset, TCondition> Create<TRuleset, TCondition>(string name)
            where TRuleset : notnull
            where TCondition : notnull
            => new RuleBuilder<TRuleset, TCondition>(name);

        /// <summary>
        /// Creates a new rule.
        /// </summary>
        /// <returns></returns>
        public static IRuleConfigureRuleset Create(string name)
            => new RuleBuilder(name);

        /// <summary>
        /// Clones the rule into a different instance.
        /// </summary>
        /// <returns></returns>
        public virtual Rule Clone() => new Rule(this.Name, this.Ruleset, this.DateBegin, this.DateEnd, this.ContentContainer)
        {
            Active = this.Active,
            Priority = this.Priority,
            RootCondition = this.RootCondition?.Clone()!,
        };
    }
}