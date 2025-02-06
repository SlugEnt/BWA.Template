using System.ComponentModel.DataAnnotations.Schema;

namespace SlugEnt.BWA.Entities;

/// <summary>
///     The base for all HRProject Entities that helps to determine a few basics about each entity.
/// </summary>
public class AbstractBaseEntity
{
    /// <summary>
    ///     If true, the entity contains all available Audit fields and is considered an Auditable entity.
    /// </summary>
    [NotMapped]
    public bool IsAuditable { get; protected set; }
};