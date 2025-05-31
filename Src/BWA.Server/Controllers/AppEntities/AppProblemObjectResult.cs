using Global;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace BWA.Server;


/// <summary>
/// A Custom Problem Details Object Result that allows for enhanced returning of error messages
/// </summary>
public class AppProblemObjectResult : ObjectResult
{
    private const int DEFAULT_STATUS_CODE = StatusCodes.Status500InternalServerError;

    /// <summary>
    /// Returns a Status code 500 with the object passed in as body of the response.
    /// </summary>
    /// <param name="error"></param>
    public AppProblemObjectResult([ActionResultObjectValue]object? error) : base(error) 
    {
        StatusCode = DEFAULT_STATUS_CODE;
    }


    /// <summary>
    /// Creates a new <see cref="AppProblemObjectResult"/> instance.  Status code is set to value in ProblemDetails object
    /// </summary>
    /// <param name="problemDetails"></param>
    public AppProblemObjectResult([ActionResultObjectValue] ProblemDetailsCustom problemDetails) : base(problemDetails)
    {
        ArgumentNullException.ThrowIfNull(problemDetails);
        StatusCode = problemDetails.StatusCode;
    }


    /// <summary>
    /// Returns the ProblemDetailsCustom object with the status code set to the value passed in.
    /// </summary>
    /// <param name="problemDetails"></param>
    /// <param name="statusCode"></param>
    public AppProblemObjectResult([ActionResultObjectValue] ProblemDetailsCustom problemDetails, int statusCode) : base(problemDetails)
    {
        ArgumentNullException.ThrowIfNull(problemDetails);
        problemDetails.StatusCode = statusCode;
        StatusCode = statusCode;
    }
}
