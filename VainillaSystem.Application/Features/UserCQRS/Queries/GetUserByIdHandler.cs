using VainillaSystem.Domain.Interfaces;

namespace VainillaSystem.Application.Features.UserCQRS.Queries
{
    /// <summary>
    /// Handles <see cref="GetUserByIdQuery"/>: retrieves the user aggregate from the repository
    /// and maps it to a <see cref="UserDto"/> read model.
    /// </summary>
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUserRepository _repository;

        /// <summary>
        /// Initializes a new instance of <see cref="GetUserByIdHandler"/>.
        /// </summary>
        /// <param name="repository">The user repository used to fetch the aggregate.</param>
        public GetUserByIdHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Retrieves the user with the given identifier and maps it to a <see cref="UserDto"/>.
        /// </summary>
        /// <param name="request">The query containing the <see cref="GetUserByIdQuery.UserId"/> to look up.</param>
        /// <param name="ct">Optional cancellation token.</param>
        /// <returns>A <see cref="UserDto"/> with the user's data.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no user with <see cref="GetUserByIdQuery.UserId"/> exists in the repository.
        /// </exception>
        public async Task<UserDto> HandleAsync(GetUserByIdQuery request, CancellationToken ct = default)
        {
            var user = await _repository.GetByIdAsync(request.UserId, ct);

            if (user is null)
                throw new KeyNotFoundException($"User with id '{request.UserId}' was not found.");

            return new UserDto(
                user.GetId().GetValue(),
                user.GetName().GetValue(),
                user.GetAge().GetValue());
        }
    }
}