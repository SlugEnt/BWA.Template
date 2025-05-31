namespace SlugEnt.BWA.Entities.NonModel;

/// <summary>
/// This is a transport object that is used to pass an entity along with additional information.  Typically
/// used at the API layer to send an entity along with the PersonId of the user who is performing the action on the entity.
/// <para>Note, if sending the whole Entity is not necessary, then use the EntityTransportById object</para>
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class EntityTransport<TEntity> where TEntity : IEntity
{
    /// <summary>
    /// This is the entity to be sent to the API or other layer.  This is typically an instance of an entity that implements the IEntity interface.
    /// </summary>
    public TEntity Entity { get; set; }


    /// <summary>
    /// Id of the person who is performing the action on the entity.  This can be used for auditing or tracking purposes.
    /// </summary>
    public int ChangeMadeByPersonId { get; set; } 
}