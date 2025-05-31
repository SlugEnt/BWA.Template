namespace SlugEnt.BWA.Entities;


/// <summary>
/// Interface for an Entity with a long based Id field.
/// </summary>
public interface IEntityLong : IEntity
{
    /// <summary>
    /// The unique identifier for this object.
    /// </summary>
    public long Id { get; set; }


    /// <summary>
    /// Returns the ID of the Entity as a string.
    /// </summary>
    public string KeyId => Id.ToString();

}
