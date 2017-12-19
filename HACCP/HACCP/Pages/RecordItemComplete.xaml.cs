using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
    public partial class RecordItemComplete : BaseView
    {

        /// <summary>
        /// Record Item Complete
        /// </summary>
        /// <param name="selectedItem"></param>
        public RecordItemComplete(object selectedItem)
        {
            InitializeComponent();

            NavigationPage.SetHasBackButton(this, false);

            var viewModel = new RecordItemCompleteViewModel(this, selectedItem);
            BindingContext = viewModel;

            actionBtn.Clicked += (sender, e) => { editorcontrol.Unfocus(); };

            optionlist.ItemSelected += (sender, e) => { optionlist.SelectedItem = null; };
        }

        /// <summary>
        /// OnBackButtonPressed
        /// </summary>
        /// <returns></returns>
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        /// <summary>
        /// OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.CurrentPageType = typeof(RecordItemComplete);
        }
    }
}