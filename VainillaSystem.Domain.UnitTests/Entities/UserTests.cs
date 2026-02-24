using VainillaSystem.Domain.Entities;

namespace VainillaSystem.Domain.UnitTests.Entities
{
    public class UserTests
    {
        [Fact]
        public void Create_WithValidData_ReturnsUserWithCorrectProperties()
        {
            var user = User.Create("Luffy", 19);

            Assert.Equal("Luffy", user.GetName().GetValue());
            Assert.Equal(19, user.GetAge().GetValue());
            Assert.NotEqual(Guid.Empty, user.GetId().GetValue());
        }

        [Fact]
        public void Create_GeneratesUniqueId_ForEachUser()
        {
            var user1 = User.Create("Luffy", 19);
            var user2 = User.Create("Zoro", 21);

            Assert.NotEqual(user1.GetId().GetValue(), user2.GetId().GetValue());
        }

        [Fact]
        public void Create_WithInvalidAge_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => User.Create("Luffy", 5));
        }

        [Fact]
        public void Create_WithEmptyName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => User.Create(string.Empty, 19));
        }

        [Fact]
        public void Reconstruct_WithExistingId_PreservesId()
        {
            var existingId = Guid.NewGuid();

            var user = User.Reconstruct(existingId, "Nami", 20);

            Assert.Equal(existingId, user.GetId().GetValue());
            Assert.Equal("Nami", user.GetName().GetValue());
            Assert.Equal(20, user.GetAge().GetValue());
        }

        [Fact]
        public void Reconstruct_WithEmptyGuid_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => User.Reconstruct(Guid.Empty, "Luffy", 19));
        }

        [Fact]
        public void ToString_ReturnsFormattedString()
        {
            var user = User.Create("Luffy", 19);
            var result = user.ToString();

            Assert.Contains("Luffy", result);
            Assert.Contains("19", result);
        }
    }
}
