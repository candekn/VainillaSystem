using Microsoft.AspNetCore.Mvc;
using VainillaSystem.Application.Features.UserCQRS.Commands;
using VainillaSystem.Application.Features.UserCQRS.Queries;
using VainillaSystem.Application.Interfaces;

namespace VainillaSystem.Api.Controllers
{
    /// <summary>
    /// REST controller that exposes user management endpoints.
    /// All business logic is delegated to the <see cref="IMediator"/> pipeline.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of <see cref="UsersController"/>.
        /// </summary>
        /// <param name="mediator">The mediator used to dispatch commands and queries.</param>
        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new user with the provided name and age.
        /// </summary>
        /// <param name="request">The request body containing the user's name and age.</param>
        /// <param name="ct">Cancellation token provided by the ASP.NET Core runtime.</param>
        /// <returns>
        /// <c>201 Created</c> with a <see cref="CreateUserResponse"/> and a <c>Location</c> header
        /// pointing to the new resource; or <c>400 Bad Request</c> if validation fails.
        /// </returns>
        /// <response code="201">User created successfully.</response>
        /// <response code="400">Invalid request data (e.g. name is empty or age is out of range).</response>
        [HttpPost]
        [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser(
            [FromBody] CreateUserRequest request,
            CancellationToken ct)
        {
            var command = new CreateUserCommand(request.Name, request.Age);
            var response = await _mediator.SendAsync(command, ct);
            return CreatedAtAction(nameof(GetUserById), new { id = response.Id }, response);
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// Results are automatically served from the in-memory cache on subsequent calls.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> of the user to retrieve.</param>
        /// <param name="ct">Cancellation token provided by the ASP.NET Core runtime.</param>
        /// <returns>
        /// <c>200 OK</c> with a <see cref="UserDto"/>; or <c>404 Not Found</c> if the user does not exist.
        /// </returns>
        /// <response code="200">User found and returned.</response>
        /// <response code="404">No user with the given <paramref name="id"/> exists.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(Guid id, CancellationToken ct)
        {
            try
            {
                var query = new GetUserByIdQuery(id);
                var response = await _mediator.SendAsync(query, ct);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }

    /// <summary>
    /// Request body used by <see cref="UsersController.CreateUser"/> to create a new user.
    /// </summary>
    /// <param name="Name">The display name for the new user. Must not be null or empty.</param>
    /// <param name="Age">The age of the new user in years. Must be between 13 and 99.</param>
    public record CreateUserRequest(string Name, int Age);
}