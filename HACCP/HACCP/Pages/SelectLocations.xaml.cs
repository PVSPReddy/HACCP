using System.Collections.Generic;
using System.Linq;
using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
    public partial class SelectLocations : BaseView
    {

        /// <summary>
        /// LocationsViewModel 
        /// </summary>
        private readonly LocationsViewModel _viewModel;

        private bool IsListViewSelected { get; set; }

        /// <summary>
        /// SelectLocations Constructor
        /// </summary>
        public SelectLocations()
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, string.Empty);
            BindingContext = _viewModel = new LocationsViewModel(this);

            searchLocation.Text = "";

            if (HaccpAppSettings.SharedInstance.IsWindows)
                ToolbarItems.Add(new ToolbarItem
                {
                    Icon = "logout.png",
                    Order = ToolbarItemOrder.Primary,
                    Command = _viewModel.LogInCommand
                });


            searchLocation.TextChanged += (object sender, TextChangedEventArgs e) =>
            {
                string searchText = searchLocation.Text.ToLower().TrimStart();

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    LocationListview.ItemsSource = _viewModel.Locations;
                    _viewModel.HasLocations = true;
                }
                else
                {
                    var locations = _viewModel.Locations.Where(i => i.Name.ToLower().Contains(searchText));
                    var menuLocations = locations as IList<MenuLocation> ?? locations.ToList();
                    LocationListview.ItemsSource = menuLocations;
                    _viewModel.HasLocations = menuLocations.Any();
                }

                if (!HaccpAppSettings.SharedInstance.IsWindows)
                {
                    searchLocation.Text = searchText;
                }
            };

            LocationListview.ItemSelected += ListItemSelected;
        }

        #region Methods

        /// <summary>
        ///List item selected event handler.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void ListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (LocationListview.SelectedItem == null)
                return;
            if (!IsListViewSelected || !HaccpAppSettings.SharedInstance.IsWindows)
            {
                IsListViewSelected = true;
                _viewModel.SelectedLocation = LocationListview.SelectedItem as MenuLocation;              
            }
            else
            {
                IsListViewSelected = false;
                LocationListview.SelectedItem = null;
            }
        }


        /// <summary>
        /// OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.CurrentPageType = typeof(SelectLocations);
        }

        /// <summary>
        /// OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            LocationListview.SelectedItem = null;

            searchLocation.Text = string.Empty;
        }

        #endregion
    }
}