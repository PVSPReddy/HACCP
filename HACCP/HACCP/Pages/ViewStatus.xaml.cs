using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
    public partial class ViewStatus : BaseView
    {
        /// <summary>
        /// ViewStatus
        /// </summary>
        public ViewStatus()
        {
            InitializeComponent();

            var viewModel = new ViewStatusViewModel(this);

            BindingContext = viewModel;
            NavigationPage.SetBackButtonTitle(this, string.Empty);
            AddToolBarButton("info.png", viewModel.ViewStatusLinesCommand);
        }

        /// <summary>
        /// OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.CurrentPageType = typeof(ViewStatus);
        }
    }
}