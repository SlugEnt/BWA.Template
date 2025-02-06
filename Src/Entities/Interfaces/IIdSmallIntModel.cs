namespace SlugEnt.BWA.Entities;

/// <summary>
///     Represents an object that must have an Id field of type Small Integer
/// </summary>
public interface IIdSmallIntModel
{
    /// <summary>
    ///     The unique identifier for this object.
    /// </summary>
    public short Id { get; set; }
}