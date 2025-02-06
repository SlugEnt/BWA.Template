using SlugEnt.BWA.Entities.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SlugEnt.BWA.Entities;

/// <summary>
///     Base class that has regular integer as key, an IsActive property and audit model properties
/// </summary>
public class Abstract_IIDInt_Audit : AbstractBaseEntity, IActiveModel, IIdIntModel, IAuditModel
{
    public Abstract_IIDInt_Audit() { IsAuditable = true; }

    // TODO Update this to use your User / Person model
    [ForeignKey("CreatedByPersonId")] public User? CreatedByPerson { get; set; }



    /// <summary>
    ///     The Object - person who last updated this entity
    /// </summary>
    public int? CreatedByPersonId { get; set; }

    [ForeignKey("LastModifiedByPersonId")] public User? LastModifiedByPerson { get; set; }



    // Relationships



    /// <summary>
    ///     The Object - person who last updated this entity
    /// </summary>
    public int? LastModifiedByPersonId { get; set; }


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
    ///     The unique identifier for a given object.
    /// </summary>
    [Key]
    public int Id { get; set; }
}