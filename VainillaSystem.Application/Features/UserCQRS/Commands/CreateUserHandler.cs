using VainillaSystem.Domain.Entities;
using VainillaSystem.Domain.Interfaces;

namespace VainillaSystem.Application.Features.UserCQRS.Commands
{
    /// <summary>
    /// Handles <see cref="CreateUserCommand"/>: creates the <see cref="User"/> aggregate and persists it.
    /// </summary>
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
    {
        private readonly IUserRepository _repository;

        /// <summary>
        /// Initializes a new instance of <see cref="CreateUserHandler"/>.
        /// </summary>
        /// <param name="repository">The user repository used to persist the new aggregate.</param>
        public CreateUserHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Creates a new <see cref="User"/> from the command data, persists it, and returns the response DTO.
        /// </summary>
        /// <param name="request">The command containing the name and age of the user to create.</param>
        /// <param name="ct">Optional cancellation token.</param>
        /// <returns>
        /// A <see cref="CreateUserResponse"/> containing the assigned ID and the provided data.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the name is null/empty or the age is outside the valid range (13–99).
        /// </exception>
        public async Task<CreateUserResponse> HandleAsync(CreateUserCommand request, CancellationToken ct = default)
        {
            var user = User.Create(request.Name, request.Age);

            await _repository.AddAsync(user, ct);

            return new CreateUserResponse(
                user.GetId().GetValue(),
                user.GetName().GetValue(),
                user.GetAge().GetValue());
        }
    }
}