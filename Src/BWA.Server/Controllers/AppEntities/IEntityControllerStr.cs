using Microsoft.AspNetCore.Mvc;
using SlugEnt.BWA.Entities;

namespace BWA.Server.Controllers;

/// <summary>
/// Interface for a controller that manages an Entity with a string based Id.
/// </summary>
/// <typeparam name="TEntityStr"></typeparam>
public interface IEntityControllerStr<TEntityStr> : IEntityController<TEntityStr> where TEntityStr : class, IEntityStr
{
    /// <summary>
    /// Gets an Entity by Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<ActionResult<TEntityStr>> GetById(string id);



    /// <summary>
    /// Performs a hard delete on the Entity.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="by"></param>
    /// <returns></returns>
    public Task<ActionResult> DeleteAsync(string id,
                                          int by);


    /// <summary>
    /// Deactives an entity, which is essentially a soft delete
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
    public Task<ActionResult> ActivateAsync(string id,
                                            int by);

} 
