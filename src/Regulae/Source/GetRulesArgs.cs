namespace Regulae.Source
{
    using System;

    internal sealed class GetRulesArgs
    {
        public required DateTime DateBegin { get; set; }

        public required DateTime DateEnd { get; set; }

        public required string Ruleset { get; set; }
    }
}