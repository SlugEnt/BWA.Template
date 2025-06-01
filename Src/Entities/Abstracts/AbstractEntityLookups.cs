using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SlugEnt.BWA.Entities;

/// <summary>
/// Abstract class that defines basic capabilities of all lookup tables.
/// It takes the Abstract_Entity as a base class and adds a Name property.
/// </summary>
public abstract class AbstractEntityLookups : AbstractEntityInt, IEntityLookupInt
{
    /// <summary>
    /// Constructor
    /// </summary>
  //  public AbstractEntityLookups() : base() { }


    /// <summary>
    ///     The name of the object
    /// </summary>
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(Constants.NAME_LONG_LENGTH)]
    [Column(Order = 20)]
    public string Name { get; set; } = "";

}