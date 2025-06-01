using ByteAether.Ulid;

namespace SlugEnt.BWA.Entities;

/// <summary>
/// Represents a Universally Unique Lexicographically Sortable Identifier (ULID) based entity.
/// </summary>
public interface IEntityULID : IEntity
{
    /// <summary>
    /// The unique identifier for this object.
    /// </summary>
    public Ulid Id { get; set; }

    /// <summary>
    /// Returns the ID of the Entity as a string.
    /// </summary>
    public string KeyId => Id.ToString();
}