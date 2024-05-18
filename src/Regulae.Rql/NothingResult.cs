namespace Regulae.Rql
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The result type that describes a nothing value.
    /// </summary>
    /// <seealso cref="IResult"/>
    [ExcludeFromCodeCoverage]
    public class NothingResult : IResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NothingResult"/> class.
        /// </summary>
        /// <param name="rql">The Rule Query Language source.</param>
        public NothingResult(string rql)
        {
            this.Rql = rql;
        }

        /// <summary>
        /// Gets the Rule Query Language source.
        /// </summary>
        /// <value>The Rule Query Language source.</value>
        public string Rql { get; }
    }
}