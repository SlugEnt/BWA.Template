
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SlugEnt.BWA.Entities;
using SlugEnt.FluentResults;

namespace BWA.BusinessComponents.Generics;


/// <summary>
/// A component based upon an entity that is keyed by a long.
/// </summary>
public abstract class AbstractE2ComponentGuid<TEntityGuid> : AbstractE2Component<TEntityGuid> where TEntityGuid : AbstractEntityGuid, IEntityGuid, new()
{
    protected Guid _currentRecordId = Guid.Empty; // The Id of the record to be edited, deleted, or viewed.  This is used to retrieve the record from the database.

    [Inject] protected IEntityRepositoryE2Guid<TEntityGuid>? _entityRepository 
    {
        get
        {
            return (IEntityRepositoryE2Guid<TEntityGuid>)_entityLookupService;
        }
        set
        {
            _entityLookupService = value;
        } }


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="entitySingularName"></param>
    /// <param name="entityPluralName"></param>
    /// <param name="returnToPage"></param>
    public AbstractE2ComponentGuid(string entitySingularName = "",
                                  string entityPluralName = "",
                                  string returnToPage = "") : base(entitySingularName, entityPluralName, returnToPage) { }


    /// <summary>
    /// Changes the activation status of the entity.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected override async Task<Result> ExecuteActivationChange(TEntityGuid entity)
    {
        try
        {
            // TODO Fix the Person ID who is updating the Entity.
            Result result;
            if (entity.IsActive)
                result = await _entityRepository.DeActivateAsync(entity.Id, 1);
            else
                result = await _entityRepository.ActivateAsync(entity.Id, 1);
            return result;
        }
        catch (Exception e)
        {
            return Result.Fail(new ExceptionalError($"Failed to successfully change the activation of the {_entitySingluarName} due to an error: {e.Message}", e));
        }
    }


    protected override async Task<Result<TEntityGuid>> ExecuteLoadSingleEntityById()
    {
        try
        {
            // We call the AnyStatus variant as we might be editing an inactive version....
            Result<TEntityGuid> x = await _entityRepository.GetByIdAnyStatusAsync(_modelDefault.Id);
            return x;
        }
        catch (Exception e)
        {
            return Result.Fail(new ExceptionalError($"Failed to successfully load the {_entitySingluarName} record due to an error: {e.Message}", e));
        }
    }


    /// <summary>
    /// Sets the Initial Current Record ID to the value passed in from the URL.
    /// </summary>
    protected override void SetInitialCurrentRecord()
    {
        if (recordId != null)
        {
            bool success = Guid.TryParse(recordId, out Guid id);
            if (!success)
            {
                _errMsg = $"The record ID passed in [{recordId}] is not a valid integer.";
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