using NSubstitute;
using VainillaSystem.Application.Features.UserCQRS.Queries;
using VainillaSystem.Domain.Interfaces;
using UserEntity = VainillaSystem.Domain.Entities.User;

namespace VainillaSystem.Application.UnitTests.Features.UserCQRS.Queries
{
    public class GetUserByIdHandlerTests
    {
        private readonly IUserRepository _repository;
        private readonly GetUserByIdHandler _handler;

        public GetUserByIdHandlerTests()
        {
            _repository = Substitute.For<IUserRepository>();
            _handler = new GetUserByIdHandler(_repository);
        }

        [Fact]
        public async Task HandleAsync_WhenUserExists_ReturnsMappedDto()
        {
            var userId = Guid.NewGuid();
            var user = UserEntity.Reconstruct(userId, "Luffy", 19);
            _repository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
                       .Returns(user);

            var query = new GetUserByIdQuery(userId);
            var result = await _handler.HandleAsync(query);

            Assert.Equal(userId, result.Id);
            Assert.Equal("Luffy", result.Name);
            Assert.Equal(19, result.Age);
        }

        [Fact]
        public async Task HandleAsync_WhenUserDoesNotExist_ThrowsKeyNotFoundException()
        {
            var userId = Guid.NewGuid();
            _repository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
                       .Returns((Domain.Entities.User?)null);

            var query = new GetUserByIdQuery(userId);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.HandleAsync(query));
        }

        [Fact]
        public async Task HandleAsync_WhenUserDoesNotExist_ExceptionMessageContainsId()
        {
            var userId = Guid.NewGuid();
            _repository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
                       .Returns((Domain.Entities.User?)null);

            var query = new GetUserByIdQuery(userId);

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.HandleAsync(query));
            Assert.Contains(userId.ToString(), ex.Message);
        }

        [Fact]
        public async Task HandleAsync_CallsRepositoryWithCorrectId()
        {
            var userId = Guid.NewGuid();
            var user = UserEntity.Reconstruct(userId, "Nami", 20);
            _repository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
                       .Returns(user);

            await _handler.HandleAsync(new GetUserByIdQuery(userId));

            await _repository.Received(1).GetByIdAsync(userId, Arg.Any<CancellationToken>());
        }
    }
}