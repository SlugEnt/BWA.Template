using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SlugEnt.BWA.Entities;

/// <summary>
/// Base class for all Entity Style objects
/// </summary>
public abstract class Abstract_Entity : AbstractAuditableEntity, IEntity
{
    /// <summary>
    /// This is the full name of the entity.  It is used for display purposes in the UI.
    /// </summary>
    [NotMapped]
    [MaxLength(175)]
    public abstract string FullName { get; }


    /// <summary>
    /// Displays identifying information for the entity.  Note:  This shows up in debugger for instance.
    /// </summary>
    /// <returns></returns>
    public override string ToString() { return FullName; }



    /// <summary>
    /// Equality Comparer
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(Abstract_Entity? left,
                                   Abstract_Entity? right)
    {
        if (left is null)
            if (right is null)
                return true;
            else
                return false;

        return Equals(left, right);
    }


    /// <summary>
    /// Not Equality comparer
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(Abstract_Entity? left,
                                   Abstract_Entity? right) =>
        !(left == right);


    /// <summary>
    /// Whether this entity allows itself to be deleted from database tables.  By default we do not allow deletes, only de-activates.
    /// </summary>
    [NotMapped]
    public virtual bool AllowDelete { get; protected set; } = false;


    /// <summary>
    /// Returns the Id of the Entity.  Must be overridden by derived classes.
    /// </summary>
    public abstract string KeyId { get; }
}