using Xamarin.Forms;

namespace HACCP.Core
{
    public class ViewStatusViewModel : BaseViewModel
    {
        private readonly SQLiteDataStore dataStore;
        private string appVersion;
        private int checklistRecords;
        private string storeName;
        private int temperatureRecords;
        private int totalRecords;
        private Command viewStatusLinesCommand;


        /// <summary>
        ///     Initializes a new instance of the <see cref="HACCP.Core.ViewStatusViewModel" /> class.
        /// </summary>
        /// <param name="page">Page.</param>
        public ViewStatusViewModel(IPage page)
            : base(page)
        {
            dataStore = new SQLiteDataStore();


            AppVerison = DependencyService.Get<IInfoService>().GetAppVersion();
            StoreName = GetStoreName();
        }

        #region Commands

        public Command ViewStatusLinesCommand
        {
            get
            {
                return viewStatusLinesCommand ??
                       (viewStatusLinesCommand =
                           new Command(async () => { await Page.NavigateTo(PageEnum.ViewStatusLines, true); }));
            }
        }

        #endregion

        public override void OnViewAppearing()
        {
            base.OnViewAppearing();

            ChecklistRecords = dataStore.GetCheckListResponses().Count;
            TemperatureRecords = dataStore.GetItemTemperatures().Count;
            TotalRecords = ChecklistRecords + TemperatureRecords;


            MessagingCenter.Subscribe<BLEBlue2SettingsUpdatedMessage>(this, HaccpConstant.Bleblue2SettingsUpdate,
                sender => { OnPropertyChanged("CustomProbDescription"); });

            MessagingCenter.Subscribe<BleConnectionStatusMessage>(this, HaccpConstant.BleconnectionStatus,
                sender => { OnPropertyChanged("CustomProbDescription"); });

            MessagingCenter.Subscribe<UploadRecordRefreshMessage>(this, HaccpConstant.UploadRecordRefresh, sender =>
            {
                ChecklistRecords = dataStore.GetCheckListResponses().Count;
                TemperatureRecords = dataStore.GetItemTemperatures().Count;
                TotalRecords = ChecklistRecords + TemperatureRecords;
            });
        }

        public override void OnViewDisappearing()
        {
            base.OnViewAppearing();


            MessagingCenter.Unsubscribe<BLEBlue2SettingsUpdatedMessage>(this, HaccpConstant.Bleblue2SettingsUpdate);
            MessagingCenter.Unsubscribe<BleConnectionStatusMessage>(this, HaccpConstant.BleconnectionStatus);
            MessagingCenter.Unsubscribe<UploadRecordRefreshMessage>(this, HaccpConstant.UploadRecordRefresh);
        }

        private string GetStoreName()
        {
            if (!string.IsNullOrEmpty(HaccpAppSettings.SharedInstance.SiteSettings.SiteName) ||
                HaccpAppSettings.SharedInstance.SiteSettings.SiteId != 0)
            {
                return string.Format("{0} {1}", HaccpAppSettings.SharedInstance.SiteSettings.SiteName,
                    HaccpAppSettings.SharedInstance.SiteSettings.SiteId != 0
                        ? HaccpAppSettings.SharedInstance.SiteSettings.SiteId.ToString()
                        : "");
            }
            return null;
        }

        #region Properties

        /// <summary>
        ///     Gets or sets the app verison.
        /// </summary>
        /// <value>The app verison.</value>
        public string AppVerison
        {
            get { return appVersion; }
            set { SetProperty(ref appVersion, value); }
        }

        /// <summary>
        ///     Gets or sets the name of the store.
        /// </summary>
        /// <value>The name of the store.</value>
        public string StoreName
        {
            get { return storeName; }
            set { SetProperty(ref storeName, value); }
        }

        /// <summary>
        ///     Gets the selected menu.
        /// </summary>
        /// <value>The selected menu.</value>
        public string SelectedMenu
        {
            get { return HaccpAppSettings.SharedInstance.SiteSettings.MenuName; }
        }

        /// <summary>
        ///     Gets the selected checklist.
        /// </summary>
        /// <value>The selected checklist.</value>
        public string SelectedChecklist
        {
            get { return HaccpAppSettings.SharedInstance.SiteSettings.CheckListName; }
        }

        /// <summary>
        ///     Gets or sets the upload time.
        /// </summary>
        /// <value>The upload time.</value>
        public string UploadTime
        {
            get { return HACCPUtil.GetFormattedDate(HaccpAppSettings.SharedInstance.SiteSettings.LastUploaded); }
        }

        /// <summary>
        ///     Gets or sets the total records.
        /// </summary>
        /// <value>The total records.</value>
        public int TotalRecords
        {
            get { return totalRecords; }
            set { SetProperty(ref totalRecords, value); }
        }

        /// <summary>
        ///     Gets or sets the temperature records.
        /// </summary>
        /// <value>The temperature records.</value>
        public int TemperatureRecords
        {
            get { return temperatureRecords; }
            set { SetProperty(ref temperatureRecords, value); }
        }

        /// <summary>
        ///     Gets or sets the checklist records.
        /// </summary>
        /// <value>The checklist records.</value>
        public int ChecklistRecords
        {
            get { return checklistRecords; }
            set { SetProperty(ref checklistRecords, value); }
        }

        /// <summary>
        ///     Gets the device Id.
        /// </summary>
        /// <value>The device Id.</value>
        public string DeviceID
        {
            get { return HaccpAppSettings.SharedInstance.DeviceId; }
        }

        /// <summary>
        ///     Gets or sets the custom prob description.
        /// </summary>
        /// <value>The custom prob description.</value>
        public string CustomProbDescription
        {
            get
            {
                if (HaccpAppSettings.SharedInstance.IsWindows)
                    return WindowsBLEManager.SharedInstance.Settings != null &&
                           !string.IsNullOrEmpty(WindowsBLEManager.SharedInstance.Settings.Prob)
                        ? WindowsBLEManager.SharedInstance.Settings.Prob
                        : string.Empty;
                return BLEManager.SharedInstance.Settings != null &&
                       !string.IsNullOrEmpty(BLEManager.SharedInstance.Settings.Prob)
                    ? BLEManager.SharedInstance.Settings.Prob
                    : string.Empty;
            }
        }

        #endregion
    }
}