using VainillaSystem.Domain.Interfaces;

namespace VainillaSystem.Application.Interfaces
{
    /// <summary>
    /// Entry point for dispatching requests through the pipeline.
    /// Concrete implementations (e.g. <c>MyVanillaMediator</c>) resolve the correct
    /// <see cref="IRequestHandler{TRequest,TResponse}"/> and its decorators from DI.
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Sends a <paramref name="request"/> through the handler pipeline and returns the result.
        /// </summary>
        /// <typeparam name="TResponse">The type of value expected from the handler.</typeparam>
        /// <param name="request">The command or query to dispatch.</param>
        /// <param name="ct">Optional cancellation token to abort the operation.</param>
        /// <returns>A <see cref="Task{TResponse}"/> representing the asynchronous result.</returns>
        Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken ct = default);
    }
}
