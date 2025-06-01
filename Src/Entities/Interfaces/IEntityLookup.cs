
namespace SlugEnt.BWA.Entities;

/// <summary>
/// Defines a lookup entity that has a Name property.
/// </summary>
public interface IEntityLookupInt : IEntityInt
{
 
    /// <summary>
    /// Name of the object.  This is the value that will be displayed in the drop down list.
    /// </summary>
    public string Name { get; set; }

}