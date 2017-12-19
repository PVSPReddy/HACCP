using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
    public partial class ViewStatusLines : BaseView
    {
        
        /// <summary>
        /// ViewStatusLines
        /// </summary>
        public ViewStatusLines()
        {
            InitializeComponent();

            var viewModel = new ViewStatusLinesViewModel(this);

            BindingContext = viewModel;
            NavigationPage.SetBackButtonTitle(this, string.Empty);
            AddToolBarButton("home.png", viewModel.HomeCommand);
        }

        /// <summary>
        /// OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.CurrentPageType = typeof(ViewStatusLines);
        }
    }
}