namespace Regulae.IntegrationTests
{
    using Regulae.Serialization;

    internal class JsonContentSerializationProvider : IContentSerializationProvider
    {
        public IContentSerializer GetContentSerializer(string ruleset)
        {
            return new JsonContentSerializer();
        }
    }
}