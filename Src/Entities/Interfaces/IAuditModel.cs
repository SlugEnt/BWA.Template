namespace SlugEnt.BWA.Entities;

/// <summary>
///     Describes an object that contains Audit related properties
/// </summary>
public interface IAuditModel
{
    /// <summary>
    ///     When the object was created
    /// </summary>
    public DateTime CreatedDateTime { get; set; }

    /// <summary>
    ///     When the object was last updated
    /// </summary>
    public DateTime? LastModifiedDateTime { get; set; }
}