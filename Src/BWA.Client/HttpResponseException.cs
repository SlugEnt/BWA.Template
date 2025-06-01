using Global;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BWA.Client;

/// <summary>
/// Provides Custom exception handling for Http Responses that are not successful.
/// </summary>
public class HttpResponseException : Exception
{
    /// <summary>
    /// Default constructor for HttpResponseException
    /// </summary>
    public HttpResponseException() : base() { }

    /// <summary>
    /// Constructor for HttpResponseException with a message
    /// </summary>
    /// <param name="message"></param>
    public HttpResponseException(string message) : base(message)
    {
    }

    /// <summary>
    /// Constructor for HttpResponseException with a message and an inner exception
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public HttpResponseException(string message, ProblemDetailsCustom problemDetails) : base(message)
    {
        Data.Add("ProblemDetailsCustom",problemDetails);
    }


    public HttpResponseException(string message,
                                 ProblemDetailsCustom problemDetails,
                                 string calledApi,
                                 string callingMethod) : base(message)
    {
        Data.Add("ProblemDetailsCustom", problemDetails);
        Data.Add("CalledApi", calledApi);
        Data.Add("CallingMethod", callingMethod);
    }


    /// <summary>
    /// The standard way to create an HttpResponseException from an HttpContent object.  This will read the content and return a message that can be used to display to the user.
    /// </summary>
    /// <param name="content"></param>
    /// <param name="methodName"></param>
    /// <param name="calledApi"></param>
    /// <returns></returns>
    public static async Task<HttpResponseException> CreateException (HttpContent content, string methodName, string calledApi = "")
    {
        if (content == null)
            return new HttpResponseException($"HttpContent cannot be null.  Must provide the HttpResponse to format a message from it.  Calling API: [{calledApi}]  Calling Method [{methodName}] ");

        // Determine if this is one of our Custom Problem Details objects or something else.
        string               j       = await content.ReadAsStringAsync();
        ProblemDetailsCustom? problem = null;
        if (j.Contains(ProblemDetailsCustom.IS_PROBLEM_DETAILS_CUSTOM) && j.Contains("customType"))
            problem =  JsonSerializer.Deserialize<ProblemDetailsCustom>(j);

        if (problem != null)
            return new HttpResponseException(problem.Title, problem, calledApi,methodName);

        // TODO fix this
        string               msg     = content.ReadAsStringAsync().Result;
        if (string.IsNullOrEmpty(msg))
            return new HttpResponseException("The Http Call did not return a successful status code, but did not provide a reason for the error either. Calling API: [{calledApi}]  Calling Method [{methodName}]");

        return new HttpResponseException($"{msg}.  Calling API: [{calledApi}]  Calling Method [{methodName}]");
    }

    
}
