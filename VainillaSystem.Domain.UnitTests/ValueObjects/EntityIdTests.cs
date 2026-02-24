using VainillaSystem.Domain.ValueObjects;

namespace VainillaSystem.Domain.UnitTests.ValueObjects
{
    public class EntityIdTests
    {
        [Fact]
        public void Create_WithValidGuid_ReturnsEntityId()
        {
            var guid = Guid.NewGuid();
            var entityId = EntityId.Create(guid);
            Assert.Equal(guid, entityId.GetValue());
        }

        [Fact]
        public void Create_WithEmptyGuid_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => EntityId.Create(Guid.Empty));
        }

        [Fact]
        public void GetValue_ReturnsUnderlyingGuid()
        {
            var guid = Guid.NewGuid();
            var entityId = EntityId.Create(guid);
            Assert.Equal(guid, entityId.GetValue());
        }
    }
}
