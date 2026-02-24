using System.Collections.Concurrent;
using VainillaSystem.Domain.Entities;
using VainillaSystem.Domain.Interfaces;

namespace VainillaSystem.Infrastructure.Persistence
{
    /// <summary>
    /// Thread-safe in-memory implementation of <see cref="IUserRepository"/>.
    /// Intended for development, testing and demo scenarios only.
    /// Uses a <see cref="ConcurrentDictionary{TKey,TValue}"/> keyed on <see cref="Guid"/> to store user data
    /// as primitive tuples, keeping the storage layer independent from the domain model.
    /// </summary>
    public sealed class InMemoryUserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<Guid, (Guid Id, string Name, int Age)> _store = new();

        /// <summary>
        /// Retrieves a <see cref="User"/> aggregate by its unique identifier,
        /// reconstructing it from the stored primitive tuple.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> of the user to retrieve.</param>
        /// <param name="ct">Optional cancellation token (not used in-memory).</param>
        /// <returns>
        /// The reconstructed <see cref="User"/> if found; otherwise <see langword="null"/>.
        /// </returns>
        public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            if (_store.TryGetValue(id, out var data))
            {
                var user = User.Reconstruct(data.Id, data.Name, data.Age);
                return Task.FromResult<User?>(user);
            }

            return Task.FromResult<User?>(null);
        }

        /// <summary>
        /// Persists a new <see cref="User"/> aggregate by storing its primitive representation.
        /// If a user with the same ID already exists it will be overwritten.
        /// </summary>
        /// <param name="user">The user aggregate to persist.</param>
        /// <param name="ct">Optional cancellation token (not used in-memory).</param>
        public Task AddAsync(User user, CancellationToken ct = default)
        {
            var id   = user.GetId().GetValue();
            var name = user.GetName().GetValue();
            var age  = user.GetAge().GetValue();

            _store[id] = (id, name, age);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Persists changes to an existing <see cref="User"/> aggregate.
        /// </summary>
        /// <param name="user">The user aggregate with updated properties.</param>
        /// <param name="ct">Optional cancellation token (not used in-memory).</param>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no user with the given ID exists in the in-memory store.
        /// </exception>
        public Task UpdateAsync(User user, CancellationToken ct = default)
        {
            var id = user.GetId().GetValue();

            if (!_store.ContainsKey(id))
                throw new KeyNotFoundException($"Cannot update: user '{id}' not found.");

            var name = user.GetName().GetValue();
            var age  = user.GetAge().GetValue();

            _store[id] = (id, name, age);
            return Task.CompletedTask;
        }
    }
}
