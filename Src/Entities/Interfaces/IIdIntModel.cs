namespace SlugEnt.BWA.Entities;

/// <summary>
///     Interface describing objects that have integer Key Id's
/// </summary>
public interface IIdIntModel
{
    /// <summary>
    ///     The unique identifier for this object.
    /// </summary>
    public int Id { get; set; }
}