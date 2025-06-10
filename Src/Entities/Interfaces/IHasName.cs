namespace SlugEnt.BWA.Entities;

/// <summary>
/// Defines an interface for an entity that has a name property.
/// </summary>
public interface IHasName
{
    /// <summary>
    /// Name of the object.  This is the value that will be displayed in the drop down list.
    /// </summary>
    public string Name { get; set; }
}