using System.Threading.Tasks;
using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
    public partial class ServerSettings : BaseView
    {
        private readonly ServerSettingsViewModel _viewModel;
        private bool IsListViewSelected { get; set; }

        /// <summary>
        /// ServerSettings
        /// </summary>
        public ServerSettings()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, string.Empty);

            BindingContext = _viewModel = new ServerSettingsViewModel(this);

            NavigationPage.SetHasBackButton(this, _viewModel.IsServerAddressExist);

            //serverAddrsEntry.Completed += serverAddrsEntryText_Input_Completed;
            //serverPortEntry.Completed += serverPortEntryText_Input_Completed;
            languageList.ItemSelected += (sender, e) =>
            {
                if (languageList.SelectedItem == null)
                    return;

                if (!IsListViewSelected || !HaccpAppSettings.SharedInstance.IsWindows)
                {
                    IsListViewSelected = true;
                    var language = (Language)languageList.SelectedItem;

                    if (_viewModel != null) _viewModel.SelectLanguageAndDownloadStrings(language);
                }
                else
                {
                    IsListViewSelected = false;
                }
            };
        }

        /// <summary>
        /// End Editing
        /// </summary>
        public override void EndEditing()
        {
            serverAddrsEntry.Unfocus();
            serverPortEntry.Unfocus();
            serverDirectoryEntry.Unfocus();
        }

        /// <summary>
        /// OnBackButtonPressed
        /// </summary>
        /// <returns></returns>
        protected override bool OnBackButtonPressed()
        {
            if (Device.OS == TargetPlatform.Android && Navigation.NavigationStack.Count < 2)
            {
                Device.BeginInvokeOnMainThread(async () => await ShowAreyousureyouwanttoexittheapp());
            }
            else
            {
                return base.OnBackButtonPressed();
            }
            return true;
        }

        /// <summary>
        /// ShowAreyousureyouwanttoexittheapp
        /// </summary>
        /// <returns></returns>
        public async Task ShowAreyousureyouwanttoexittheapp()
        {
            var res =
                await
                    ShowConfirmAlert(HACCPUtil.GetResourceString("HACCP"),
                        HACCPUtil.GetResourceString("Areyousureyouwanttoexittheapp"));
            if (res)
            {
                DependencyService.Get<IAppExit>().CloseApp();
            }
        }

        /// <summary>
        /// OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.CurrentPageType = typeof(ServerSettings);
        }
    }
}