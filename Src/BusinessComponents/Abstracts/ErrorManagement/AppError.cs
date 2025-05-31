using SlugEnt.FluentResults;

namespace SlugEnt.BWA.BusinessComponents.Abstracts.ErrorManagement;


/// <summary>
/// Describes an error the client has received.
/// </summary>
public class AppError
{
    /// <summary>
    /// Creates an error object from an exception.
    /// </summary>
    /// <param name="e"></param>
    public AppError(Exception e,
                    string message = "")
    {
        ErrorType = EnumErrorType.Exception;
        Exception = e;
        Message   = message;
        CreatedAt = DateTime.Now;
    }


    /// <summary>
    /// Creates an error object from a message or string.
    /// </summary>
    /// <param name="message"></param>
    public AppError(string message)
    {
        ErrorType = EnumErrorType.Message;
        Message   = message;
        CreatedAt = DateTime.Now;
    }


    /// <summary>
    /// Creates an error object from a Result object.
    /// </summary>
    /// <param name="result"></param>
    public AppError(IResultBase result)
    {
        ErrorType = EnumErrorType.Result;
        Result    = result;
        CreatedAt = DateTime.Now;
    }


    /// <summary>
    /// Creates an error object from a Result object.
    /// </summary>
    /// <param name="result"></param>
    public AppError(Result result)
    {
        ErrorType = EnumErrorType.Result;
        Result    = result;
        CreatedAt = DateTime.Now;
    }


    /// <summary>
    /// Creates an error object from a HttpResponseError object.
    /// </summary>
    /// <param name="httpError"></param>
    public AppError(HttpResponseError httpError)
    {
        ErrorType         = EnumErrorType.Http;
        HttpResponseError = httpError;
        CreatedAt = DateTime.Now;
    }


    /// <summary>
    /// The type of error object.
    /// </summary>
    public EnumErrorType ErrorType { get; protected set; }


    /// <summary>
    /// Summarized textual description of the error
    /// </summary>
    public string Text
    {
        get
        {
            if (ErrorType == EnumErrorType.Message)
                return Message;
            else if (ErrorType == EnumErrorType.Exception)
                return Exception.Message;
            else if (ErrorType == EnumErrorType.Http)
                return HttpResponseError.Title;
            else
                return Result.ErrorTitle != null ? Result.ErrorTitle : "";
        }
    }


    /// <summary>
    /// Will return the entire Result object
    /// </summary>
    public IResultBase? Result { get; protected set; } = null;


    /// <summary>
    /// Will return the message object of the error.
    /// </summary>
    public string Message { get; protected set; } = "";

    /// <summary>
    /// Will return the exception object.
    /// </summary>
    public Exception? Exception { get; protected set; } = null;


    /// <summary>
    /// Will return the HttpResponseError object.
    /// </summary>
    public HttpResponseError? HttpResponseError { get; protected set; } = null;


    /// <summary>
    /// When the error was added to this list.  Note, this is not the time the error occurred.  Although in most cases will be very close...
    /// </summary>
    public DateTime CreatedAt { get; private set; }


    /// <summary>
    /// Returns the string representation of the error.
    /// </summary>
    /// <returns></returns>
    public string ToString()
    {
        if (ErrorType == EnumErrorType.Result)
        {
            return Result == null ? "" : Result.ToString();
        }
        else if (ErrorType == EnumErrorType.Exception)
        {
            return Exception == null ? "" : Exception.ToString();
        }
        else if (ErrorType == EnumErrorType.Http)
        {
            return HttpResponseError == null ? "" : HttpResponseError.ProblemDetailsCustom.Detail;
        }
        else
        {
            return Message;
        }
    }
}