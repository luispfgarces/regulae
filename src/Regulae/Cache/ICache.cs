namespace Regulae.Cache
{
    /// <summary>
    /// Defines the interface for a cache system used by the Regulae rules engine.
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Evicts the cache entry identified by the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        void Evict(string key);

        /// <summary>
        /// Evicts all cache entries that have identifiers prefixed by the specified key prefix.
        /// </summary>
        /// <param name="keyPrefix">The key prefix.</param>
        void EvictMany(string keyPrefix);

        /// <summary>
        /// Sets a cache entry identified by the specified key with the specified value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        object Set(string key, object value);

        /// <summary>
        /// Tries to get a value from a cache entry identified by the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        bool TryGet(string key, out object value);
    }
}