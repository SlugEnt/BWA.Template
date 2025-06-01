namespace Global;

/// <summary>
/// Class used to transmit Problem Details from the API to the caller.
/// </summary>
public class ProblemDetailsCustom
{
    public const string IS_PROBLEM_DETAILS_CUSTOM = "PDC2025";


    public ProblemDetailsCustom()
    {
        StatusCode = 500;
        CustomType = IS_PROBLEM_DETAILS_CUSTOM;
    }


    public string CustomType { get; protected set; }


    // These properties are part of the ProblemDetails base class.  Reproduced here.


#region "ProblemDetailBase"

    /// <summary>
    /// An IETF Error type code that references a URI that identifies the error type.
    /// </summary>
    public string Type { get; set; } = "";

    /// <summary>
    /// Short human readable summary of the problem.  It should NOT change from one occurrence to another.
    /// </summary>
    public string Title { get; set; } = "";


    /// <summary>
    /// Human readable explanation of the problem.  It likely will change from one occurrence to another.
    /// </summary>
    public string Detail { get; set; } = "";


    /// <summary>
    /// The Status code.  Same as StatusCode
    /// </summary>
    public int Status { get { return StatusCode; } set { StatusCode = Status; } }

    /// <summary>
    /// TraceId
    /// </summary>
    public string TraceId { get; set; } = "";

    /// <summary>
    /// List of String Error Codes
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// The Category of Error.
    /// </summary>
    public string Category { get; set; }

#endregion


    // These are custom properties that are not part of the ProblemDetails base class.

    // Description of the type of error
    public string ErrorType { get; set; } = "";

    /// <summary>
    /// The Status code.  Same as Status.
    /// </summary>
    public int StatusCode { get; set; } = 0;


    public string RequestMethod { get; set; } = "";

    public string RequestId { get; set; } = "";

    /// <summary>
    /// Class Mame that reported the error, this may not be the actual class the error occurred in.
    /// </summary>
    public string ClassName { get; set; } = "";

    /// <summary>
    /// Name of the method that reported the error, this may not be the actual method the error occurred in.
    /// </summary>
    public string MethodName { get; set; } = "";


    /// <summary>
    /// The Id of the entity that caused the error.
    /// </summary>
    public string EntityId { get; set; } = "";


    /// <summary>
    /// Metadata about the error
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// List of Reasons why the error occurred.
    /// </summary>
    public List<string> Reasons { get; set; } = new();


    /// <summary>
    /// Convert MetaData to a Dictionary [string, Object]
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, object> ToMetaData()
    {
        Dictionary<string, object> metaData = new();
        if (!string.IsNullOrEmpty(Title))
            metaData.Add("Title", Title);
        if (!string.IsNullOrEmpty(Detail))
            metaData.Add("Detail", Detail);
        if (!string.IsNullOrEmpty(ClassName))
            metaData.Add("ClassName", ClassName);
        if (!string.IsNullOrEmpty(MethodName))
            metaData.Add("MethodName", MethodName);
        if (!string.IsNullOrEmpty(ErrorType))
            metaData.Add("ErrorType", ErrorType);
        if (!string.IsNullOrEmpty(RequestMethod))
            metaData.Add("RequestMethod", RequestMethod);
        if (!string.IsNullOrEmpty(RequestId))
            metaData.Add("RequestId", RequestId);
        if (!string.IsNullOrEmpty(TraceId))
            metaData.Add("TraceId", TraceId);
        if (StatusCode != 0)
            metaData.Add("StatusCode", StatusCode);
        return metaData;
    }


    public void AsMetaData(Dictionary<string, object> d)
    {
        if (!string.IsNullOrEmpty(Title))
            d.Add("Title", Title);
        if (!string.IsNullOrEmpty(Detail))
            d.Add("Message", Detail);
        if (StatusCode != 0)
            d.Add("StatusCode", StatusCode);
        if (!string.IsNullOrEmpty(ErrorType))
            d.Add("ErrorType", ErrorType);
        if (!string.IsNullOrEmpty(ClassName))
            d.Add("ClassName", ClassName);
        if (!string.IsNullOrEmpty(MethodName))
            d.Add("MethodName", MethodName);

        if (!string.IsNullOrEmpty(RequestMethod))
            d.Add("RequestMethod", RequestMethod);
        if (!string.IsNullOrEmpty(RequestId))
            d.Add("RequestId", RequestId);
        if (!string.IsNullOrEmpty(TraceId))
            d.Add("TraceId", TraceId);
    }
}