using Humanizer;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudExtensions;
using System.ComponentModel.DataAnnotations;


namespace SlugEnt.BWA.BusinessComponents.Abstracts;

/// <summary>
/// The SelectorInt is a generic component that allows the user to select an entity from a list of entities.
/// This is used for entities that have an integer key and implement the IEntityLookup interface.
/// </summary>
/// <typeparam name="TEnum"></typeparam>
public partial class SelectorEnum<TEnum> : ComponentBase where TEnum : struct, Enum
{
    /// <summary> Optional argument.  If set, this will be the Id of the entity object that is selected.  Supply this or ValueObject. </summary>
    [Parameter]
    public TEnum ValueId
    {
        get;
        set;
    }

    /// <summary> This is the event that is called when the user selects a new Entity.  The new value will be passed to the parent component. </summary>
    [Parameter]
    public EventCallback<TEnum> ValueUpdated
    {
        get;
        set;
    }


    /// <summary> This is the label for the select box.  If not set, the ApiName will be used instead. </summary>
    [Parameter]
    public string? Label

    {
        get;
        set;
    } = "";

    /// <summary> This is used to determine how the selected value is displayed in the select box.  Text or Icon. </summary>
    [Parameter]
    public ValuePresenter ValuePresenter { get; set; } = ValuePresenter.Text;


    [Parameter] public Variant Variant { get; set; } = Variant.Text;


    [Parameter] public bool SortDropDown { get; set; }= true;

    [Parameter] [Required] public TEnum NotSpecifiedItem { get; set; }



    private int V_ValueIdAsInt = -999;



    // Internally used variables for the ID and the object.
    private EnumItem       _enumItemSelected;
    private bool           _sortDropDown = false;
    private List<EnumItem> _enumList     = new();


    bool _initializationCompleted = false; // set of OnInitialized has run.

    private string?
        _labelText =
            string.Empty; // This is the label for the select box.  If not set, the name of the entity will be used instead.

    private bool
        _isReady = false; // Set after the component is initialized and the entity list is loaded.  This is used to prevent the component from rendering before the data is ready.

    private static bool isProcessing = false;
    


    /// <summary>
    /// The user has selected a new entity.  This will set the internal ValueObject to the new entity and call the ValueUpdated event, so the caller is notified.
    /// </summary>
    /// <param name="enumSelected"></param>
    /// <returns></returns>
    private async Task EnumSelectChanged(EnumItem enumSelected)
    {
        if (enumSelected.Value != _enumItemSelected.Value)
        {
            _enumItemSelected = enumSelected;
            V_ValueIdAsInt = enumSelected.Value;
            
            ValueUpdated.InvokeAsync((TEnum)Enum.ToObject(typeof(TEnum), enumSelected));
        }
    }


    /// <summary>
    /// Load enum list from enum
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        if (!_initializationCompleted)
            return;

        bool listReadSuccess = false;

        if (isProcessing)
            return;

        


        try
        {
            isProcessing = true;

            if (_isReady == false)
            {
                int notSpec = Convert.ToInt32(NotSpecifiedItem);
                _enumList         = GetSortedEnumItems<TEnum>(SortDropDown, notSpec);
                V_ValueIdAsInt    = Convert.ToInt32(ValueId);
                _enumItemSelected = _enumList.Find(x => x.Value == V_ValueIdAsInt);

            }
            else
            {
                // Simple Parameter checks...
                if (Label != _labelText) // If the label has changed, set the label text to the new value.
                    _labelText = Label;

                // Drop down sort
                if (_sortDropDown != SortDropDown)
                    _sortDropDown = SortDropDown;

                int x = Convert.ToInt32(ValueId);

                // The valuID.  Need to determine if user set or set by us...
                if (V_ValueIdAsInt == -999 || x != V_ValueIdAsInt) // Initial value
                {
                    V_ValueIdAsInt    = x;
                    _enumItemSelected = _enumList.Find(x => x.Value == V_ValueIdAsInt);
                }

            }
            if (_isReady == false)
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


    /// <summary>
    /// Accepts an Enum and them as a list of EnumItems that can be more easily used by components.  The list can be sorted and also
    /// optionally has the ability to add a Not Specified item to the list.  The not specified item can be set on a per enum basis.
    /// </summary>
    /// <typeparam name="T">The enum we want to load.</typeparam>
    /// <param name="sortTheList">If you want the list sorted by name (Ascending).  </param>
    /// <param name="notSpecifiedValue">Set to -1 to ignore checking for this or placing it at top of list.</param>
    /// <returns></returns>
    public List<EnumItem> GetSortedEnumItems<T>(bool sortTheList, int notSpecifiedValue = 0) where T : struct, Enum
    {
        List<EnumItem> items = new();
        EnumItem? notSpecifiedItem = null;

        foreach (T e in Enum.GetValues<T>())
        {
            int enumValue = Convert.ToInt32(e);
            if (enumValue == notSpecifiedValue)
            {
                notSpecifiedItem = new EnumItem(e.Humanize(), enumValue);
                continue;
            }

            Console.WriteLine("[ " + enumValue + " ]" + e.Humanize());
            EnumItem item = new EnumItem(e.Humanize(), enumValue);
            items.Add(item);
        }

        // Sort the list.
        if (sortTheList)
            items.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));


        // Add the not specified item to beginning of sorted list.
        if (notSpecifiedItem != null)
            items.Insert(0, notSpecifiedItem);
        return items;
    }
}

/// <summary>
/// Used to represent an Enum Item, with a Name and Value
/// </summary>
public class EnumItem
{
    /// <summary> Value of the Enum </summary>
    public int Value { get; set; }

    /// <summary> Name of the Enum </summary>
    public string Name { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public EnumItem(string name, int value)
    {
        Value = value;
        Name = name;
    }

}