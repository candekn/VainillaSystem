namespace VainillaSystem.Domain.ValueObjects
{
    /// <summary>
    /// Value Object representing the unique identifier of a domain entity.
    /// Wraps a <see cref="Guid"/> and enforces that it must not be empty.
    /// </summary>
    public class EntityId
    {
        private Guid _value;

        private EntityId(Guid value)
        {
            _value = value;
        }

        /// <summary>
        /// Creates a new <see cref="EntityId"/> from the given <paramref name="guid"/>.
        /// </summary>
        /// <param name="guid">The GUID to wrap. Must not be <see cref="Guid.Empty"/>.</param>
        /// <returns>A valid <see cref="EntityId"/> instance.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="guid"/> equals <see cref="Guid.Empty"/>.
        /// </exception>
        public static EntityId Create(Guid guid)
        {
            if (Guid.Empty.Equals(guid))
            {
                throw new ArgumentException("Guid cannot be empty.", nameof(guid));
            }
            return new EntityId(guid);
        }

        /// <summary>
        /// Returns the underlying <see cref="Guid"/> value.
        /// </summary>
        /// <returns>The raw <see cref="Guid"/> wrapped by this value object.</returns>
        public Guid GetValue() => _value;
    }
}