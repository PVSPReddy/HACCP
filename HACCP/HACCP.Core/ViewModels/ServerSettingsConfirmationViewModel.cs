using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HACCP.Core
{
    public class ServerSettingsConfirmationViewModel : BaseViewModel
    {
        private readonly IDataStore dataStore;
        private readonly IHACCPService haccpService;
        private readonly SiteSettings siteSettings;
        private Command saveCommand;
        private short serverResetOption;
        private Command settingOptionCommand;


        /// <summary>
        ///     Initializes a new instance of the <see cref="HACCP.Core.ServerSettingsConfirmationViewModel" /> class.
        /// </summary>
        /// <param name="page">Page.</param>
        /// <param name="_siteSettings">Site settings.</param>
        public ServerSettingsConfirmationViewModel(IPage page, SiteSettings _siteSettings) : base(page)
        {
            dataStore = new SQLiteDataStore();

            haccpService = new HACCPService(dataStore);
            serverResetOption = 1; //default option: keep the pending recorded items, just pointed to new server. 
            siteSettings = _siteSettings;
        }

        #region Properties

        /// <summary>
        ///     Gets or sets the radio button image no.
        /// </summary>
        /// <value>The radio button image no.</value>
        public short ServerResetOption
        {
            get { return serverResetOption; }
            set { SetProperty(ref serverResetOption, value); }
        }


        public Command SettingsOptionCommand
        {
            get
            {
                return settingOptionCommand ??
                       (settingOptionCommand = new Command<string>(param =>
                       {
                           if (param != null)
                           {
                               short parameterValue;
                               short.TryParse(param.ToString(), out parameterValue);
                               ServerResetOption = parameterValue;
                           }
                       }));
            }
        }

        /// <summary>
        ///     Gets the save command.
        /// </summary>
        /// <value>The save command.</value>
        public Command SaveCommand
        {
            get
            {
                return saveCommand ??
                       (saveCommand = new Command(async () => await ExecuteSaveCommand(), () => !IsBusy));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Settingses the option command.
        /// </summary>
        /// <returns>The option command.</returns>
        private async Task ExecuteSaveCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            var saved = false;
            SaveCommand.ChangeCanExecute();
            try
            {
                if (serverResetOption == 3)
                {
                    // erase all the data
                    if (
                        await
                            Page.ShowConfirmAlert(HACCPUtil.GetResourceString("Warning"),
                                HACCPUtil.GetResourceString("TheapplicationandDeviceIDwillbereset")))
                    {
                        dataStore.EraseAllData();

                        //Reset Default Values to Shared Variables
                        HaccpAppSettings.SharedInstance.SiteSettings.ServerAddress = string.Empty;
                        HaccpAppSettings.SharedInstance.SiteSettings.ServerPort = string.Empty;
                        HaccpAppSettings.SharedInstance.SiteSettings.ServerDirectory =
                            HaccpConstant.DefultServerDirectyory;
                        HaccpAppSettings.SharedInstance.SiteSettings.CheckListId = 0;
                        HaccpAppSettings.SharedInstance.SiteSettings.CheckListName = string.Empty;
                        HaccpAppSettings.SharedInstance.SiteSettings.LastBatchNumber = 0;
                        HaccpAppSettings.SharedInstance.SiteSettings.LastUploaded = null;
                        HaccpAppSettings.SharedInstance.SiteSettings.MenuId = 0;
                        HaccpAppSettings.SharedInstance.SiteSettings.MenuName = string.Empty;
                        HaccpAppSettings.SharedInstance.SiteSettings.SiteId = 0;
                        HaccpAppSettings.SharedInstance.SiteSettings.SiteName = string.Empty;
                        HaccpAppSettings.SharedInstance.SiteSettings.TimeZoneId = string.Empty;

                        HaccpAppSettings.SharedInstance.DeviceSettings.AllowManualTemp = 0;
                        HaccpAppSettings.SharedInstance.DeviceSettings.AllowTextMemo = 0;
                        HaccpAppSettings.SharedInstance.DeviceSettings.AutoAdvance = false;
                        HaccpAppSettings.SharedInstance.DeviceSettings.AutoOffTime = 0;
                        HaccpAppSettings.SharedInstance.DeviceSettings.CustomProbDescription = string.Empty;
                        HaccpAppSettings.SharedInstance.DeviceSettings.DateFormat = 0;
                        HaccpAppSettings.SharedInstance.DeviceSettings.Line1 = string.Empty;
                        HaccpAppSettings.SharedInstance.DeviceSettings.Line2 = string.Empty;
                        HaccpAppSettings.SharedInstance.DeviceSettings.RequirePin = 0;
                        HaccpAppSettings.SharedInstance.DeviceSettings.SkipRecordPreview = false;
                        HaccpAppSettings.SharedInstance.DeviceSettings.TempScale = 0;
                        HaccpAppSettings.SharedInstance.DeviceSettings.TimeFormat = 0;

                        HaccpAppSettings.SharedInstance.ResetDeviceId();

                        HaccpAppSettings.SharedInstance.SiteSettings.ServerAddress = siteSettings.ServerAddress.Trim();
                        HaccpAppSettings.SharedInstance.SiteSettings.ServerPort = siteSettings.ServerPort.Trim();
                        HaccpAppSettings.SharedInstance.SiteSettings.ServerDirectory =
                            siteSettings.ServerDirectory.Trim();

                        dataStore.SaveSiteSettings(HaccpAppSettings.SharedInstance.SiteSettings);
                        HaccpAppSettings.SharedInstance.CurrentUserId = 0;
                        HaccpAppSettings.SharedInstance.CheckPendingRecords = false;
                        Settings.RecordingMode = RecordingMode.BlueTooth;

                        saved = true;
                    }
                }
                else if (serverResetOption == 2)
                {
                    //upload pending recorded items to old server then save the new settings.

                    //upload current records to old server 

                    /*HACCPAppSettings.SharedInstance.SiteSettings.ServerAddress = this.siteSettings.ServerAddress.Trim ();
					HACCPAppSettings.SharedInstance.SiteSettings.ServerPort = this.siteSettings.ServerPort.Trim ();
					HACCPAppSettings.SharedInstance.SiteSettings.ServerDirectory = this.siteSettings.ServerDirectory.Trim ();
					dataStore.SaveSiteSettings (HACCPAppSettings.SharedInstance.SiteSettings);
					saved = true;*/
                    IsBusy = false;
                    await ExecuteUploadRecordCommand();
                    HaccpAppSettings.SharedInstance.CheckPendingRecords = false;
                    //this.page.LoadHomePage ();
                    Page.ReloadHomePage();
                }
                else
                {
                    //serverResetOption == 1  keep the pending recorded items just pointed to new server. These pending items can be  uploaded to new server later. 
                    HaccpAppSettings.SharedInstance.SiteSettings.ServerAddress = siteSettings.ServerAddress.Trim();
                    HaccpAppSettings.SharedInstance.SiteSettings.ServerPort = siteSettings.ServerPort.Trim();
                    HaccpAppSettings.SharedInstance.SiteSettings.ServerDirectory = siteSettings.ServerDirectory.Trim();

                    dataStore.SaveSiteSettings(HaccpAppSettings.SharedInstance.SiteSettings);
                    saved = true;
                    HaccpAppSettings.SharedInstance.CheckPendingRecords = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ooops! Something went wrong while save connection. Exception: {0}", ex);
            }
            finally
            {
                IsBusy = false;
                SaveCommand.ChangeCanExecute();
            }
            if (saved)
            {
                await Page.ShowAlert("", HACCPUtil.GetResourceString("ServerSettingsupdatedsuccessfully"));
                //this.page.LoadHomePage ();
                Page.ReloadHomePage();
                //await this.page.PopPages(3);
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


                if (HaccpAppSettings.SharedInstance.SiteSettings.SiteId > 0)
                {
                    //Getting check lists and temperatures
                    var checklists = dataStore.GetCheckListResponses();
                    var temperatures = dataStore.GetItemTemperatures();

                    if ((checklists == null || checklists.Count == 0) &&
                        (temperatures == null || temperatures.Count == 0))
                    {
                        IsBusy = false;
                        Page.DismissPopup();
                        await Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("Norecordsfoundtoupload"));
                    }
                    else
                    {
                        if (checklists != null && (checklists.Count > 0 || temperatures.Count > 0))
                        {
                            IsBusy = true;
                            //this.page.ShowProgressIndicator ();
                            //Calling upload records method
                            var response = await haccpService.UploadRecords(checklists, temperatures);

                            if (response.IsSuccess)
                            {
                                IsBusy = false;
                                Page.DismissUploadProgress();
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
                            await Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("Norecordsfoundtoupload"));
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
            }
        }

        #endregion
    }
}