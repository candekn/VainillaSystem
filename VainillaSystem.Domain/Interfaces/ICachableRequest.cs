namespace VainillaSystem.Domain.Interfaces
{
    /// <summary>
    /// Opt-in marker for requests whose responses should be cached.
    /// When a request implements this interface, the <c>CachingBehavior</c> decorator
    /// will store and retrieve the result from <c>IMemoryCache</c> automatically.
    /// </summary>
    public interface ICachableRequest
    {
        /// <summary>
        /// Gets the unique key used to store and retrieve the cached response.
        /// </summary>
        string CacheKey { get; }

        /// <summary>
        /// Gets the optional duration after which the cached entry expires.
        /// If <see langword="null"/>, the entry never expires.
        /// </summary>
        TimeSpan? Expiration { get; }
    }
}