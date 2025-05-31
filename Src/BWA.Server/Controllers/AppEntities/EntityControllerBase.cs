using Global;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using SlugEnt.BWA.Database;
using SlugEnt.BWA.Entities;
using SlugEnt.BWA.Entities.NonModel;
using SlugEnt.FluentResults;



namespace BWA.Server.Controllers.AppEntities;

/// <summary>
/// Base API controller for all controllers that serve up  IEntity based API's
/// </summary>
/// <typeparam name="TEntity">Must be an Entity Type.</typeparam>
public abstract class EntityControllerBase<TEntity> : ODataController where TEntity : class, IEntity
{
    /// <summary>
    /// Logger
    /// </summary>
    protected readonly ILogger _logger;

    /// <summary>
    /// Database Context for the application.  This is used to access the database and perform CRUD operations on the entities.
    /// </summary>
    protected readonly AppDbContext _db;

    /// <summary>
    /// The services collection in case Controller needs other access
    /// </summary>
    protected readonly IServiceProvider _services;

    /// <summary>
    /// The repositoryE2Entity that contains the Connection to the Database.  This is used to access the database and perform CRUD operations on the entities.
    /// </summary>
    protected virtual IEntityRepositoryE2<TEntity> ApiEntity { get; set; }


    /// <summary>
    /// The name of the API Controller Entity object
    /// </summary>

    public string ControllerName { get; set; }


    /// <summary>
    /// Constructor for the EntityLookupControllerBase.  This will set the database context and logger for the controller.  This is a generic controller that can be used to work with any entity that has an integer key and implements the IIdIntModel interface.
    /// </summary>
    /// <param name="logger">Where to log messages too</param>
    /// <param name="db">Database Context</param>
    /// <param name="apiEntity">The repositoryE2Entity that contains the Connection to the Database</param>
    /// <param name="services"></param>
    public EntityControllerBase(ILogger logger,
                                AppDbContext db,
                                IEntityRepositoryE2<TEntity> apiEntity,
                                IServiceProvider services) : base()
    {
        _logger   = logger;
        _db       = db;
        ApiEntity = apiEntity;
        _services = services;
    }


    [HttpGet]
    [EnableQuery]
    public async Task<ActionResult<List<TEntity>>> List2()
    {
        return await List("B");
    }

    /// <summary>
    /// Retrieves a list of the given entity type.  Can return Active, InActive or Both based upon Status
    /// Flag.
    /// <para>Status = A = Active</para>
    /// <para>Status = I = InActive</para>
    /// <para>Status = B = Active and Inactive</para>
    /// </summary>
    /// <param name="status">Whether to include Active, InActive or both.</param>
    /// <returns>On Success:  List of Entities.  - 200 |  On Failure:  404 Not Found</returns>
    [HttpGet]
    [EnableQuery]
    [Route("List/{status}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public virtual async Task<ActionResult<List<TEntity>>> List(string status)
    {
        try
        {
            bool includeActive   = status == "A" || status == "B";
            bool includeInactive = status == "I" || status == "B";

            Result<List<TEntity>> result = await ApiEntity.GetAllAsync(includeActive, includeInactive);
            if (result.IsFailed)
                return NotFound(result.Errors.FirstOrDefault()?.Message);

            Response.StatusCode = StatusCodes.Status200OK;
            return result.Value;
        }
        catch (Exception e)
        {
            string msg = ControllerName + ":List : AppError:  " + e.Message;

            _logger.LogError(msg);
            Response.StatusCode = StatusCodes.Status500InternalServerError;

            // TODO Fix AppError message
            return Problem(msg, ControllerName + "Controller:List");
        }
    }



    /// <summary>
    /// Adds a new entity to the database.  This will set the CreatedAt timestamp and the IsActive flag to true.  The entity will be stored in the database and returned with its ID populated if successful.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>On Success:  ActionResult[Entity] - 201 |  On Failure:  AppProblemObjectResult[ProblemDetailsCustom] 400/412/500</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public virtual async Task<ActionResult<TEntity>> AddAsync([FromBody] EntityTransport<TEntity> entityTransport)
    {
        TEntity entityFail = null;

        try
        {
            // A. Perform Validations
            Result<string> validateResult = ValidateEntityTranspot(entityTransport);
            if (validateResult.IsFailed)
            {
                // Validation failed, return a BadRequest with the error message.
                if (entityTransport != null)
                    entityFail = entityTransport.Entity;
                return ReturnWithProblemFromResult(nameof(EntityControllerBase<TEntity>),
                                                   nameof(AddAsync),
                                                   entityFail,
                                                   validateResult,
                                                   StatusCodes.Status412PreconditionFailed);
            }

            Result resultNewCheck = ValidateAddNew(entityTransport.Entity);
            if (resultNewCheck.IsFailed)
            {
                return ReturnWithProblemFromResult(nameof(EntityControllerBase<TEntity>),
                                                   nameof(AddAsync),
                                                   entityTransport.Entity,
                                                   resultNewCheck,
                                                   StatusCodes.Status412PreconditionFailed);
            }


            // B.  Add the entity to the database
            TEntity entity = entityTransport.Entity;
            entityFail = entity;

            Result<TEntity> result = await ApiEntity.AddAsync(entity, entityTransport.ChangeMadeByPersonId);
            if (result.IsFailed)
            {
                Result resultFailed = Result.Fail(new Error("Error adding the entity to database.").CausedBy(result.Errors));
                return ReturnWithProblemFromResult(nameof(EntityControllerBase<TEntity>),
                                                   nameof(AddAsync),
                                                   entity,
                                                   resultFailed,
                                                   StatusCodes.Status500InternalServerError);
            }

            Response.StatusCode = StatusCodes.Status201Created;
            return result.Value;
        }
        catch (Exception ex)
        {
            return ReturnWithProblemFromException(nameof(EntityControllerBase<TEntity>),
                                                  nameof(AddAsync),
                                                  entityFail,
                                                  ex,
                                                  "",
                                                  StatusCodes.Status500InternalServerError);
        }
    }



    /// <summary>
    /// Updates the provided entity into the database. 
    /// </summary>
    /// <param name="entityTransport"></param>
    /// <returns>Ok Result  or BadRequest</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public virtual async Task<ActionResult> Update([FromBody] EntityTransport<TEntity> entityTransport)
    {
        TEntity entityFail = null;

        try
        {
            // A. Validate
            Result<string> validateResult = ValidateEntityTranspot(entityTransport);
            if (validateResult.IsFailed)
            {
                if (entityTransport != null)
                    entityFail = entityTransport.Entity;

                // Validation failed, return a BadRequest with the error message.
                string msg = validateResult.Errors.FirstOrDefault()?.Message ?? "Validation failed for the provided EntityTransport.";
                return BadRequest(msg);
            }

            TEntity entity = entityTransport.Entity;
            entityFail = entity;

            // B. Update the entity in the database
            Result<TEntity> result = await ApiEntity.UpdateAsync(entity, entityTransport.ChangeMadeByPersonId);
            if (result.IsFailed)
            {
                Result resultFailed = Result.Fail(new Error("Error Updating the entity from database.").CausedBy(result.Errors))
                                            .AddMetaData("EntityId", entityTransport.Entity.KeyId);
                return ReturnWithProblemFromResult(nameof(EntityControllerBase<TEntity>),
                                                   nameof(Update),
                                                   entity,
                                                   resultFailed,
                                                   StatusCodes.Status500InternalServerError);
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            return ReturnWithProblemFromException(nameof(EntityControllerBase<TEntity>),
                                                  nameof(Update),
                                                  entityFail,
                                                  ex,
                                                  "",
                                                  StatusCodes.Status500InternalServerError);
        }
    }



#region "Support Functions"


    /// <summary>
    /// Confirms the Entity is in proper format for being inserted into the Repository.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected abstract Result ValidateAddNew(TEntity entity);


    /// <summary>
    /// Performs validation on the provided EntityTransport object.  This will check for null values and ensure that the ChangeMadeByPersonId is a valid Id (greater than 0).
    /// </summary>
    /// <param name="entityTransport"></param>
    /// <returns></returns>
    [NonAction]
    private Result<string> ValidateEntityTranspot(EntityTransport<TEntity> entityTransport)
    {
        if (entityTransport == null)
            return Result.Fail<string>("Provided Transport object was Null");
        if (entityTransport.Entity == null)
            return Result.Fail<string>("Provided Entity was Null");
        if (entityTransport.ChangeMadeByPersonId <= 0)
            return Result.Fail<string>("No Id provided for the person making this change.  This should be set to a valid Id of an existing Person.");

        // If we get here, all is good.
        return Result.Ok();
    }


    /// <summary>
    /// Performs validation on the provided EntityTransportById object.  This will check for null values and ensure that the EntityId and ChangeMadeByPersonId are valid Ids (greater than 0).
    /// </summary>
    /// <param name="transportById"></param>
    /// <returns></returns>
    [NonAction]
    private Result<string> ValidateEntityTranspotById(EntityTransportById transportById)
    {
        if (transportById == null)
            return Result.Fail<string>("Provided Transport object was Null");
        if (string.IsNullOrEmpty(transportById.EntityId))
            return Result.Fail<string>("Invalid EntityId provided in the transport object.  This should be set to a valid, existing Id of the entity you want to delete.");
        if (transportById.ChangeMadeByPersonId <= 0)
            return Result.Fail<string>("No Id provided for the person making this change.  This should be set to a valid Id of an existing Person.");

        // If we get here, all is good.
        return Result.Ok();
    }




    /// <summary>
    /// Returns a NotFound Problem reason.  This is used when the entity is not found in the database.  This will return a 404 Not Found error with the given entityId.
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    protected AppProblemObjectResult FailedToFindEntityError(IResultBase result,
                                                             string entityIdSearchingFor,
                                                             string className,
                                                             string methodName)
    {
        Result x = Result.Fail(new Error($"Entity with requested Id [ {entityIdSearchingFor} ] not found in database.").CausedBy(result.Errors))
                         .AddMetaData("EntityId", entityIdSearchingFor);
        return ReturnWithProblemFromResult(className,
                                           methodName,
                                           null,
                                           x,
                                           StatusCodes.Status404NotFound);
    }


    /// <summary>
    /// Creates a custom ProblemDetails response object based upon a Failed Result.  Logs to the logger and then returns an AppProblemObjectResult
    /// </summary>
    /// <param name="className"></param>
    /// <param name="methodName"></param>
    /// <param name="result"></param>
    /// <param name="statusCode"></param>
    /// <param name="logLevel"></param>
    /// <returns>AppProblemObjectResult with information about the problem filled out</returns>
    protected AppProblemObjectResult ReturnWithProblemFromResult(string className,
                                                                 string methodName,
                                                                 TEntity? entity,
                                                                 IResultBase result,
                                                                 int statusCode,
                                                                 LogLevel logLevel = LogLevel.Information)
    {
        ProblemDetailsCustom problem = LogAndCreateProblemFromResult(className,
                                                                     methodName,
                                                                     entity,
                                                                     result,
                                                                     statusCode,
                                                                     logLevel);
        return new AppProblemObjectResult(problem);
    }


    /// <summary>
    /// Creates a custom ProblemDetails response object based upon an Exception.  Logs to the logger and then returns an AppProblemObjectResult
    /// </summary>
    /// <param name="className"></param>
    /// <param name="methodName"></param>
    /// <param name="entity"></param>
    /// <param name="exception"></param>
    /// <param name="customMsg"></param>
    /// <param name="statusCode"></param>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    protected AppProblemObjectResult ReturnWithProblemFromException(string className,
                                                                    string methodName,
                                                                    TEntity? entity,
                                                                    Exception exception,
                                                                    string customMsg,
                                                                    int statusCode,
                                                                    LogLevel logLevel = LogLevel.Information)
    {
        ProblemDetailsCustom problem = LogAndCreateProblemFromException(className,
                                                                        methodName,
                                                                        entity,
                                                                        exception,
                                                                        customMsg,
                                                                        statusCode,
                                                                        logLevel);
        return new AppProblemObjectResult(problem);
    }



    /// <summary>
    /// Creates a custom ProblemDetails response object based upon a message.  Logs to the logger and then returns an AppProblemObjectResult
    /// </summary>
    /// <param name="className"></param>
    /// <param name="methodName"></param>
    /// <param name="entity"></param>
    /// <param name="messageToReturn"></param>
    /// <param name="statusCode"></param>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    protected AppProblemObjectResult ReturnWithProblemFromMessage(string className,
                                                                  string methodName,
                                                                  TEntity? entity,
                                                                  string messageToReturn,
                                                                  int statusCode,
                                                                  LogLevel logLevel = LogLevel.Information)
    {
        ProblemDetailsCustom problem = LogAndCreateProblemFromMessage(className,
                                                                        methodName,
                                                                        entity,
                                                                        messageToReturn,
                                                                        statusCode,
                                                                        logLevel);
        return new AppProblemObjectResult(problem);
    }



    /// <summary>
    /// Creates a custom ProblemDetails response object to be returned to caller.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="statusCode"></param>
    /// <returns></returns>
    protected ProblemDetailsCustom LogAndCreateProblemFromResult(string className,
                                                                 string methodName,
                                                                 TEntity? entity,
                                                                 IResultBase result,
                                                                 int statusCode,
                                                                 LogLevel logLevel = LogLevel.Information)
    {
        ProblemDetailsCustom customErr = new ProblemDetailsCustom()
        {
            RequestMethod = HttpContext.Request.Method + " " + HttpContext.Request.Path,
            TraceId       = HttpContext.Features.Get<IHttpActivityFeature>()?.Activity?.Id,
            Title         = result.ErrorTitle,
            ErrorType     = "Bad Request",
            StatusCode    = statusCode,
            ClassName     = className,
            MethodName    = methodName,
            EntityId      = entity != null ? entity.KeyId : "Not Specified",
        };
        foreach (IError reason in result.Reasons)
            AppendReasonToCustomErr(customErr, reason);


        if (result.Errors[0].Metadata.Count > 0)
            foreach (KeyValuePair<string, object> kvPair in result.Errors[0].Metadata)
                customErr.Metadata.Add(kvPair.Key, kvPair.Value.ToString());

        HttpContext.Response.StatusCode  = customErr.StatusCode;
        HttpContext.Response.ContentType = "application/problem+json";

        _logger.Log(logLevel, result.ErrorTitle, customErr.ToMetaData());

        return customErr;
    }



    /// <summary>
    /// Creates a custom ProblemDetails response object to be returned to caller.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="statusCode"></param>
    /// <returns></returns>
    protected ProblemDetailsCustom LogAndCreateProblemFromMessage(string className,
                                                                 string methodName,
                                                                 TEntity? entity,
                                                                 string message,
                                                                 int statusCode,
                                                                 LogLevel logLevel = LogLevel.Information)
    {
        ProblemDetailsCustom customErr = new ProblemDetailsCustom()
        {
            RequestMethod = HttpContext.Request.Method + " " + HttpContext.Request.Path,
            TraceId       = HttpContext.Features.Get<IHttpActivityFeature>()?.Activity?.Id,
            Title         = message,
            ErrorType     = "Bad Request",
            StatusCode    = statusCode,
            ClassName     = className,
            MethodName    = methodName,
            EntityId      = entity != null ? entity.KeyId : "Not Specified",
        };

        HttpContext.Response.StatusCode  = customErr.StatusCode;
        HttpContext.Response.ContentType = "application/problem+json";

        _logger.Log(logLevel, customErr.Title, customErr.ToMetaData());

        return customErr;
    }

    /// <summary>
    /// Creates a custom ProblemDetails response object to be returned to caller AND logs the exception to the Logger.
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="customMsg"></param>
    /// <param name="statusCode"></param>
    /// <returns></returns>
    protected ProblemDetailsCustom LogAndCreateProblemFromException(string className,
                                                                    string methodName,
                                                                    TEntity entity,
                                                                    Exception exception,
                                                                    string customMsg,
                                                                    int statusCode = StatusCodes.Status500InternalServerError,
                                                                    LogLevel logLevel = LogLevel.Error)
    {
        ProblemDetailsCustom customErr = new ProblemDetailsCustom()
        {
            RequestMethod = HttpContext.Request.Method + " " + HttpContext.Request.Path,
            TraceId       = HttpContext.Features.Get<IHttpActivityFeature>()?.Activity?.Id,
            Title         = customMsg,
            ErrorType     = statusCode > 499 ? "Internal Server Error" : "Bad Request",
            StatusCode    = statusCode,
            ClassName     = className,
            MethodName    = methodName,
            EntityId      = entity != null ? entity.KeyId : "Not Specified",
        };

#if DEBUG
        customErr.Detail = exception.ToString();
#endif

        HttpContext.Response.StatusCode  = customErr.StatusCode;
        HttpContext.Response.ContentType = "application/problem+json";

        _logger.Log(logLevel,
                    exception,
                    "Unhandled Exception encountered in Class [ {ClassName}:{MethodName} ]  processing EntityId [{EntityId}] |  Message: " + customMsg,
                    className,
                    methodName,
                    entity.KeyId);
        return customErr;
    }



    /// <summary>
    /// Creates the full Method name for the current class and the given method name.
    /// </summary>
    /// <param name="methodName"></param>
    /// <returns></returns>
    protected string CalledMethodName(string methodName)
    {
        string className = this.GetType().Name;
        string method    = $"{className}::{ControllerName}::{methodName}";
        return method;
    }



    /// <summary>
    /// Appends the reason to the custom error object.  This will add the reason to the custom error object and then recursively add any reasons that are contained within the reason object.
    /// </summary>
    /// <param name="customErr"></param>
    /// <param name="reason"></param>
    protected void AppendReasonToCustomErr(ProblemDetailsCustom customErr,
                                           IError reason)
    {
        customErr.Reasons.Add(reason.Message);
        foreach (IError error in reason.Reasons)
        {
            AppendReasonToCustomErr(customErr, error);
        }
    }

#endregion
}