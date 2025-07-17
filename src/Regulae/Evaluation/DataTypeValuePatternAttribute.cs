namespace Regulae.Evaluation
{
    using System;

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    internal class DataTypeValuePatternAttribute : Attribute
    {
        public DataTypeValuePatternAttribute(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                throw new ArgumentException($"'{nameof(pattern)}' cannot be null or whitespace.", nameof(pattern));
            }

            this.Pattern = pattern;
        }

        public string Pattern { get; }
    }
}