using Global;
using Humanizer;
using System.Net;
using System.Text.Json;

namespace SlugEnt.BWA.BusinessComponents.Abstracts.ErrorManagement;


public class HttpResponseError
{
    public HttpResponseError(string title,
                             string calledApi = "",
                             string callingMethod = "",
                             string serviceClass = "",
                             string apiUrl = "")
    {
        Title            = title;
        CalledApi        = calledApi;
        CallingMethod    = callingMethod;
        ServiceClassName = serviceClass;
        ApiUrl = apiUrl;
    }


    public HttpResponseError(string title,
                             ProblemDetailsCustom problemDetails,
                             string calledApi,
                             string callingMethod,
                             string serviceClass = "",
                             string apiUrl = "") : this(title, calledApi, callingMethod, apiUrl)
    {
        ProblemDetailsCustom = problemDetails;
    }


    /// <summary>
    /// Ensure JSON serialization Options are set correctly.
    /// </summary>
    private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions()
    {
        PropertyNameCaseInsensitive = true,
    };


    /// <summary>
    /// The error message title
    /// </summary>
    public string Title { get; protected set; }


    /// <summary>
    /// The Problem Details Custom object associate with this error.
    /// </summary>
    public ProblemDetailsCustom? ProblemDetailsCustom { get; protected set; } = null;


    /// <summary>
    /// The API that was called that generated this error.
    /// </summary>
    public string CalledApi { get; protected set; }


    /// <summary>
    /// The method that called the API that generated this error.
    /// </summary>
    public string CallingMethod { get; protected set; }


    /// <summary>
    /// The service or class that contains the method that detected the failure.
    /// </summary>
    public string ServiceClassName { get; protected set; }


    /// <summary>
    /// The API Url that was used to call the API.
    /// </summary>
    public string ApiUrl { get; set; }


    public static async Task<HttpResponseError> CreateHttpResponseError(HttpStatusCode statusCode,
                                                                        string calledApi,
                                                                        string callingMethod,
                                                                        string serviceClassName,
                                                                        string apiUrl,
                                                                        HttpContent content)
    {
        ProblemDetailsCustom? problem = null;

        // There was no content, just a failed status code.  Convert it to a ProblemDetailsCustom object.
        if (content == null)
        {
            string text = statusCode.Humanize();
            problem = new ProblemDetailsCustom
            {
                Title      = text,
                StatusCode = (int)statusCode,
            };
            return new HttpResponseError(text,
                                         problem,
                                         calledApi,
                                         callingMethod,
                                         apiUrl,
                                         serviceClassName);
        }

        string j = await content.ReadAsStringAsync();

        // Convert to ProblemDetailsCustom if response is of that type.
        if (j.Contains(ProblemDetailsCustom.IS_PROBLEM_DETAILS_CUSTOM) && j.Contains("customType"))
        {
            problem = JsonSerializer.Deserialize<ProblemDetailsCustom>(j, _jsonOptions);
            return new HttpResponseError(problem.Title,
                                         problem,
                                         calledApi,
                                         callingMethod);
        }

        // If here, then we do not have ProblemDetailsCustom object, see if it's a ProblemDetails object.  If so we force it into a ProblemDetailsCustom object 
        // since ProblemDetailsCustom contains the minimum fields as a ProblemDetails object.
        if (j.Contains("title") && j.Contains("detail"))
        {
            problem            = JsonSerializer.Deserialize<ProblemDetailsCustom>(j, _jsonOptions);
            problem.StatusCode = (int)statusCode;
            return new HttpResponseError(problem.Title,
                                         problem,
                                         calledApi,
                                         callingMethod);
        }

        // If here, we do not have a ProblemDetailsCustom, nor a ProblemDetails object.  We will create a ProblemDetailsCustom object with the detail set to the content field as a string of JSON.
        problem = new ProblemDetailsCustom
        {
            Title      = statusCode.Humanize(),
            StatusCode = (int)statusCode,
            Detail     = j
        };

        return new
            HttpResponseError("API call resulted in a non-successful status code.  However the reason code was in a non-standard format.  See Details property for the response value.",
                              problem,
                              calledApi,
                              callingMethod);


        return new HttpResponseError($" Calling API: [{calledApi}]  Calling Method [{callingMethod}] ");
    }
}