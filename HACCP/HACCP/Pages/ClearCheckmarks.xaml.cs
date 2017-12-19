using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
    public partial class ClearCheckmarks : BaseView
    {
        #region Member Variables

        private readonly ClearCheckmarksViewModel _viewModel;

        #endregion

        /// <summary>
        /// ClearCheckmarks Constructor
        /// </summary>
        public ClearCheckmarks()
        {
            InitializeComponent();

            _viewModel = new ClearCheckmarksViewModel(this);
            BindingContext = _viewModel;

            _viewModel.SetPropertyEnabledValues();

            NavigationPage.SetBackButtonTitle(this, string.Empty);
        }

        #region Methods

        /// <summary>
        /// EndEditing
        /// </summary>
        public override void EndEditing()
        {
            base.EndEditing();

            btnTemperature.IsEnabled = _viewModel.TemperatureEnabled;
            btnCheklist.IsEnabled = _viewModel.ChecklistEnabled;
            btnBoth.IsEnabled = _viewModel.BothEnabled;
        }

        /// <summary>
        /// OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.CurrentPageType = typeof(ClearCheckmarks);
        }

        #endregion

    }
}