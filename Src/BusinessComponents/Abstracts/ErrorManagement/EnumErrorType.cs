
namespace SlugEnt.BWA.BusinessComponents.Abstracts.ErrorManagement;
/// <summary>
/// Describes the type of error object.  
/// </summary>
public enum EnumErrorType
{
    /// <summary> A Result object that was in status failed. </summary>
    Result = 1,

    /// <summary> An error that was generated via an Exception. </summary>
    Exception = 2,

    /// <summary> Simple string message that describes the error.  This is typically used for validation errors or other simple messages. </summary>
    Message = 3,

    /// <summary> An HttpResponseError (Custom error object) that provides a more detailed error message than a standard error message. </summary>
    Http = 4,
}