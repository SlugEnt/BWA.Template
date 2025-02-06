using System.ComponentModel.DataAnnotations;

namespace SlugEnt.BWA.Entities;

/// <summary>
///     Base class that has a small integer as key, an IsActive property and audit model properties
/// </summary>
public class Abstract_IIDSmallInt_Audit : AbstractBaseEntity, IActiveModel, IIdSmallIntModel, IAuditModel
{
    public Abstract_IIDSmallInt_Audit() { IsAuditable = true; }


    /// <summary>
    ///     True if the object is currently active, false if not.
    /// </summary>
    public bool IsActive { get; set; }


    /// <summary>
    ///     When the object was created
    /// </summary>
    public DateTime CreatedDateTime { get; set; }


    /// <summary>
    ///     When the object was last updated
    /// </summary>
    public DateTime? LastModifiedDateTime { get; set; }

    /// <summary>
    ///     The unique identifier for this object.
    /// </summary>
    [Key]
    public short Id { get; set; }
}