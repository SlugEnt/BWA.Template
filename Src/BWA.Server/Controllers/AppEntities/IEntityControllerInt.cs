using Microsoft.AspNetCore.Mvc;
using SlugEnt.BWA.Entities;


namespace BWA.Server.Controllers;

/// <summary>
/// Base interface for all Entity Controllers based upon an Entity that has an integer Id.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IEntityControllerInt<TEntity>  : IEntityController<TEntity>  where TEntity : class, IEntityInt
{
    /// <summary>
    /// Gets an Entity by Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<ActionResult<TEntity?>> GetById(int id);



    /// <summary>
    /// Performs a hard delete on the Entity.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="by"></param>
    /// <returns></returns>
    public Task<ActionResult> DeleteAsync(int id,
                                          int by);


    /// <summary>
    /// Deactives an entity, which is essentially a soft delete
    /// </summary>
    /// <param name="id"></param>
    /// <param name="changedByPersonId"></param>
    /// <returns></returns>
    public Task<ActionResult> DeactivateAsync(int id,
                                         int changedByPersonId);


    /// <summary>
    /// Actives an entity, which is essentially a soft undelete
    /// </summary>
    /// <param name="id"></param>
    /// <param name="by"></param>
    /// <returns></returns>
    public Task<ActionResult> ActivateAsync(int id,
                                              int by);

    
}