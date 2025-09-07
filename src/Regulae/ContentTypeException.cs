namespace Regulae
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Defines a content type exception thrown when a content type is unable to be processed.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class ContentTypeException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="ContentTypeException"/>.
        /// </summary>
        /// <param name="message">the message.</param>
        /// <param name="innerException">the inner exception.</param>
        public ContentTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}