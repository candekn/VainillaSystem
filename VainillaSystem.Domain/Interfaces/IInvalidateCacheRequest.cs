namespace VainillaSystem.Domain.Interfaces
{
    /// <summary>
    /// Opt-in marker for commands that should invalidate one or more cache entries upon completion.
    /// When a request implements this interface, the <c>InvalidateCacheBehavior</c> decorator
    /// will remove the specified keys from <c>IMemoryCache</c> after the handler succeeds.
    /// </summary>
    public interface IInvalidateCacheRequest
    {
        /// <summary>
        /// Gets the cache keys that must be removed after this request is handled successfully.
        /// </summary>
        string[] CacheKeysToInvalidate { get; }
    }
}