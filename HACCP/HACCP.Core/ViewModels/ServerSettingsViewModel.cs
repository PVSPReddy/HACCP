using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HACCP.Core
{
    public class ServerSettingsViewModel : BaseViewModel
    {
        private readonly IDataStore dataStore;
        private readonly IHACCPService service;
        private string addDirectoryPathoptional = string.Empty;
        private string notifEYEUrlLabel = string.Empty;
        private string notifEYEUsernameLabel = string.Empty;
        private string notifEYEPasswordLabel = string.Empty;

        private string notifEYEUrl = string.Empty;
        private string notifEYEUsername = string.Empty;
        private string notifEYEPassword = string.Empty;

        private string deviceID = string.Empty;
        private string enterPortoptional = string.Empty;
        private bool isLanguageListVisible;
        private ObservableCollection<Language> languageList = new ObservableCollection<Language>();
        private string languages = string.Empty;
        private string port = string.Empty;
        private string saveandContinue = string.Empty;
        private Command saveCommand;
        private string selectLanguage = string.Empty;
        private Command selectLanguageCommand;
        private string serverAddress = string.Empty;
        private string serverDirectory = string.Empty;
        private string serverIPAddressorDomainName = string.Empty;
        private string serverSettings = string.Empty;
        private Command testConnectionCommand;
        private string testServerConnectionandWebService = string.Empty;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HACCP.Core.ServerSettingsViewModel" /> class.
        /// </summary>
        /// <param name="page">Page.</param>
        public ServerSettingsViewModel(IPage page)
            : base(page)
        {
            dataStore = new SQLiteDataStore();
            service = new HACCPService(dataStore);


            ServerDirectory = HaccpConstant.DefultServerDirectyory;

            BindeAllLablelProperties();
            if (HaccpAppSettings.SharedInstance.SiteSettings.ServerAddress != null &&
                HaccpAppSettings.SharedInstance.SiteSettings.ServerAddress.Trim().Length > 0)
            {
                ServerAddress = HaccpAppSettings.SharedInstance.SiteSettings.ServerAddress;
                if (HaccpAppSettings.SharedInstance.SiteSettings.ServerPort != null)
                    Port = HaccpAppSettings.SharedInstance.SiteSettings.ServerPort;
                if (HaccpAppSettings.SharedInstance.SiteSettings.ServerDirectory != null)
                    ServerDirectory = HaccpAppSettings.SharedInstance.SiteSettings.ServerDirectory;
            }
        }

        #region Properties

        /// <summary>
        ///     ServerSettingsLable
        /// </summary>
        public string ServerSettingsLabel
        {
            get { return serverSettings; }
            set { SetProperty(ref serverSettings, value); }
        }

        /// <summary>
        ///     DeviceIDLable
        /// </summary>
        public string DeviceIDLabel
        {
            get { return deviceID; }
            set { SetProperty(ref deviceID, value); }
        }

/// <summary>
        /// ServerIPAddressorDomainNameLabel
/// </summary>
        public string ServerIPAddressorDomainNameLabel
        {
            get { return serverIPAddressorDomainName; }
            set { SetProperty(ref serverIPAddressorDomainName, value); }
        }

        /// <summary>
        ///     EnterPortoptionalLabel
        /// </summary>
        public string EnterPortoptionalLabel
        {
            get { return enterPortoptional; }
            set { SetProperty(ref enterPortoptional, value); }
        }

        /// <summary>
        ///     AddDirectoryPathoptionalLabel
        /// </summary>
        public string AddDirectoryPathoptionalLabel
        {
            get { return addDirectoryPathoptional; }
            set { SetProperty(ref addDirectoryPathoptional, value); }
        }

        /// <summary>
        ///     NotifyEYEPasswordLabel
        /// </summary>
        public string NotifyEYEPasswordLabel
        {
            get { return notifEYEPasswordLabel; }
            set { SetProperty(ref notifEYEPasswordLabel, value); }
        }

        /// <summary>
        ///     NotifEYEUrlLabel
        /// </summary>
        public string NotifEYEUrlLabel
        {
            get { return notifEYEUrlLabel; }
            set { SetProperty(ref notifEYEUrlLabel, value); }
        }

        /// <summary>
        ///     NotifyEYEUsernameLabel
        /// </summary>
        public string NotifyEYEUsernameLabel
        {
            get { return notifEYEUsernameLabel; }
            set { SetProperty(ref notifEYEUsernameLabel, value); }
        }

        /// <summary>
        ///     SaveandContinueLable
        /// </summary>
        public string SaveandContinueLabel
        {
            get { return saveandContinue; }
            set { SetProperty(ref saveandContinue, value); }
        }

        /// <summary>
        ///     SelectLanguageLabel
        /// </summary>
        public string SelectLanguageLabel
        {
            get { return selectLanguage; }
            set { SetProperty(ref selectLanguage, value); }
        }

        /// <summary>
        ///     LanguagesLable
        /// </summary>
        public string LanguagesLabel
        {
            get { return languages; }
            set { SetProperty(ref languages, value); }
        }

        /// <summary>
        ///     TestServerConnectionandWebServiceLabel
        /// </summary>
        public string TestServerConnectionandWebServiceLabel
        {
            get { return testServerConnectionandWebService; }
            set { SetProperty(ref testServerConnectionandWebService, value); }
        }


        /// <summary>
        ///     Gets or sets a value indicating whether this instance is language list visible.
        /// </summary>
        /// <value><c>true</c> if this instance is language list visible; otherwise, <c>false</c>.</value>
        public bool IsLanguageListVisible
        {
            get { return isLanguageListVisible; }
            set { SetProperty(ref isLanguageListVisible, value); }
        }

        /// <summary>
        ///     Gets or sets the language list.
        /// </summary>
        /// <value>The language list.</value>
        public ObservableCollection<Language> LanguageList
        {
            get { return languageList; }
            set { SetProperty(ref languageList, value); }
        }


        /// <summary>
        ///     Gets or sets the server address.
        /// </summary>
        /// <value>The server address.</value>
        public string ServerAddress
        {
            get { return serverAddress; }
            set
            {
                if (SetProperty(ref serverAddress, value))
                {
                    SaveCommand.ChangeCanExecute();
                    OnPropertyChanged("ShouldSave");
                }
            }
        }

        /// <summary>
        ///     Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public string Port
        {
            get { return port; }
            set
            {
                if (SetProperty(ref port, value))
                {
                    SaveCommand.ChangeCanExecute();
                    OnPropertyChanged("ShouldSave");
                }
            }
        }

        /// <summary>
        ///     Gets or sets the server directory.
        /// </summary>
        /// <value>The server directory.</value>
        public string ServerDirectory
        {
            get { return serverDirectory; }
            set
            {
                if (SetProperty(ref serverDirectory, value))
                {
                    SaveCommand.ChangeCanExecute();
                    OnPropertyChanged("ShouldSave");
                }
            }
        }

        /// <summary>
        ///     Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public string NotifEYEUrl
        {
            get { return notifEYEUrl; }
            set
            {
                if (SetProperty(ref notifEYEUrl, value))
                {
                    SaveCommand.ChangeCanExecute();
                    OnPropertyChanged("ShouldSave");
                }
            }
        }

        /// <summary>
        ///     Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public string NotifEYEUsername
        {
            get { return notifEYEUsername; }
            set
            {
                if (SetProperty(ref notifEYEUsername, value))
                {
                    SaveCommand.ChangeCanExecute();
                    OnPropertyChanged("ShouldSave");
                }
            }
        }

        /// <summary>
        ///     Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public string NotifEYEPassword
        {
            get { return notifEYEPassword; }
            set
            {
                if (SetProperty(ref notifEYEPassword, value))
                {
                    SaveCommand.ChangeCanExecute();
                    OnPropertyChanged("ShouldSave");
                }
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
                       (saveCommand =
                           new Command(async () => await ExecuteSaveCommand(), () => !IsBusy && ShouldSave));
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
        ///     Gets or sets a value indicating whether this <see cref="HACCP.Core.ServerSettingsViewModel" /> has settings
        ///     entries.
        /// </summary>
        /// <value><c>true</c> if has settings entries; otherwise, <c>false</c>.</value>
        public bool ShouldSave
        {
            get
            {
                if (IsServerAddressExist)
                    return HaccpAppSettings.SharedInstance.SiteSettings.ServerAddress != ServerAddress.Trim() ||
                           HaccpAppSettings.SharedInstance.SiteSettings.ServerPort != Port.Trim() ||
                           HaccpAppSettings.SharedInstance.SiteSettings.ServerDirectory != ServerDirectory.Trim();
                return ServerAddress.Trim().Length > 0;
            }
        }


        public Command SelectLanguageCommand
        {
            get
            {
                return selectLanguageCommand ??
                       (selectLanguageCommand =
                           new Command(async () => await ExecuteSelectLanguageCommand(), () => !IsBusy));
            }
        }

        #endregion

        #region Methods

        private void BindeAllLablelProperties()
        {
            try
            {
                ServerSettingsLabel = HACCPUtil.GetResourceString("ServerSettings");
                DeviceIDLabel = HACCPUtil.GetResourceString("DeviceID");
                ServerIPAddressorDomainNameLabel =
                    HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("ServerIPAddressorDomainName"), true);
                EnterPortoptionalLabel = HACCPUtil.GetResourceString("EnterPortoptional");
                AddDirectoryPathoptionalLabel = HACCPUtil.GetResourceString("AddDirectoryPathoptional");
                TestServerConnectionandWebServiceLabel = HACCPUtil.GetResourceString("TestServerConnectionandWebService");
                SaveandContinueLabel = HACCPUtil.GetResourceString("SaveandContinue");
                SelectLanguageLabel = HACCPUtil.GetResourceString("SelectLanguage");
                LanguagesLabel = HACCPUtil.GetResourceString("Languages");
                NotifEYEUrlLabel = HACCPUtil.GetResourceString("NotifEyeUrl");
                NotifyEYEUsernameLabel = HACCPUtil.GetResourceString("NotifEYEUserName");
                NotifyEYEPasswordLabel = HACCPUtil.GetResourceString("NotifEyeUserPassword");

            }
            catch (Exception)
            {
                // ignored
            }
        }


        /// <summary>
        ///     Executes the save command.
        /// </summary>
        /// <returns>The save command.</returns>
        private async Task ExecuteSaveCommand()
        {
            var saved = false;

            if (IsBusy)
                return;

            if (ServerAddress.Trim() == null || ServerAddress.Trim().Length < 1)
            {
                Page.DisplayAlertMessage("", HACCPUtil.GetResourceString("EnterServerIPAddressorDomainName"));
                return;
            }
            var isDataExists = false;
            Page.EndEditing();
            IsBusy = true;
            SaveCommand.ChangeCanExecute();
			Page.ShowProgressIndicator ();
            var currentServerAddressStatus = IsServerAddressExist;
            try
            {
                var isConnected = await TestConnection(false);
                if (isConnected)
                {
                    if (currentServerAddressStatus)
                        isDataExists = dataStore.CheckTemperaturesExists() || dataStore.CheckCheckListsExists();

                    if (isDataExists == false)
                    {
                        HaccpAppSettings.SharedInstance.SiteSettings.ServerAddress = ServerAddress.Trim();
                        HaccpAppSettings.SharedInstance.SiteSettings.ServerPort = Port.Trim();
                        HaccpAppSettings.SharedInstance.SiteSettings.ServerDirectory = ServerDirectory.Trim();

                        dataStore.SaveSiteSettings(HaccpAppSettings.SharedInstance.SiteSettings);

                        saved = true;
                    }
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
				Page.DismissPopup ();
            }
            if (saved)
            {
                await Page.ShowAlert("", HACCPUtil.GetResourceString("ServerSettingsupdatedsuccessfully"));

                if (currentServerAddressStatus)
                    await Page.PopPage();
                else
                {
                    //if (HaccpAppSettings.SharedInstance.IsWindows)
                    //{
                    //    var style = Application.Current.Resources["WindowsListScrollHelpGridStyle"] as Style;
                    //    if (style != null)
                    //    {
                    //        style.Setters.RemoveAt(0);
                    //        style.Setters.Insert(0, new Setter {Property = VisualElement.HeightRequestProperty, Value = 90});
                    //    }

                    //    var recordstyle = Application.Current.Resources["WindowsScrollHelpGridStyle"] as Style;
                    //    if (recordstyle != null)
                    //    {
                    //        recordstyle.Setters.RemoveAt(0);
                    //        recordstyle.Setters.Insert(0,
                    //            new Setter {Property = VisualElement.HeightRequestProperty, Value = 85});
                    //    }
                    //    // related to scroll issue fix after changing language
                    //}
                    Page.LoadHomePage();
                }
            }
            else if (isDataExists)
            {
                var siteSettings = new SiteSettings
                {
                    ServerAddress = ServerAddress.Trim(),
                    ServerPort = Port.Trim(),
                    ServerDirectory = ServerDirectory.Trim()
                };
                await Page.NavigateToWithSelectedObject(PageEnum.ServerSettingsConfirmation, true, siteSettings);
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private async Task ExecuteSelectLanguageCommand()
        {
            // await page.ShowAlert("", HACCPUtil.GetResourceString("NotImplemented"));
            // return;

            if (IsBusy)
                return;
            Page.EndEditing();
            IsBusy = true;
            SelectLanguageCommand.ChangeCanExecute();
            try
            {
                var response = await service.DownloadLanguages();
                LanguageList = (ObservableCollection<Language>) response.Results;

                if (LanguageList != null && LanguageList.Count > 0)
                {
                    IsLanguageListVisible = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    "Ooops! Something went wrong while Executing the SelectLanguageCommand. Exception: {0}", ex);
            }
            finally
            {
                IsBusy = false;
                SelectLanguageCommand.ChangeCanExecute();
            }
        }


        /// <summary>
        ///     Tests the connection.
        /// </summary>
        /// <returns>The connection.</returns>
        /// <param name="shouldShowSuccessMessage">If set to <c>true</c> should show success message.</param>
        private async Task<bool> TestConnection(bool shouldShowSuccessMessage)
        {
            var isConnected = false;
            if (serverAddress == null || serverAddress.Trim().Length < 1)
            {
                Page.DisplayAlertMessage("", HACCPUtil.GetResourceString("EnterServerIPAddressorDomainName"));
                return false;
            }

            IHACCPService haccpService = new HACCPService(dataStore);
            if (haccpService.IsConnected())
            {
                //string webUrl = HACCPUtil.BuildServerEndpoint (serverAddress, port, serverDirectory);

                if (HaccpAppSettings.SharedInstance.IsWindows)
                    isConnected = await haccpService.IsWsdlAccess(serverAddress, port, serverDirectory);
                else
                    isConnected = await haccpService.IsRemoteReachable(serverAddress, port);
                if (isConnected == false)
                    Page.DisplayAlertMessage(HACCPUtil.GetResourceString("ConnectionTimeout"),
                        HACCPUtil.GetResourceString(
                            "UnabletoconnecttotheserverPleaseverifyconnectionsettingsandtryagain"));
                else
                {
                    isConnected = await haccpService.IsWsdlAccess(serverAddress, port, serverDirectory);

                    if (isConnected == false)
                        Page.DisplayAlertMessage(HACCPUtil.GetResourceString("NoWebService"),
                            HACCPUtil.GetResourceString(
                                "UnabletofindthewebservicePleaseverifyconnectionsettingsandtryagain"));
                    else if (shouldShowSuccessMessage)
                        Page.DisplayAlertMessage(HACCPUtil.GetResourceString("ConnectionSuccessful"),
                            HACCPUtil.GetResourceString(
                                "Theconnectionwiththeserverandthewebservicehasbeenestablishedsuccessfully"));
                }
            }
            else
                Page.DisplayAlertMessage(HACCPUtil.GetResourceString("EnableNetworkConnection"),
                    HACCPUtil.GetResourceString(
                        "YourequireanactiveInternetconnectiontoperformsomefunctionsWerecommendthatyouenableWiFiforthispurposeDatachargesmayapplyifWiFiisnotenabled"));

            return isConnected;
        }


        /// <summary>
        ///     Executes the test connection command.
        /// </summary>
        /// <returns>The test connection command.</returns>
        private async Task ExecuteTestConnectionCommand()
        {
            if (IsBusy)
                return;

            Page.EndEditing();
            IsBusy = true;
            Page.ShowProgressIndicator();
            TestConnectionCommand.ChangeCanExecute();
            try
            {
                await TestConnection(true);
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Page.DismissPopup();
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
        ///     Selects the language and download strings.
        /// </summary>
        /// <returns>The language and download strings.</returns>
        /// <param name="selectedLanguage">Selected language.</param>
        public async Task SelectLanguageAndDownloadStrings(Language selectedLanguage)
        {
            if (IsBusy)
                return;
            IsBusy = true;


            try
            {
                if (selectedLanguage.LanguageId == HaccpAppSettings.SharedInstance.LanguageId)
                {
                    IsBusy = false;
                    return;
                }

                if (
                    await
                        Page.ShowConfirmAlert(string.Empty,
                            string.Format(
                                HACCPUtil.GetResourceString("Areyousureyouwanttoswitchthelanguageofthisappto0"),
                                selectedLanguage.LanguageName)))
                {
                    IsBusy = true;
                    Page.ShowProgressIndicator();
                    var response = await service.SaveLanguageStrings(selectedLanguage.LanguageId);

                    if (response != null && response.IsSuccess)
                    {
                        foreach (var lng in LanguageList)
                        {
                            lng.IsSelected = lng.LanguageId == selectedLanguage.LanguageId;
                        }

                        HaccpAppSettings.SharedInstance.LanguageId = selectedLanguage.LanguageId;
                        Settings.CurrentLanguageID = selectedLanguage.LanguageId;
                        try
                        {
                            await Task.Delay(5000);
                            //HACCPAppSettings.SharedInstance.ResourceString =  DependencyService.Get<IResourceFileHelper> ().LoadResource ("ResourceFile.xml");


                            if (HaccpAppSettings.SharedInstance.IsWindows)

                                await Task.Run(
                                    async () =>
                                    {
                                        HaccpAppSettings.SharedInstance.ResourceString =
                                            await
                                                DependencyService.Get<IResourceFileHelper>()
                                                    .LoadResourceAsync("ResourceFile.xml");
                                    });

                            else
                                try
                                {
                                    HaccpAppSettings.SharedInstance.ResourceString =
                                        DependencyService.Get<IResourceFileHelper>().LoadResource("ResourceFile.xml");
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.Message);
                                }
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                        IsLanguageListVisible = false;
                        if (!string.IsNullOrEmpty(HaccpAppSettings.SharedInstance.SiteSettings.ServerAddress))
                        {
                            //if (HaccpAppSettings.SharedInstance.IsWindows)
                            //{
                            //    var style = Application.Current.Resources["WindowsListScrollHelpGridStyle"] as Style;
                            //    if (style != null)
                            //    {
                            //        style.Setters.RemoveAt(0);
                            //        style.Setters.Insert(0,
                            //            new Setter {Property = VisualElement.HeightRequestProperty, Value = 90});
                            //    }

                            //    var recordstyle = Application.Current.Resources["WindowsScrollHelpGridStyle"] as Style;
                            //    if (recordstyle != null)
                            //    {
                            //        recordstyle.Setters.RemoveAt(0);
                            //        recordstyle.Setters.Insert(0,
                            //            new Setter {Property = VisualElement.HeightRequestProperty, Value = 85});
                            //    }
                            //    // related to scroll issue fix after changing language
                            //}
                            Page.UnSubscribeMessage();
							HaccpAppSettings.SharedInstance.IsLanguageChanged = true;
                            Page.LoadHomePage();
                        }
                        else
                            BindeAllLablelProperties();

                        IsBusy = false;
                        Page.DismissPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Page.DismissPopup();

                Debug.WriteLine("Ooops! Something went wrong ShowSelectMenuChecklistAlert. Exception: {0}", ex);
            }
            finally
            {
                IsBusy = false;
                Page.DismissPopup();
                IsLanguageListVisible = false;
                if (HaccpAppSettings.SharedInstance.IsWindows)
                {
                    BindeAllLablelProperties();
                }
            }
        }

        #endregion
    }
}