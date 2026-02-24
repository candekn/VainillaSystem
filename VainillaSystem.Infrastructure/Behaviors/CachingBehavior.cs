using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using VainillaSystem.Domain.Interfaces;

namespace VainillaSystem.Infrastructure.Behaviors
{
    /// <summary>
    /// Decorator that caches handler responses in <see cref="IMemoryCache"/>.
    /// Only activates for requests that implement <see cref="ICachableRequest"/>.
    /// On a cache miss the inner handler is called and the result is stored for future requests.
    /// </summary>
    /// <typeparam name="TRequest">The request type being handled.</typeparam>
    /// <typeparam name="TResponse">The response type produced by the handler.</typeparam>
    public sealed class CachingBehavior<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _inner;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="CachingBehavior{TRequest,TResponse}"/>.
        /// </summary>
        /// <param name="inner">The next handler in the pipeline.</param>
        /// <param name="cache">The in-memory cache used to store and retrieve responses.</param>
        /// <param name="logger">The logger used to record cache hits and misses.</param>
        public CachingBehavior(
            IRequestHandler<TRequest, TResponse> inner,
            IMemoryCache cache,
            ILogger<CachingBehavior<TRequest, TResponse>> logger)
        {
            _inner  = inner;
            _cache  = cache;
            _logger = logger;
        }

        /// <summary>
        /// Returns the cached response if available; otherwise calls the inner handler,
        /// stores the result, and returns it.
        /// Non-cachable requests are passed through immediately without touching the cache.
        /// </summary>
        /// <param name="request">The incoming request.</param>
        /// <param name="ct">Optional cancellation token.</param>
        /// <returns>The cached or freshly produced response.</returns>
        public async Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default)
        {
            if (request is not ICachableRequest cachableRequest)
                return await _inner.HandleAsync(request, ct);

            var cacheKey = cachableRequest.CacheKey;

            if (_cache.TryGetValue(cacheKey, out TResponse? cached))
            {
                _logger.LogInformation("[CACHE HIT]  Key: {CacheKey}", cacheKey);
                return cached!;
            }

            _logger.LogInformation("[CACHE MISS] Key: {CacheKey}", cacheKey);
            var response = await _inner.HandleAsync(request, ct);

            var options = new MemoryCacheEntryOptions();
            if (cachableRequest.Expiration.HasValue)
                options.AbsoluteExpirationRelativeToNow = cachableRequest.Expiration;

            _cache.Set(cacheKey, response, options);
            return response;
        }
    }
}
