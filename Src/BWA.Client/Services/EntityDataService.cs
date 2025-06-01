using Global;
using Microsoft.Extensions.Caching.Memory;
using Nextended.Core.Extensions;
using SlugEnt.FluentResults;
using System.Net.Http.Json;
using SlugEnt.BWA.BusinessComponents.Abstracts.ErrorManagement;
using SlugEnt.BWA.Entities;
using SlugEnt.BWA.Entities.NonModel;

namespace BWA.Client.Services;

/// <summary>
/// Basic class for data service for retrieving and updating entities on the server via API calls.
/// This class will communicate with the server via HTTP calls to the API controllers.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class EntityDataService<TEntity>  where TEntity : class, IEntity
{
    private readonly HttpClient?  _httpClient;
    private          IMemoryCache _memoryCache;


    /// <summary> Backing Field for ApiName. This is the name of the API that this service will use to communicate with the server. </summary>
    protected string _apiName = string.Empty;

    /// <summary>
    /// The name of the API that this service will use to communicate with the server. This is used to construct the URL for the API calls.
    /// <para">Only need to specify the controller name</param>
    /// </summary>
    public string ApiName
    {
        get { return _apiName; }
        set
        {
            _apiName       = "api/" + value + "/";
            CacheKeyPrefix = "LE_" + value; // CacheKey is used to store the list of entities in memory cache
        }
    }


    /// <summary>
    ///  Cache key prefix for this Service
    /// </summary>
    protected string CacheKeyPrefix { get; set; }


    /// <summary>
    /// Dispose of the HttpClient when the service is no longer needed. This is important to prevent memory leaks
    /// </summary>
    public void Dispose() => _httpClient?.Dispose();


    private ErrorManager _errorManager = null;


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="httpClient"></param>
    [ActivatorUtilitiesConstructor]
    public EntityDataService(HttpClient httpClient,
                             IMemoryCache memoryCache,
                             ErrorManager errorManager)
    {
        _httpClient   = httpClient;
        _memoryCache  = memoryCache;
        _errorManager = errorManager;
    }



    /// <summary>
    /// Adds the given entity to the system, by calling the controller action Post.
    /// </summary>
    /// <param name="entity">Entity to be added</param>
    /// <param name="addedByPersonId">The Id of the person who added them</param>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    public async Task<Result<TEntity>> AddAsync(TEntity entity,
                                                int addedByPersonId)
    {
        string apiUrl = ApiName;
        try
        {
            EntityTransport<TEntity> entityTransport = new()
            {
                ChangeMadeByPersonId = addedByPersonId,
                Entity               = entity
            };


            using (HttpResponseMessage response = await _httpClient.PostAsJsonAsync(apiUrl, entityTransport))
            {
                response.WriteRequestToConsole();
                if (!response.IsSuccessStatusCode)
                {
                    HttpResponseError error = await HttpResponseError.CreateHttpResponseError(response.StatusCode,
                                                                                              typeof(TEntity).Name,
                                                                                              nameof(AddAsync),
                                                                                              nameof(EntityDataService<TEntity>),
                                                                                              apiUrl,
                                                                                              response.Content);
                    return LogError(error);
                }

                // Retrieve the entity and return to caller.
                entity = await response.Content.ReadFromJsonAsync<TEntity>();
                return Result.Ok(entity);
            }
        }
            catch (Exception e)
            {
                Result result = LogError(e,
                                         nameof(EntityDataService<TEntity>),
                                         nameof(AddAsync),
                                         typeof(TEntity).Name,
                                         $"Failed to save the new entity",
                                         apiUrl);
                return result;
            }
    }


    /// <summary>
    /// Returns the entire list of entities.
    /// </summary>
    /// <param name="includeActive">Whether to include Active entities.</param>
    /// <param name="includeInactive">Whether to include Inactive entities.</param>
    /// <returns></returns>
    public async Task<Result<List<TEntity>>> GetAllAsync(bool includeActive,
                                                         bool includeInactive)
    {
        string apiPath = ApiName + "List/";
        string apiUrl  = "";

        try
        {
            // TODO Junk code - just testing something

            Exception e     = new Exception("Test Exception " + DateTime.Now);
            AppError  error = new AppError(e);
            _errorManager.ErrorList.AddError(error);
            _errorManager.AddError(e);
            _errorManager.AddError("Test Error Message " + DateTime.Now);

            // TODO - END JUNK CODE

            bool useCache = false;

            

            char activeCriteria = ' ';
            if (includeActive && includeInactive)
                activeCriteria = 'B';
            else if (includeActive)
                activeCriteria = 'A';
            else if (includeInactive)
                activeCriteria = 'I';

            List<TEntity> list;

            // Check Cache - We only store cache for Active objects
            if (activeCriteria == 'A')
                useCache = true;
            string cacheKey = "";

            if (useCache)
            {
                cacheKey = CacheKeyPrefix + "_GetAllAsync"; // CacheKey is used to store the list of entities in memory cache
                if (_memoryCache.TryGetValue(cacheKey, out list))
                    return Result.Ok(list);
            }


            apiUrl = apiPath + activeCriteria;
            using (HttpResponseMessage? response = await _httpClient.GetAsync(apiUrl))
            {
                response.WriteRequestToConsole();

                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpResponseException.CreateException(response.Content, nameof(GetAllAsync), apiUrl);
                }

                //throw new HttpResponseException (response.Content);
                response.EnsureSuccessStatusCode();
                list = await response.Content.ReadFromJsonAsync<List<TEntity>>();

                // Store in cache
                if (useCache)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                                            .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                                            .SetAbsoluteExpiration(TimeSpan.FromHours(1)); // Cache for 30 minutes / MAx 1 hour
                    _memoryCache.Set(cacheKey, list, cacheEntryOptions);
                }

                return Result.Ok(list);
            }
        }
        catch (Exception e)
        {
            Result result = LogError(e,
                                     nameof(EntityDataService<TEntity>),
                                     nameof(GetAllAsync),
                                     typeof(TEntity).Name,
                                     $"Failed to Retrieve the list of entities",
                                     apiUrl);
            return result;
        }
    }


    /// <summary>
    /// Retrieves the requested entity by Id.  This will return the entity with the given Id, or null if it does not exist.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Result.Ok [TEntity> or Result.Failed [Errors]</returns>
    protected async Task<Result<TEntity?>> GetByIdAnyStatusAsync(string idAsString)
    {
        string apiUrl = $"{ApiName}{idAsString}/any";
        try
        {
            using (HttpResponseMessage response = await _httpClient.GetAsync(apiUrl))
            {
                response.WriteRequestToConsole();
                if (!response.IsSuccessStatusCode)
                {
                    HttpResponseError error = await HttpResponseError.CreateHttpResponseError(response.StatusCode,
                                                                                              typeof(TEntity).Name,
                                                                                              nameof(GetByIdAnyStatusAsync),
                                                                                              nameof(EntityDataService<TEntity>),
                                                                                              apiUrl,
                                                                                              response.Content);
                    return LogError(error);
                }

                TEntity entity = await response.Content.ReadFromJsonAsync<TEntity>();
                return Result.Ok(entity);
            }
        }
        catch (Exception e)
        {
            Result result = LogError(e,
                                     nameof(EntityDataService<TEntity>),
                                     nameof(GetByIdAnyStatusAsync),
                                     typeof(TEntity).Name,
                                     $"Failed to retrieve an entity with Id: {idAsString} with ANY IsActive Value",
                                     apiUrl);
            return result;
        }
    }



    /// <summary>
    /// Retrieves an Entity that has the provided Id value.  
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Result.Ok [Entity]  OR  Result.Fail [Errors]</returns>
    public async Task<Result<TEntity?>> GetByIdAsync(string idAsString)
    {
        string apiUrl = $"{ApiName}{idAsString}";
        try
        {
            using (HttpResponseMessage response = await _httpClient.GetAsync(apiUrl))
            {
                response.WriteRequestToConsole();
                if (!response.IsSuccessStatusCode)
                {
                    HttpResponseError error = await HttpResponseError.CreateHttpResponseError(response.StatusCode,
                                                                                              typeof(TEntity).Name,
                                                                                              nameof(GetByIdAnyStatusAsync),
                                                                                              nameof(EntityDataService<TEntity>),
                                                                                              apiUrl,
                                                                                              response.Content);
                    return LogError(error);
                }

                TEntity entity = await response.Content.ReadFromJsonAsync<TEntity>();
                return Result.Ok(entity);
            }
        }
        catch (Exception e)
        {
            Result result = LogError(e,
                                     nameof(EntityDataService<TEntity>),
                                     nameof(GetByIdAsync),
                                     typeof(TEntity).Name,
                                     $"Failed to retrieve an entity with Id: {idAsString}",
                                     apiUrl);
            return result;
        }
    }



    /// <summary>
    /// Updates the entity using the API Controller.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="updatedByPersonId"></param>
    /// <returns></returns>
    public async Task<Result> UpdateAsync(TEntity entity,
                                          int updatedByPersonId)
    {
        string apiUrl = ApiName;

        try
        {
            //string apiPath = ApiName;

            EntityTransport<TEntity> entityTransport = new()
            {
                ChangeMadeByPersonId = updatedByPersonId,
                Entity               = entity
            };


            using (HttpResponseMessage response = await _httpClient.PutAsJsonAsync(ApiName, entityTransport))
            {
                response.WriteRequestToConsole();
                if (!response.IsSuccessStatusCode)
                {
                    HttpResponseError error = await HttpResponseError.CreateHttpResponseError(response.StatusCode,
                                                                                              typeof(TEntity).Name,
                                                                                              nameof(UpdateAsync),
                                                                                              nameof(EntityDataService<TEntity>),
                                                                                              ApiName,
                                                                                              response.Content);
                    return LogError(error);
                }

                return Result.Ok();
            }
        }
        catch (Exception e)
        {
            Result result = LogError(e,
                                     nameof(EntityDataService<TEntity>),
                                     nameof(GetByIdAsync),
                                     typeof(TEntity).Name,
                                     $"Failed to update the entity with Id: {entity.KeyId}",
                                     apiUrl);
            return result;
        }
    }


    /// <summary>
    /// Adds the Entity using the API Controller.
    /// </summary>
    /// <param name="entity">Entity to be added to the system</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Result<TEntity>> AddAsync(TEntity entity)
    {
        try
        {
            // TODO This needs to be replaced with the logged in user.
            return await AddAsync(entity, 1);
        }
        catch (Exception e)
        {

            Result result = LogError(e,
                                     "EntityDataService",
                                     "AddAsync(Entity)",
                                     typeof(TEntity).Name,
                                     "Failed to save the new entity.");
            return result;
        }
    }


    protected Task<Result> DeleteAsync(string idAsString,
                                    int updatedByPersonId)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Deactivates the entity.  This will set the IsActive flag to false and set the UpdatedByPersonId to the given value.
    /// </summary>
    /// <param name="id">Id of the entity to deactivate</param>
    /// <param name="updatedByPersonId">The person who is requesting the deactivation</param>
    /// <returns></returns>
    protected async Task<Result> DeActivateAsync(string idAsString,
                                              int updatedByPersonId)
    {
        string apiUrl = ApiName + idAsString + "/" + "deactivate" + "/" + updatedByPersonId;
        try
        {
            

            EntityTransportById entityTransportById = new()
            {
                ChangeMadeByPersonId = 1,
                EntityId             = idAsString
            };


            using (HttpResponseMessage response = await _httpClient.PutAsync(apiUrl, null))
            {
                response.WriteRequestToConsole();
                response.EnsureSuccessStatusCode();
                return Result.Ok();
            }
        }
        catch (Exception e)
        {
            Result result = LogError(e,
                                     nameof(EntityDataService<TEntity>),
                                     nameof(DeActivateAsync),
                                     typeof(TEntity).Name,
                                     $"Failed to De-activate the entity with Id: {idAsString}",
                                     apiUrl);
            return result;
        }

    }



    /// <summary>
    /// Activates the entity.  This will set the IsActive flag to true and set the UpdatedByPersonId to the given value.
    /// </summary>
    /// <param name="id">Id of the entity to Activate</param>
    /// <param name="updatedByPersonId">The person who is requesting the activation</param>
    /// <returns></returns>
    protected async Task<Result> ActivateAsync(string idAsString,
                                            int updatedByPersonId)
    {
        string apiUrl = ApiName + idAsString + "/" + "activate" + "/" + updatedByPersonId;
        try
        {
            

            EntityTransportById entityTransportById = new()
            {
                ChangeMadeByPersonId = 1,
                EntityId             = idAsString
            };


            using (HttpResponseMessage response = await _httpClient.PutAsync(apiUrl, null))
            {
                response.WriteRequestToConsole();
                response.EnsureSuccessStatusCode();
                return Result.Ok();
            }
        }
        catch (Exception e)
        {
            Result result = LogError(e,
                                     nameof(EntityDataService<TEntity>),
                                     nameof(ActivateAsync),
                                     typeof(TEntity).Name,
                                     $"Failed to Activate the entity with Id: {idAsString}",
                                     apiUrl);
            return result;
        }
    }


    /// <summary>
    /// Logs the Error to the Error Manager and generates a Result object from it and then returns the Result object.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    private Result LogError(HttpResponseError error)
    {
        _errorManager.AddError(error);
        return Result.Fail(error.Title);
    }


    /// <summary>
    /// Logs the Exception to the logger and generates a Result object from it and then returns the Result object.
    /// </summary>
    /// <param name="e">The Exception</param>
    /// <param name="serviceName">Name of the service that the exception occurred in</param>
    /// <param name="methodName">Name of the method that the exception occurred or was detected in</param>
    /// <param name="entityType">The name of the entity class that the exception occurred in</param>
    /// <param name="customMsg">Message to be placed on the error.</param>
    /// <returns></returns>
    private Result LogError(Exception e,
                            string serviceName,
                            string methodName,
                            string entityType,
                            string customMsg,
                            string apiUrl = "")
    {
        string msg;
        Result result = Result.Fail(customMsg)
                              .AddError(new ExceptionalError(e))
                              .AddMetaData("Method", $"{entityType}::EntityDataService::{methodName}")
                              .AddMetaData("Service", serviceName)
                              .AddMetaData("Api Url", apiUrl);
        return result;
    }



    /// <summary>
    /// Logs the error message and the problem details to the logger and generates a Result object from it and then returns the Result object.
    /// </summary>
    /// <param name="serviceName">Name of the service that the exception occurred in</param>
    /// <param name="methodName">Name of the method that the exception occurred or was detected in</param>
    /// <param name="entityType">The name of the entity class that the exception occurred in</param>
    /// <param name="title">The summarization of the error message.</param>
    /// <param name="problem">The actual ProblemDetailsCustom message that we are logging about</param>
    /// <returns></returns>
    private Result LogSendResponseError(string serviceName,
                                        string methodName,
                                        string entityType,
                                        string title,
                                        string apiUrl,
                                        ProblemDetailsCustom problem)
    {
        string msg;

        Error error = new Error(title);

        // Convert the problem fields into metadata
        problem.AsMetaData(error.Metadata);

        error.Metadata.Add("Service Class", serviceName);
        error.Metadata.Add("Api Url", apiUrl);

        // Add the actual error message metadata as metadata.
        foreach (KeyValuePair<string, string> kvPair in problem.Metadata)
            error.Metadata.Add(kvPair.Key, kvPair.Value);

        foreach (string problemReason in problem.Reasons)
            error.Reasons.Add(new Error(problemReason));

        Result result = Result.Fail(error);
        return result;
    }



    /// <summary>
    /// Logs the error message and the problem details to the logger and generates a Result object from it and then returns the Result object.
    /// </summary>
    /// <param name="serviceName">Name of the service that the exception occurred in</param>
    /// <param name="methodName">Name of the method that the exception occurred or was detected in</param>
    /// <param name="entityType">The name of the entity class that the exception occurred in</param>
    /// <param name="jsonProblem">A json string that provides information about the issue the API had.  This is used for APIs outside of our control that return a different response</param>
    /// <returns></returns>
    private Result LogSendResponseError(string serviceName,
                                        string methodName,
                                        string entityType,
                                        string jsonProblem,
                                        string apiUrl = "")
    {
        string msg;

        Error error = new Error("Api Call Failed - ");
        error.Metadata.Add("Info", jsonProblem);
        error.Metadata.Add("Service Class", serviceName);
        error.Metadata.Add("Api Url", apiUrl);


        Result result = Result.Fail(error);
        return result;
    }
}


/// <summary>
/// Extension to the HTTP Response object that provides more detailed information by writing the request to the console
/// </summary>
static class HttpResponseMessageExtensions
{
    internal static void WriteRequestToConsole(this HttpResponseMessage response)
    {
        if (response is null)
        {
            return;
        }

        var request = response.RequestMessage;
        Console.Write($"{request?.Method} ");
        Console.Write($"{request?.RequestUri} ");
        Console.WriteLine($"HTTP/{request?.Version}");
    }
}