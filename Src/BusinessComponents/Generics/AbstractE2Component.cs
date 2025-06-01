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
public abstract class AbstractE2Component<TEntity> : AbstractCRUDComponent where TEntity : class,  IEntity, new()
{
    protected TEntity model = new();
    protected string  _apiName;
    protected IEntityRepositoryE2<TEntity> _entityLookupService;

    


    

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
                    Snackbar.Add($"Error Saving Entity - " + resultSave.ErrorTitle, Severity.Error);
                    return;
                }

                // TODO Change Entity to some property on the Entity that we need to define.
                Snackbar.Add("Entity Saved!");

                // Switch to List Mode
                await SetListMode();
            }
        }
        catch (Exception e)
        {
            // TODO Change Entity to some property on the Entity that we need to define
            ErrorManager.AddError(e, "Unable to Update the Company");
            Snackbar.Add("Error Updating the Company - Exception: " + e.Message, Severity.Error);
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
                Snackbar.Add("Error Updating the Company - " + updateResult.ErrorTitle, Severity.Error);
                return;
            }

            // TODO Change Entity to some property on the Entity that we need to define
            Snackbar.Add("Company Updated");

            // Switch to List Mode
            await SetListMode();

        }
        catch (Exception e)
        {
            // TODO Change Entity to some property on the Entity that we need to define
            ErrorManager.AddError(e, "Unable to Update the Company");
            Snackbar.Add("Error Updating the Company - Exception: " + e.Message, Severity.Error);
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
            return "Deactivate Company";

        return "Activate Company";
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
}