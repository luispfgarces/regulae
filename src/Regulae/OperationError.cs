namespace Regulae
{
    using System;

    /// <summary>
    /// Represents an error occurred during an operation performed on the rules engine.
    /// </summary>
    public sealed class OperationError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationError"/> class.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="message">The error message.</param>
        public OperationError(string code, string message)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(code);
            ArgumentException.ThrowIfNullOrWhiteSpace(message);

            this.Code = code;
            this.Message = message;
        }

        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; }

        /// <summary>
        /// Creates a new <see cref="OperationError"/>.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="message">The error message.</param>
        /// <returns></returns>
        public static OperationError Create(string code, string message) => new(code, message);
    }
}
