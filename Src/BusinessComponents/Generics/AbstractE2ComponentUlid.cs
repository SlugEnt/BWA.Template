using ByteAether.Ulid;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SlugEnt.BWA.Entities;
using SlugEnt.FluentResults;

namespace BWA.BusinessComponents.Generics;


/// <summary>
/// A component based upon an entity that is keyed by an integer.
/// </summary>
public abstract class AbstractE2ComponentUlid<TEntityUlid> : AbstractE2Component<TEntityUlid> where TEntityUlid : AbstractEntityULID, IEntityULID, new()
{
    protected Ulid? _currentRecordId = null; // The Id of the record to be edited, deleted, or viewed.  This is used to retrieve the record from the database.

    [Inject] protected IEntityRepositoryE2Ulid<TEntityUlid>? _entityRepository { get { return (IEntityRepositoryE2Ulid<TEntityUlid>)_entityLookupService; } set { _entityLookupService = value; } }


    /// <summary>
    /// Processes the user clicking the Activate/Deactivate button
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected async Task OnChangeActivationClick(TEntityUlid entity)
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
            Result result;

            // TODO Fix the Person ID who is updating the Entity.
            if (entity.IsActive)
                result = await _entityRepository.DeActivateAsync(entity.Id, 1);
            else
                result = await _entityRepository.ActivateAsync(entity.Id, 1);

            if (!result.IsSuccess)
            {
                ErrorManager.AddError(result);
                Snackbar.Add($"Error {wordPresent} the Company - " + result.ErrorTitle, Severity.Error);
                return;
            }

            entity.IsActive = !entity.IsActive;
            Snackbar.Add($"Company {wordPast}");
        }

        catch (Exception e)
        {
            ErrorManager.AddError(e, $"Failed {wordPresent} the Company");
            Snackbar.Add($"Error {wordPresent} the Company - Exception: " + e.Message, Severity.Error);
            return;
        }

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
            Result<TEntityUlid> x = await _entityRepository.GetByIdAnyStatusAsync((Ulid)_currentRecordId);
            if (x.IsFailed)
            {
                _errMsg = $"Failed to load the requested entity record [{_currentRecordId.ToString()}].  Error was: {x.ErrorTitle}";
                _errVisible = true;
                model = null;
                return;
            }

            model = x.Value;
        }

        catch (Exception e)
        {
            ErrorManager.AddError(e, "LoadRecordToBeEdited had unexpected error");
            _errMsg = $"Unhandled error in LoadRecordToBeEdited:  {e.Message}";
            _errVisible = true;
        }
    }



    /// <summary>
    /// Sets the Initial Current Record ID to the value passed in from the URL.
    /// </summary>
    protected override void SetInitialCurrentRecord()
    {
        if (recordId != null)
        {

            bool success = Ulid.TryParse(recordId,null, out Ulid id);
            if (!success)
            {
                _errMsg = $"The record ID passed in [{recordId}] is not a valid ULid String value.";
                _errVisible = true;
                return;
            }

            _currentRecordId = id;
            return;
        }

        if ((mode == "E" || mode == "D") && recordId == null)
        {
            _errMsg     = $"The record ID passed in [{recordId}] was null.";
            _errVisible = true;
            return;
        }

    }


}