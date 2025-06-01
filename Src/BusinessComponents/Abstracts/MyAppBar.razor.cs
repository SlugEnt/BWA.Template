using Microsoft.AspNetCore.Components;
using SlugEnt.BWA.BusinessComponents.Abstracts.ErrorManagement;

namespace SlugEnt.BWA.BusinessComponents.Abstracts
{

    public partial class MyAppBar : ComponentBase
    {
        private List<AppError> _errorList = new(20);

        // TODO Error Manager Stuff
        //        private List<TreeItemData<AppError>> _listOfErrors { get; set; }


        /// <summary>
        /// User has either closed or opened the error window.
        /// </summary>
        /// <param name="toggled"></param>
        private void OnErrorWindowToggle(bool toggled)
        {
            // if closing the window, exit.
            //          if (!toggled)
            _showErrorPopup = !_showErrorPopup;
            int maxErrors = 25;


            ErrorList tempList = new(maxErrors);
            if (_showErrorPopup)
            {
                int i = 0;

                foreach (AppError appError in ErrorManager.ErrorList)
                {
                    if (i > maxErrors)
                        break;
                    tempList.AddError(appError);
                }

                _errorList = tempList.OrderByDescending(x => x.CreatedAt).ToList<AppError>();
            }
        }


        [Inject] private ErrorManager ErrorManager { get; set; }

        private bool _ErrorStateHasChanged;
        private bool _showErrorPopup;

        private string time = "initial time - midnight";


        protected override void OnInitialized()
        {
            base.OnInitialized();


            // TEMPORARY TESTING CODE ONLY.  YOU CAN DELETE THIS IF YOU HAVE NO IDEA WHY ITS HERE!!!!!!
            var timer = new Timer(new TimerCallback(_ =>
            {
                //ErrorManager.AddError($"Error  at {DateTime.Now}");
                time = DateTime.Now.ToString();
                InvokeAsync(() => { StateHasChanged(); });
            }),
                                  null,
                                  2000,
                                  3100);

            ErrorManager.PropertyChanged += (s,
                                             e) =>
            {
                if (e.PropertyName == nameof(ErrorManager.HasUnreadErrors))
                {
                    InvokeAsync(() => { Test(); });
                }
            };
        }


        private async Task Test()
        {
            if (ErrorManager.HasUnreadErrors != _ErrorStateHasChanged)
                _ErrorStateHasChanged = ErrorManager.HasUnreadErrors;


            bool x = ShouldRender();

            StateHasChanged();
            await Task.Yield();

            x = ShouldRender();
        }




        /// <summary>
        /// Creates a new indent string with the requested number of spaces.
        /// </summary>
        /// <param name="indent"></param>
        /// <returns></returns>
        internal static string CreateIndent(int indent)
        {
            string newLine = "";
            for (int i = 0; i < indent; i++)
                newLine = newLine + " ";
            return newLine;
        }

    }
}