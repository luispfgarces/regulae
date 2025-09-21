namespace Regulae
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the result of an operation performed on the rules engine.
    /// </summary>
    public class OperationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult"/> class.
        /// </summary>
        /// <param name="errors">The errors.</param>
        internal OperationResult(IList<OperationError> errors)
        {
            ArgumentNullException.ThrowIfNull(errors);
            this.Errors = errors;
        }

        /// <summary>
        /// Gets the errors occurred during the operation.
        /// </summary>
        /// <value>The errors.</value>
        public IList<OperationError> Errors { get; }

        /// <summary>
        /// Gets a value indicating whether the operation was successfull or not.
        /// </summary>
        /// <value><see langword="true"/> if rule operation was successfull; otherwise, <see langword="false"/>.</value>
        public bool IsSuccess => this.Errors.Count == 0;


    }
}