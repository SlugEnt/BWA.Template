using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SlugEnt.BWA.Entities;



/// <summary>
/// Represents an entity with an integer identifier.
/// </summary>
public  class AbstractEntityInt : Abstract_Entity, IEntityInt
{
    /// <summary>
    /// Default constructor for the AbstractEntityInt class.
    /// </summary>
    /// <remarks>This is required to support Logger[T] dependency injection</T></remarks>
    public AbstractEntityInt() : base() { }


    /// <summary>
    ///     The unique identifier for a given object.
    /// </summary>
    [Key]
    [Column(Order = 10)]
    public int Id { get; set; }


    /// <summary>
    /// equality Comparer.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        if (obj is not AbstractEntityInt other)
            return false;

        return Id == other.Id;
    }


    /// <summary>
    /// Compares two objects.  Must be of same type.  
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(AbstractEntityInt? other)
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
    public override string KeyId { get { return Id.ToString(); } }

    public override string FullName => throw new NotImplementedException();
}