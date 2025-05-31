
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SlugEnt.BWA.Database;
using SlugEnt.BWA.Entities;
using SlugEnt.FluentResults;

namespace SlugEnt.BWA.Common;
/// <summary>
/// Abstract Entity with an integer based Id field.
/// </summary>
/// <typeparam name="TEntityInt"></typeparam>
public class E2EntityRepositoryInt<TEntityInt> : E2EntityRepository<TEntityInt>, IDisposable, IEntityRepositoryE2Int<TEntityInt>
    where TEntityInt :  AbstractEntityInt, IEntityInt, new()
{
    /// <summary>
    ///     Default Constructor for the Person Engine
    /// </summary>
    /// <param name="db">Database Context to use</param>
    /// <param name="logger">The logger to log to.)</param>
    [ActivatorUtilitiesConstructor]
    public E2EntityRepositoryInt(AppDbContext db,
                                 ILogger<E2EntityRepositoryInt<TEntityInt>> logger) : base(db, logger) 
    { }


    /// <summary>
    /// Retrieves a single entity by its ID whose IsActive flag is true.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Result.Ok::TEntityInt if success.  Failed if errors or record not found.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Result<TEntityInt?>> GetByIdAsync(int id)
    {
        return await GetByIdAsync(id, true);
    }


    /// <summary>
    /// Retrieves a single entity by its ID whose IsActive flag is true.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Result.Ok::TEntityInt if success.  Failed if errors or record not found.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Result<TEntityInt?>> GetByIdAnyStatusAsync(int id)
    {
        return await GetByIdAsync(id, false);
    }


    /// <summary>
    /// Retrieves a single entity by its ID.  You can specify whether to include active or inactive entities.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="respectActiveFlag">If true, only Active records are returned.  If False, it will return the record whether active or not</param>
    /// <returns>Result.Ok::TEntityInt if success.  Failed if errors or record not found.</returns>
    protected async Task<Result<TEntityInt?>> GetByIdAsync(int id,
                                                           bool respectActiveFlag)
    {
        try
        {
            var qry = _db.Set<TEntityInt>().Where(e => e.Id == id);
            if (respectActiveFlag)
                qry = qry.Where(e => e.IsActive == true);
            TEntityInt? entity = await qry.FirstOrDefaultAsync();
            if (entity == null)
                return Result.Fail("Unable to find " + typeof(TEntityInt).Name + " with Id: " + id);

            return Result.Ok(entity);
        }
        catch (Exception e)
        {
            return LogError(e, "GetByIdAsync", "");
        }
    }


    /// <summary>
    /// Deactivates an entity, allowing the caller to specify the ID of the entity to be deactivated.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatedByPersonId"></param>
    /// <returns></returns>
    public async Task<Result> DeActivateAsync(int id,
                                              int updatedByPersonId)
    {
        try
        {
            // Lookup the Entity.
            Result<TEntityInt?> entityResult = await GetByIdAsync(id, false);
            if (entityResult.IsFailed)
            {
                // If we failed to find the entity, return the failure.
                return Result.Fail("Unable to find " + typeof(TEntityInt).Name + " with Id: " + id);
            }

            TEntityInt entity = entityResult.Value;
            if (entity == null)
            {
                // This should never happen, but just in case.
                return Result.Fail("Unable to find " + typeof(TEntityInt).Name + " with Id: " + id);
            }

            return await DeActivateAsync(entity, updatedByPersonId);
        }
        catch (Exception e)
        {
            return LogError(e, "DeActivateAsync", "");
        }
    }


    /// <summary>
    /// Physically deletes a row from the database.  This is a hard delete and should be used with caution.
    /// </summary>
    /// <param name="id">Id of the entity to be deleted</param>
    /// <returns>Result::Int where int is the number of rows deleted.</returns>
    public async Task<Result> DeleteAsync(int id,
                                          int updatedByPersonId)
    {
        try
        {
            // TODO We should log who deleted this somewhere....
            // First check if the entity exists
            Result<TEntityInt?> entityResult = await GetByIdAsync(id, false); // We want to check if it exists regardless of active status

            if (entityResult.IsFailed)

                // If we failed to find the entity, return the failure.
                return Result.Fail("Unable to find " + typeof(TEntityInt).Name + " with Id: " + id);

            _db.Set<TEntityInt>().Remove(entityResult.Value);
            await _db.SaveChangesAsync();

            // This was not removing it from the local DB store.... Not worth the work around at this time.
            // int rowsDeleted =  await _db.Set<TEntityInt>().Where(e => e.Id == id).ExecuteDeleteAsync();
            return Result.Ok();
        }
        catch (Exception e)
        {
            return LogError(e, "DeleteAsync", "");
        }
    }


    /// <summary>
    /// Activates an entity, allowing the caller to specify the ID of the entity to be activated.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatedByPersonId"></param>
    /// <returns></returns>
    public async Task<Result> ActivateAsync(int id,
                                            int updatedByPersonId)
    {
        try
        {
            // Lookup the Entity.
            Result<TEntityInt?> entityResult = await GetByIdAsync(id, false);
            if (entityResult.IsFailed)
            {
                // If we failed to find the entity, return the failure.
                return Result.Fail("Unable to find " + typeof(TEntityInt).Name + " with Id: " + id);
            }

            TEntityInt entity = entityResult.Value;
            if (entity == null)
            {
                // This should never happen, but just in case.
                return Result.Fail("Unable to find " + typeof(TEntityInt).Name + " with Id: " + id);
            }

            if (entity.IsActive != true)
            {
                entity.IsActive         = true;
                entity.LastModifiedById = updatedByPersonId; // Set the LastModifiedById to the person who is deactivating it.
                _db.Set<TEntityInt>().Update(entity);
                await _db.SaveChangesAsync();
                return Result.Ok();
            }

            /*
            _db.Set<TEntity>().Update(entity);
            await _db.SaveChangesAsync();
            */
            // Nothing to do, was already deactivated.
            return Result.Ok();

        }
        catch (Exception e)
        {
            return LogError(e, "ActivateAsync", "");
        }
    }


    /// <summary>
    /// Returns the Id of the Entity as a string
    /// </summary>
    public string KeyId { get { return KeyId; } }
}
