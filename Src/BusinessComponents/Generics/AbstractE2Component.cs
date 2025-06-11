using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SlugEnt.BWA.Entities;
using SlugEnt.FluentResults;


namespace BWA.BusinessComponents.Generics;


/// <summary>
/// Base class for an Entity Component Page.  Provides for basic, Load, List, Edit, Add, Update capabilities of
/// a component page to an HTTP API or an Entity Repository.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public abstract class AbstractE2Component<TEntity> : AbstractCRUDComponent where TEntity : class, IEntity, new()
{
    protected TEntity model = new();    // This is used during edit operations of a single entity.
    protected TEntity? _modelDefault = new();    // This is used to basically store the entity ID for retrieval and other operations since the Id value can be of different types.
    protected string _apiName;
    protected IEntityRepositoryE2<TEntity> _entityLookupService;


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="entitySingularName"></param>
    /// <param name="entityPluralName"></param>
    /// <param name="returnToPage"></param>
    public AbstractE2Component(string entitySingularName = "", string entityPluralName = "", string returnToPage = "") : base(entitySingularName, entityPluralName, returnToPage) { }



    /// <summary>
    ///     Performs Initialization of the component.
    /// <para>Concrete classes MUST SET _isInitialized to TRUE as very last statement of their implementation of this method.</para>
    /// </summary>
    /// <remarks></remarks>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (_isInitialized)
            return;

        _entityLookupService!.ApiName = _apiName;
    }


    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
    }





    /// <summary>
    /// Creates the entity by attempting to add it to the Database.  If successful returns the user to the List View and displays a popup message of success.
    /// </summary>
    /// <returns></returns>
    protected virtual async Task CreateEntity()
    {
        try
        {
            await form.Validate();

            if (form.IsValid)
            {
                // Save Permanently
                Result<TEntity>? resultSave = await _entityLookupService.AddAsync(model);
                if (!resultSave.IsSuccess)
                {
                    ErrorManager.AddError(resultSave);

                    // TODO Change Entity to some property on the Entity that we need to define.
                    Snackbar.Add($"Error Saving {_entitySingluarName} - " + resultSave.ErrorTitle, Severity.Error);
                    return;
                }

                // TODO Change Entity to some property on the Entity that we need to define.
                Snackbar.Add($"{_entitySingluarName} Saved!");

                // Switch to List Mode
                await SetListMode();
            }
        }
        catch (Exception e)
        {
            // TODO Change Entity to some property on the Entity that we need to define
            ErrorManager.AddError(e, $"Unable to Save the new {_entitySingluarName}");
            Snackbar.Add($"Error Creating the new {_entitySingluarName} - Exception: " + e.Message, Severity.Error);
        }
    }


    /// <summary>
    /// Updates the entity by saving to database.  Assumes the entity is the one from the form.  If successful returns the user to the List View and displays a popup message of success.
    /// </summary>
    /// <returns></returns>
    protected async Task UpdateEntity()
    {
        await UpdateEntity(model);
    }


    /// <summary>
    /// Updates the entity by saving to database.
    /// </summary>
    /// <param name="entity">The Entity to be updated.</param>
    /// <returns></returns>
    protected async Task UpdateEntity(TEntity entity)
    {
        try
        {
            // TODO Fix the Person ID who is updating the Entity.
            Result updateResult = await _entityLookupService.UpdateAsync(entity, 1);
            if (!updateResult.IsSuccess)
            {
                ErrorManager.AddError(updateResult);
                // TODO Change Entity to some property on the Entity that we need to define
                Snackbar.Add($"Error Updating the {_entitySingluarName} - " + updateResult.ErrorTitle, Severity.Error);
                return;
            }

            // TODO Change Entity to some property on the Entity that we need to define
            Snackbar.Add($"{_entitySingluarName} Updated");

            // Switch to List Mode
            await SetListMode();

        }
        catch (Exception e)
        {
            // TODO Change Entity to some property on the Entity that we need to define
            ErrorManager.AddError(e, $"Unable to Update the {_entitySingluarName}");
            Snackbar.Add($"Error Updating the {_entitySingluarName} - Exception: " + e.Message, Severity.Error);
        }
    }




    /// <summary>
    /// USed to set the text to display in the Tooltip for the Activate/Deactivate button in List mode.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected string IsActiveIconButtonText(TEntity entity)
    {
        if (entity.IsActive)
            return $"Deactivate {_entitySingluarName}";

        return $"Activate {_entitySingluarName}";
    }


    /// <summary>
    /// Sets the color of the DeActivate button based upon current value.  If true, sets to Red. if False sets to Blue.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected MudBlazor.Color ListMode_ActivationButtonColor(TEntity entity)
    {
        if (entity.IsActive)
            return Color.Error;
        else
            return Color.Primary;
    }



    /// <summary>
    /// Loads the record to be edited into the model object
    /// </summary>
    /// <returns></returns>
    protected override async Task LoadRecordToBeEdited()
    {
        try
        {
            // We call the AnyStatus variant as we might be editing an inactive version....
            Result<TEntity> x = await ExecuteLoadSingleEntityById();
            if (x.IsFailed)
            {
                ErrorManager.AddError(x);
                Snackbar.Add($"Failed to load the {_entitySingluarName} record: " + x.ErrorTitle, Severity.Error);
                _errMsg = $"Failed to load the requested entity record [{_modelDefault.KeyId}].  Error was: {x.ErrorTitle}";
                _errVisible = true;
                model = null;
                return;
            }

            model = x.Value;
        }

        catch (Exception e)
        {
            ErrorManager.AddError(e, $"Failed to load {_entitySingluarName} with id of {_modelDefault.KeyId}");
            Snackbar.Add($"Failed to load the {_entitySingluarName} record  - Exception: " + e.Message, Severity.Error);
            _errMsg = $"Unhandled error in LoadRecordToBeEdited:  {e.Message}";
            _errVisible = true;
        }
    }



    /// <summary>
    /// Method in  derived class that loads a single entity by its Id.
    /// </summary>
    /// <returns></returns>
    protected abstract Task<Result<TEntity>> ExecuteLoadSingleEntityById();



    /// <summary>
    /// Processes the user clicking the Activate/Deactivate button
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected async Task OnChangeActivationClick(TEntity entity)
    {
        string wordPresent = "de-activating";
        string wordPast = "de-activated";

        if (!entity.IsActive)
        {
            wordPresent = "activating";
            wordPast = "activated";
        }

        try
        {

            Result result = await ExecuteActivationChange(entity);

            if (!result.IsSuccess)
            {
                ErrorManager.AddError(result);
                Snackbar.Add($"Error {wordPresent} the {_entitySingluarName} - " + result.ErrorTitle, Severity.Error);
                return;
            }

            entity.IsActive = !entity.IsActive;
            Snackbar.Add($"{_entitySingluarName} was {wordPast}");
        }

        catch (Exception e)
        {
            ErrorManager.AddError(e, $"Failed {wordPresent} the {_entitySingluarName}");
            Snackbar.Add($"Error {wordPresent} the {_entitySingluarName} - Exception: " + e.Message, Severity.Error);
        }

    }



    /// <summary>
    /// Changes the Activation state of the given entity
    /// </summary>
    /// <param name="entity">The entity whose activation state should be changed</param>
    /// <returns></returns>
    protected abstract Task<Result> ExecuteActivationChange(TEntity entity);


    #region "Quick Editing Methods"


    /// <summary>
    /// This is called when a user starts to edit a Data Grid Row
    /// </summary>
    /// <param name="entity"></param>
    protected void StartedQuickEditing(TEntity entity)
    {
        // Put any logic here you need to do before you start editing data in the grid.    
    }


    /// <summary>
    /// This is called if a user cancels a Data Grid Edit
    /// </summary>
    /// <param name="entity"></param>
    protected void CancelledQuickEditing(TEntity entity)
    {
        // Put any logic here you need to do before you start editing data in the grid.    
    }


    /// <summary>
    /// This is called when a user is saving a Data Grid row.
    /// </summary>
    /// <param name="entity"></param>
    protected void CommitQuickEditingSave(TEntity entity)
    {
        UpdateEntity(entity);

        // Put any logic here you need to do After committing the changes when editing data in the grid.    
    }
    #endregion    
}