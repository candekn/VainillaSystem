using VainillaSystem.Domain.Interfaces;

namespace VainillaSystem.Application.Features.UserCQRS.Queries
{
    /// <summary>
    /// Query to retrieve a user by their unique identifier.
    /// Implements <see cref="ICachableRequest"/> so the <c>CachingBehavior</c> decorator
    /// caches the result automatically for 5 minutes.
    /// </summary>
    /// <param name="UserId">The unique identifier of the user to retrieve.</param>
    public record GetUserByIdQuery(Guid UserId) : IRequest<UserDto>, ICachableRequest
    {
        /// <inheritdoc/>
        public string CacheKey  => $"user-{UserId}";

        /// <inheritdoc/>
        public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
    }

    /// <summary>
    /// Read model returned by <see cref="GetUserByIdQuery"/>.
    /// </summary>
    /// <param name="Id">The unique identifier of the user.</param>
    /// <param name="Name">The display name of the user.</param>
    /// <param name="Age">The age of the user in years.</param>
    public record UserDto(Guid Id, string Name, int Age);
}