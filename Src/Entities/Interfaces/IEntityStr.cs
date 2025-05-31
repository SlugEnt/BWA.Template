namespace SlugEnt.BWA.Entities;

/// <summary>
/// Interface for an Entity with a string based Id field.
/// </summary>
public interface IEntityStr : IEntity
{
    /// <summary>
    /// The unique identifier for this object.
    /// </summary>
    public string Id { get; set; }


    /// <summary>
    /// Returns the ID of the Entity as a string.
    /// </summary>
    public string KeyId => Id;
}