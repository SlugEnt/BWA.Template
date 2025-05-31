using MudBlazor.Extensions.Components;
using SlugEnt.FluentResults;
using System.Collections;
using System.Collections.Specialized;

namespace SlugEnt.BWA.BusinessComponents.Abstracts.ErrorManagement;

/// <summary>
/// The ErrorList class is designed to manage a list of AppError objects.  It includes a maximum error count and
/// automatically rolls the oldest off.
/// </summary>
public class ErrorList : IEnumerable<AppError>
{
    internal LinkedList<AppError> _linkedList = new();
    private readonly System.Threading.Lock removeLock = new (); // Lock to handle thread safety when removing items from the list.

    /// <summary>
    /// Constructs an ErrorList object with a specified maximum number of errors.
    /// </summary>
    /// <param name="maxErrors"></param>
    public ErrorList(int maxErrors) : base()
    {
        MaxErrorCount = maxErrors; // Set the maximum number of errors allowed in the list.
    }


    /// <summary>
    /// Maximum number of errors that can be stored in the list.
    /// </summary>
    public int MaxErrorCount { get; set; } = 20;

    public int Count { get { return _linkedList.Count; } }


    /// <summary>
    /// If there are any unread errors in the list.  This is determined by if there is a first unread error.
    /// </summary>
    public bool HasUnreadErrors { get; protected set; }

    public bool IsReadOnly => throw new NotImplementedException();

    public AppError this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void AddError(AppError appError)
    {
        
        lock (removeLock)
        {
            if (_linkedList.Count == MaxErrorCount)
            {
                _linkedList.RemoveFirst();
            }
        }

        _linkedList.AddLast(appError);
        HasUnreadErrors = true; // We have at least one unread error now.

        // If the only item in the list is the one we just added then we need to set
        // it as the first unread appError.
        if (_linkedList.Count == 1)
            _firstUnReadError = _linkedList.First;
    }


    LinkedListNode<AppError> _firstUnReadError;


    /// <summary>
    /// Returns the first unread error in the list.  Returns Failed Result if no unread errors.
    /// <para>Moves the First Unread Error to the next error.</para>
    /// </summary>
    /// <returns></returns>
    public Result<AppError> FirstUnReadError()
    {
        if (_firstUnReadError == null)
            return Result.Fail<AppError>("No unread errors in the list.");
        AppError firstUnread = _firstUnReadError.Value;

        // Move pointer to next unread error.  This will be null if we are currently on the last element.
        _firstUnReadError = _firstUnReadError.Next;
        if (_firstUnReadError == null)
            HasUnreadErrors = false; // No more unread errors in the list.
        return Result.Ok(firstUnread);
    }

    /// <summary>
    /// Returns a list of all unread errors in the list.  Ordered from oldest to newest.
    /// <para>All errors will be marked as read</para>
    /// </summary>
    /// <returns></returns>
    public Result<List<AppError>> GetAllUnreadErrors()
    {
        List<AppError> errors = new List<AppError>();
        while (_firstUnReadError != null)
        {
            errors.Add(_firstUnReadError.Value);
            _firstUnReadError = _firstUnReadError.Next;
        }

        HasUnreadErrors = false; // No more unread errors in the list.
        return Result.Ok(errors);
    }

    public IEnumerator<AppError> GetEnumerator()
    {
        foreach (AppError error in _linkedList) // Iterate through the linked list of errors  
        {
            yield return error;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

}
