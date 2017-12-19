using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HACCP.Core
{
    public class WirelessTasksViewModel : BaseViewModel
    {
        private readonly IDataStore dataStore;
        private readonly IHACCPService haccpService;
        private Command changeCheckListCommand;
        private Command changeMenuCommand;
        private List<CheckListResponse> checklists;
        private Command downloadUsersCommand;
        public bool isBackNavigation = true;
        
        private bool onceLoaded;
        private bool pendingCheckListToUpload;
        private bool pendingTemperatureRecordsToUpload;
        private double progress;
        private Command serverSettingsCommand;
        private List<ItemTemperature> temperatures;
        private Command testConnectionCommand;
        private Command updateSiteCommand;
        private Command uploadRecordCommand;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HACCP.Core.WirelessTasksViewModel" /> class.
        /// </summary>
        /// <param name="page">Page.</param>
        public WirelessTasksViewModel(IPage page) : base(page)
        {
            onceLoaded = false;
            dataStore = new SQLiteDataStore();
            haccpService = new HACCPService(dataStore);
            
            checklists = dataStore.GetCheckListResponses();
            temperatures = dataStore.GetItemTemperatures();
            if (temperatures != null && temperatures.Count > 0)
            {
                pendingTemperatureRecordsToUpload = true;
            }

            if (checklists != null && checklists.Count > 0)
            {
                pendingCheckListToUpload = true;
            }
            MessagingCenter.Subscribe<ProgressMessage>(this, HaccpConstant.UploadrecordMessage, sender =>
            {
                var message = sender;
                Progress = message.UploadCount/(double) message.TotalCount;
            });

            MessagingCenter.Subscribe<UploadRecordRefreshMessage>(this, HaccpConstant.UploadRecordRefresh, sender =>
            {
                checklists = dataStore.GetCheckListResponses();
                temperatures = dataStore.GetItemTemperatures();
                if (temperatures != null && temperatures.Count > 0)
                {
                    pendingTemperatureRecordsToUpload = true;
                }
                else
                    pendingTemperatureRecordsToUpload = false;

                if (checklists != null && checklists.Count > 0)
                {
                    pendingCheckListToUpload = true;
                }
                else
                    pendingCheckListToUpload = false;
            });
        }

        #region Properties

        /// <summary>
        ///     Gets or sets the progress.
        /// </summary>
        /// <value>The progress.</value>
        public double Progress
        {
            get { return progress; }
            set { SetProperty(ref progress, value); }
        }


        
        /// <summary>
        ///     Gets the change menu command.
        /// </summary>
        /// <value>The change menu command.</value>
        public Command ChangeMenuCommand
        {
            get
            {
                return changeMenuCommand ??
                       (changeMenuCommand =
                           new Command(async () => await ExecuteChangeMenuCommand(), () => !IsBusy));
            }
        }


        /// <summary>
        ///     Gets the change check list command.
        /// </summary>
        /// <value>The change check list command.</value>
        public Command ChangeCheckListCommand
        {
            get
            {
                return changeCheckListCommand ??
                       (changeCheckListCommand =
                           new Command(async () => await ExecuteChangeCheckListCommand(), () => !IsBusy));
            }
        }

        /// <summary>
        ///     Gets the server settings command.
        /// </summary>
        /// <value>The server settings command.</value>
        public Command ServerSettingsCommand
        {
            get
            {
                return serverSettingsCommand ??
                       (serverSettingsCommand =
                           new Command(ExecuteServerSettingsCommand, () => !IsBusy));
            }
        }

        /// <summary>
        ///     Gets the update site command.
        /// </summary>
        /// <value>The update site command.</value>
        public Command UpdateSiteCommand
        {
            get
            {
                return updateSiteCommand ??
                       (updateSiteCommand =
                           new Command(async () => await ExecuteUpdateSiteCommand(), () => !IsBusy));
            }
        }

        /// <summary>
        ///     Gets the download users command.
        /// </summary>
        /// <value>The download users command.</value>
        public Command DownloadUsersCommand
        {
            get
            {
                return downloadUsersCommand ??
                       (downloadUsersCommand =
                           new Command(async () => await ExecuteDownloadUsersCommand(), () => !IsBusy));
            }
        }

        /// <summary>
        ///     Gets the test connection command.
        /// </summary>
        /// <value>The test connection command.</value>
        public Command TestConnectionCommand
        {
            get
            {
                return testConnectionCommand ??
                       (testConnectionCommand =
                           new Command(async () => await ExecuteTestConnectionCommand(), () => !IsBusy));
            }
        }


        /// <summary>
        ///     Gets the upload record command.
        /// </summary>
        /// <value>The upload record command.</value>
        public Command UploadRecordCommand
        {
            get
            {
                return uploadRecordCommand ??
                       (uploadRecordCommand =
                           new Command(async () => await ExecuteUploadRecordCommand(), () => !IsBusy));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when the view appears.
        /// </summary>
        public override void OnViewAppearing()
        {
            base.OnViewAppearing();
            if (IsServerAddressExist == false)
                Page.NavigateTo(PageEnum.ServerSettings, false);
            else if (onceLoaded == false)
            {
                TestConnectionCommand.Execute(null);
            }
            onceLoaded = true;
            OnPropertyChanged(IsMenuCheckListActivePropertyName);
        }


        public override void OnViewDisappearing()
        {
            base.OnViewDisappearing();
            MessagingCenter.Unsubscribe<ProgressMessage>(this, HaccpConstant.UploadrecordMessage);
            if (isBackNavigation)
                MessagingCenter.Unsubscribe<UploadRecordRefreshMessage>(this, HaccpConstant.UploadRecordRefresh);

            isBackNavigation = true;
        }

        /// <summary>
        ///     Raises the login changed event.
        /// </summary>
        protected override void OnLoginChanged()
        {
            base.OnLoginChanged();
            ChangeMenuCommand.ChangeCanExecute();
            ChangeCheckListCommand.ChangeCanExecute();
        }

        /// <summary>
        ///     Executes the server settings command.
        /// </summary>
        /// <returns>The server settings command.</returns>
        private void ExecuteServerSettingsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            Page.ShowProgressIndicator();
            ServerSettingsCommand.ChangeCanExecute();

            isBackNavigation = false;

            Task.Run(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Page.NavigateTo(PageEnum.ServerSettings, true);
                    IsBusy = false;
                    Page.DismissPopup();
                    ServerSettingsCommand.ChangeCanExecute();
                });
            });
        }

        /// <summary>
        ///     Executes the update site command.
        /// </summary>
        /// <returns>The update site command.</returns>
        private async Task ExecuteUpdateSiteCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            UpdateSiteCommand.ChangeCanExecute();
            try
            {
                if (haccpService.IsConnected() == false)
                {
                    Page.DismissPopup();
                    IsBusy = false;
                    Page.DisplayAlertMessage(HACCPUtil.GetResourceString("EnableNetworkConnection"),
                        HACCPUtil.GetResourceString(
                            "YourequireanactiveInternetconnectiontoperformsomefunctionsWerecommendthatyouenableWiFiforthispurposeDatachargesmayapplyifWiFiisnotenabled"));
                    return;
                }
                if (!pendingCheckListToUpload && !pendingTemperatureRecordsToUpload)
                {
                    Page.ShowProgressIndicator();
                    var res = await haccpService.DownloadSiteAndSettings(false);
                    if (Device.OS == TargetPlatform.WinPhone || Device.OS == TargetPlatform.WinPhone)
                        await Task.Delay(500);
                    if (res.IsSuccess)
                    {
                        Page.DismissPopup();
                        IsBusy = false;
                        Page.DisplayAlertMessage("", HACCPUtil.GetResourceString("SiteandSettingsupdatedsuccessfully"));
                    }
                    else
                    {
                        Page.DismissPopup();
                        IsBusy = false;
                        Page.DisplayAlertMessage("", res.Message);
                    }
                }
                else
                {
                    IsBusy = false;
                    Page.DismissPopup();
                    await
                        Page.ShowAlert(string.Empty,
                            HACCPUtil.GetResourceString("UploadRecordsPendingOnUpdateSiteSettingsMessage"));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ooops! Something went wrong while fetch users from server. Exception: {0}", ex);
            }
            finally
            {
                IsBusy = false;
                Page.DismissPopup();
                UpdateSiteCommand.ChangeCanExecute();
            }
        }

        /// <summary>
        ///     Executes the download users command.
        /// </summary>
        /// <returns>The download users command.</returns>
        private async Task ExecuteDownloadUsersCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            Page.ShowProgressIndicator();
            DownloadUsersCommand.ChangeCanExecute();
            try
            {
                if (haccpService.IsConnected() == false)
                {
                    IsBusy = false;
                    Page.DismissPopup();
                    Page.DisplayAlertMessage(HACCPUtil.GetResourceString("EnableNetworkConnection"),
                        HACCPUtil.GetResourceString(
                            "YourequireanactiveInternetconnectiontoperformsomefunctionsWerecommendthatyouenableWiFiforthispurposeDatachargesmayapplyifWiFiisnotenabled"));
                    return;
                }
                if (HaccpAppSettings.SharedInstance.SiteSettings.SiteId <= 1)
                {
                    IsBusy = false;
                    Page.DismissPopup();
                    await
                        Page.ShowAlert(HACCPUtil.GetResourceString("NoSiteInformationFound"),
                            HACCPUtil.GetResourceString(
                                "NositeinformationsfoundPleasetapUpdateSiteandSettingsintheWirelessTasksmenu"));
                    return;
                }
                if (!pendingCheckListToUpload && !pendingTemperatureRecordsToUpload)
                {
                    var res = await haccpService.DownloadUsers();
                    if (res.IsSuccess)
                    {
                        IsBusy = false;
                        Page.DismissPopup();
                        Page.DisplayAlertMessage("", HACCPUtil.GetResourceString("UserListupdatedsuccessfully"));
                    }
                    else
                    {
                        IsBusy = false;
                        Page.DismissPopup();
                        Page.DisplayAlertMessage("", res.Message);
                    }
                }
                else
                {
                    IsBusy = false;
                    Page.DismissPopup();
                    await
                        Page.ShowAlert(string.Empty,
                            HACCPUtil.GetResourceString("UploadRecordsPendingOnUpdateUsersMessage"));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ooops! Something went wrong while fetch users from server. Exception: {0}", ex);
            }
            finally
            {
                IsBusy = false;
                Page.DismissPopup();
                DownloadUsersCommand.ChangeCanExecute();
            }
        }

        /// <summary>
        ///     Executes the test connection command.
        /// </summary>
        /// <returns>The test connection command.</returns>
        private async Task ExecuteTestConnectionCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            Page.ShowProgressIndicator();
            TestConnectionCommand.ChangeCanExecute();
            try
            {
                if (haccpService.IsConnected())
                {
                    bool isConnected;
                    if (HaccpAppSettings.SharedInstance.IsWindows)
                        isConnected =
                            await
                                haccpService.IsWsdlAccess(HaccpAppSettings.SharedInstance.SiteSettings.ServerAddress,
                                    HaccpAppSettings.SharedInstance.SiteSettings.ServerPort,
                                    HaccpAppSettings.SharedInstance.SiteSettings.ServerDirectory);
                    else
                        isConnected =
                            await
                                haccpService.IsRemoteReachable(
                                    HaccpAppSettings.SharedInstance.SiteSettings.ServerAddress,
                                    HaccpAppSettings.SharedInstance.SiteSettings.ServerPort);

                    if (isConnected == false)
                    {
                        IsBusy = false;
                        Page.DismissPopup();
                        Page.DisplayAlertMessage(HACCPUtil.GetResourceString("ConnectionTimeout"),
                            HACCPUtil.GetResourceString(
                                "UnabletoconnecttotheserverPleaseverifyconnectionsettingsandtryagain"));
                    }
                    else
                    {
                        isConnected =
                            await
                                haccpService.IsWsdlAccess(HaccpAppSettings.SharedInstance.SiteSettings.ServerAddress,
                                    HaccpAppSettings.SharedInstance.SiteSettings.ServerPort,
                                    HaccpAppSettings.SharedInstance.SiteSettings.ServerDirectory);
                        if (isConnected == false)
                        {
                            IsBusy = false;
                            Page.DismissPopup();
                            Page.DisplayAlertMessage(HACCPUtil.GetResourceString("NoWebService"),
                                HACCPUtil.GetResourceString(
                                    "UnabletofindthewebservicePleaseverifyconnectionsettingsandtryagain"));
                        }
                    }
                }
                else
                {
                    IsBusy = false;
                    Page.DismissPopup();
                    Page.DisplayAlertMessage(HACCPUtil.GetResourceString("EnableNetworkConnection"),
                        HACCPUtil.GetResourceString(
                            "YourequireanactiveInternetconnectiontoperformsomefunctionsWerecommendthatyouenableWiFiforthispurposeDatachargesmayapplyifWiFiisnotenabled"));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ooops! Something went wrong while test connection. Exception: {0}", ex);
            }
            finally
            {
                IsBusy = false;
                Page.DismissPopup();
                TestConnectionCommand.ChangeCanExecute();
            }
        }

        /// <summary>
        ///     Executes the change check list command.
        /// </summary>
        /// <returns>The change check list command.</returns>
        private async Task ExecuteChangeCheckListCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            ChangeCheckListCommand.ChangeCanExecute();
            Page.ShowProgressIndicator();
            try
            {
                if (haccpService.IsConnected() == false)
                {
                    IsBusy = false;
                    Page.DismissPopup();
                    Page.DisplayAlertMessage(HACCPUtil.GetResourceString("EnableNetworkConnection"),
                        HACCPUtil.GetResourceString(
                            "YourequireanactiveInternetconnectiontoperformsomefunctionsWerecommendthatyouenableWiFiforthispurposeDatachargesmayapplyifWiFiisnotenabled"));
                    return;
                }

                if (HaccpAppSettings.SharedInstance.SiteSettings.SiteId > 0)
                {
                    var res = await haccpService.DownloadChecklists();
                    if (res.IsSuccess)
                    {
                        if (!pendingCheckListToUpload)
                        {
                            object isMenu = false;
                            isBackNavigation = false;
                            await Page.NavigateToWithSelectedObject(PageEnum.MenuChecklist, true, isMenu);
                        }
                        else
                        {
                            IsBusy = false;
                            Page.DismissPopup();
                            await
                                Page.ShowAlert(string.Empty,
                                    HACCPUtil.GetResourceString(
                                        "CannotchangetheChecklistasrecordsarependingtobeuploadedTouploadtherecordsselectUploadRecordsundertheWirelessTasksmenu"));
                        }
                    }
                    else
                    {
                        IsBusy = false;
                        Page.DismissPopup();
                        await Page.ShowAlert(string.Empty, res.Message);
                    }
                }
                else
                {
                    IsBusy = false;
                    Page.DismissPopup();
                    await
                        Page.ShowAlert(HACCPUtil.GetResourceString("NoSiteInformationFound"),
                            HACCPUtil.GetResourceString(
                                "NositeinformationsfoundPleasetapUpdateSiteandSettingsintheWirelessTasksmenu"));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ooops! Something went wrong while select check list. Exception: {0}", ex);
            }
            finally
            {
                IsBusy = false;
                Page.DismissPopup();
                ChangeCheckListCommand.ChangeCanExecute();
            }
        }

        /// <summary>
        ///     Executes the change menu command.
        /// </summary>
        /// <returns>The change menu command.</returns>
        private async Task ExecuteChangeMenuCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            ChangeMenuCommand.ChangeCanExecute();
            Page.ShowProgressIndicator();

            try
            {
                if (haccpService.IsConnected() == false)
                {
                    IsBusy = false;
                    Page.DismissPopup();

                    Page.DisplayAlertMessage(HACCPUtil.GetResourceString("EnableNetworkConnection"),
                        HACCPUtil.GetResourceString(
                            "YourequireanactiveInternetconnectiontoperformsomefunctionsWerecommendthatyouenableWiFiforthispurposeDatachargesmayapplyifWiFiisnotenabled"));

                    return;
                }

                if (HaccpAppSettings.SharedInstance.SiteSettings.SiteId > 0)
                {
                    var res = await haccpService.DownloadMenus();
                    if (res.IsSuccess)
                    {
                        var menuLists = (IList<Menu>) res.Results;
                        if (menuLists.Count > 0)
                        {
                            if (!pendingTemperatureRecordsToUpload)
                            {
                                object isMenu = true;
                                isBackNavigation = false;
                                await Page.NavigateToWithSelectedObject(PageEnum.MenuChecklist, true, isMenu);
                            }
                            else
                            {
                                IsBusy = false;
                                Page.DismissPopup();
                                await
                                    Page.ShowAlert(string.Empty,
                                        HACCPUtil.GetResourceString(
                                            "CannotchangetheMenuasrecordsarependingtobeuploadedTouploadtherecordsselectUploadRecordsundertheWirelessTasksmenu"));
                            }
                        }
                    }
                    else
                    {
                        IsBusy = false;
                        Page.DismissPopup();
                        await Page.ShowAlert("", res.Message);
                    }
                }
                else
                {
                    IsBusy = false;
                    Page.DismissPopup();
                    await
                        Page.ShowAlert(HACCPUtil.GetResourceString("NoSiteInformationFound"),
                            HACCPUtil.GetResourceString(
                                "NositeinformationsfoundPleasetapUpdateSiteandSettingsintheWirelessTasksmenu"));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ooops! Something went wrong while select menu. Exception: {0}", ex);
            }
            finally
            {
                IsBusy = false;
                Page.DismissPopup();
                ChangeMenuCommand.ChangeCanExecute();
            }
        }

        /// <summary>
        ///     Executes the upload record command.
        /// </summary>
        private async Task ExecuteUploadRecordCommand()
        {
            if (IsBusy)
                return;

            try
            {
                if (haccpService.IsConnected() == false)
                {
                    Page.DisplayAlertMessage(HACCPUtil.GetResourceString("EnableNetworkConnection"),
                        HACCPUtil.GetResourceString(
                            "YourequireanactiveInternetconnectiontoperformsomefunctionsWerecommendthatyouenableWiFiforthispurposeDatachargesmayapplyifWiFiisnotenabled"));
                    return;
                }


                IsBusy = true;
                Page.ShowUploadProgress();
                UploadRecordCommand.ChangeCanExecute();
                if (HaccpAppSettings.SharedInstance.SiteSettings.SiteId > 0)
                {
                    //Getting check lists and temperatures
                    checklists = dataStore.GetCheckListResponses();
                    temperatures = dataStore.GetItemTemperatures();

                    if ((checklists == null || checklists.Count == 0) &&
                        (temperatures == null || temperatures.Count == 0))
                    {
                        IsBusy = false;
                        Page.DismissUploadProgress();
                        await Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("Norecordsfoundtoupload"));
                    }
                    else if (
                        await
                            Page.ShowConfirmAlert(HACCPUtil.GetResourceString("UploadRecords"),
                                HACCPUtil.GetResourceString(
                                    "Alltherecordswillbeuploadedandthecheckmarkswillbecleared")))
                    {
//confirmation alert
                        if (checklists != null && (checklists.Count > 0 || temperatures.Count > 0))
                        {
                            //IsBusy = true;
                            //this.page.ShowUploadProgress ();
                            //Calling upload records method
                            var response = await haccpService.UploadRecords(checklists, temperatures);

                            if (response.IsSuccess)
                            {
                                IsBusy = false;
                                Page.DismissUploadProgress();
                                pendingCheckListToUpload = false;
                                pendingTemperatureRecordsToUpload = false;
                                await
                                    Page.ShowAlert(string.Empty,
                                        HACCPUtil.GetResourceString(
                                            "UploadedalltherecordstoHACCPEnterpriseapplicationsuccessfully"));
                            }
                            else if (!string.IsNullOrEmpty(response.Message))
                            {
                                IsBusy = false;
                                Page.DismissUploadProgress();
                                await Page.ShowAlert(string.Empty, response.Message);
                            }
                            else
                            {
                                IsBusy = false;
                                Page.DismissUploadProgress();
                                await
                                    Page.ShowAlert(string.Empty,
                                        HACCPUtil.GetResourceString(
                                            "AnerroroccurredwhileuploadingtherecordsPleasetryagainlater"));
                            }
                        }
                        else
                        {
                            IsBusy = false;
                            Page.DismissUploadProgress();
                            await
                                Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("Norecordsfoundtoupload"));
                        }
                    }
                }
                else
                {
                    IsBusy = false;
                    Page.DismissUploadProgress();
                    await
                        Page.ShowAlert(HACCPUtil.GetResourceString("NoSiteInformationFound"),
                            HACCPUtil.GetResourceString(
                                "NositeinformationsfoundPleasetapUpdateSiteandSettingsintheWirelessTasksmenu"));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ooops! Something went wrong while select menu. Exception: {0}", ex);
            }
            finally
            {
                IsBusy = false;
                Page.DismissUploadProgress();
                UploadRecordCommand.ChangeCanExecute();
            }
        }

        #endregion
    }
}