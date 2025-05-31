using Microsoft.AspNetCore.Components;
using MudBlazor;
using SlugEnt.BWA.BusinessComponents.Abstracts.ErrorManagement;
using SlugEnt.FluentResults;


namespace BWA.BusinessComponents.Generics;

/// <summary>
/// Represents the foundation for a CRUD (Create, Read, Update, Delete) component in a Blazor application.  
/// </summary>
public abstract class AbstractCRUDComponent : ComponentBase
{
    [Inject] protected ISnackbar Snackbar { get; set; }
    [Inject] protected ErrorManager ErrorManager { get; set; }

    protected string _entityName = "";
    protected string _errMsg = "";
    protected bool _errVisible;

    // Modes
    protected bool _isCreateMode;
    protected bool _isDeleteMode;
    protected bool _isEditMode;
    protected bool _isListMode;

    protected bool   _isDisabled; // These are fields that can only be edited during creation.  They are not editable in edit mode.
    protected bool   _isReadOnly;   // Whether the list can be edited in place.
    protected bool   _isViewOnly;
    protected string _pageTitle     = "";
    protected string _returnToPage  = "";
    protected bool   _isInitialized = false;

    protected string _successMsg    = "";
    protected bool   _wasDeleted    = false;

    protected MudForm? form;
    


    /// <summary>
    /// Base Constructor for the CRUD Component.  
    /// </summary>
    /// <param name="entityName"></param>
    /// <param name="returnToPage"></param>
    public AbstractCRUDComponent(string entityName,
                                 string returnToPage)
    {
        _entityName = entityName;
        _returnToPage = returnToPage;
    }


    /// <summary>
    /// Empty Constructor
    /// </summary>
    public AbstractCRUDComponent() { }


    [Inject] protected NavigationManager _navigationManager { get; set; }

    /// <summary>
    /// Sets the mode for the page, which can be Create (C), Edit (E), Delete (D), or View (V), or List.
    /// </summary>
    [Parameter] public string mode { get; set; }

    /// <summary>
    /// The Id of the record to be edited, deleted, or viewed.  This is used to retrieve the record from the database.
    /// </summary>
    /// <remarks>Note, this is always receiced as a string, but needs to be converted into derived classes correct value object</remarks>
    [Parameter] public string? recordId { get; set; }



    /// <summary>
    ///     Cancels and returns to the List View
    /// </summary>
    public void Cancel() { ReturnToList(); }


    
    /// <summary>
    ///     Explanation of the Modes.  Sets the page up according to the requested mode.
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        SetInitialCurrentRecord();

        // Was moved to derived classes
        //if (recordId != null)
          //  _currentRecordId = (int)recordId;

        // If Edit or Delete mode then a record Id must have been provided.
        if ((mode == "E" || mode == "D") && recordId == null)
        {
            _errVisible = true;
            _errMsg     = "No value provided for the Id to retrieve.";
        }

        // Set the page mode.
        if (mode == "L")
            await SetListMode();
        else if (mode == "C")
            SetCreateMode();
        else if (mode == "E")
            await SetEditMode();
        else if (mode == "D")
            SetDeleteMode();
        else
            SetViewMode();


        // Call derived class and tell it the Base Parameters have been set.
        await PostSetParametersAsync();
    }


    /// <summary>
    /// Resets all mode indicators to false.
    /// </summary>
    protected virtual void ResetModes()
    {
        _isCreateMode = false;
        _isReadOnly   = false;
        _isDeleteMode = false;
        _isListMode   = false;
        _isEditMode   = false;

        _isReadOnly = false;
        _isDisabled = false;
    }



    /// <summary>
    /// Sets the page up for Creation mode.
    /// </summary>
    protected virtual void SetCreateMode()
    {
        ResetModes();
        _isCreateMode = true;
        _pageTitle    = "Create " + _entityName;
    }



    /// <summary>
    /// Sets the List Mode
    /// </summary>
    public virtual async Task SetListMode()
    {
        ResetModes();
        _isListMode = true;
        _pageTitle = "Companies";
        
        // Loads the list entities
        await ListLoadEntities();
    }


    /// <summary>
    /// Sets the page up for Edit mode.
    /// </summary>
    protected virtual async Task SetEditMode()
    {
        ResetModes();
        _isEditMode = true;
        _isDisabled = true; // Prevents us from editing fields that should never be changed after creation
        _pageTitle  = "Edit " + _entityName;

        // Load the record to be edited.
        await LoadRecordToBeEdited();
    }

    /// <summary>
    /// Sets page up for Deletion Mode
    /// </summary>
    protected virtual void SetDeleteMode ()
    {
        ResetModes();
        _isDeleteMode = true;
        _isReadOnly = true;
        _pageTitle = "Confirm Deletion of " + _entityName;
    }

    protected virtual void SetViewMode()
    {
        ResetModes();
        _isViewOnly = true;
        _isReadOnly = true;
        _pageTitle = "View " + _entityName;
    }




    /// <summary>
    /// Changes the form to be a Creation form.
    /// </summary>
    /// <returns></returns>
    protected async Task AddEntity()
    {
        // Change to Create Mode.
        SetCreateMode();
    }


    /// <summary>
    /// Used to set the current record on parameter setting
    /// </summary>
    protected abstract void SetInitialCurrentRecord();


    /// <summary>
    /// Loads the entities for the List Mode.
    /// </summary>
    protected abstract Task ListLoadEntities();


    /// <summary>
    /// Loads the record to be edited.  This is where you would read in the record(s) you need.
    /// </summary>
    /// <returns></returns>
    protected abstract Task LoadRecordToBeEdited();


    /// <summary>
    ///     Derived Objects should set this.  This is where you would read in the record(s) you need.
    /// </summary>
    /// <returns></returns>
    protected virtual Task PostSetParametersAsync() => Task.CompletedTask;



    /// <summary>
    ///     Returns to the objects Index page
    /// </summary>
    protected void ReturnToList() { _navigationManager.NavigateTo("/" + _returnToPage); }


    /// <summary>
    /// Sets the parameters for the component. This method is called by the Blazor framework when the component receives new parameters.
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public override async Task SetParametersAsync(ParameterView parameters) { await base.SetParametersAsync(parameters); }
}