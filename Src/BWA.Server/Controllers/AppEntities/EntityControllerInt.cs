using Global;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using SlugEnt.BWA.Database;
using SlugEnt.BWA.Entities;
using SlugEnt.FluentResults;


namespace BWA.Server.Controllers.AppEntities;

/// <summary>
/// Base class for all Entity Controllers based upon an Entity that has an integer Id.
/// </summary>
/// <typeparam name="TEntityInt"></typeparam>
public class EntityControllerInt<TEntityInt> : EntityControllerBase<TEntityInt>, IEntityControllerInt<TEntityInt>
    where TEntityInt : AbstractEntityInt, IEntityInt, new()
{

    /// <summary>
    /// Constructor for the EntityLookupControllerBase.  This will set the database context and logger for the controller.  This is a generic controller that can be used to work with any entity that has an integer key and implements the IIdIntModel interface.
    /// </summary>
    /// <param name="logger">Where to log messages too</param>
    /// <param name="db">Database Context</param>
    /// <param name="apiEntity">The repositoryE2Entity that contains the Connection to the Database</param>
    /// <param name="services"></param>
    public EntityControllerInt(ILogger logger,
                               AppDbContext db,
                               IEntityRepositoryE2Int<TEntityInt> apiEntity,
                               IServiceProvider services) : base(logger,
                                                                 db,
                                                                 apiEntity,
                                                                 services) { }



    /// <summary>
    /// Retrieves just a single entity with the given Id
    /// </summary>
    /// <param name="id">Id of the entity to retrieve.</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public virtual async Task<ActionResult<TEntityInt?>> GetById(int id)
    {
        try
        {
            // Note, we are calling this with a hardcoded changedById of 1, since we do not have this value on a Get.
            AppProblemObjectResult problemResult = CheckAndValidateIdAndChangedById(id,
                                                                                    1,
                                                                                    nameof(EntityControllerInt<TEntityInt>),
                                                                                    nameof(GetById));
            if (problemResult != null)
                return problemResult;


            Result<TEntityInt?> result = await ((IEntityRepositoryE2Int<TEntityInt>)ApiEntity).GetByIdAsync(id);
            if (result.IsFailed)
                return FailedToFindEntityError(result,
                                               id.ToString(),
                                               nameof(EntityControllerInt<TEntityInt>),
                                               nameof(GetById));

            Response.StatusCode = StatusCodes.Status200OK;
            return result.Value;
        }
        catch (Exception ex)
        {
            return ReturnWithProblemFromException(nameof(EntityControllerInt<TEntityInt>),
                                                  nameof(GetById),
                                                  null,
                                                  ex,
                                                  $"Failure during attempt to find Entity Id [ {id} ]",
                                                  StatusCodes.Status500InternalServerError);
        }
    }



    /// <summary>
    /// Retrieves just a single entity with the given Id, but it does not care what the IsActive Status is.  
    /// </summary>
    /// <param name="id">Id of the entity to retrieve.</param>
    /// <returns></returns>
    [HttpGet("{id}/any")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public virtual async Task<ActionResult<TEntityInt?>> GetByIdAnyActivationStatus(int id)
    {
        try
        {
            // Note, we are calling this with a hardcoded changedById of 0, since we do not have this value on a Get.
            AppProblemObjectResult problemResult = CheckAndValidateIdAndChangedById(id,
                                                                                    1,
                                                                                    nameof(EntityControllerInt<TEntityInt>),
                                                                                    nameof(GetByIdAnyActivationStatus));
            if (problemResult != null)
                return problemResult;


            Result<TEntityInt?> result = await ((IEntityRepositoryE2Int<TEntityInt>)ApiEntity).GetByIdAnyStatusAsync(id);
            if (result.IsFailed)
                return FailedToFindEntityError(result,
                                               id.ToString(),
                                               nameof(EntityControllerInt<TEntityInt>),
                                               nameof(GetByIdAnyActivationStatus));

            Response.StatusCode = StatusCodes.Status200OK;
            return result.Value;
        }
        catch (Exception ex)
        {
            return ReturnWithProblemFromException(nameof(EntityControllerInt<TEntityInt>),
                                                  nameof(GetByIdAnyActivationStatus),
                                                  null,
                                                  ex,
                                                  $"Failure during attempt to find Entity Id [ {id} ]",
                                                  StatusCodes.Status500InternalServerError);
        }
    }



    /// <summary>
    /// Deletes an entity from the database.  Must specify the Id and By
    /// </summary>
    /// <param name="id">The ID of the record to delete</param>
    /// <param name="by">The ID of the person who deleted it.</param>
    /// <returns></returns>
    [HttpPut("{id}/{by}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<ActionResult> DeleteAsync(int id,
                                                        int by)
    {
        try
        {
            AppProblemObjectResult problemResult = CheckAndValidateIdAndChangedById(id,
                                                                                    by,
                                                                                    nameof(EntityControllerInt<TEntityInt>),
                                                                                    nameof(DeleteAsync));
            if (problemResult != null)
                return problemResult;


            Result<TEntityInt?> result = await ((IEntityRepositoryE2Int<TEntityInt>)ApiEntity).DeleteAsync(id, by);
            if (result.IsFailed)
            {
                Result resultFailed = Result.Fail(new Error("Error deleting the entity from database.").CausedBy(result.Errors)).AddMetaData("EntityId", id);
                return ReturnWithProblemFromResult(nameof(EntityControllerInt<TEntityInt>),
                                                   nameof(DeleteAsync),
                                                   null,
                                                   resultFailed,
                                                   StatusCodes.Status500InternalServerError);
            }

            Response.StatusCode = StatusCodes.Status200OK;
            return Ok("");
        }
        catch (Exception ex)
        {
            return ReturnWithProblemFromException(nameof(EntityControllerInt<TEntityInt>),
                                                  nameof(DeleteAsync),
                                                  null,
                                                  ex,
                                                  $"Failure trying to delete entity with Id [ {id} ]",
                                                  StatusCodes.Status500InternalServerError);
        }
    }


    /// <summary>
    /// De activates an entity.  This will set the IsActive flag to false and set the UpdatedByPersonId to the given value.
    /// </summary>
    /// <param name="transportById"></param>
    /// <returns></returns>
    [HttpPut("{id}/deactivate/{changedByPersonId}")]

    //[Route("Deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public virtual async Task<ActionResult> DeactivateAsync(int id,
                                                            int changedByPersonId)
    {
        try
        {
            AppProblemObjectResult problemResult = CheckAndValidateIdAndChangedById(id,
                                                                                    changedByPersonId,
                                                                                    nameof(EntityControllerInt<TEntityInt>),
                                                                                    nameof(DeactivateAsync));
            if (problemResult != null)
                return problemResult;

            Result<TEntityInt> result = await ((IEntityRepositoryE2Int<TEntityInt>)ApiEntity).DeActivateAsync(id, changedByPersonId);
            if (result.IsFailed)
            {
                Result               resultFailed = Result.Fail(new Error("Error Deactivating the entity from database.").CausedBy(result.Errors)).AddMetaData("EntityId", id);
                return ReturnWithProblemFromResult(nameof(EntityControllerInt<TEntityInt>),
                                                   nameof(DeactivateAsync),
                                                   null,
                                                   resultFailed,
                                                   StatusCodes.Status500InternalServerError);
            }


            return Ok("");
        }
        catch (Exception ex)
        {
            return ReturnWithProblemFromException(nameof(EntityControllerInt<TEntityInt>),
                                                  nameof(DeactivateAsync),
                                                  null,
                                                  ex,
                                                  $"Failure during attempt to Deactivate Entity Id [ {id} ]",
                                                  StatusCodes.Status500InternalServerError);
        }
    }



    /// <summary>
    /// Activates an entity.  This will set the IsActive flag to true and set the UpdatedByPersonId to the given value.
    /// </summary>
    /// <param name="transportById"></param>
    /// <returns></returns>
    [HttpPut("{id}/activate/{by}")]

    //[Route("Deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public virtual async Task<ActionResult> ActivateAsync(int id,
                                                          int by)
    {
        try
        {
            AppProblemObjectResult problemResult = CheckAndValidateIdAndChangedById(id,
                                                                                    by,
                                                                                    nameof(EntityControllerInt<TEntityInt>),
                                                                                    nameof(ActivateAsync));
            if (problemResult != null)
                return problemResult;


            Result<TEntityInt> result = await ((IEntityRepositoryE2Int<TEntityInt>)ApiEntity).ActivateAsync(id, by);
            if (result.IsFailed)
            {
                Result               resultFailed = Result.Fail(new Error("Error Activating the entity from database.").CausedBy(result.Errors)).AddMetaData("EntityId", id);
                return ReturnWithProblemFromResult(nameof(EntityControllerInt<TEntityInt>),
                                                   nameof(ActivateAsync),
                                                   null,
                                                   resultFailed,
                                                   StatusCodes.Status500InternalServerError);
            }


            return Ok("");
        }
        catch (Exception ex)
        {
            return ReturnWithProblemFromException(nameof(EntityControllerInt<TEntityInt>),
                                                  nameof(ActivateAsync),
                                                  null,
                                                  ex,
                                                  $"Failure during attempt to find Activate Entity with Id [ {id} ]",
                                                  StatusCodes.Status500InternalServerError);
        }
    }



    /// <summary>
    /// String based entities must have the Id property set by the caller, the Database cannot assign them automatically.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected override Result ValidateAddNew(TEntityInt entity)
    {
        if (entity.Id <0 )
            return Result.Fail(new Error("A string based Entity Object MUST have its Id property set prior to calling save."));

        return Result.Ok();
    }



    /// <summary>
    /// Checks to make sure the Id and ChangedById are valid.  If they are not valid then it returns an AppProblemObjectResult with the error message.  If valid it returns null.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="changedById"></param>
    /// <param name="methodName"></param>
    /// <param name="className"></param>
    /// <returns>Ok:  null  |  Failure: AppProblemObjectResult</returns>
    protected AppProblemObjectResult CheckAndValidateIdAndChangedById(int id,
                                                                      int changedById,
                                                                      string methodName,
                                                                      string className)
    {
        if (id <= 0)
            return ReturnWithProblemFromMessage(className,
                                                methodName,
                                                null,
                                                $"Invalid Id Specified Id [ {id} ]",
                                                StatusCodes.Status412PreconditionFailed);
        if (changedById <= 0)
            return ReturnWithProblemFromMessage(className,
                                                methodName,
                                                null,
                                                "The ChangeBy parameter must be a valid User Id.",
                                                StatusCodes.Status412PreconditionFailed);

        return null;
    }
}