namespace Regulae.Builder
{
    using System;
    using Regulae;

    /// <summary>
    /// Exposes the interface contract for a configured rules engine builder. Allows to perform
    /// additional optional configurations and finish rules engine build.
    /// </summary>
    public interface IConfiguredRulesEngineBuilder
    {
        /// <summary>
        /// Builds a rules engine instance using all supplied configuration options.
        /// </summary>
        /// <returns>the rules engine instance.</returns>
        IRulesEngine Build();

        /// <summary>
        /// Allows configuration of the rules engine.
        /// </summary>
        /// <param name="configurationAction">
        /// the action with configuration logic for the rules engine.
        /// </param>
        /// <returns>the configured rules engine builder.</returns>
        IConfiguredRulesEngineBuilder Configure(Action<IRulesEngineConfiguration> configurationAction);
    }
}