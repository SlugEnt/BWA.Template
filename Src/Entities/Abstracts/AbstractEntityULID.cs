using ByteAether.Ulid;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SlugEnt.BWA.Entities;

/// <summary>
/// Abstract class for entities that use ULID as their identifier.
/// </summary>
public class AbstractEntityULID : Abstract_Entity, IEntityULID
{
    /// <summary>
    /// Default Constructor
    /// </summary>
    public AbstractEntityULID() : base()
    {
        Id = Ulid.New();
        CreatedAt = DateTime.UtcNow;
    }


    /// <summary>
    /// The unique identifier for a given object.
    /// </summary>
    [Key]
    [Column(Order = 10)]
    public Ulid Id { get; set; }

    /// <summary>
    /// FullName of the Entity
    /// </summary>
    public override string FullName => throw new NotImplementedException();


    /// <summary>
    /// Returns the String version of the Id
    /// </summary>
    public override string KeyId => Id.ToString();


    [NotMapped] public static Ulid MaxValue = Ulid.Parse("99999999999999999999999999");
    [NotMapped] public static Ulid MinValue = Ulid.Parse("00000000000000000000000000");


}

