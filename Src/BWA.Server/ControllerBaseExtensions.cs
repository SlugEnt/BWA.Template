using Global;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace BWA.Server;

/// <summary>
/// Custom Extensions for the ControllerBase class.
/// </summary>
public static class ControllerBaseExtensions
{
    /// <summary>
    /// Returns a Status code 500 with the object passed in as body of the response.
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="problemDetails"></param>
    /// <returns></returns>
    [NonAction]
    public static AppProblemObjectResult AppProblem (this ControllerBase controller, [ActionResultObjectValue] ProblemDetailsCustom problemDetails)
    {
        return new AppProblemObjectResult(problemDetails);
    }


    /// <summary>
    /// Returns a ProblemDetailsCustom object with the status code set to the value passed in.
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="problemDetails">The ProblemDetailCustom object that describes the problem</param>
    /// <param name="statusCode">Status code to set.  If 0 it will use the one in ProblemDetails Or if both are zero it will set to 500</param>
    /// <returns></returns>
    [NonAction]
    public static AppProblemObjectResult AppProblem(this ControllerBase controller,
                                                    string controllerClassName, string methodName,
                                                    [ActionResultObjectValue] ProblemDetailsCustom problemDetails, int statusCode = 0)
    {
        // If status code is not specified in arguments or in problemDetails set to Server Error
        if (statusCode == 0 && problemDetails.StatusCode == 0)
            statusCode = problemDetails.StatusCode = StatusCodes.Status500InternalServerError;
        if (statusCode == 0)
            statusCode = problemDetails.StatusCode;
        return new AppProblemObjectResult(problemDetails, statusCode);
    }
}
