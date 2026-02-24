using VainillaSystem.Domain.Interfaces;
using VainillaSystem.Domain.ValueObjects;

namespace VainillaSystem.Application.Features.UserCQRS.Commands
{
    /// <summary>
    /// Handles <see cref="UpdateUserCommand"/>: loads the user aggregate, applies changes
    /// via <c>UpdateDetails</c>, and persists the result.
    /// The <c>InvalidateCacheBehavior</c> decorator clears the stale cache entries automatically.
    /// </summary>
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IUserRepository _repository;

        /// <summary>
        /// Initializes a new instance of <see cref="UpdateUserHandler"/>.
        /// </summary>
        /// <param name="repository">The user repository used to load and persist the aggregate.</param>
        public UpdateUserHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Loads the existing user, validates and applies the new name and age,
        /// then persists the updated aggregate.
        /// </summary>
        /// <param name="request">The command containing the user ID and new property values.</param>
        /// <param name="ct">Optional cancellation token.</param>
        /// <returns><see langword="true"/> when the update completes successfully.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no user with the given <see cref="UpdateUserCommand.UserId"/> exists.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <see cref="UpdateUserCommand.Name"/> is null/empty
        /// or <see cref="UpdateUserCommand.Age"/> is outside the valid range (13–99).
        /// </exception>
        public async Task<bool> HandleAsync(UpdateUserCommand request, CancellationToken ct = default)
        {
            var user = await _repository.GetByIdAsync(request.UserId, ct);

            if (user is null)
                throw new KeyNotFoundException($"User with id '{request.UserId}' was not found.");

            var newName = Name.Create(request.Name);
            var newAge  = Age.Create(request.Age);

            user.UpdateDetails(newName, newAge);

            await _repository.UpdateAsync(user, ct);

            return true;
        }
    }
}