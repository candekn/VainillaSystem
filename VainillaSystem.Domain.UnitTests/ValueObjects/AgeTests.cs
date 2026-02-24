using VainillaSystem.Domain.ValueObjects;

namespace VainillaSystem.Domain.UnitTests.ValueObjects
{
    public class AgeTests
    {
        [Theory]
        [InlineData(13)]
        [InlineData(50)]
        [InlineData(99)]
        public void Create_WithValidAge_ReturnsAge(int value)
        {
            var age = Age.Create(value);
            Assert.Equal(value, age.GetValue());
        }

        [Theory]
        [InlineData(12)]
        [InlineData(0)]
        [InlineData(-1)]
        public void Create_WithAgeBelowMinimum_ThrowsArgumentException(int value)
        {
            Assert.Throws<ArgumentException>(() => Age.Create(value));
        }

        [Theory]
        [InlineData(100)]
        [InlineData(150)]
        public void Create_WithAgeAboveMaximum_ThrowsArgumentException(int value)
        {
            Assert.Throws<ArgumentException>(() => Age.Create(value));
        }

        [Fact]
        public void GetValue_ReturnsUnderlyingInt()
        {
            var age = Age.Create(25);
            Assert.Equal(25, age.GetValue());
        }
    }
}
