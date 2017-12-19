using System.Threading.Tasks;
using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
    public partial class Home : BaseView
    {

        /// <summary>
        /// Home Screen Constructor
        /// </summary>
        public Home()
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, string.Empty);

            var viewModel = new HomeViewModel(this);

            BindingContext = viewModel;
            var logInButton = new ToolbarItem
            {
                Order = ToolbarItemOrder.Primary
            };
            logInButton.SetBinding(MenuItem.IconProperty,
                new Binding(BaseViewModel.IsLoggedInPropertyName, BindingMode.Default, new LoginStatusConverter(), 1,
                    null, null));
            logInButton.SetBinding(MenuItem.CommandProperty, new Binding(BaseViewModel.LogInCommandPropertyName));

            ToolbarItems.Add(logInButton);
        }

        #region Methods

        /// <summary>
        /// On BackButton Pressed
        /// </summary>
        /// <returns></returns>
        protected override bool OnBackButtonPressed()
        {
            if (Device.OS != TargetPlatform.Android)
                return false;

            Device.BeginInvokeOnMainThread(async () => await ShowAreyousureyouwanttoexittheapp());
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
            App.CurrentPageType = typeof(Home);
        }

        #endregion
    }
}