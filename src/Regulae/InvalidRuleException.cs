namespace Regulae
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Exception thrown when a operation is attempted with a invalid rule.
    /// </summary>
    /// <seealso cref="Exception" />
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class InvalidRuleException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRuleException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidRuleException(string message)
            : base(message)
        {
            this.Errors = Enumerable.Empty<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRuleException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errors">The errors.</param>
        public InvalidRuleException(
            string message,
            IEnumerable<string> errors)
            : base(message)
        {
            this.Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>
        /// The errors.
        /// </value>
        public IEnumerable<string> Errors { get; }
    }
}