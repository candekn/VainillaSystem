using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using VainillaSystem.Application.Features.UserCQRS.Queries;
using VainillaSystem.Domain.Interfaces;
using VainillaSystem.Infrastructure.Behaviors;

namespace VainillaSystem.Infrastructure.UnitTests.Behaviors
{
    public class CachingBehaviorTests
    {
        private readonly IMemoryCache _cache;
        private readonly IRequestHandler<GetUserByIdQuery, UserDto> _innerHandler;
        private readonly CachingBehavior<GetUserByIdQuery, UserDto> _behavior;

        public CachingBehaviorTests()
        {
            var cacheOptions = new MemoryCacheOptions();
            _cache = new MemoryCache(cacheOptions);
            _innerHandler = Substitute.For<IRequestHandler<GetUserByIdQuery, UserDto>>();
            _behavior = new CachingBehavior<GetUserByIdQuery, UserDto>(
                _innerHandler,
                _cache,
                NullLogger<CachingBehavior<GetUserByIdQuery, UserDto>>.Instance);
        }

        [Fact]
        public async Task HandleAsync_OnCacheMiss_CallsInnerHandler()
        {
            var query = new GetUserByIdQuery(Guid.NewGuid());
            var expected = new UserDto(query.UserId, "Luffy", 19);
            _innerHandler.HandleAsync(query, Arg.Any<CancellationToken>()).Returns(expected);

            await _behavior.HandleAsync(query);

            await _innerHandler.Received(1).HandleAsync(query, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task HandleAsync_OnCacheHit_DoesNotCallInnerHandler()
        {
            var query = new GetUserByIdQuery(Guid.NewGuid());
            var expected = new UserDto(query.UserId, "Luffy", 19);
            _innerHandler.HandleAsync(query, Arg.Any<CancellationToken>()).Returns(expected);

            await _behavior.HandleAsync(query);
            await _behavior.HandleAsync(query);

            await _innerHandler.Received(1).HandleAsync(query, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task HandleAsync_OnCacheHit_ReturnsCachedResult()
        {
            var query = new GetUserByIdQuery(Guid.NewGuid());
            var expected = new UserDto(query.UserId, "Luffy", 19);
            _innerHandler.HandleAsync(query, Arg.Any<CancellationToken>()).Returns(expected);

            await _behavior.HandleAsync(query);
            var cached = await _behavior.HandleAsync(query);

            Assert.Equal(expected.Id, cached.Id);
            Assert.Equal(expected.Name, cached.Name);
        }

        [Fact]
        public async Task HandleAsync_FirstCall_ReturnsResultFromInnerHandler()
        {
            var query = new GetUserByIdQuery(Guid.NewGuid());
            var expected = new UserDto(query.UserId, "Nami", 20);
            _innerHandler.HandleAsync(query, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _behavior.HandleAsync(query);

            Assert.Equal("Nami", result.Name);
        }
    }
}