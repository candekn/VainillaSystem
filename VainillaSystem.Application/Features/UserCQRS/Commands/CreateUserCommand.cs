using VainillaSystem.Domain.Interfaces;

namespace VainillaSystem.Application.Features.UserCQRS.Commands
{
    /// <summary>
    /// Command to create a new user.
    /// Implements <see cref="IInvalidateCacheRequest"/> so the
    /// <c>InvalidateCacheBehavior</c> decorator removes stale cache entries after execution.
    /// </summary>
    /// <param name="Name">The display name for the new user.</param>
    /// <param name="Age">The age of the new user in years.</param>
    public record CreateUserCommand(string Name, int Age)
        : IRequest<CreateUserResponse>, IInvalidateCacheRequest
    {
        /// <inheritdoc/>
        public string[] CacheKeysToInvalidate => ["users-list"];
    }

    /// <summary>
    /// Response returned after successfully creating a user.
    /// </summary>
    /// <param name="Id">The unique identifier assigned to the new user.</param>
    /// <param name="Name">The display name of the created user.</param>
    /// <param name="Age">The age of the created user.</param>
    public record CreateUserResponse(Guid Id, string Name, int Age);
}