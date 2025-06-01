
namespace SlugEnt.BWA.Entities;

public interface IEntityGuid : IEntity
{
    /// <summary>
    /// The unique identifier for this object.
    /// </summary>
    public Guid Id { get; set; }

    public string KeyId => Id.ToString();
}