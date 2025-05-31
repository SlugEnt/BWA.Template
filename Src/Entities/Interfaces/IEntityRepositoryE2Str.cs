using SlugEnt.FluentResults;

namespace SlugEnt.BWA.Entities;

/// <summary>
///
/// An Entity Repository that is based upon a String Id
/// </summary>
/// <typeparam name="TEntityStr"></typeparam>
public interface IEntityRepositoryE2Str<TEntityStr> : IEntityRepositoryE2<TEntityStr>
        where TEntityStr : AbstractEntityStr, IEntityStr, new()
{
    /// <summary>
    /// Get the entity with the given ID.  The entity's Active Flag must be set to true.  This will return null if the entity is not found or is inactive.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<Result<TEntityStr?>> GetByIdAsync(string id);
    /// <summary>
    /// Get the entity with the given ID, no matter it's IsActive flag setting.  This will return null if the entity is not found.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<Result<TEntityStr?>> GetByIdAnyStatusAsync(string id);


        /// <summary>
        /// Physically deletes a row from the database.  This is a hard delete and should be used with caution.
        /// </summary>
        /// <param name="id">Id of the entity to be deleted</param>
        /// <returns>Result::Int where int is the number of rows deleted.</returns>
        public Task<Result> DeleteAsync(string id,
                                        int updatedByPersonId);


        /// <summary>
        /// De-activate or soft delete the entity.  
        /// </summary>
        /// <param name="id">Id of entity to de activate</param>
        /// <returns>Success / Failed</returns>
        public Task<Result> DeActivateAsync(string id,
                                            int updatedByPersonId);



        /// <summary>
        /// Activate or undoes a soft delete
        /// </summary>
        /// <param name="id">Id of entity to activate</param>
        /// <returns>Success / Failed</returns>
        public Task<Result> ActivateAsync(string id,
                                          int updatedByPersonId);


    }
