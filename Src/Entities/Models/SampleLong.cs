using SlugEnt.BWA.Entities;

namespace SlugEnt.HR.NextGen.Entities.Models;

/// <summary>
/// Sample of a class based upon a Long Value
/// </summary>
public class SampleLong : AbstractEntityLong, IEntityLong
{
    public string SampleName { get; set; }

    public int SampleNumber { get; set; }


    public override string FullName { get { return SampleName + " [ " + SampleNumber + " ]"; } }

    public override string ToString() { return FullName; }


}