namespace Regulae.WebUI.Services
{
    using System;
    using Regulae;

    internal class RulesEngineInstance
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IRulesEngine RulesEngine { get; set; }
    }
}