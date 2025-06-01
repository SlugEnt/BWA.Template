namespace SlugEnt.BWA.Entities;

/// <summary>
/// Class that adds CreatedBy and LastModified Fields to Entity.
/// </summary>
public abstract class AbstractAuditableEntity : AbstractBaseEntity, IAuditModel
{
    /// <summary>
    /// Base Constructor.  Sets' IsAuditable to true.
    /// </summary>
    public AbstractAuditableEntity() : base() { IsAuditable = true; }


    //[ForeignKey("CreatedById")] public Person? CreatedByPerson { get; set; }

    /// <summary>
    ///     The Object - person who last updated this entity
    /// </summary>
    public int? CreatedById { get; set; }


    //[ForeignKey("LastModifiedById")] public Person? LastModifiedByPerson { get; set; }


    /// <summary>
    ///     The Object - person who last updated this entity
    /// </summary>
    public int? LastModifiedById { get; set; }

    /// <summary>
    ///     When the object was created
    /// </summary>
    [SqlDefaultValue("getutcdate()")]
    public DateTimeOffset CreatedAt { get; set; }



    /// <summary>
    ///     When the object was last updated
    /// </summary>
    [SqlDefaultValue("getutcdate()")]
    public DateTimeOffset? LastModifiedAt { get; set; }

}