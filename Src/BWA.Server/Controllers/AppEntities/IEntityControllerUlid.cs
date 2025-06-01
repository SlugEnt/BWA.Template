using ByteAether.Ulid;
using Microsoft.AspNetCore.Mvc;
using SlugEnt.BWA.Entities;

namespace BWA.Server.Controllers;

/// <summary>
/// Interface for a controller that manages an Entity with a string based Id.
/// </summary>
/// <typeparam name="TEntityUlid"></typeparam>
public interface IEntityControllerUlid<TEntityUlid> : IEntityController<TEntityUlid> where TEntityUlid : class, IEntityULID
{
    /// <summary>
    /// Gets an Entity by Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<ActionResult<TEntityUlid>> GetById(Ulid id);


    /// <summary>
    /// Gets an Entity by Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<ActionResult<TEntityUlid>> GetById(string id);
    
    


    /// <summary>
    /// Performs a hard delete on the Entity.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="by"></param>
    /// <returns></returns>
    public Task<ActionResult> DeleteAsync(Ulid id, int by);




    /// <summary>
    /// Performs a hard delete on the Entity.  This accepts the string version of the Id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="by"></param>
    /// <returns></returns>public Task<ActionResult> DeleteAsync(string id, int by);


    /// <summary>
    /// Deactives an entity, which is essentially a soft delete
    /// </summary>
    /// <param name="id"></param>
    /// <param name="changedByPersonId"></param>
    /// <returns></returns>
    public Task<ActionResult> DeactivateAsync(Ulid  id, int changedByPersonId);


    /// <summary>
    /// Deactives an entity, which is essentially a soft delete.  This accepts a string version of the ULID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="changedByPersonId"></param>
    /// <returns></returns>
    public Task<ActionResult> DeactivateAsync(string id,
                                              int changedByPersonId);

    /// <summary>
    /// Actives an entity, which is essentially a soft undelete
    /// </summary>
    /// <param name="id"></param>
    /// <param name="by"></param>
    /// <returns></returns>
    public Task<ActionResult> ActivateAsync(Ulid id, int by);


    /// <summary>
    /// Actives an entity, which is essentially a soft undelete.  This accepts the string version of the ULID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="by"></param>
    /// <returns></returns>
    public Task<ActionResult> ActivateAsync(string id,
                                            int by);
}
