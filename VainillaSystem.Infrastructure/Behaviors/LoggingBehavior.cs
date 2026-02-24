using Microsoft.Extensions.Logging;
using VainillaSystem.Domain.Interfaces;

namespace VainillaSystem.Infrastructure.Behaviors
{
    /// <summary>
    /// Decorator that logs the request name and execution time before delegating to the inner handler.
    /// Acts as the outermost layer of the pipeline, wrapping all other behaviors.
    /// </summary>
    /// <typeparam name="TRequest">The request type being handled.</typeparam>
    /// <typeparam name="TResponse">The response type produced by the handler.</typeparam>
    public sealed class LoggingBehavior<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _inner;
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="LoggingBehavior{TRequest,TResponse}"/>.
        /// </summary>
        /// <param name="inner">The next handler in the pipeline (another decorator or the concrete handler).</param>
        /// <param name="logger">The logger used to record request lifecycle events.</param>
        public LoggingBehavior(
            IRequestHandler<TRequest, TResponse> inner,
            ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _inner  = inner;
            _logger = logger;
        }

        /// <summary>
        /// Logs the start of request processing, delegates to the inner handler, and logs completion or failure.
        /// </summary>
        /// <param name="request">The incoming request.</param>
        /// <param name="ct">Optional cancellation token.</param>
        /// <returns>The response produced by the inner handler.</returns>
        /// <exception cref="Exception">Re-throws any exception raised by the inner pipeline after logging it.</exception>
        public async Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogInformation("[START] Handling {Request}", requestName);

            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var response = await _inner.HandleAsync(request, ct);
                sw.Stop();
                _logger.LogInformation("[END]   {Request} handled in {ElapsedMs}ms", requestName, sw.ElapsedMilliseconds);
                return response;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "[ERROR] {Request} failed after {ElapsedMs}ms", requestName, sw.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
