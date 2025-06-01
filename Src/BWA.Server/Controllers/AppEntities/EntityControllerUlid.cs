using ByteAether.Ulid;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using SlugEnt.BWA.Database;
using SlugEnt.BWA.Entities;
using SlugEnt.FluentResults;

namespace BWA.Server.Controllers.AppEntities;

/// <summary>
/// An Entity Controller that is based upon an Entity that has a String Id.
/// </summary>
/// <typeparam name="TEntityUlid"></typeparam>
public class EntityControllerUlid<TEntityUlid> : EntityControllerBase<TEntityUlid>, IEntityControllerUlid<TEntityUlid>
    where TEntityUlid : AbstractEntityULID, IEntityULID, new()
{
    /// <summary>
    /// Constructor for the EntityLookupControllerBase.  This will set the database context and logger for the controller.  This is a generic controller that can be used to work with any entity that has an integer key and implements the IIdIntModel interface.
    /// </summary>
    /// <param name="logger">Where to log messages too</param>
    /// <param name="db">Database Context</param>
    /// <param name="apiEntity">The repositoryE2Entity that contains the Connection to the Database</param>
    /// <param name="services"></param>
    public EntityControllerUlid(ILogger logger,
                                AppDbContext db,
                                IEntityRepositoryE2Ulid<TEntityUlid> apiEntity,
                                IServiceProvider services) : base(logger,
                                                                  db,
                                                                  apiEntity,
                                                                  services)
    { }





    /// <summary>
    /// Retrieves just a single entity with the given Id
    /// </summary>
    /// <param name="id">Id of the entity to retrieve.</param>
    /// <returns>Entity that was requested.  On Failure it returns AppProblemObjectResult with ProblemDetailsCustom details.</returns>
    [HttpGet("{id}")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public virtual async Task<ActionResult<TEntityUlid?>> GetById(Ulid id)
    {
        try
        {
            // Note, we are calling this with a hardcoded changedById of 0, since we do not have this value on a Get.
            AppProblemObjectResult problemResult = CheckAndValidateIdAndChangedById(id,
                                                                                    1,
                                                                                    nameof(EntityControllerUlid<TEntityUlid>),
                                                                                    nameof(GetById));
            if (problemResult != null)
                return problemResult;

            Result<TEntityUlid?> result = await ((IEntityRepositoryE2Ulid<TEntityUlid>)ApiEntity).GetByIdAsync(id);
            if (result.IsFailed)
                return FailedToFindEntityError(result,
                                               id.ToString(),
                                               nameof(EntityControllerUlid<TEntityUlid>),
                                               nameof(GetById));

            Response.StatusCode = StatusCodes.Status200OK;
            return result.Value;
        }
        catch (Exception ex)
        {
            return ReturnWithProblemFromException(nameof(EntityControllerUlid<TEntityUlid>),
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
    public virtual async Task<ActionResult<TEntityUlid?>> GetByIdAnyActivationStatus(Ulid id)
    {
        try
        {
            // Note, we are calling this with a hardcoded changedById of 0, since we do not have this value on a Get.
            AppProblemObjectResult problemResult = CheckAndValidateIdAndChangedById(id,
                                                                                    1,
                                                                                    nameof(EntityControllerUlid<TEntityUlid>),
                                                                                    nameof(GetByIdAnyActivationStatus));
            if (problemResult != null)
                return problemResult;


            Result<TEntityUlid?> result = await ((IEntityRepositoryE2Ulid<TEntityUlid>)ApiEntity).GetByIdAnyStatusAsync(id);
            if (result.IsFailed)
                return FailedToFindEntityError(result,
                                               id.ToString(),
                                               nameof(EntityControllerUlid<TEntityUlid>),
                                               nameof(GetByIdAnyActivationStatus));


            Response.StatusCode = StatusCodes.Status200OK;
            return result.Value;
        }
        catch (Exception ex)
        {
            return ReturnWithProblemFromException(nameof(EntityControllerUlid<TEntityUlid>),
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
    public virtual async Task<ActionResult> DeleteAsync(Ulid id,
                                                        int by)
    {
        try
        {
            AppProblemObjectResult problemResult = CheckAndValidateIdAndChangedById(id,
                                                                                    by,
                                                                                    nameof(EntityControllerUlid<TEntityUlid>),
                                                                                    nameof(DeleteAsync));
            if (problemResult != null)
                return problemResult;


            Result<TEntityUlid?> result = await ((IEntityRepositoryE2Ulid<TEntityUlid>)ApiEntity).DeleteAsync(id, by);
            if (result.IsFailed)
            {
                Result resultFailed = Result.Fail(new Error("Error deleting the entity from database.").CausedBy(result.Errors)).AddMetaData("EntityId", id);
                return ReturnWithProblemFromResult(nameof(EntityControllerUlid<TEntityUlid>),
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
            return ReturnWithProblemFromException(nameof(EntityControllerUlid<TEntityUlid>),
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
    public virtual async Task<ActionResult> DeactivateAsync(Ulid id, int changedByPersonId)
    {
        try
        {
            AppProblemObjectResult problemResult = CheckAndValidateIdAndChangedById(id,
                                                                                    changedByPersonId,
                                                                                    nameof(EntityControllerUlid<TEntityUlid>),
                                                                                    nameof(DeactivateAsync));
            if (problemResult != null)
                return problemResult;

            Result<TEntityUlid> result = await ((IEntityRepositoryE2Ulid<TEntityUlid>)ApiEntity).DeActivateAsync(id, changedByPersonId);
            if (result.IsFailed)
            {
                Result resultFailed = Result.Fail(new Error("Error Deactivating the entity from database.").CausedBy(result.Errors)).AddMetaData("EntityId", id);
                return ReturnWithProblemFromResult(nameof(EntityControllerUlid<TEntityUlid>),
                                                   nameof(DeactivateAsync),
                                                   null,
                                                   resultFailed,
                                                   StatusCodes.Status500InternalServerError);
            }


            return Ok("");
        }
        catch (Exception ex)
        {
            return ReturnWithProblemFromException(nameof(EntityControllerUlid<TEntityUlid>),
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public virtual async Task<ActionResult> ActivateAsync(Ulid id,
                                                            int by)
    {
        try
        {
            AppProblemObjectResult problemResult = CheckAndValidateIdAndChangedById(id,
                                                                                    by,
                                                                                    nameof(EntityControllerUlid<TEntityUlid>),
                                                                                    nameof(ActivateAsync));
            if (problemResult != null)
                return problemResult;


            Result<TEntityUlid> result = await ((IEntityRepositoryE2Ulid<TEntityUlid>)ApiEntity).ActivateAsync(id, by);
            if (result.IsFailed)
            {
                Result resultFailed = Result.Fail(new Error("Error Activating the entity from database.").CausedBy(result.Errors)).AddMetaData("EntityId", id);
                return ReturnWithProblemFromResult(nameof(EntityControllerUlid<TEntityUlid>),
                                                   nameof(ActivateAsync),
                                                   null,
                                                   resultFailed,
                                                   StatusCodes.Status500InternalServerError);
            }


            return Ok("");
        }
        catch (Exception ex)
        {
            return ReturnWithProblemFromException(nameof(EntityControllerUlid<TEntityUlid>),
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
    protected override Result ValidateAddNew(TEntityUlid entity)
    {
        if (IsEmptyUlid(entity.Id))
            return Result.Fail(new Error("A string based Entity Object MUST have its Id property set prior to calling save.It cannot be null or empty"));

        return Result.Ok();
    }


    /// <summary>
    /// Returns True if the given Id is an empty ULID.  This is used to check if the Id is valid or not.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    protected bool IsEmptyUlid (Ulid id)
    {
        return id.ToString() == "00000000000000000000000000";
    }


    /// <summary>
    /// Checks to make sure the Id and ChangedById are valid.  If they are not valid then it returns an AppProblemObjectResult with the error message.  If valid it returns null.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="changedById"></param>
    /// <param name="methodName"></param>
    /// <param name="className"></param>
    /// <returns>Ok:  null  |  Failure: AppProblemObjectResult</returns>
    protected AppProblemObjectResult CheckAndValidateIdAndChangedById(Ulid id,
                                                                      int changedById,
                                                                      string methodName,
                                                                      string className)
    {
        if (IsEmptyUlid(id))
            return ReturnWithProblemFromMessage(className,
                                                methodName,
                                                null,
                                                $"Invalid Id Specified. Id was empty string.",
                                                StatusCodes.Status412PreconditionFailed);
        if (changedById <= 0)
            return ReturnWithProblemFromMessage(className,
                                                methodName,
                                                null,
                                                "The ChangeBy parameter must be a valid User Id.",
                                                StatusCodes.Status412PreconditionFailed);

        return null;
    }


    /// <summary>
    /// Retrieves a single entity by its ID.  This accepts a string version of the ID, so it must be converted to a ULID first.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TEntityUlid>> GetById(string id)
    {
        try
        {
            bool isValid = Ulid.TryParse(id, null, out Ulid ulid);
            if (!isValid)
                return ReturnWithProblemFromMessage(nameof(EntityControllerUlid<TEntityUlid>),
                                                    nameof(GetById),
                                                    null,
                                                    $"Invalid Id Specified. Id was not a valid ULID value.",
                                                    StatusCodes.Status412PreconditionFailed);
         
            return await GetById(ulid);
        }
        catch (Exception ex)
        {
            return ReturnWithProblemFromException(nameof(EntityControllerUlid<TEntityUlid>),
                                                  nameof(GetById),
                                                  null,
                                                  ex,
                                                  $"Failure during attempt to find Entity Id [ {id} ]",
                                                  StatusCodes.Status500InternalServerError);
        }
    }


    /// <summary>
    /// Deletes an entity from the database.  Must specify the Id and By.  The Id is the string version of the ULID.
    /// </summary>
    /// <param name="id">The ID of the record to delete</param>
    /// <param name="by">The ID of the person who deleted it.</param>
    /// <returns></returns>
    [HttpPut("{id}/{by}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]

    public async Task<ActionResult> DeleteAsync(string id, int by)
    {
        try
        {
            bool isValid = Ulid.TryParse(id, null, out Ulid ulid);
            if (!isValid)
                return ReturnWithProblemFromMessage(nameof(EntityControllerUlid<TEntityUlid>),
                                                    nameof(DeleteAsync),
                                                    null,
                                                    $"Invalid Id Specified. Id was not a valid ULID value.",
                                                    StatusCodes.Status412PreconditionFailed);

            return await DeleteAsync(ulid,by);
        }
        catch (Exception ex)
        {
            return ReturnWithProblemFromException(nameof(EntityControllerUlid<TEntityUlid>),
                                                  nameof(DeleteAsync),
                                                  null,
                                                  ex,
                                                  $"Failure during attempt to Delete Entity Id [ {id} ]",
                                                  StatusCodes.Status500InternalServerError);
        }
    }



    /// <summary>
    /// De activates an entity.  This will set the IsActive flag to false and set the UpdatedByPersonId to the given value.
    /// </summary>
    /// <param name="transportById"></param>
    /// <returns></returns>
    [HttpPut("{id}/deactivate/{changedByPersonId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeactivateAsync(string id, int changedByPersonId)
    {
        try
        {
            bool isValid = Ulid.TryParse(id, null, out Ulid ulid);
            if (!isValid)
                return ReturnWithProblemFromMessage(nameof(EntityControllerUlid<TEntityUlid>),
                                                    nameof(DeleteAsync),
                                                    null,
                                                    $"Invalid Id Specified. Id was not a valid ULID value.",
                                                    StatusCodes.Status412PreconditionFailed);

            return await DeactivateAsync(ulid, changedByPersonId);
        }
        catch (Exception ex)
        {
            return ReturnWithProblemFromException(nameof(EntityControllerUlid<TEntityUlid>),
                                                  nameof(DeactivateAsync),
                                                  null,
                                                  ex,
                                                  $"Failure during attempt to DeActive Entity Id [ {id} ]",
                                                  StatusCodes.Status500InternalServerError);
        }
    }


    /// <summary>
    /// Activates an entity.  This will set the IsActive flag to true and set the UpdatedByPersonId to the given value.  Id must be the string version of ULID
    /// </summary>
    /// <param name="transportById"></param>
    /// <returns></returns>
    [HttpPut("{id}/activate/{by}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ActivateAsync(string id, int by)
    {
        try
        {
            bool isValid = Ulid.TryParse(id, null, out Ulid ulid);
            if (!isValid)
                return ReturnWithProblemFromMessage(nameof(EntityControllerUlid<TEntityUlid>),
                                                    nameof(ActivateAsync),
                                                    null,
                                                    $"Invalid Id Specified. Id was not a valid ULID value.",
                                                    StatusCodes.Status412PreconditionFailed);

            return await ActivateAsync(ulid, by);
        }
        catch (Exception ex)
        {
            return ReturnWithProblemFromException(nameof(EntityControllerUlid<TEntityUlid>),
                                                  nameof(ActivateAsync),
                                                  null,
                                                  ex,
                                                  $"Failure during attempt to Activate Entity Id [ {id} ]",
                                                  StatusCodes.Status500InternalServerError);
        }
    }
}
