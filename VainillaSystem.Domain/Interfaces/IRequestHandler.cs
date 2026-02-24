namespace VainillaSystem.Domain.Interfaces
{
    /// <summary>
    /// Defines the handler contract for a request/response pair.
    /// Implement this interface in the Application layer for each use case.
    /// </summary>
    /// <typeparam name="TRequest">The request type this handler can process.</typeparam>
    /// <typeparam name="TResponse">The type of value this handler returns.</typeparam>
    public interface IRequestHandler<in TRequest, TResponse>
       where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Processes the given <paramref name="request"/> and returns a result.
        /// </summary>
        /// <param name="request">The request object containing all input data.</param>
        /// <param name="ct">Optional cancellation token to abort the operation.</param>
        /// <returns>A <see cref="Task{TResponse}"/> representing the asynchronous result.</returns>
        Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default);
    }
}