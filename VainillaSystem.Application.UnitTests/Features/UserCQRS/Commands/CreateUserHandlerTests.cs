using NSubstitute;
using VainillaSystem.Application.Features.UserCQRS.Commands;
using VainillaSystem.Domain.Interfaces;

namespace VainillaSystem.Application.UnitTests.Features.UserCQRS.Commands
{
    public class CreateUserHandlerTests
    {
        private readonly IUserRepository _repository;
        private readonly CreateUserHandler _handler;

        public CreateUserHandlerTests()
        {
            _repository = Substitute.For<IUserRepository>();
            _handler = new CreateUserHandler(_repository);
        }

        [Fact]
        public async Task HandleAsync_WithValidCommand_CallsRepositoryAddAsync()
        {
            var command = new CreateUserCommand("Luffy", 19);

            await _handler.HandleAsync(command);

            await _repository.Received(1).AddAsync(Arg.Any<Domain.Entities.User>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task HandleAsync_WithValidCommand_ReturnsResponseWithCorrectData()
        {
            var command = new CreateUserCommand("Luffy", 19);

            var response = await _handler.HandleAsync(command);

            Assert.Equal("Luffy", response.Name);
            Assert.Equal(19, response.Age);
            Assert.NotEqual(Guid.Empty, response.Id);
        }

        [Fact]
        public async Task HandleAsync_EachCall_GeneratesUniqueId()
        {
            var command1 = new CreateUserCommand("Luffy", 19);
            var command2 = new CreateUserCommand("Zoro", 21);

            var response1 = await _handler.HandleAsync(command1);
            var response2 = await _handler.HandleAsync(command2);

            Assert.NotEqual(response1.Id, response2.Id);
        }

        [Fact]
        public async Task HandleAsync_WithInvalidAge_ThrowsArgumentException()
        {
            var command = new CreateUserCommand("Luffy", 5);

            await Assert.ThrowsAsync<ArgumentException>(() => _handler.HandleAsync(command));
        }

        [Fact]
        public async Task HandleAsync_WithEmptyName_ThrowsArgumentException()
        {
            var command = new CreateUserCommand(string.Empty, 19);

            await Assert.ThrowsAsync<ArgumentException>(() => _handler.HandleAsync(command));
        }
    }
}