using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SlugEnt.FluentResults;

using System.Runtime.CompilerServices;
using SlugEnt.BWA.Database;
using SlugEnt.BWA.Entities;

[assembly: InternalsVisibleTo("HRNextGen_Test")]

namespace SlugEnt.BWA.Common;


/// <summary>
/// This serves as the base class for E2Entities.  Basically everything in the database is an E2Entity.  For general purpose access without custom logic
/// this can immediately manage CRUD operations for the entity.  More complex entities can derive from this class and override methods as needed.
/// </summary>
public class E2EntityRepository<TEntity> where TEntity :  Abstract_Entity, IEntity,new()
{
    public delegate IEntityRepositoryE2<TEntity> CreateEngine(AppDbContext db);

    /// <summary>Database context</summary>
    protected AppDbContext _db;

    /// <summary>Logger</summary>
    protected ILogger<E2EntityRepository<TEntity>> _logger;
    

    /// <summary>
    /// Empty Constructor
    /// </summary>
    protected E2EntityRepository() {}

    /// <summary>
    ///     Default Constructor for the Person Engine
    /// </summary>
    /// <param name="db">Database Context to use</param>
    /// <param name="logger">The logger to log to.)</param>
    [ActivatorUtilitiesConstructor]
    public E2EntityRepository(AppDbContext db,
                        ILogger<E2EntityRepository<TEntity>> logger)
    {
        _db = db;
        _logger = logger;


        // Provide for alternative DateTime implementations (mainly for unit testing)
        DateTimeOffsetProvider = new DateTimeOffsetProvider();
        DateTimeProvider = new DateTimeProvider();
    }



    /// <summary>
    ///     Allows for the replacement of the DateTimeOffset Provider for unit testing purposes.
    /// </summary>
    public IDateTimeOffsetProvider DateTimeOffsetProvider { get; set; }


    /// <summary>
    /// Allows for the replacement of the DateTime Provider for unit testing purposes.
    /// </summary>
    public IDateTimeProvider DateTimeProvider { get; set; }


    /// <summary>
    /// Name of the API used to communicate with this repository.  Not applicable in all cases.
    /// </summary>
    public virtual string ApiName { get; set; }


    /// <summary>
    /// Dispose the Database Context
    /// </summary>
    public void Dispose()
    {
        if (_db != null)
            _db.Dispose();
    }



    /// <summary>
    /// Retrieves all the entities from the database.  You can adjust whether it will return active, inactive or both.
    /// </summary>
    /// <param name="includeActive"></param>
    /// <param name="includeInactive"></param>
    /// <returns></returns>
    public async Task<Result<List<TEntity>>> GetAllAsync(bool includeActive, bool includeInactive)
    {
        try
        {
            if (includeActive)
            {
                if (includeInactive)
                {
                    List<TEntity> listAll = await _db.Set<TEntity>().ToListAsync();
                    return Result.Ok(listAll);
                }

                List<TEntity> listAct = await _db.Set<TEntity>().Where(s => s.IsActive == true).ToListAsync();
                return Result.Ok(listAct);
            }
            else if (includeInactive)
            {
                List<TEntity> listI = await _db.Set<TEntity>().Where(s => s.IsActive == false).ToListAsync();
                return Result.Ok(listI);
            }

            return Result.Fail("you cannot choose includeActive = false and includeInActive = false");
        }
        catch (Exception e)
        {
            return LogError(e, "GetAllAsync", "");
        }
    }

    /// <summary>
    /// Deactivates an entity, allowing the caller to specify the ID of the entity to be deactivated.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatedByPersonId"></param>
    /// <returns></returns>
    protected async Task<Result> DeActivateAsync(TEntity entity, int updatedByPersonId)
    {
        try
        {
            entity.IsActive         = false;
            entity.LastModifiedById = updatedByPersonId; // Set the LastModifiedById to the person who is deactivating it.
            _db.Set<TEntity>().Update(entity);
            await _db.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception e)
        {
            return LogError(e, "DeActivateAsync", "");
        }
    }
     


    /// <summary>
    /// Adds a new Entity to the database.  The CretedById must be set in the entity before calling this method.  The entity must not have an Id already set.
    /// </summary>
    /// <param name="entity">The Entity to be added</param>
    /// <returns>Result.Success or Result.Fail</returns>
    public async Task<Result<TEntity>> AddAsync(TEntity entity)
    {
        try
        {
            if (entity == null) return Result.Fail<TEntity>("Cannot add a null entity");

            if (entity.CreatedById ==0 || entity.CreatedById == null)
                // This means the entity was not created by a valid user.  This should be set before calling this method.
                return Result.Fail<TEntity>("Cannot add an entity that does not have a valid CreatedById set.");
            
            await _db.Set<TEntity>().AddAsync(entity);
            await _db.SaveChangesAsync();
            return Result.Ok(entity);
        }
        catch (Exception e)
        {
            return LogError(e, "AddAsync", "Error attempting to add Entity.");
        }
    }


    /// <summary>
    /// Saves the Entity to the Database.  The entity must not have an Id already set.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="addedByPersonId">Person who added the entity</param>
    /// <returns></returns>
    public async Task<Result<TEntity>> AddAsync(TEntity entity, int addedByPersonId)
    {
        try
        {
            entity.CreatedById = addedByPersonId;
            return await AddAsync(entity);
        }
        catch (Exception e)
        {
            return LogError(e, "AddAsync", "Error attempting to add Entity.");
        }
    }


    /// <summary>
    /// Updates the existing entity in the database.  This will set the UpdatedByPersonId to the given value.  The entity must already exist in the database and have a valid Id set.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="updatedByPersonId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Result> UpdateAsync(TEntity entity, int updatedByPersonId)
    {
        try
        {
            entity.LastModifiedById = updatedByPersonId;
             _db.Set<TEntity>().Update(entity);
             await _db.SaveChangesAsync();
             return Result.Ok(); 
        }
        catch (Exception e)
        {
            return LogError(e, "UpdateAsync",  $"Error attempting to update Entity with Id: {entity.KeyId}");
        }
    }




    
    /// <summary>
    /// Logs the error to the logger and then converts it into an Error result and returns the Error Result.
    /// </summary>
    /// <param name="e"></param>
    /// <param name="methodName"></param>
    /// <param name="entityType"></param>
    /// <param name="customMsg"></param>
    /// <returns></returns>
    protected Result LogError(Exception e, string methodName, string customMsg)
    {
        string entityName = typeof(TEntity).Name;
        _logger.LogError("{MethodName}::{EntityType} --> {ErrorMsg}  <-> Exception: {Exception}", methodName, entityName, customMsg, e.ToString());
        Result result = Result.Fail(customMsg)
                              .AddError(new ExceptionalError(e))
                              .AddMetaData("Method",$"E2EntityRepository::{entityName}::{methodName}");
        return result;
    }



    /// <summary>
    /// The standard GetAll repository access methods have 2 parameters to tell the query whether to include active and/or inactive
    /// entities.  This computes the Status code.
    /// </summary>
    /// <param name="includeActive"></param>
    /// <param name="includeInactive"></param>
    /// <returns></returns>
    public static string ComputeGetAllStatucCode (bool includeActive, bool includeInactive)
    {
        if (includeActive)
        {
            if (includeInactive)
                return "B";
            return "A";
        }
        else
            return "I";


    }
}
