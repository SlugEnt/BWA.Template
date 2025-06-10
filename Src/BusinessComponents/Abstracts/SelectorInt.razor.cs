using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudExtensions;
using SlugEnt.BWA.Entities;
using SlugEnt.FluentResults;

namespace SlugEnt.BWA.BusinessComponents.Abstracts;

/// <summary>
/// The SelectorInt is a generic component that allows the user to select an entity from a list of entities.
/// This is used for entities that have an integer key and implement the IEntityLookup interface.
/// </summary>
/// <typeparam name="TEntityInt"></typeparam>
public partial class SelectorInt<TEntityInt> : ComponentBase where TEntityInt : AbstractEntityInt, new()
{
    /// <summary>
    /// This is the default constructor.  It is used to set the default values for the component.
    /// </summary>
    public SelectorInt()
    {
        // Set the default values for the component.
        _intValueId = -1;
        _intValueObject = null;
        _isReady = false;
        _initializationCompleted = false;
    }

#region Parameters

    /// <summary> Optional argument.  If set, this will be the Id of the entity object that is selected.  Supply this or ValueObject. </summary>
    [Parameter]
    public int? ValueId
    {
        get;
        set;
    } = -1;


    /// <summary> This is the event that is called when the user selects a SubDivision.  The new value will be passed to the parent component. </summary>
    [Parameter]
    public EventCallback<TEntityInt> ValueUpdated { get; set; }


    /// <summary> This is the label for the select box.  If not set, the ApiName will be used instead. </summary>
    [Parameter]
    public string? Label { get; set; } = "";



    /// <summary> Thia is the name of the API controller, usually just a plural of the singular entity.  So office would be offices. </summary>
    [Parameter]
    public string ApiName { get; set; } = "";

    /// <summary> This is used to determine how the selected value is displayed in the select box.  Text or Icon. </summary>
    [Parameter]
    public ValuePresenter ValuePresenter { get; set; } = ValuePresenter.Text;


    [Parameter] public Variant Variant { get; set; } = Variant.Text;


    /// <summary> The Repository Service that will be used to retrieve the entity data from the server. </summary>
    [Inject]
    protected IEntityRepositoryE2Int<TEntityInt>? _entityLookupService { get; set; }

#endregion


#region "Local Variables"

    // Internally used variables for the ID and the object.
    private TEntityInt? _intValueObject = null;
    private int      _intValueId     = -1;
    

    // Set after the component is initialized and the entity list is loaded.  This is used to prevent the component from rendering before the data is ready.
    private bool _isReady = false;

    private List<TEntityInt> _entityList = new();

    private bool isProcessing = false;

    //private        short         _initialScenario         = 0;
    bool _initializationCompleted = false; // set of OnInitialized has run.

#endregion



    /// <summary>
    /// The user has selected a new entity.  This will set the internal ValueObject to the new entity and call the ValueUpdated event, so the caller is notified.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    private async Task EntitySelectChanged(TEntityInt entity)
    {
        if (_intValueObject != entity)
        {
            _intValueObject = entity;
            _intValueId     = entity == null ? 0 : entity.Id; // if null, set to 0

            await ValueUpdated.InvokeAsync(_intValueObject);
        }
    }



    /// <summary>
    /// Load entity list and peform other actions once all parameters are set.
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        if (!_initializationCompleted)
            return;

        bool listReadSuccess = false;

        if (isProcessing)
            return;

        // Simple Parameter checks...
        if (string.IsNullOrEmpty(Label)) // If the label has changed, set the label text to the new value.
            Label = ApiName;
        try
        {
            isProcessing = true;

            if (_isReady == false)
            {
                _entityLookupService!.ApiName = ApiName;
                Result<List<TEntityInt>> result = await _entityLookupService!.GetAllAsync(true, false);
                if (result.IsFailed)
                {
                    // TODO Do Something with error
                }
                else
                {
                    _entityList     = result.Value;
                    listReadSuccess = true;
                }
            }
            else
            {
                if (_intValueId != ValueId)
                {
                    _intValueId = ValueId ?? 0; // if null, set to 0

                    // The user has changed the ID parameter, so set the ValueObject to the selected entity.
                    _intValueObject = _entityList.SingleOrDefault(s => s.Id == _intValueId);
                }
            }

            if (_isReady == false && listReadSuccess)
                _isReady = true;
        }

        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            isProcessing = false;
        }
    }



    /// <summary>
    /// This is called when the component is initialized.  
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (!RendererInfo.IsInteractive)
            return;

        _initializationCompleted = true;
    }
}