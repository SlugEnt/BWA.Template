namespace SlugEnt.BWA.BusinessComponents.Abstracts.ErrorManagement;

public class ErrorListEnumerator
{
    private ErrorList _errorList;
    public ErrorListEnumerator(ErrorList errorList)
    {
        _errorList = errorList ?? throw new ArgumentNullException(nameof(errorList));
    }

    public IEnumerator<AppError> GetEnumerator()
    {
        foreach (AppError error in _errorList)
        {
            
        }
        return _errorList.GetEnumerator();
    }
}