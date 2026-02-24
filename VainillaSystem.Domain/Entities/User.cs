using VainillaSystem.Domain.ValueObjects;

namespace VainillaSystem.Domain.Entities
{
    /// <summary>
    /// Aggregate root representing a registered user in the system.
    /// All state changes must go through the entity's public methods to preserve invariants.
    /// </summary>
    public class User
    {
        private EntityId _id;
        private Name _name;
        private Age _age;

        private User(EntityId id, Name name, Age age)
        {
            _id = id;
            _name = name;
            _age = age;
        }

        /// <summary>
        /// Creates a new <see cref="User"/> with a freshly generated unique identifier.
        /// Validates all inputs through their respective Value Objects.
        /// </summary>
        /// <param name="name">The user's display name. Must not be null or empty.</param>
        /// <param name="age">The user's age in years. Must be between 13 and 99.</param>
        /// <returns>A new <see cref="User"/> instance with a unique <see cref="EntityId"/>.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="name"/> is null/empty or <paramref name="age"/> is out of range.
        /// </exception>
        public static User Create(string name, int age)
        {
            var nameVo = Name.Create(name);
            var ageVo  = Age.Create(age);
            var idVo   = EntityId.Create(Guid.NewGuid());
            return new User(idVo, nameVo, ageVo);
        }

        /// <summary>
        /// Reconstructs a <see cref="User"/> from persisted data without generating a new identity.
        /// Use this factory when rehydrating an entity from a repository.
        /// </summary>
        /// <param name="id">The persisted unique identifier. Must not be <see cref="Guid.Empty"/>.</param>
        /// <param name="name">The persisted display name.</param>
        /// <param name="age">The persisted age in years.</param>
        /// <returns>A <see cref="User"/> instance with the provided identity and data.</returns>
        public static User Reconstruct(Guid id, string name, int age)
        {
            return new User(EntityId.Create(id), Name.Create(name), Age.Create(age));
        }

        /// <summary>
        /// Gets the unique identifier of this user.
        /// </summary>
        /// <returns>The <see cref="EntityId"/> value object.</returns>
        public EntityId GetId()   => _id;

        /// <summary>
        /// Gets the display name of this user.
        /// </summary>
        /// <returns>The <see cref="Name"/> value object.</returns>
        public Name     GetName() => _name;

        /// <summary>
        /// Gets the age of this user.
        /// </summary>
        /// <returns>The <see cref="Age"/> value object.</returns>
        public Age      GetAge()  => _age;

        /// <summary>
        /// Applies new values to the user's mutable properties.
        /// Receives already-validated Value Objects to keep the entity boundary clean.
        /// </summary>
        /// <param name="name">The new name to assign.</param>
        /// <param name="age">The new age to assign.</param>
        public void UpdateDetails(Name name, Age age)
        {
            _name = name;
            _age  = age;
        }

        /// <summary>
        /// Returns a human-readable string representation of the user.
        /// </summary>
        /// <returns>A string in the format <c>Name (Age)</c>.</returns>
        public override string ToString() => $"{_name.GetValue()} ({_age.GetValue()})";
    }
}