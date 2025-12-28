namespace Regulae.Rql.Runtime
{
    /// <summary>
    /// Defines the interface contract for objects that can provide a pretty-printed string representation of themselves.
    /// </summary>
    internal interface IPrettyPrintable
    {
        /// <summary>
        /// Returns a formatted string representation of the current object that is intended to be more readable or
        /// user-friendly than the default string output.
        /// </summary>
        /// <returns>A string that presents the object's data in a human-readable, well-formatted manner.</returns>
        string ToPrettyString();

        /// <summary>
        /// Returns a formatted string representation of the current object with specified indentation level that is
        /// intended to be more readable or user-friendly than the default string output.
        /// </summary>
        /// <param name="indentLevel">the indentation level</param>
        /// <returns>A string that presents the object's data in a human-readable, well-formatted manner.</returns>
        string ToPrettyString(int indentLevel);
    }
}
