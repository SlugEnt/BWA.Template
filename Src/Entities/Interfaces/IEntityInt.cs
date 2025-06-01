namespace SlugEnt.BWA.Entities;

public interface IEntityInt : IEntity
{
    /// <summary>
    /// The unique identifier for this object.
    /// </summary>
    public int Id { get; set; }

    public string KeyId => Id.ToString();
}