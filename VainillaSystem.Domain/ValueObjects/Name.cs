namespace VainillaSystem.Domain.ValueObjects
{
    /// <summary>
    /// Value Object representing a person's name.
    /// Enforces that the name cannot be null or empty.
    /// </summary>
    public class Name
    {
        private string _value;

        private Name(string value)
        {
            _value = value;
        }

        /// <summary>
        /// Creates a new <see cref="Name"/> from the given string value.
        /// </summary>
        /// <param name="value">The name string. Must not be <see langword="null"/> or empty.</param>
        /// <returns>A valid <see cref="Name"/> instance.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/> or an empty string.
        /// </exception>
        public static Name Create(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Name cannot be null or empty.");
            }
            return new Name(value);
        }

        /// <summary>
        /// Returns the underlying string value of this name.
        /// </summary>
        /// <returns>The raw <see cref="string"/> wrapped by this value object.</returns>
        public string GetValue()
        {
            return _value;
        }
    }
}