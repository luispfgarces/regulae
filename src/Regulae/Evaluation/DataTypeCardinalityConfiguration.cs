namespace Regulae.Evaluation
{
    using System;

    internal sealed class DataTypeCardinalityConfiguration
    {
        public DataTypeCardinalityConfiguration(Type type, object @default)
        {
            this.Type = type;
            this.Default = @default;
        }

        public object Default { get; private set; }

        public Type Type { get; private set; }
    }
}