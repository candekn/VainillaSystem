using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using VainillaSystem.Application.Features.UserCQRS.Queries;
using VainillaSystem.Domain.Interfaces;
using VainillaSystem.Infrastructure.Behaviors;

namespace VainillaSystem.Infrastructure.UnitTests.Behaviors
{
    public class LoggingBehaviorTests
    {
        private readonly IRequestHandler<GetUserByIdQuery, UserDto> _innerHandler;
        private readonly LoggingBehavior<GetUserByIdQuery, UserDto> _behavior;

        public LoggingBehaviorTests()
        {
            _innerHandler = Substitute.For<IRequestHandler<GetUserByIdQuery, UserDto>>();
            _behavior = new LoggingBehavior<GetUserByIdQuery, UserDto>(
                _innerHandler,
                NullLogger<LoggingBehavior<GetUserByIdQuery, UserDto>>.Instance);
        }

        [Fact]
        public async Task HandleAsync_CallsInnerHandler()
        {
            var query = new GetUserByIdQuery(Guid.NewGuid());
            var expected = new UserDto(query.UserId, "Luffy", 19);
            _innerHandler.HandleAsync(query, Arg.Any<CancellationToken>()).Returns(expected);

            await _behavior.HandleAsync(query);

            await _innerHandler.Received(1).HandleAsync(query, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task HandleAsync_ReturnsResultFromInnerHandler()
        {
            var query = new GetUserByIdQuery(Guid.NewGuid());
            var expected = new UserDto(query.UserId, "Nami", 20);
            _innerHandler.HandleAsync(query, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _behavior.HandleAsync(query);

            Assert.Equal(expected.Name, result.Name);
            Assert.Equal(expected.Age, result.Age);
        }

        [Fact]
        public async Task HandleAsync_WhenInnerHandlerThrows_PropagatesException()
        {
            var query = new GetUserByIdQuery(Guid.NewGuid());
            _innerHandler.HandleAsync(query, Arg.Any<CancellationToken>())
                         .Returns<UserDto>(_ => throw new KeyNotFoundException("User not found."));

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _behavior.HandleAsync(query));
        }
    }
}