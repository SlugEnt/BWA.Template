using System.ComponentModel.DataAnnotations;
using SlugEnt.BWA.Entities;

namespace SlugEnt.HR.NextGen.Entities.Models;

/// <summary>
/// Sample of a class based upon a String Value
/// </summary>
public class SampleString : AbstractEntityStr, IEntityStr
{
    /// <summary>
    /// The Primary Key.
    /// </summary>
    [MaxLength(12)] public override string Id { get; set; }

    public string SampleName { get; set; }
    public int SampleNumber { get; set; }


    public override string FullName { get { return SampleName + " [ " + SampleNumber + " ]"; } }

    public override string ToString() { return FullName; }
}