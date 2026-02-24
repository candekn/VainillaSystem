using VainillaSystem.Domain.Entities;

namespace VainillaSystem.Domain.Interfaces
{
    /// <summary>
    /// Persistence contract for <see cref="User"/> aggregates.
    /// Concrete implementations (e.g. EF Core, in-memory) must register as this interface in DI.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves a <see cref="User"/> by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to look up.</param>
        /// <param name="ct">Optional cancellation token.</param>
        /// <returns>
        /// The matching <see cref="User"/> if found; otherwise <see langword="null"/>.
        /// </returns>
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// Persists a new <see cref="User"/> aggregate.
        /// </summary>
        /// <param name="user">The user to add. Must not already exist in the store.</param>
        /// <param name="ct">Optional cancellation token.</param>
        Task AddAsync(User user, CancellationToken ct = default);

        /// <summary>
        /// Persists changes to an existing <see cref="User"/> aggregate.
        /// </summary>
        /// <param name="user">The user with updated properties. Must already exist in the store.</param>
        /// <param name="ct">Optional cancellation token.</param>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no user with the given identifier exists in the store.
        /// </exception>
        Task UpdateAsync(User user, CancellationToken ct = default);
    }
}
