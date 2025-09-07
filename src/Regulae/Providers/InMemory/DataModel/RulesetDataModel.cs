namespace Regulae.Providers.InMemory.DataModel
{
    using System;
    using System.Collections.Generic;

    internal class RulesetDataModel
    {
        public required DateTime Creation { get; set; }

        public required string Name { get; set; }

        public required List<RuleDataModel> Rules { get; set; }
    }
}