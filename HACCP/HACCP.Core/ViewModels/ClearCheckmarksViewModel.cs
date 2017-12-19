using Xamarin.Forms;

namespace HACCP.Core
{
    public class ClearCheckmarksViewModel : BaseViewModel
    {
        #region Member Variable

        private readonly IDataStore _dataStore;
        private bool _bothEnabled;
        private bool _checklistEnabled;
        private Command _checkmarksCommand;
        private bool _tempEnabled;

        #endregion

        /// <summary>
        /// ClearCheckmarksViewModel Constructor
        /// </summary>
        /// <param name="page"></param>
        public ClearCheckmarksViewModel(IPage page) : base(page)
        {
            _dataStore = new SQLiteDataStore();
            //	SetPropertyEnabledValues ();
        }

        #region Commands

        /// <summary>
        ///     Gets the timing command.
        /// </summary>
        /// <value>The timing command.</value>
        public Command CheckmarksCommand
        {
            get
            {
                return _checkmarksCommand ??
                       (_checkmarksCommand = new Command<string>(async parameter =>
                       {
                           if (IsBusy)
                               return;

                           IsBusy = true;
                           var updated = false;

                           if (parameter == "Temperature" && TemperatureEnabled)
                           {
                               if (
                                   await
                                       Page.ShowConfirmAlert(HACCPUtil.GetResourceString("Warning"),
                                           HACCPUtil.GetResourceString("ThecheckmarksofalltheTemperatureswillbecleared")))
                               {
                                   _dataStore.ClearTemperatures(false);
                                   updated = true;
                               }
                           }
                           else if (parameter == "Checklist" && ChecklistEnabled)
                           {
                               if (
                                   await
                                       Page.ShowConfirmAlert(HACCPUtil.GetResourceString("Warning"),
                                           HACCPUtil.GetResourceString("ThecheckmarksofalltheChecklistswillbecleared")))
                               {
                                   _dataStore.ClearCheckList(false);
                                   updated = true;
                               }
                           }
                           else if (parameter == "Both" && BothEnabled)
                           {
                               if (
                                   await
                                       Page.ShowConfirmAlert(HACCPUtil.GetResourceString("Warning"),
                                           HACCPUtil.GetResourceString(
                                               "ThecheckmarksofalltheTemperaturesandChecklistswillbecleared")))
                               {
                                   _dataStore.ClearTemperatures(false);
                                   _dataStore.ClearCheckList(false);
                                   updated = true;
                               }
                           }

                           if (updated)
                           {
                               SetPropertyEnabledValues();
                               await
                                   Page.ShowAlert("",
                                       HACCPUtil.GetResourceString("Thecheckmarkshavebeenclearedsuccessfully"));
                               Page.ReloadHomePage();
                           }

                           IsBusy = false;
                       }));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a value indicating whether this <see cref="HACCP.Core.ClearCheckmarksViewModel" /> temperature visibility.
        /// </summary>
        /// <value><c>true</c> if temperature visibility; otherwise, <c>false</c>.</value>
        public bool TemperatureEnabled
        {
            get { return _tempEnabled; }
            set { SetProperty(ref _tempEnabled, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="HACCP.Core.ClearCheckmarksViewModel" /> checklist
        ///     visibility.
        /// </summary>
        /// <value><c>true</c> if checklist visibility; otherwise, <c>false</c>.</value>
        public bool ChecklistEnabled
        {
            get { return _checklistEnabled; }
            set { SetProperty(ref _checklistEnabled, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="HACCP.Core.ClearCheckmarksViewModel" /> both visibility.
        /// </summary>
        /// <value><c>true</c> if both visibility; otherwise, <c>false</c>.</value>
        public bool BothEnabled
        {
            get { return _bothEnabled; }
            set { SetProperty(ref _bothEnabled, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Sets the property enabled values based on the data.
        /// </summary>
        public void SetPropertyEnabledValues()
        {
            TemperatureEnabled = _dataStore.CheckTemperaturesExists();
            ChecklistEnabled = _dataStore.CheckCheckListsExists();
            BothEnabled = TemperatureEnabled && ChecklistEnabled;

            Page.EndEditing();
        }

        /// <summary>
        /// OnViewAppearing
        /// </summary>
        public override void OnViewAppearing()
        {
            base.OnViewAppearing();

            MessagingCenter.Subscribe<UploadRecordRefreshMessage>(this, HaccpConstant.UploadRecordRefresh,
                sender => { SetPropertyEnabledValues(); });
        }

        /// <summary>
        /// OnViewDisappearing
        /// </summary>
        public override void OnViewDisappearing()
        {
            base.OnViewDisappearing();

            MessagingCenter.Unsubscribe<UploadRecordRefreshMessage>(this, HaccpConstant.UploadRecordRefresh);
        }

        #endregion
    }
}