using Microsoft.AspNetCore.Mvc;
using SlugEnt.BWA.Entities;


namespace BWA.Server.Controllers;

/// <summary>
/// Base interface for all Entity Controllers based upon an Entity that has an integer Id.
/// </summary>
/// <typeparam name="TEntityLong"></typeparam>
public interface IEntityControllerLong<TEntityLong>  : IEntityController<TEntityLong>  where TEntityLong : class, IEntityLong
{
    /// <summary>
    /// Gets an Entity by Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<ActionResult<TEntityLong>> GetById(long id);



    /// <summary>
    /// Performs a hard delete on the Entity.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="by"></param>
    /// <returns></returns>
    public Task<ActionResult> DeleteAsync(long id,
                                          int by);


    /// <summary>
    /// Deactives an entity, which is essentially a soft delete
    /// </summary>
    /// <param name="id"></param>
    /// <param name="changedByPersonId"></param>
    /// <returns></returns>
    public Task<ActionResult> DeactivateAsync(long id,
                                         int changedByPersonId);


    /// <summary>
    /// Actives an entity, which is essentially a soft undelete
    /// </summary>
    /// <param name="id"></param>
    /// <param name="by"></param>
    /// <returns></returns>
    public Task<ActionResult> ActivateAsync(long id,
                                              int by);

    
}