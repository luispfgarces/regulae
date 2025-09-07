namespace Regulae.Providers.InMemory.DataModel
{
    using System;

    internal sealed class RuleDataModel
    {
        public bool Active { get; set; } = true;

        public required object Content { get; set; }

        public required DateTime DateBegin { get; set; }

        public DateTime? DateEnd { get; set; }

        public required string Name { get; set; }

        public int Priority { get; set; }

        public ConditionNodeDataModel? RootCondition { get; set; }

        public required string Ruleset { get; set; }
    }
}