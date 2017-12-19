using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HACCP.Core
{
    public class LocationItemVIewModel : BaseViewModel
    {
        private readonly IDataStore _dataStore;
        private readonly MenuLocation _location;
        private bool _hasItems = true;
        private bool _isReviewAnswerVisible;
        private Command _itemFilterCommand;
        private ObservableCollection<LocationMenuItem> _items;
        private bool _lstCompletedItems;
        private string _noItemFoundText;
        private Command _reviewItemOkCommand;
        private Command _reviewListCommand;
        public bool IsBackNavigation = true;
		public ObservableCollection<LocationMenuItem> locationitems;


        /// <summary>
        ///     Initializes a new instance of the <see cref="HACCP.Core.LocationItemVIewModel" /> class.
        /// </summary>
        /// <param name="page">Page.</param>
        /// <param name="menuLocation"></param>
        public LocationItemVIewModel(IPage page, MenuLocation menuLocation)
            : base(page)
        {
            ListCompletedItems = false; // Set default value to ListCompletedItems
            Title = HACCPUtil.GetResourceString("SelectItem");

            NoItemFoundText = HACCPUtil.GetResourceString("Noitemsfound");
            _dataStore = new SQLiteDataStore();
            AutoBack = false;
            _location = menuLocation;
            LocationName = menuLocation.Name;
            LocationId = menuLocation.LocationId;

            var items = _dataStore.GetItems(LocationId, ListCompletedItems);
            HasItems = items.Any();
			locationitems= Items = new ObservableCollection<LocationMenuItem>(items);

            MessagingCenter.Subscribe<CleanUpMessage>(this, HaccpConstant.CleanupMessage, sender =>
            {
                MessagingCenter.Unsubscribe<LocationMenuItem>(this, HaccpConstant.RecorditemMessage);
                MessagingCenter.Unsubscribe<AutoAdvanceLocationMessage>(this, HaccpConstant.AutoadvancelocationMessage);
                MessagingCenter.Unsubscribe<CleanUpMessage>(this, HaccpConstant.CleanupMessage);
            });

            //MessagingCenter.Unsubscribe<LocationMenuItem> (this, "sample");

            MessagingCenter.Subscribe<LocationMenuItem>(this, HaccpConstant.RecorditemMessage, sender =>
            {
                var selectedItem = sender;
                if (selectedItem == null) return;
                var first = Items.FirstOrDefault(x => x.ItemId == selectedItem.ItemId);
                if (first != null)
                {
                    first.RecordStatus = 1;
                }
            });

            MessagingCenter.Subscribe<AutoAdvanceLocationMessage>(this, HaccpConstant.AutoadvancelocationMessage,
                sender =>
                {
                    var message = sender;
                    if (message != null)
                    {
                        var currentItem = Items.FirstOrDefault(x => x.ItemId == message.CurrentId);
                        if (currentItem != null)
                        {
                            var index = Items.IndexOf(currentItem);
                            if (Items.Count - 1 > index)
                            {
                                // AutoBack = index + 2 == Items.Count;
                                IsBackNavigation = false;
                                page.NavigateToWithSelectedObject(PageEnum.RecordItem, true, Items[index + 1]);
                            }
                            else
                            {
                                AutoBack = true;
                                if (!HaccpAppSettings.SharedInstance.IsWindows)
                                    page.PopPage(); // it won't work in windows..
                            }
                        }
                    }
                });


            MessagingCenter.Subscribe<UploadRecordRefreshMessage>(this, HaccpConstant.UploadRecordRefresh, sender =>
            {
                var list = _dataStore.GetItems(LocationId, ListCompletedItems);
                HasItems = list.Any();
                Items = new ObservableCollection<LocationMenuItem>(list);
                IsReviewItemVisible = false;
            });
        }

        #region Properties

        public bool AutoBack { get; set; }

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public ObservableCollection<LocationMenuItem> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        private LocationMenuItem _selectedLocationItem;

        /// <summary>
        ///     Gets or sets the selected location.
        /// </summary>
        /// <value>The selected location.</value>
        public LocationMenuItem SelectedLocationItem
        {
            get { return _selectedLocationItem; }
            set
            {
                SetProperty(ref _selectedLocationItem, value);
                if (value != null)
                {
                    IsBackNavigation = false;
                    LoadMenuItem(value);
                }
            }
        }

        /// <summary>
        ///     Sets the name of the location.
        /// </summary>
        /// <value>The name of the location.</value>
        public string LocationName { get; set; }

        /// <summary>
        ///     Gets or sets the location ID.
        /// </summary>
        /// <value>The location ID.</value>
        public long LocationId { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="HACCP.Core.LocationItemVIewModel" /> list completed items.
        /// </summary>
        /// <value><c>true</c> if list completed items; otherwise, <c>false</c>.</value>
        public bool ListCompletedItems
        {
            get { return _lstCompletedItems; }
            set { SetProperty(ref _lstCompletedItems, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is review answer visible.
        /// </summary>
        /// <value><c>true</c> if this instance is review answer visible; otherwise, <c>false</c>.</value>
        public bool IsReviewItemVisible
        {
            get { return _isReviewAnswerVisible; }
            set { SetProperty(ref _isReviewAnswerVisible, value); }
        }

        /// <summary>
        ///     Gets the save command.
        /// </summary>
        /// <value>The save command.</value>
        public Command ItemsFilterCommand
        {
            get
            {
                return _itemFilterCommand ??
                       (_itemFilterCommand = new Command(ExecuteItemsFilterCommand, () => !IsBusy));
            }
        }

        /// <summary>
        ///     Gets the save command.
        /// </summary>
        /// <value>The save command.</value>
        public Command ReviewListCommand
        {
            get
            {
                return _reviewListCommand ??
                       (_reviewListCommand = new Command(ExecuteReviewListCommand, () => !IsBusy));
            }
        }

        /// <summary>
        ///     Gets the review answer OK command.
        /// </summary>
        /// <value>The review answer OK command.</value>
        public Command ReviewItemOkCommand
        {
            get
            {
                return _reviewItemOkCommand ??
                       (_reviewItemOkCommand =
                           new Command(ExecuteReviewAnswerOkCommand, () => !IsBusy));
            }
        }


        /// <summary>
        ///     Gets or sets a value indicating whether this instance hasitems.
        /// </summary>
        /// <value><c>true</c> if this instance hasitems; otherwise, <c>false</c>.</value>
        public bool HasItems
        {
            get { return _hasItems; }
            set { SetProperty(ref _hasItems, value); }
        }

        public string NoItemFoundText
        {
            get { return _noItemFoundText; }
            set { SetProperty(ref _noItemFoundText, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when the view appears.
        /// </summary>
        public override void OnViewAppearing()
        {
            base.OnViewAppearing();
            if (Items.Count < 1)
            {
                Page.DisplayAlertMessage(string.Empty, HACCPUtil.GetResourceString("Noitemsfound"));
            }

            if (HaccpAppSettings.SharedInstance.IsWindows && HaccpAppSettings.SharedInstance.DeviceSettings.AutoAdvance &&
                AutoBack)
            {
                Page.PopPage();
            }
        }


        public override void OnViewDisappearing()
        {
            if (IsBackNavigation)
            {
                MessagingCenter.Unsubscribe<LocationMenuItem>(this, HaccpConstant.RecorditemMessage);
                MessagingCenter.Unsubscribe<AutoAdvanceLocationMessage>(this, HaccpConstant.AutoadvancelocationMessage);
                MessagingCenter.Unsubscribe<CleanUpMessage>(this, HaccpConstant.CleanupMessage);
                MessagingCenter.Unsubscribe<UploadRecordRefreshMessage>(this, HaccpConstant.UploadRecordRefresh);
            }
            base.OnViewDisappearing();
            IsBackNavigation = true;
        }

        /// <summary>
        ///     Executes the review list command.
        /// </summary>
        private void ExecuteReviewListCommand()
        {
            IsBackNavigation = false;
            Page.NavigateToWithSelectedObject(PageEnum.Temperaturereview, true, _location);
        }

        /// <summary>
        ///     Executes the items filter command.
        /// </summary>
        /// <returns>The items filter command.</returns>
        private void ExecuteItemsFilterCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            ItemsFilterCommand.ChangeCanExecute();

            try
            {
                ListCompletedItems = !ListCompletedItems; //The default value of ListCompletedItems is false

                Title = ListCompletedItems
                    ? HACCPUtil.GetResourceString("ReviewItem")
                    : HACCPUtil.GetResourceString("SelectItem");

                NoItemFoundText = ListCompletedItems
                    ? HACCPUtil.GetResourceString("Noitemsfoundtoreview")
                    : HACCPUtil.GetResourceString("Noitemsfound");

                var items = _dataStore.GetItems(LocationId, ListCompletedItems);
                var filteredItems = new List<LocationMenuItem>();
                if (ListCompletedItems && items != null)
                {
                    foreach (var item in items)
                    {
                        var record = _dataStore.GetTemperatureRecordById(item.ItemId, item.LocationId);
                        var temp = HACCPUtil.ConvertToDouble(record.Temperature);

                        if (item.RecordStatus == 1 && record.IsNA == 0 && temp < HACCPUtil.ConvertToDouble(item.Min) ||
                            temp > HACCPUtil.ConvertToDouble(item.Max))
                        {
                            filteredItems.Add(item);
                        }
                    }
                    items = filteredItems;
                }

                Items = new ObservableCollection<LocationMenuItem>(items);
                HasItems = Items.Any();
                OnPropertyChanged("Items");
            }
            catch
            {
                // ignored
            }
            finally
            {
                IsBusy = false;
                ItemsFilterCommand.ChangeCanExecute();
            }
        }

        /// <summary>
        ///     Executes the review answer OK command.
        /// </summary>
        /// <returns>The review answer OK command.</returns>
        private void ExecuteReviewAnswerOkCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            ReviewItemOkCommand.ChangeCanExecute();

            IsReviewItemVisible = false;

            IsBusy = false;
            ReviewItemOkCommand.ChangeCanExecute();
        }

        /// <summary>
        ///     Handles the item click.
        /// </summary>
        public ItemTemperature HandleItemClick(LocationMenuItem item)
        {
            return _dataStore.GetTemperatureRecordById(item.ItemId, item.LocationId);
        }


        /// <summary>
        ///     Loads the menu item.
        /// </summary>
        /// <returns>The menu item.</returns>
        public async Task LoadMenuItem(LocationMenuItem item)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (item != null)
            {
                if (ListCompletedItems)
                {
                    var record = HandleItemClick(item);

                    if (record != null)
                    {
                        MessagingCenter.Send(new ShowReviewMessage(record, item), HaccpConstant.ReviewMessage);
                    }
                }
                else
                {
                    IsReviewItemVisible = false;
                    IsBackNavigation = false;

                    await Page.NavigateToWithSelectedObject(PageEnum.RecordItem, true, item);
                }
            }


            IsBusy = false;
        }

        #endregion
    }
}