using Microsoft.AspNetCore.Mvc;
using SlugEnt.BWA.Entities;


namespace BWA.Server.Controllers;

/// <summary>
/// Base interface for all Entity Controllers based upon an Entity that has a Guid Id.
/// </summary>
/// <typeparam name="TEntityGuid"></typeparam>
public interface IEntityControllerGuid<TEntityGuid>  : IEntityController<TEntityGuid>  where TEntityGuid : class, IEntityGuid
{
    /// <summary>
    /// Gets an Entity by Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<ActionResult<TEntityGuid>> GetById(Guid id);



    /// <summary>
    /// Performs a hard delete on the Entity.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="by"></param>
    /// <returns></returns>
    public Task<ActionResult> DeleteAsync(Guid id,
                                          int by);


    /// <summary>
    /// Deactives an entity, which is essentially a soft delete
    /// </summary>
    /// <param name="id"></param>
    /// <param name="changedByPersonId"></param>
    /// <returns></returns>
    public Task<ActionResult> DeactivateAsync(Guid id,
                                         int changedByPersonId);


    /// <summary>
    /// Actives an entity, which is essentially a soft undelete
    /// </summary>
    /// <param name="id"></param>
    /// <param name="by"></param>
    /// <returns></returns>
    public Task<ActionResult> ActivateAsync(Guid id,
                                              int by);

    
}