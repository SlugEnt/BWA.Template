using SlugEnt.BWA.Entities;

namespace SlugEnt.HR.NextGen.Entities.Models;

/// <summary>
/// This is a sample Lookup Entity.  A Lookup Entity is one that typically is Read from and rarely updated.
/// </summary>
public class SampleInt : AbstractEntityLookups, IEntity 
{

    /// <summary>
    /// The full name of the entity.    This is required.  It can be overridden in this derived class if needed.
    /// </summary>
    public override string FullName { get { return Name; } }


    /// <summary>
    /// Writes the Entity to string.  This is used for debugging and logging.
    /// </summary>
    /// <returns></returns>
    public override string ToString() { return FullName; }


    public override bool AllowDelete
    {
        get { return true; }
    }
}