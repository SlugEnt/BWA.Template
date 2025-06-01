using SlugEnt.BWA.Entities;

namespace SlugEnt.HR.NextGen.Entities.Models;

public class SampleUlid : AbstractEntityULID, IEntityULID
{
    /// <summary>
    /// The Primary Key.
    /// </summary>
    //public Ulid Id { get; set; }

    public string SampleName { get; set; }

    public int SampleNumber { get; set; }


    public override string FullName { get { return SampleName + " [ " + SampleNumber + " ]"; } }

    public override string ToString() { return FullName; }


}