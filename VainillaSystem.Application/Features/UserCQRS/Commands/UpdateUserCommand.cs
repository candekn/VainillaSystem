using VainillaSystem.Domain.Interfaces;
using VainillaSystem.Domain.ValueObjects;

namespace VainillaSystem.Application.Features.UserCQRS.Commands
{
    /// <summary>
    /// Command to update an existing user's name and age.
    /// Implements <see cref="IInvalidateCacheRequest"/> so the
    /// <c>InvalidateCacheBehavior</c> removes stale cache entries for the updated user automatically.
    /// </summary>
    /// <param name="UserId">The unique identifier of the user to update.</param>
    /// <param name="Name">The new display name to assign.</param>
    /// <param name="Age">The new age (in years) to assign.</param>
    public record UpdateUserCommand(Guid UserId, string Name, int Age)
        : IRequest<bool>, IInvalidateCacheRequest
    {
        /// <inheritdoc/>
        public string[] CacheKeysToInvalidate => [$"user-{UserId}", "users-list"];
    }
}