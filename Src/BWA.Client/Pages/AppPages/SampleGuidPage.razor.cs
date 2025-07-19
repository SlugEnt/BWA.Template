using BWA.BusinessComponents.Generics;
using SlugEnt.BWA.Entities.FluentValidators;
using SlugEnt.FluentResults;
using SlugEnt.HR.NextGen.Entities.Models;

namespace SlugEnt.BWA.Client.Pages;

/// <summary>
/// This is the Company Component Page which handles CRUD operations on the Company Entity
/// </summary>
public partial class SampleGuidPage : AbstractE2ComponentGuid<SampleGuid>
{

    private List<SampleGuid>  _entities;
    SampleGuidFluentValidator _validator = new SampleGuidFluentValidator();


    /// <summary>
    /// Constructor.  Sets APIName to be called.
    /// </summary>
    public SampleGuidPage() : base("Sample Guid", "Sample Guid")
    {
        _apiName = "SampleGuids";
    }

    /// <summary>
    ///     Method invoked when the component is ready to start, having received its
    ///     initial parameters from its parent in the render tree.
    ///     Override this method if you will perform an asynchronous operation and
    ///     want the component to refresh when that operation is completed.
    /// </summary>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // This MUST ALWAYS BE SET in the concrete class or ELSE!
        _isInitialized = true;
    }


    /// <summary>
    /// Called after all parameters have been set.
    /// </summary>
    /// <returns></returns>
    protected override async Task PostSetParametersAsync()
    {
        if (_isCreateMode) { }
        else if (_isListMode)
        {
            //            ListLoadEntities();
        }
    }


    /// <summary>
    /// Loads the entities for the List Mode.
    /// </summary>
    protected override async Task ListLoadEntities()
    {
        Result<List<SampleGuid>> loadResult = await _entityLookupService.GetAllAsync(true, true);
        if (loadResult.IsFailed) { return; }

        _entities = loadResult.Value;
    }


    protected async Task StartNormalEditOfRecord(SampleGuid entity)
    {
        _modelDefault = entity;
        await SetEditMode();
    }

}
