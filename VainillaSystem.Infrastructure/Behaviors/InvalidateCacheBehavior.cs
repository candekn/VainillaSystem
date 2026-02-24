using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using VainillaSystem.Domain.Interfaces;

namespace VainillaSystem.Infrastructure.Behaviors
{
    /// <summary>
    /// Decorator that removes one or more <see cref="IMemoryCache"/> entries after the inner handler succeeds.
    /// Only activates for requests that implement <see cref="IInvalidateCacheRequest"/>.
    /// The inner handler is always called first; invalidation happens on success only.
    /// </summary>
    /// <typeparam name="TRequest">The request type being handled.</typeparam>
    /// <typeparam name="TResponse">The response type produced by the handler.</typeparam>
    public sealed class InvalidateCacheBehavior<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _inner;
        private readonly IMemoryCache _cache;
        private readonly ILogger<InvalidateCacheBehavior<TRequest, TResponse>> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="InvalidateCacheBehavior{TRequest,TResponse}"/>.
        /// </summary>
        /// <param name="inner">The next handler in the pipeline.</param>
        /// <param name="cache">The in-memory cache whose entries will be invalidated.</param>
        /// <param name="logger">The logger used to record invalidation events.</param>
        public InvalidateCacheBehavior(
            IRequestHandler<TRequest, TResponse> inner,
            IMemoryCache cache,
            ILogger<InvalidateCacheBehavior<TRequest, TResponse>> logger)
        {
            _inner  = inner;
            _cache  = cache;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the inner handler and, if the request implements <see cref="IInvalidateCacheRequest"/>,
        /// removes the specified keys from the cache after a successful execution.
        /// </summary>
        /// <param name="request">The incoming request.</param>
        /// <param name="ct">Optional cancellation token.</param>
        /// <returns>The response produced by the inner handler.</returns>
        public async Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default)
        {
            var response = await _inner.HandleAsync(request, ct);

            if (request is IInvalidateCacheRequest invalidateRequest)
            {
                foreach (var key in invalidateRequest.CacheKeysToInvalidate)
                {
                    _cache.Remove(key);
                    _logger.LogInformation("[CACHE INVALIDATED] Key: {CacheKey}", key);
                }
            }

            return response;
        }
    }
}
