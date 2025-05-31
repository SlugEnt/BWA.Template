using Microsoft.AspNetCore.Mvc;
using SlugEnt.BWA.Entities;
using SlugEnt.BWA.Entities.NonModel;

namespace BWA.Server.Controllers;

/// <summary>
/// Base controller methods for all Entity Controllers.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IEntityController<TEntity>  where TEntity : class, IEntity
{
    /// <summary>
    /// Lists all Entities based on the status provided.  This will return a list of entities that match the given status.  The status can be "active", "inactive", or "all".
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public Task<ActionResult<List<TEntity>>> List(string status);


    /// <summary>
    /// Adds a new entity.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public Task<ActionResult<TEntity>> AddAsync([FromBody] EntityTransport<TEntity> transport);

}