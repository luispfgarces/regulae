namespace Regulae.Generic
{
    /// <summary>
    /// The generic rules engine options.
    /// </summary>
    public sealed class GenericRulesEngineOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether the conditions should be automatically created.
        /// When <c>true</c> is specified, the rules engine uses the conditions type specified to
        /// infer conditions to be created and creates them.
        /// </summary>
        /// <value><c>true</c> if the conditions should be automatically created; otherwise, <c>false</c>.</value>
        public bool AutoCreateConditions { get; set; } = false;
    }
}