using System;
using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
    public partial class MenuChecklist : BaseView
    {
        #region Member Variables

        private readonly MenuChecklistViewModel _viewModel;
        private bool IsListViewSelected { get; set; }

        #endregion


        /// <summary>
        /// Menu Check list Screen Constructor
        /// </summary>
        /// <param name="isMenu"></param>
        public MenuChecklist(object isMenu)
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, string.Empty);

            BindingContext = _viewModel = new MenuChecklistViewModel(this);

            _viewModel.IsMenu = Convert.ToBoolean(isMenu);

            _viewModel.Title = HACCPUtil.GetResourceString(_viewModel.IsMenu ? "SelectMenu" : "SelectChecklist");


            menuListView.ItemSelected += (sender, e) =>
            {
                if (menuListView.SelectedItem == null)
                    return;

                if (!IsListViewSelected || !HaccpAppSettings.SharedInstance.IsWindows)
                {
                    IsListViewSelected = true;
                    var menu = menuListView.SelectedItem;

                    if (_viewModel != null)
                        _viewModel.ShowSelectMenuChecklistAlert(message: HACCPUtil.GetResourceString("Themenuhasbeenupdatedsuccessfully"), selectedItem: menu);

                    menuListView.SelectedItem = null;
                }
                else
                {
                    IsListViewSelected = false;
                }
            };

            checklistListView.ItemSelected += (sender, e) =>
            {
                if (checklistListView.SelectedItem == null)
                    return;


                if (!IsListViewSelected || !HaccpAppSettings.SharedInstance.IsWindows)
                {
                    IsListViewSelected = true;
                    if (checklistListView.SelectedItem == null)
                        return;
                    var checklist = (Checklist)checklistListView.SelectedItem;

                    //       if (!HaccpAppSettings.SharedInstance.IsWindows)
                    checklistListView.SelectedItem = null;

                    if (_viewModel != null)
                        _viewModel.ShowSelectMenuChecklistAlert(HACCPUtil.GetResourceString("Thechecklisthasbeenaddedsuccessfully"), checklist);
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
            App.CurrentPageType = typeof(MenuChecklist);
        }

        /// <summary>
        /// OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            checklistListView.SelectedItem = null;
        }

        #endregion
    }
}