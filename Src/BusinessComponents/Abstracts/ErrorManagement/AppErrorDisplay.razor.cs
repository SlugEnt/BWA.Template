using Microsoft.AspNetCore.Components;

namespace SlugEnt.BWA.BusinessComponents.Abstracts.ErrorManagement;

public partial class AppErrorDisplay : ComponentBase
{
#region Parameters

    [Parameter]
    public AppError? AppError
    {
        get;
        set;
    } = null;

    #endregion


#region "Local Variables"

    // Set after the component is initialized and the entity list is loaded.  This is used to prevent the component from rendering before the data is ready.
    private bool _isReady = false;
    #endregion



    /// <summary>
    /// Called after all Parameters have been set.  This is used to initialize the component and load any data that is needed.
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        _isReady = true;
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

    }
}