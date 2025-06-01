using SlugEnt.FluentResults;
using System.ComponentModel;


namespace SlugEnt.BWA.BusinessComponents.Abstracts.ErrorManagement;

/// <summary>
/// Handles the global wide errors by the application
/// </summary>
public class ErrorManager : INotifyPropertyChanged
{
    /// <summary>
    /// Constructor
    /// </summary>
    public ErrorManager() { }


    /// <summary>
    /// The List of errors.
    /// </summary>
    public ErrorList ErrorList { get; protected set; } = new ErrorList(20);

    /// <summary>
    /// True, if the ErrorList contains new errors
    /// </summary>
    public bool HasUnreadErrors { get { return ErrorList.HasUnreadErrors; } }


    /// <summary>
    /// True if the ErrorList has any errors in it.
    /// </summary>
    public bool HasErrors { get { return ErrorList.Count > -0; } }

    /// <summary>
    /// Returns the list as an Enumerable
    /// </summary>
    public IEnumerator<AppError> AsEnumerable => ErrorList.GetEnumerator();


    /// <summary>
    /// Something has changed on the ErrorList
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;


    /// <summary>
    ///Invokes the PropertyChanged Event when something on the List has Changed.
    /// </summary>
    /// <param name="propertyName"></param>
    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    /// <summary>
    /// Adds a new error
    /// </summary>
    /// <param name="appError"></param>
    private void AddError(AppError appError)
    {
        ErrorList.AddError(appError);
        OnPropertyChanged(nameof(HasUnreadErrors));
    }


    /// <summary>
    /// Adds a new error
    /// </summary>
    /// <param name="result"></param>
    public void AddError(IResultBase result)
    {
        AppError appError = new AppError(result);
        AddError(appError);
    }


    /// <summary>
    /// Adds a new error
    /// </summary>
    /// <param name="result"></param>
    public void AddError(IResult<Object> result)
    {
        AppError appError = new AppError(result);
        AddError(appError);
    }


    /// <summary>
    /// Adds a new string message Error
    /// </summary>
    /// <param name="message"></param>
    public void AddError(string message)
    {
        AppError appError = new AppError(message);
        AddError(appError);
    }


    /// <summary>
    /// Adds a new Exception Error.  The Error message will be the MessagePrefix plus exception message or just the exception message if no prefix provided.
    /// </summary>
    /// <param name="e"></param>
    public void AddError(Exception e,
                         string messagePrefix = "")
    {
        string errorMsg = "";
        if (messagePrefix == string.Empty)
            errorMsg = e.Message;
        else
            errorMsg = $"{messagePrefix} --> {e.Message}";

        AppError appError = new AppError(e, errorMsg);
        ErrorList.AddError(appError);
    }


    public void AddError(HttpResponseError error)
    {
        AppError appError = new AppError(error);
        AddError(appError);
    }


}

