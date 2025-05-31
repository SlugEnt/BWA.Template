
using Microsoft.Extensions.Caching.Memory;
using SlugEnt.BWA.BusinessComponents.Abstracts.ErrorManagement;
using SlugEnt.BWA.Entities;
using SlugEnt.FluentResults;

namespace BWA.Client.Services;

/// <summary>
/// Data Service for Guid based Data Services.
/// This class will communicate with the server via HTTP calls to the API controllers.
/// </summary>
/// <typeparam name="TEntityGuid"></typeparam>
public class EntityDataServiceGuid<TEntityGuid> : EntityDataService<TEntityGuid>, IEntityRepositoryE2Guid<TEntityGuid>
    where TEntityGuid : AbstractEntityGuid, IEntityGuid, new()

{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="httpClient"></param>
    [ActivatorUtilitiesConstructor]
    public EntityDataServiceGuid(HttpClient httpClient,
        IMemoryCache memoryCache,
        ErrorManager errorManager) : base(httpClient, memoryCache, errorManager)
    {
    }


    /// <summary>
    /// Retrieves a single entity that is based upon an integer Id, no matter what its IsActive flag status is.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Result<TEntityGuid>> GetByIdAnyStatusAsync(Guid id)
    {
        return await base.GetByIdAnyStatusAsync(id.ToString());
    }


    /// <summary>
    /// Retrieves a single entity that is based upon an integer Id, whose isActive flag is set to true
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Result<TEntityGuid?>> GetByIdAsync(Guid id)
    {
        return await base.GetByIdAsync(id.ToString());
    }


    /// <summary>
    /// Physically deletes a row from the database.  This is a hard delete and should be used with caution.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatedByPersonId"></param>
    /// <returns></returns>
    public async Task<Result> DeleteAsync(Guid id,
        int updatedByPersonId)
    {
        return await base.DeleteAsync(id.ToString(), updatedByPersonId);
    }


    /// <summary>
    /// De-activate or soft delete the entity.  This will set the IsActive flag to false.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatedByPersonId"></param>
    /// <returns></returns>
    public async Task<Result> DeActivateAsync(Guid id,
        int updatedByPersonId)
    {
        return await base.DeActivateAsync(id.ToString(), updatedByPersonId);
    }


    /// <summary>
    /// Activate or undoes a soft delete.  This will set the IsActive flag to true.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatedByPersonId"></param>
    /// <returns></returns>
    public async Task<Result> ActivateAsync(Guid id,
        int updatedByPersonId)
    {
        return await base.ActivateAsync(id.ToString(), updatedByPersonId);
    }
}