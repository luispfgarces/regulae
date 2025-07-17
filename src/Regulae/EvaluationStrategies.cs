namespace Regulae
{
    /// <summary>
    /// Defines the evaluation strategies available for the rules engine.
    /// </summary>
    public enum EvaluationStrategies
    {
        /// <summary>
        /// The interpreted evalution strategy, which interprets the rule's conditions model at
        /// evaluation time.
        /// </summary>
        Interpreted = 0,

        /// <summary>
        /// The compiled evaluation strategy, which compiles the rule's conditions model into
        /// executable code before evaluation time.
        /// </summary>
        Compiled = 1,
    }
}