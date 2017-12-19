using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
    public partial class ServerSettingsConfirmation : BaseView
    {
        /// <summary>
        /// ServerSettingsConfirmation Constructor
        /// </summary>
        /// <param name="settings"></param>
        public ServerSettingsConfirmation(object settings)
        {
            InitializeComponent();

            var siteSettings = settings as SiteSettings;

            NavigationPage.SetBackButtonTitle(this, string.Empty);

            var viewModel = new ServerSettingsConfirmationViewModel(this, siteSettings);
            BindingContext = viewModel;
        }

        /// <summary>
        /// OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.CurrentPageType = typeof(ServerSettingsConfirmation);
        }
    }
}