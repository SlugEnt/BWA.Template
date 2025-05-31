using Microsoft.AspNetCore.Components;
using SlugEnt.BWA.Entities;


namespace BWA.BusinessComponents.Generics;

public partial class IntLookupTable<TItem>  where TItem: IEntityInt
{
    /// <summary>
    /// Optional Argument.  This is the complex object that represents a single entity.  Either supply this or the Id parameter.
    /// </summary>
    [Parameter]
    public TItem? ValueObject { get; set; }

    /// <summary>
    /// Optional argument.  This is the Id (key) of the complex object that represents the entity.  Either Supply this or ValueObject.
    /// </summary>
    [Parameter]
    public int? ValueId { get; set; } = 0;


    /// <summary>
    /// This is the event that is called when the user selects a new value from the drop down.  The new value will be passed to the parent component.
    /// </summary>
    [Parameter]
    public EventCallback<TItem> ValueUpdated { get; set; }


    //bool _initializationCompleted = false;


    /// <summary>
    /// Called when the Drop Down Selection has been changed by user selection.  This will update the ValueObject and call the ValueUpdated event to notify the parent component of the change.
    /// </summary>
    /// <param name="updatedValue"></param>
    /// <returns></returns>
    private async Task SelectionChanged(TItem updatedValue)
    {
        if (ValueObject.Id != updatedValue.Id)
        {
            ValueObject = updatedValue;
            await ValueUpdated.InvokeAsync(ValueObject);
        }
    }
}