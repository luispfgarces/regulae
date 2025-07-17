namespace Regulae.Cache
{
    using System;
    using System.Linq;
    using System.Runtime.Caching;

    internal class InMemoryCache : ICache
    {
        private readonly MemoryCache memoryCache;

        public InMemoryCache(string name)
        {
            this.memoryCache = new MemoryCache(name);
        }

        public void Evict(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));
            }

            if (memoryCache.Contains(key))
            {
                memoryCache.Remove(key);
            }
        }

        public void EvictMany(string keyPrefix)
        {
            if (string.IsNullOrEmpty(keyPrefix))
            {
                throw new ArgumentException("Key prefix cannot be null or empty.", nameof(keyPrefix));
            }

            var keysToRemove = memoryCache.Select(kvp => kvp.Key)
                .Where(key => key.StartsWith(keyPrefix, StringComparison.Ordinal)).ToList();

            foreach (var key in keysToRemove)
            {
                memoryCache.Remove(key);
            }
        }

        public object Set(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));
            }

            this.memoryCache.Set(key, value, DateTimeOffset.MaxValue);
            return value;
        }

        public bool TryGet(string key, out object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));
            }

            value = this.memoryCache.Get(key);
            if (value is null)
            {
                return false;
            }

            return true;
        }
    }
}