using VainillaSystem.Domain.Entities;
using VainillaSystem.Infrastructure.Persistence;

namespace VainillaSystem.Infrastructure.UnitTests.Persistence
{
    public class InMemoryUserRepositoryTests
    {
        private readonly InMemoryUserRepository _repository = new();

        [Fact]
        public async Task AddAsync_ThenGetByIdAsync_ReturnsCorrectUser()
        {
            var user = User.Create("Luffy", 19);

            await _repository.AddAsync(user);
            var found = await _repository.GetByIdAsync(user.GetId().GetValue());

            Assert.NotNull(found);
            Assert.Equal(user.GetId().GetValue(), found.GetId().GetValue());
            Assert.Equal("Luffy", found.GetName().GetValue());
            Assert.Equal(19, found.GetAge().GetValue());
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
        {
            var result = await _repository.GetByIdAsync(Guid.NewGuid());
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_MultipleUsers_AllCanBeRetrieved()
        {
            var user1 = User.Create("Luffy", 19);
            var user2 = User.Create("Zoro", 21);

            await _repository.AddAsync(user1);
            await _repository.AddAsync(user2);

            var found1 = await _repository.GetByIdAsync(user1.GetId().GetValue());
            var found2 = await _repository.GetByIdAsync(user2.GetId().GetValue());

            Assert.NotNull(found1);
            Assert.NotNull(found2);
            Assert.Equal("Luffy", found1.GetName().GetValue());
            Assert.Equal("Zoro", found2.GetName().GetValue());
        }

        [Fact]
        public async Task AddAsync_OverwritingExistingId_UpdatesEntry()
        {
            var id = Guid.NewGuid();
            var user1 = User.Reconstruct(id, "Luffy", 19);
            var user2 = User.Reconstruct(id, "Luffy Updated", 20);

            await _repository.AddAsync(user1);
            await _repository.AddAsync(user2);

            var found = await _repository.GetByIdAsync(id);

            Assert.NotNull(found);
            Assert.Equal("Luffy Updated", found.GetName().GetValue());
            Assert.Equal(20, found.GetAge().GetValue());
        }
    }
}
