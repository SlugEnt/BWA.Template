using SlugEnt.FluentResults;

namespace SlugEnt.BWA.Entities;


/// <summary>
/// Defines the core set of functionalities that are required for all E2 Repositories.  This defines the core methods of communication
/// with whatever the backend storage method for the Entity is.  So for BWA.Client apps, it is calls to API Controllers.  For the API
/// controllers it is calls to the database.
/// </summary>
public interface IEntityRepositoryE2<TEntity> where TEntity : class
{
    //public delegate IEntityRepositoryE2<TEntity> CreateEngine(AppDbContext db);

    /// <summary>
    /// Get all entities.  This will return all entities that are active or inactive based on the given parameters.
    /// </summary>
    /// <param name="includeActive"></param>
    /// <param name="includeInactive"></param>
    /// <returns></returns>
    public Task<Result<List<TEntity>>> GetAllAsync(bool includeActive,
                                                    bool includeInactive);



    /// <summary>
    /// Add the entity, with the CreatedById set to the given value.  This will set the CreatedAt timestamp and the IsActive flag to true.  The entity will be stored in the database and returned with its ID populated if successful.
    /// </summary>
    /// <param name="entity">The entity that should be stored.</param>
    /// <param name="addedByPersonId">Person who added this object</param>
    /// <returns></returns>
    public Task<Result<TEntity>> AddAsync(TEntity entity,
                                    int addedByPersonId);


    /// <summary>
    /// Add the entity.  CreatedById will need to be set by caller.
    /// </summary>
    /// <param name="entity">The entity that should be stored.</param>
    /// <returns></returns>
    public Task<Result<TEntity>> AddAsync(TEntity entity);

    /// <summary>
    /// Update the entity.  This will set the IsActive flag to true and set the UpdatedByPersonId to the given value.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="updatedByPersonId"></param>
    /// <returns></returns>
    public Task<Result> UpdateAsync(TEntity entity,
                                    int updatedByPersonId);

    /// <summary>
    /// The name of the API that this service will use to communicate with the server. This is used to construct the URL for the API calls.
    /// </summary>
    public string ApiName { get; set; } 

}