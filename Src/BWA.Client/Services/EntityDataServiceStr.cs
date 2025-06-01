
using Microsoft.Extensions.Caching.Memory;
using SlugEnt.BWA.BusinessComponents.Abstracts.ErrorManagement;
using SlugEnt.BWA.Entities;
using SlugEnt.FluentResults;


namespace BWA.Client.Services;

/// <summary>
/// An Entity Data Service that is based upon an Entity using a string Id.
/// </summary>
/// <typeparam name="TEntityStr"></typeparam>
public class EntityDataServiceStr<TEntityStr> : EntityDataService<TEntityStr>, IEntityRepositoryE2Str<TEntityStr>
where TEntityStr : AbstractEntityStr, IEntityStr, new()
    //where TEntityStr : AbstractEntityStr, IEntityStr
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="httpClient"></param>
    [ActivatorUtilitiesConstructor]
    public EntityDataServiceStr(HttpClient httpClient,
                                IMemoryCache memoryCache,
                                ErrorManager errorManager) : base(httpClient, memoryCache, errorManager) { }


    /// <summary>
    /// Retrieves an Entity by its ID, no matter what the IsActive flag is set to.  This will return null if the entity is not found.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Result<TEntityStr>> GetByIdAnyStatusAsync(string id)
    {
        return await base.GetByIdAnyStatusAsync(id.ToString());
    }


    /// <summary>
    /// Retrieves a single entity by its ID whose IsActive flag is true.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Result<TEntityStr?>> GetByIdAsync(string id)
    {
        return await base.GetByIdAsync(id);
    }


    /// <summary>
    /// Deletes the entity with the given ID.  This is a hard delete and should be used with caution.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatedByPersonId"></param>
    /// <returns></returns>
    public async Task<Result> DeleteAsync(string id,
                                       int updatedByPersonId)
    {
        return await base.DeleteAsync(id, updatedByPersonId);
    }


    /// <summary>
    /// De-activate or soft delete the entity.  This will set the IsActive flag to false.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatedByPersonId"></param>
    /// <returns></returns>
    public async Task<Result> DeActivateAsync(string id,
                                              int updatedByPersonId)
    {
        return await base.DeActivateAsync(id, updatedByPersonId);
    }



    /// <summary>
    /// Activate or undoes a soft delete.  This will set the IsActive flag to true.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatedByPersonId"></param>
    /// <returns></returns>
    public async Task<Result> ActivateAsync(string id,
                                            int updatedByPersonId)
    {
        return await base.ActivateAsync(id, updatedByPersonId);
    }
}
