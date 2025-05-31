using System.ComponentModel.DataAnnotations.Schema;

namespace SlugEnt.BWA.Entities;

/// <summary>
///     Defines a base class that represents an Entity
/// </summary>
public class AbstractBaseEntity
{
    /// <summary>
    ///     If true, the entity contains all available Audit fields and is considered an Auditable entity.
    /// </summary>
    [NotMapped]
    public bool IsAuditable { get; protected set; }

    /// <summary>
    ///     True if the object is currently active, false if not.
    /// </summary>
    [Column(Order = 30)]
    public bool IsActive { get; set; }

}