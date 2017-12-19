using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
    public partial class PerformCheckList : BaseView
    {

        PerformCheckListViewModel viewModel;
        private bool IsListViewSelected { get; set; }
        /// <summary>
        /// PerformCheckList  Screen Constructor
        /// </summary>
        public PerformCheckList()
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, string.Empty);
            viewModel = new PerformCheckListViewModel(this);
            BindingContext = viewModel;

            if (HaccpAppSettings.SharedInstance.IsWindows)
                ToolbarItems.Add(new ToolbarItem
                {
                    Icon = "logout.png",
                    Order = ToolbarItemOrder.Primary,
                    Command = viewModel.LogInCommand
                });

            if (HaccpAppSettings.SharedInstance.IsWindows)
            {
                CategoryListView.ItemsSource = viewModel.Categories;
            }


            CategoryListView.ItemSelected += (sender, e) =>
            {

                if (CategoryListView.SelectedItem == null)
                    return;
                if (!IsListViewSelected || !HaccpAppSettings.SharedInstance.IsWindows)
                {
                    IsListViewSelected = true;
                    viewModel.SelectedCategory = CategoryListView.SelectedItem as Category;
                    CategoryListView.SelectedItem = null;
                }
                else
                {
                    IsListViewSelected = false;
                }
            };
        }

        #region Methods

        /// <summary>
        /// OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.CurrentPageType = typeof(PerformCheckList);
        }

        /// <summary>
        /// OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            CategoryListView.SelectedItem = null;
        }

        #endregion
    }
}