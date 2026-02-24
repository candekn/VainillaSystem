using VainillaSystem.Domain.ValueObjects;

namespace VainillaSystem.Domain.UnitTests.ValueObjects
{
    public class NameTests
    {
        [Fact]
        public void Create_WithValidName_ReturnsName()
        {
            var name = Name.Create("Luffy");
            Assert.Equal("Luffy", name.GetValue());
        }

        [Fact]
        public void Create_WithNullName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Name.Create(null!));
        }

        [Fact]
        public void Create_WithEmptyName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Name.Create(string.Empty));
        }

        [Fact]
        public void GetValue_ReturnsUnderlyingString()
        {
            var name = Name.Create("Zoro");
            Assert.Equal("Zoro", name.GetValue());
        }
    }
}
