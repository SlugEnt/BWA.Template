namespace SlugEnt.BWA.Entities;


/// <summary>
/// Used to communicate entity information and actions to the API or other layers.  Use this when sending the whole entity object is not necessary or when you only need to send the Id of the entity along with other information.
/// </summary>
public class EntityTransportById
{
    /// <summary>
    /// This is the Id of the entity that is being transported.  This is typically the primary key of the entity in the database.
    /// </summary>
    public string EntityId { get; set; }

    /// <summary>
    /// This is the PersonId of the user who is performing the action on the entity.  This can be used for auditing or tracking purposes.
    /// </summary>
    public int ChangeMadeByPersonId { get; set; } 
}