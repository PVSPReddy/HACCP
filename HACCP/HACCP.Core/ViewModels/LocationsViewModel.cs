using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HACCP.Core
{
    public class LocationsViewModel : BaseViewModel
    {
        #region Member Variables

        private readonly IDataStore _dataStore;
        private bool _hasLocations;
        private ObservableCollection<MenuLocation> _locations;
        public bool IsBackNavigation = true;
        private MenuLocation _selectedLocation;
        private Command logInCommand;

        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="HACCP.Core.LocationsViewModel" /> class.
        /// </summary>
        /// <param name="page">Page.</param>
        public LocationsViewModel(IPage page)
            : base(page)
        {
            _dataStore = new SQLiteDataStore();
            var locations = _dataStore.GetLocations();
            var menuLocations = locations as IList<MenuLocation> ?? locations.ToList();
            HasLocations = menuLocations.Any();
            Locations = new ObservableCollection<MenuLocation>(menuLocations);


            MessagingCenter.Subscribe<CleanUpMessage>(this, HaccpConstant.CleanupMessage, sender =>
            {
                MessagingCenter.Unsubscribe<MenuLocationId>(this, HaccpConstant.MenulocationMessage);
                MessagingCenter.Unsubscribe<CleanUpMessage>(this, HaccpConstant.CleanupMessage);
            });


            MessagingCenter.Subscribe<MenuLocationId>(this, HaccpConstant.MenulocationMessage, sender =>
            {
                if (sender == null) return;
                var locationId = sender.LocationId;
                var location = Locations.FirstOrDefault(x => x.LocationId == locationId);
                if (location != null)
                {
                    location.RecordStatus = _dataStore.GetLocationItemRecordStatus(locationId);
                }
            });

            MessagingCenter.Subscribe<UploadRecordRefreshMessage>(this, HaccpConstant.UploadRecordRefresh, sender =>
            {
                var locs = _dataStore.GetLocations();
                var enumerable = locs as IList<MenuLocation> ?? locs.ToList();
                HasLocations = enumerable.Any();
                Locations = new ObservableCollection<MenuLocation>(enumerable);
            });
        }

        #region Properties


        /// <summary>
        ///     Gets or sets the selected location.
        /// </summary>
        /// <value>The selected location.</value>
        public MenuLocation SelectedLocation
        {
            get { return _selectedLocation; }
            set
            {
                SetProperty(ref _selectedLocation, value);
                if (value != null)
                {
                    IsBackNavigation = false;
                    LoadMenuItem(value);
                }
            }
        }


        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public ObservableCollection<MenuLocation> Locations
        {
            get { return _locations; }
            set { SetProperty(ref _locations, value); }
        }


        /// <summary>
        ///     Gets or sets a value indicating whether this instance has locations.
        /// </summary>
        /// <value><c>true</c> if this instance has locations; otherwise, <c>false</c>.</value>
        public bool HasLocations
        {
            get { return _hasLocations; }
            set { SetProperty(ref _hasLocations, value); }
        }


        /// <summary>
        ///     Gets the save command.
        /// </summary>
        /// <value>The save command.</value>
        public new Command LogInCommand
        {
            get
            {
                return logInCommand ??
                       (logInCommand = new Command(ExecuteLogInCommand, () => !IsBusy));
            }
        }

        #endregion

        #region

        /// <summary>
        ///     Called when the view appears.
        /// </summary>
        public override void OnViewAppearing()
        {
            base.OnViewAppearing();
            if (Locations.Count < 1)
            {
                Page.DisplayAlertMessage(HACCPUtil.GetResourceString("NoLocationsFound"),
                    HACCPUtil.GetResourceString("NolocationsfoundPleasetapSelectChangeMenuintheWirelessTasksMenu"));
            }
        }

        /// <summary>
        /// OnViewDisappearing
        /// </summary>
        public override void OnViewDisappearing()
        {
            if (IsBackNavigation)
            {
                MessagingCenter.Unsubscribe<MenuLocationId>(this, HaccpConstant.MenulocationMessage);
                MessagingCenter.Unsubscribe<CleanUpMessage>(this, HaccpConstant.CleanupMessage);
                MessagingCenter.Unsubscribe<UploadRecordRefreshMessage>(this, HaccpConstant.UploadRecordRefresh);
            }
            base.OnViewDisappearing();
            IsBackNavigation = true;
        }


        /// <summary>
        ///     Executes the log in command.
        /// </summary>
        /// <returns>The log in command.</returns>
        private async void ExecuteLogInCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            LogInCommand.ChangeCanExecute();
            var checkLogout = false;
            try
            {
                checkLogout = await ShowLogoutConfirm();
                if (checkLogout)
                    HaccpAppSettings.SharedInstance.CurrentUserId = 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ooops! Something went wrong while login. Exception: {0}", ex);
            }
            finally
            {
                IsBusy = false;
                LogInCommand.ChangeCanExecute();
                if (checkLogout)
                {
                    Page.PopPage();
                }
            }
        }


        /// <summary>
        ///     Loads the menu item.
        /// </summary>
        /// <returns>The menu item.</returns>
        /// <param name="location">Location.</param>
        public async Task LoadMenuItem(MenuLocation location)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            if (_dataStore.CheckItemExists(location.LocationId))
                await Page.NavigateToWithSelectedObject(PageEnum.SelectLocationItem, true, location);
            else
                await Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("Noitemsfound"));

            IsBusy = false;
        }

        #endregion
    }
}