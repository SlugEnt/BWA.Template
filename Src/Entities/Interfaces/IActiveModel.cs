namespace SlugEnt.BWA.Entities;

/// <summary>
///     Interface describing an object that has an IsActive property
/// </summary>
public interface IActiveModel
{
    /// <summary>
    ///     True if the object is currently active, false if not.
    /// </summary>
    public bool IsActive { get; set; }
}