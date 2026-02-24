namespace VainillaSystem.Domain.ValueObjects
{
    /// <summary>
    /// Value Object representing a person's age.
    /// Enforces the business rule that age must be between 13 and 99 (inclusive).
    /// </summary>
    public class Age
    {
        private int _value;

        private Age(int value)
        {
            _value = value;
        }

        /// <summary>
        /// Creates a new <see cref="Age"/> from the given integer value.
        /// </summary>
        /// <param name="value">The age in years. Must be between 13 and 99 (inclusive).</param>
        /// <returns>A valid <see cref="Age"/> instance.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="value"/> is less than 13 or greater than 99.
        /// </exception>
        public static Age Create(int value)
        {
            if (value < 13 || value > 99)
            {
                throw new ArgumentException("Age must be between 13 and 99.");
            }
            return new Age(value);
        }

        /// <summary>
        /// Returns the underlying integer value representing the age in years.
        /// </summary>
        /// <returns>The raw <see cref="int"/> wrapped by this value object.</returns>
        public int GetValue() => _value;
    }
}