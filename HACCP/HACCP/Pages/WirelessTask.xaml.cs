using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
    public partial class WirelessTask : BaseView
    {
        /// <summary>
        /// WirelessTask
        /// </summary>
        public WirelessTask()
        {
            InitializeComponent();


            NavigationPage.SetBackButtonTitle(this, string.Empty);

            var viewModel = new WirelessTasksViewModel(this);
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

        /// <summary>
        /// OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.CurrentPageType = typeof(WirelessTask);
        }
    }
}