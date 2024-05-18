namespace Regulae.Providers.MongoDb.Serialization
{
    using System;
    using Regulae.Serialization;

    /// <summary>
    /// Defines a content serialization provider for dynamic types.
    /// </summary>
    /// <seealso cref="IContentSerializationProvider"/>
    public class DynamicToStrongTypeContentSerializationProvider : IContentSerializationProvider
    {
        private readonly Lazy<IContentSerializer> contentSerializerLazy;

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="DynamicToStrongTypeContentSerializationProvider"/> class.
        /// </summary>
        public DynamicToStrongTypeContentSerializationProvider()
        {
            this.contentSerializerLazy = new Lazy<IContentSerializer>(
                () => new DynamicToStrongTypeContentSerializer(),
                System.Threading.LazyThreadSafetyMode.PublicationOnly);
        }

        /// <inheritdoc/>
        public IContentSerializer GetContentSerializer(string ruleset) => this.contentSerializerLazy.Value;
    }
}