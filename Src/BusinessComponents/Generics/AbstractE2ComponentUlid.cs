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
    /// Constructor
    /// </summary>
    /// <param name="entitySingularName"></param>
    /// <param name="entityPluralName"></param>
    /// <param name="returnToPage"></param>
    public AbstractE2ComponentUlid(string entitySingularName = "",
                                  string entityPluralName = "",
                                  string returnToPage = "") : base(entitySingularName, entityPluralName, returnToPage) { }


    /// <summary>
    /// Changes the activation status of the entity.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected override async Task<Result> ExecuteActivationChange(TEntityUlid entity)
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


    protected override async Task<Result<TEntityUlid>> ExecuteLoadSingleEntityById()
    {
        try
        {
            // We call the AnyStatus variant as we might be editing an inactive version....
            Result<TEntityUlid> x = await _entityRepository.GetByIdAnyStatusAsync(_modelDefault.Id);
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