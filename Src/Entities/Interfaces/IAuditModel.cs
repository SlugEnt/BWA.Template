using SlugEnt.BWA.Entities.Models;
using SlugEnt.HR.NextGen.Entities.Models;

namespace SlugEnt.BWA.Entities;

/// <summary>
///     Describes an object that contains Audit related properties
/// </summary>
public interface IAuditModel
{
    /// <summary>
    ///     When the object was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    ///     When the object was last updated
    /// </summary>
    public DateTimeOffset? LastModifiedAt { get; set; }

    /// <summary>
    /// Who created the object
    /// </summary>
    public int? CreatedById { get; set; }

    /// <summary>
    /// Who last modified the object
    /// </summary>
    public int? LastModifiedById { get; set; }

    /// <summary>
    /// Who Created the Object
    /// </summary>
    //public User? CreatedByUser { get; set; }


    /// <summary>
    /// Who Modified the object
    /// </summary>
    //public User? LastModifiedByUser { get; set; }
}