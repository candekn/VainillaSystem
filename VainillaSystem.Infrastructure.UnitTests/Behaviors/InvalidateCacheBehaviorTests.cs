using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using VainillaSystem.Application.Features.UserCQRS.Commands;
using VainillaSystem.Domain.Interfaces;
using VainillaSystem.Infrastructure.Behaviors;

namespace VainillaSystem.Infrastructure.UnitTests.Behaviors
{
    public class InvalidateCacheBehaviorTests
    {
        private readonly IMemoryCache _cache;
        private readonly IRequestHandler<CreateUserCommand, CreateUserResponse> _innerHandler;
        private readonly InvalidateCacheBehavior<CreateUserCommand, CreateUserResponse> _behavior;

        public InvalidateCacheBehaviorTests()
        {
            var cacheOptions = new MemoryCacheOptions();
            _cache = new MemoryCache(cacheOptions);
            _innerHandler = Substitute.For<IRequestHandler<CreateUserCommand, CreateUserResponse>>();
            _behavior = new InvalidateCacheBehavior<CreateUserCommand, CreateUserResponse>(
                _innerHandler,
                _cache,
                NullLogger<InvalidateCacheBehavior<CreateUserCommand, CreateUserResponse>>.Instance);
        }

        [Fact]
        public async Task HandleAsync_AfterExecution_RemovesCacheKeysFromCommand()
        {
            var cacheKey = "users-list";
            _cache.Set(cacheKey, "some-cached-value");

            var command = new CreateUserCommand("Luffy", 19);
            var response = new CreateUserResponse(Guid.NewGuid(), "Luffy", 19);
            _innerHandler.HandleAsync(command, Arg.Any<CancellationToken>()).Returns(response);

            await _behavior.HandleAsync(command);

            Assert.False(_cache.TryGetValue(cacheKey, out _));
        }

        [Fact]
        public async Task HandleAsync_CallsInnerHandlerBeforeInvalidation()
        {
            var command = new CreateUserCommand("Luffy", 19);
            var response = new CreateUserResponse(Guid.NewGuid(), "Luffy", 19);
            _innerHandler.HandleAsync(command, Arg.Any<CancellationToken>()).Returns(response);

            await _behavior.HandleAsync(command);

            await _innerHandler.Received(1).HandleAsync(command, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task HandleAsync_ReturnsResponseFromInnerHandler()
        {
            var expected = new CreateUserResponse(Guid.NewGuid(), "Luffy", 19);
            var command = new CreateUserCommand("Luffy", 19);
            _innerHandler.HandleAsync(command, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _behavior.HandleAsync(command);

            Assert.Equal(expected.Id, result.Id);
            Assert.Equal(expected.Name, result.Name);
        }
    }
}