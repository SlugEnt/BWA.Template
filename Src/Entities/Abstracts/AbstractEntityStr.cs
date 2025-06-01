using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SlugEnt.BWA.Entities;

public class AbstractEntityStr : Abstract_Entity, IEntityStr
{

    /// <summary>
    /// Default constructor for the AbstractEntityStr class.
    /// </summary>
    /// <remarks>This is required to support Logger[T] dependency injection</T></remarks>
    public AbstractEntityStr() : base() { }


    /// <summary>
    ///     The unique identifier for a given object.
    /// </summary>
    /// <remarks>This should be overridden in a derived class to set the column Length.</remarks>
    [Key]
    [Column(Order = 10)]
    public virtual string Id { get; set; }


    /// <summary>
    /// equality Comparer.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        if (obj is not AbstractEntityStr other)
            return false;

        return Id == other.Id;
    }


    /// <summary>
    /// Compares two objects.  Must be of same type.  
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(AbstractEntityStr? other)
    {
        if (other is null)
            return false;
        if (GetType() != other.GetType())
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return Id == other.Id;
    }


    /// <summary>
    /// Returns the hash code for the object.  This is used for collections and comparisons.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }


    /// <summary>
    /// Returns the Id of the Entity as a string.  Useful for messaging and error displays.
    /// </summary>
    public override string KeyId { get { return Id; } }

    /// <summary>
    /// Returns the full name, should be overridden in derived classes.
    /// </summary>
    public override string FullName => throw new NotImplementedException();
}