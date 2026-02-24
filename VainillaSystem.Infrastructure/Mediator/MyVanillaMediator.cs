using System.Reflection;
using Microsoft.Extensions.Logging;
using VainillaSystem.Application.Interfaces;
using VainillaSystem.Domain.Interfaces;

namespace VainillaSystem.Infrastructure.Mediator
{
    /// <summary>
    /// Vanilla implementation of <see cref="IMediator"/> that resolves the decorated handler chain
    /// from <see cref="IServiceProvider"/> using Reflection, without any external library dependency.
    /// The decoration order is determined by the order of registration in <c>DependencyInjection.cs</c>.
    /// </summary>
    public sealed class MyVanillaMediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MyVanillaMediator> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="MyVanillaMediator"/>.
        /// </summary>
        /// <param name="serviceProvider">
        /// The DI container used to resolve <see cref="IRequestHandler{TRequest,TResponse}"/> instances.
        /// </param>
        /// <param name="logger">The logger used to record dispatch events.</param>
        public MyVanillaMediator(IServiceProvider serviceProvider, ILogger<MyVanillaMediator> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Resolves the outermost <see cref="IRequestHandler{TRequest,TResponse}"/> for the given
        /// <paramref name="request"/> type and invokes <c>HandleAsync</c> via Reflection.
        /// </summary>
        /// <typeparam name="TResponse">The expected return type.</typeparam>
        /// <param name="request">The command or query to dispatch.</param>
        /// <param name="ct">Optional cancellation token.</param>
        /// <returns>A <see cref="Task{TResponse}"/> representing the pipeline result.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no handler is registered in DI for the given request type.
        /// </exception>
        public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
        {
            var requestType  = request.GetType();
            var handlerType  = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

            var handler = _serviceProvider.GetService(handlerType)
                ?? throw new InvalidOperationException(
                    $"No handler registered for request type '{requestType.Name}'.");

            _logger.LogDebug("Dispatching {Request} to {Handler}", requestType.Name, handler.GetType().Name);

            // Invoke HandleAsync via reflection (handler may be a decorator)
            var method = handlerType.GetMethod("HandleAsync")!;
            var task   = (Task<TResponse>)method.Invoke(handler, [request, ct])!;
            return await task;
        }
    }
}
