namespace Regulae
{
    /// <summary>
    /// Defines a content container that can hold content of a specific type.
    /// </summary>
    public interface IContentContainer
    {
        /// <summary>
        /// Gets the content as the specified type.
        /// </summary>
        /// <typeparam name="TContent">The type of the content.</typeparam>
        /// <returns></returns>
        TContent GetContentAs<TContent>();
    }
}