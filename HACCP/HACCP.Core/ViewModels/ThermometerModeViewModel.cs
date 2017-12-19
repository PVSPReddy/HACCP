using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HACCP.Core
{
    public class ThermometerModeViewModel : BaseViewModel
    {
        private double _currentTemperature;
        private string _currentTemperatureString;
        private string appVersion;
        private int batteryLevel;
        private Style blue2ButtonStyle;
        private Command blue2Command;
        private int blue2TempFontSize = 32;
        private readonly string btnDeSelectImage = "yellowBtnDeselect.png";
        private readonly string btnSelectImage = "yellowBtn.png";
        private string celciusButtonBackground = "yellowBtnDeselect.png";
        private Color celciusButtonForegroundColor = Color.FromHex("#FDDB00");
        private string connectionStatus;
        private bool enableControls;
        private string fahrenheitButtonBackground = "yellowBtnDeselect.png";
        private Color fahrenheitButtonForegroundColor = Color.FromHex("#FDDB00");
        private bool isAlertActive;
        private bool isUnitButtonVisible;
        private LineBreakMode lineBreakMode;
        private double prevtemp;
        private bool probDescriptionVisibility;
        private double rotation;
        private bool showBluetoothIcon;
        private TemperatureUnit temperatureUnit = TemperatureUnit.Fahrenheit;
        private Command toggleTemperatureUnitCommand;
        private Command wakeProbeCommand;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HACCP.Core.ThermometerModeViewModel" /> class.
        /// </summary>
        /// <param name="page">Page.</param>
        public ThermometerModeViewModel(IPage page)
            : base(page)
        {
            AppVerison = string.Format("{0}: {1}", HACCPUtil.GetResourceString("VersionLabel"),
                DependencyService.Get<IInfoService>().GetAppVersion());


            MessagingCenter.Subscribe<WindowsScanningStatusMessage>(this, HaccpConstant.WindowsScanningStatus, sender =>
            {
                if (sender.IsScanning && ConnectionStatus != HACCPUtil.GetResourceString("ConnectionStatusDisconnect"))
                    ConnectionStatus = HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("Connecting"), false);
                else
                    ConnectionStatus =
                        HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("ConnectionStatusScanBlue2"), false);
            });

            MessagingCenter.Subscribe<WindowsBleConnectionStatusMessage>(this, HaccpConstant.BleconnectionStatus, sender =>
                {
                    OnPropertyChanged("IsWakeButtonVisible");

                    if (!WindowsBLEManager.SharedInstance.HasAnyPairedDevice)
                    {
                        EnableControls = false;
                        ConnectionStatus =
                            HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("ConnectionStatusScanBlue2"),
                                false);
                        Blue2ButtonStyle = Application.Current.Resources["Blue2ButtonNotActiveStyle"] as Style;
                        CurrentTemperatureFormatted = HACCPUtil.GetResourceString("ConnectBlue2Label");
                        TemperatureUnit = TemperatureUnit.Fahrenheit;
                        Blue2TempFontSize = 22;
                        LineBreakMode = LineBreakMode.TailTruncation;
                        MessagingCenter.Send(new Blue2PlaceHolderVisibility(true),
                            HaccpConstant.Blue2PlaceholderVisibility);
                    }

                    OnPropertyChanged("CustomProbDescription");
                });


            MessagingCenter.Subscribe<BleConnectionStatusMessage>(this, HaccpConstant.BleconnectionStatus,
                async sender =>
                {
                    OnPropertyChanged("IsWakeButtonVisible");
                    if (!BLEManager.SharedInstance.Adapter.IsBluetoothEnabled)
                    {
                        // bluetooth in phone is disabled
                        ConnectionStatus =
                            HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("ConnectionStatusScanBlue2"),
                                false);
                        Blue2ButtonStyle = Application.Current.Resources["Blue2ButtonNotActiveStyle"] as Style;
                        CurrentTemperatureFormatted = HACCPUtil.GetResourceString("ConnectBlue2Label");
                        Blue2TempFontSize = 22;
                        LineBreakMode = LineBreakMode.TailTruncation;
                        EnableControls = false;
                        if (!isAlertActive)
                        {
                            isAlertActive = true;
                            await
                                page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("TurnONBluetoothonyourdevice"));
                            isAlertActive = false;
                        }
                    }
                    else if (!BLEManager.SharedInstance.IsConnected)
                    {
                        // connection to the blue2 device is disabled										
                        CurrentTemperatureFormatted = HACCPUtil.GetResourceString("ConnectBlue2Label");
                        Blue2TempFontSize = 22;
                        LineBreakMode = LineBreakMode.TailTruncation;
                        ConnectionStatus =
                            HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("ConnectionStatusScanBlue2"),
                                false);
                        Blue2ButtonStyle = Application.Current.Resources["Blue2ButtonNotActiveStyle"] as Style;
                        EnableControls = false;
                    }
                    else if (BLEManager.SharedInstance.IsConnected)
                    {
                        CurrentTemperatureFormatted = string.Empty;
                        ConnectionStatus = HACCPUtil.GetResourceString("ConnectionStatusDisconnect");
                        Blue2ButtonStyle = Application.Current.Resources["Blue2ButtonActiveStyle"] as Style;
                    }
                    OnPropertyChanged("CustomProbDescription");
                });

            MessagingCenter.Subscribe<BLEBlue2SettingsUpdatedMessage>(this, HaccpConstant.Bleblue2SettingsUpdate,
                sender =>
                {
                    if (HaccpAppSettings.SharedInstance.IsWindows)
                        LoadWindowsBlue2Settings();
                    else if (BLEManager.SharedInstance.Settings != null)
                        LoadBlue2Settings(false);
                });

            MessagingCenter.Subscribe<BleConnectionTimeOutMessage>(this, HaccpConstant.BleconnectionTimeout,
                async sender =>
                {
                    await page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("Blue2TurnOnMessage"));
                });


            MessagingCenter.Subscribe<BleScanCompleteMessage>(this, HaccpConstant.Blue2ScanComplete,
                sender =>
                {
                    ConnectionStatus =
                        HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("ConnectionStatusScanBlue2"), false);
                });
        }

        #region Properties

        /// <summary>
        ///     Gets or sets the fahrenheit button background.
        /// </summary>
        /// <value>The fahrenheit button background.</value>
        public string FahrenheitButtonBackground
        {
            get { return fahrenheitButtonBackground; }
            set { SetProperty(ref fahrenheitButtonBackground, value); }
        }

        /// <summary>
        ///     Gets or sets the celcius button background.
        /// </summary>
        /// <value>The celcius button background.</value>
        public string CelciusButtonBackground
        {
            get { return celciusButtonBackground; }
            set { SetProperty(ref celciusButtonBackground, value); }
        }

        /// <summary>
        ///     Gets or sets the color of the fahrenheit button foreground.
        /// </summary>
        /// <value>The color of the fahrenheit button foreground.</value>
        public Color FahrenheitButtonForegroundColor
        {
            get { return fahrenheitButtonForegroundColor; }
            set { SetProperty(ref fahrenheitButtonForegroundColor, value); }
        }

        /// <summary>
        ///     Gets or sets the color of the celcius button foreground.
        /// </summary>
        /// <value>The color of the celcius button foreground.</value>
        public Color CelciusButtonForegroundColor
        {
            get { return celciusButtonForegroundColor; }
            set { SetProperty(ref celciusButtonForegroundColor, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="HACCP.Core.ThermometerModeViewModel" /> prob description
        ///     visibility.
        /// </summary>
        /// <value><c>true</c> if prob description visibility; otherwise, <c>false</c>.</value>
        public bool ProbDescriptionVisibility
        {
            get { return probDescriptionVisibility; }
            set { SetProperty(ref probDescriptionVisibility, value); }
        }

        /// <summary>
        ///     Gets or sets the size of the blue2 temp font.
        /// </summary>
        /// <value>The size of the blue2 temp font.</value>
        public int Blue2TempFontSize
        {
            get { return blue2TempFontSize; }
            set { SetProperty(ref blue2TempFontSize, value); }
        }

        /// <summary>
        ///     Gets or sets the temperature unit.
        /// </summary>
        /// <value>The temperature unit.</value>
        public TemperatureUnit TemperatureUnit
        {
            get { return temperatureUnit; }
            set
            {
                temperatureUnit = value;
                if (temperatureUnit == TemperatureUnit.Fahrenheit)
                {
                    FahrenheitButtonBackground = btnSelectImage;
                    CelciusButtonBackground = btnDeSelectImage;
                    FahrenheitButtonForegroundColor = Color.Black;
                    CelciusButtonForegroundColor = Color.FromHex("#FDDB00");
                }
                else
                {
                    CelciusButtonBackground = btnSelectImage;
                    FahrenheitButtonBackground = btnDeSelectImage;
                    CelciusButtonForegroundColor = Color.Black;
                    FahrenheitButtonForegroundColor = Color.FromHex("#FDDB00");
                }
            }
        }

        /// <summary>
        ///     Gets the current date time.
        /// </summary>
        /// <value>The current date time.</value>
        public string CurrentDateTime
        {
            get { return HACCPUtil.GetFormattedDate(DateTime.Now.ToString()); }
        }

        /// <summary>
        ///     Gets the current date time.
        /// </summary>
        /// <value>The current date time.</value>
        public string CurrentDay
        {
            get { return DateTime.Now.ToString("dddd", CultureInfo.CurrentCulture); }
        }

        /// <summary>
        ///     Gets or sets the current temperature.
        /// </summary>
        /// <value>The current temperature.</value>
        public string CurrentTemperatureFormatted
        {
            get { return _currentTemperatureString; }
            set { SetProperty(ref _currentTemperatureString, value); }
        }

        /// <summary>
        ///     Gets or sets the current temperature.
        /// </summary>
        /// <value>The current temperature.</value>
        public double CurrentTemperature
        {
            get { return _currentTemperature; }
            set
            {
                SetProperty(ref _currentTemperature, value);
                CurrentTemperatureFormatted = TemperatureUnit == TemperatureUnit.Fahrenheit
                    ? value.ToString("0.0° F")
                    : value.ToString("0.0° C");
            }
        }


        /// <summary>
        ///     Gets or sets the app verison.
        /// </summary>
        /// <value>The app verison.</value>
        public string AppVerison
        {
            get { return appVersion; }
            set { SetProperty(ref appVersion, value); }
        }


        //BatteryLevelConverter
        /// <summary>
        ///     Gets or sets the battery level.
        /// </summary>
        /// <value>The battery level.</value>
        public int BatteryLevel
        {
            get { return batteryLevel; }
            set { SetProperty(ref batteryLevel, value); }
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
                        : HACCPUtil.GetResourceString("ProbeDescriptionTitle");
                return BLEManager.SharedInstance.Settings != null &&
                       !string.IsNullOrEmpty(BLEManager.SharedInstance.Settings.Prob)
                    ? BLEManager.SharedInstance.Settings.Prob
                    : HACCPUtil.GetResourceString("ProbeDescriptionTitle");
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="HACCP.Core.ThermometerModeViewModel" /> show bluetooth
        ///     icon.
        /// </summary>
        /// <value><c>true</c> if show bluetooth icon; otherwise, <c>false</c>.</value>
        public bool ShowBluetoothIcon
        {
            get { return showBluetoothIcon; }
            set { SetProperty(ref showBluetoothIcon, value); }
        }

        /// <summary>
        ///     Gets or sets the rotation.
        /// </summary>
        /// <value>The rotation.</value>
        public double Rotation
        {
            get { return rotation; }
            set { SetProperty(ref rotation, value); }
        }


        /// <summary>
        ///     Gets or sets a value indicating whether this instance is unit button visible.
        /// </summary>
        /// <value><c>true</c> if this instance is unit button visible; otherwise, <c>false</c>.</value>
        public bool IsUnitButtonVisible
        {
            get { return isUnitButtonVisible; }
            set { SetProperty(ref isUnitButtonVisible, value); }
        }


        /// <summary>
        ///     Gets or sets the connection status.
        /// </summary>
        /// <value>The connection status.</value>
        public string ConnectionStatus
        {
            get { return connectionStatus; }
            set { SetProperty(ref connectionStatus, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="HACCP.Core.ThermometerModeViewModel" /> enable controls.
        /// </summary>
        /// <value><c>true</c> if enable controls; otherwise, <c>false</c>.</value>
        public bool EnableControls
        {
            get { return enableControls; }
            set { SetProperty(ref enableControls, value); }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is wake button visible.
        /// </summary>
        /// <value><c>true</c> if this instance is wake button visible; otherwise, <c>false</c>.</value>
        public bool IsWakeButtonVisible
        {
            get
            {
                if (HaccpAppSettings.SharedInstance.IsWindows)
                    return WindowsBLEManager.SharedInstance.IsSleeping;

                return BLEManager.SharedInstance.IsSleeping;
            }
        }


        /// <summary>
        ///     Gets or sets the blue2 button style.
        /// </summary>
        /// <value>The blue2 button style.</value>
        public Style Blue2ButtonStyle
        {
            get { return blue2ButtonStyle; }
            set { SetProperty(ref blue2ButtonStyle, value); }
        }


        /// <summary>
        ///     Gets the width of the battery panel.
        /// </summary>
        /// <value>The width of the battery panel.</value>
        public double BatteryPanelWidth
        {
            get
            {
                if (Device.Idiom == TargetIdiom.Tablet)
                    return 100;
                return 60;
            }
        }

        /// <summary>
        ///     Gets or sets the line break mode.
        /// </summary>
        /// <value>The line break mode.</value>
        public LineBreakMode LineBreakMode
        {
            get { return lineBreakMode; }
            set { SetProperty(ref lineBreakMode, value); }
        }

        #endregion

        #region Commands

        /// <summary>
        ///     Gets the blue2 command.
        /// </summary>
        /// <value>The blue2 command.</value>
        public Command Blue2Command
        {
            get
            {
                return blue2Command ??
                       (blue2Command = new Command(ExecuteBlue2Command));
            }
        }

        /// <summary>
        ///     Gets the toggle temperature entry command.
        /// </summary>
        /// <value>The toggle temperature entry co}mmand.</value>
        public Command ToggleTemperatureUnitCommand
        {
            get
            {
                return toggleTemperatureUnitCommand ??
                       (toggleTemperatureUnitCommand = new Command<string>(parameter =>
                       {
                           if (IsBusy)
                               return;

                           if (HaccpAppSettings.SharedInstance.IsWindows &&
                               (WindowsBLEManager.SharedInstance.IsSleeping || WindowsBLEManager.SharedInstance.IsHigh ||
                                WindowsBLEManager.SharedInstance.IsLow))
                               return;

                           if (!HaccpAppSettings.SharedInstance.IsWindows &&
                               (BLEManager.SharedInstance.IsSleeping || BLEManager.SharedInstance.IsHigh ||
                                BLEManager.SharedInstance.IsLow))
                           {
                               return;
                           }

                           ToggleTemperatureUnitCommand.ChangeCanExecute();

                           if (parameter == "Celcius" && TemperatureUnit != TemperatureUnit.Celcius)
                           {
                               TemperatureUnit = TemperatureUnit.Celcius;
                               CurrentTemperature = HACCPUtil.ConvertFahrenheitToCelsius(CurrentTemperature);
                           }
                           else if (parameter == "Fahrenheit" && TemperatureUnit != TemperatureUnit.Fahrenheit)
                           {
                               TemperatureUnit = TemperatureUnit.Fahrenheit;
                               CurrentTemperature = HACCPUtil.ConvertCelsiusToFahrenheit(CurrentTemperature);
                           }
                       }));
            }
        }


        /// <summary>
        ///     Called when the view disappears.
        /// </summary>
        public override void OnViewDisappearing()
        {
            base.OnViewDisappearing();
            MessagingCenter.Unsubscribe<BleTemperatureReadingMessage>(this, HaccpConstant.BletemperatureReading);
            MessagingCenter.Unsubscribe<BleConnectionStatusMessage>(this, HaccpConstant.BleconnectionStatus);
            MessagingCenter.Unsubscribe<BLEBlue2SettingsUpdatedMessage>(this, HaccpConstant.Bleblue2SettingsUpdate);
            MessagingCenter.Unsubscribe<BleConnectionTimeOutMessage>(this, HaccpConstant.BleconnectionTimeout);
            MessagingCenter.Unsubscribe<BleScanCompleteMessage>(this, HaccpConstant.Blue2ScanComplete);
            MessagingCenter.Unsubscribe<WindowsBleConnectionStatusMessage>(this, HaccpConstant.BleconnectionStatus);
            MessagingCenter.Unsubscribe<WindowsScanningStatusMessage>(this, HaccpConstant.WindowsScanningStatus);
        }


        public override void OnViewAppearing()
        {
            base.OnViewAppearing();
            ShowBluetoothIcon = false;

            if (HaccpAppSettings.SharedInstance.IsWindows)
                LoadWindowsBlue2Settings();
            else
                LoadBlue2Settings(true);


            MessagingCenter.Subscribe<BleTemperatureReadingMessage>(this, HaccpConstant.BletemperatureReading, sender =>
            {
                OnPropertyChanged("IsWakeButtonVisible");

                bool connected;

                connected = HaccpAppSettings.SharedInstance.IsWindows ? WindowsBLEManager.SharedInstance.HasAnyPairedDevice : BLEManager.SharedInstance.IsConnected;

                if (connected)
                {
                    var msg = sender;
                    if (msg != null)
                    {
                        bool isbatteryLow;
                        double lastReading;
                        if (HaccpAppSettings.SharedInstance.IsWindows)
                        {
                            isbatteryLow = WindowsBLEManager.SharedInstance.IsBatteryLow;
                            lastReading = WindowsBLEManager.SharedInstance.LastReading;
                        }
                        else
                        {
                            isbatteryLow = BLEManager.SharedInstance.IsBatteryLow;
                            lastReading = BLEManager.SharedInstance.LastReading;
                        }

                        if (msg.IsSleeping)
                        {
                            CurrentTemperatureFormatted = HACCPUtil.GetResourceString("Blue2SleepString");
                            Blue2TempFontSize = 22;
                            LineBreakMode = LineBreakMode.TailTruncation;
                        }
                        else if (msg.IsHigh)
                        {
                            CurrentTemperatureFormatted = HACCPUtil.GetResourceString("Blue2HighString");
                            Blue2TempFontSize = 22;
                            LineBreakMode = LineBreakMode.TailTruncation;
                        }
                        else if (msg.IsLow)
                        {
                            CurrentTemperatureFormatted = HACCPUtil.GetResourceString("Blue2LowString");
                            Blue2TempFontSize = 22;
                            LineBreakMode = LineBreakMode.TailTruncation;
                        }
                        else if (isbatteryLow)
                        {
                            CurrentTemperatureFormatted = HACCPUtil.GetResourceString("Blue2BatteryString");
                            Blue2TempFontSize = 22;
                            LineBreakMode = LineBreakMode.TailTruncation;
                        }
                        else
                        {
                            DisplayTemperatureReading(lastReading);
                        }
                    }
                }
            });
        }


        // <summary>
        /// Executes the blue2 command.
        /// 
        private async void ExecuteBlue2Command()
        {
            if (HaccpAppSettings.SharedInstance.IsWindows)
            {
                WindowsBlue2Command();
                return;
            }

            if (!BLEManager.SharedInstance.Adapter.IsBluetoothEnabled)
            {
                await Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("TurnONBluetoothonyourdevice"));
                return;
            }

            if (BLEManager.SharedInstance.ReadingBlue2Data)
                return;

            Blue2Command.ChangeCanExecute();

            bool gotPermission = await getLocationPermission();
            if (gotPermission)
            {
                BLEManager.SharedInstance.ResetConnection();

                if (!BLEManager.SharedInstance.ScanTimeOutElapsed)
                {
                    ConnectionStatus =
                        HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("ConnectionStatusScanning"), false);
                }
                else if (!BLEManager.SharedInstance.IsConnected)
                {
                    ConnectionStatus =
                        HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("ConnectionStatusScanBlue2"), false);
                }
            }  

            Blue2Command.ChangeCanExecute();
        }


        public void WindowsBlue2Command()
        {
            if (WindowsBLEManager.SharedInstance.HasAnyPairedDevice)
                WindowsBLEManager.SharedInstance.DisConnectDevice();
            else
                WindowsBLEManager.SharedInstance.NavigateBluetoothSettingsPage();
            // await Launcher.LaunchUriAsync(new Uri("ms-settings-bluetooth:"));
        }


        /// <summary>
        ///     Gets the home command.
        /// </summary>
        /// <value>The home command.</value>
        public Command WakeProbeCommand
        {
            get
            {
                return wakeProbeCommand ?? (wakeProbeCommand = new Command(async() =>
                {
                    if (HaccpAppSettings.SharedInstance.IsWindows)
                        await WindowsBLEManager.SharedInstance.ResetBlue2AutoOff();
                    else
                        BLEManager.SharedInstance.ResetBlue2AutoOff();
                    OnPropertyChanged("IsWakeButtonVisible");
                }));
            }
        }

        /// <summary>
        ///     Loads the blue2 settings.
        /// </summary>
        public async void LoadBlue2Settings(bool tempReady)
        {
            if (BLEManager.SharedInstance.IsConnected)
            {
                EnableControls = true;
                ConnectionStatus = HACCPUtil.GetResourceString("ConnectionStatusDisconnect");
                Blue2ButtonStyle = Application.Current.Resources["Blue2ButtonActiveStyle"] as Style;
                if (BLEManager.SharedInstance.Settings != null)
                {
                    var settings = BLEManager.SharedInstance.Settings;

                    BatteryLevel = settings.BatteryLevel;
                    ShowBluetoothIcon = true;

                    TemperatureUnit = settings.Scale == 0 ? TemperatureUnit.Celcius : TemperatureUnit.Fahrenheit;

                    if (tempReady)
                    {
                        if (BLEManager.SharedInstance.IsSleeping)
                        {
                            CurrentTemperatureFormatted = HACCPUtil.GetResourceString("Blue2SleepString");
                            Blue2TempFontSize = 22;
                            LineBreakMode = LineBreakMode.TailTruncation;
                        }
                        else if (BLEManager.SharedInstance.IsHigh)
                        {
                            CurrentTemperatureFormatted = HACCPUtil.GetResourceString("Blue2HighString");
                            Blue2TempFontSize = 22;
                            LineBreakMode = LineBreakMode.TailTruncation;
                        }
                        else if (BLEManager.SharedInstance.IsLow)
                        {
                            CurrentTemperatureFormatted = HACCPUtil.GetResourceString("Blue2LowString");
                            Blue2TempFontSize = 22;
                            LineBreakMode = LineBreakMode.TailTruncation;
                        }
                        else if (BLEManager.SharedInstance.IsBatteryLow)
                        {
                            CurrentTemperatureFormatted = HACCPUtil.GetResourceString("Blue2BatteryString");
                            Blue2TempFontSize = 22;
                            LineBreakMode = LineBreakMode.TailTruncation;
                        }
                        else
                            DisplayTemperatureReading(BLEManager.SharedInstance.LastReading);
                    }
                }
            }
            else
            {
                EnableControls = false;

                PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Location);
                if (status == PermissionStatus.Granted)
                {
                    ConnectionStatus = BLEManager.SharedInstance.ScanTimeOutElapsed
                    ? HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("ConnectionStatusScanBlue2"), false)
                    : HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("ConnectionStatusScanning"), false);
                }
                else
                {
                    ConnectionStatus = HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("ConnectionStatusScanBlue2"), false);
                    BLEManager.SharedInstance.Adapter.CancelScanning();
                    BLEManager.SharedInstance.ScanTimeOutElapsed = true;
                }

                Blue2ButtonStyle = Application.Current.Resources["Blue2ButtonNotActiveStyle"] as Style;
                ShowBluetoothIcon = false;
                CurrentTemperatureFormatted = HACCPUtil.GetResourceString("ConnectBlue2Label");
                Blue2TempFontSize = 22;
                LineBreakMode = LineBreakMode.TailTruncation;

                if (!BLEManager.SharedInstance.Adapter.IsBluetoothEnabled)
                {
                    // bluetooth in phone is disabled					
                    await Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("TurnONBluetoothonyourdevice"));
                }
                else if (BLEManager.SharedInstance.Adapter.IsScanning)
                {
                    // connection to the blue2 device is disabled					
                    await Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("Blue2TurnOnMessage"));
                }
            }

            OnPropertyChanged("CustomProbDescription");
        }


        /// <summary>
        ///     Method for loading ble settings for windows
        /// </summary>
        public void LoadWindowsBlue2Settings()
        {
            if (WindowsBLEManager.SharedInstance.HasAnyPairedDevice)
            {
                EnableControls = true;
                ConnectionStatus = HACCPUtil.GetResourceString("ConnectionStatusDisconnect");
                Blue2ButtonStyle = Application.Current.Resources["Blue2ButtonActiveStyle"] as Style;

                if (WindowsBLEManager.SharedInstance.Settings != null)
                {
                    var settings = WindowsBLEManager.SharedInstance.Settings;

                    BatteryLevel = settings.BatteryLevel;
                    ShowBluetoothIcon = true;

                    TemperatureUnit = settings.Scale == 0 ? TemperatureUnit.Celcius : TemperatureUnit.Fahrenheit;

                    if (WindowsBLEManager.SharedInstance.IsSleeping)
                    {
                        CurrentTemperatureFormatted = HACCPUtil.GetResourceString("Blue2SleepString");
                        Blue2TempFontSize = 22;
                        LineBreakMode = LineBreakMode.TailTruncation;
                    }
                    else if (WindowsBLEManager.SharedInstance.IsHigh)
                    {
                        CurrentTemperatureFormatted = HACCPUtil.GetResourceString("Blue2HighString");
                        Blue2TempFontSize = 22;
                        LineBreakMode = LineBreakMode.TailTruncation;
                    }
                    else if (WindowsBLEManager.SharedInstance.IsLow)
                    {
                        CurrentTemperatureFormatted = HACCPUtil.GetResourceString("Blue2LowString");
                        Blue2TempFontSize = 22;
                        LineBreakMode = LineBreakMode.TailTruncation;
                    }
                    else if (WindowsBLEManager.SharedInstance.IsBatteryLow)
                    {
                        CurrentTemperatureFormatted = HACCPUtil.GetResourceString("Blue2BatteryString");
                        Blue2TempFontSize = 22;
                        LineBreakMode = LineBreakMode.TailTruncation;
                    }
                    else
                        DisplayTemperatureReading(WindowsBLEManager.SharedInstance.LastReading);
                }
            }
            else
            {
                EnableControls = false;
                ConnectionStatus =
                    HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("ConnectionStatusScanBlue2"), false);
                Blue2ButtonStyle = Application.Current.Resources["Blue2ButtonNotActiveStyle"] as Style;
                CurrentTemperatureFormatted = HACCPUtil.GetResourceString("ConnectBlue2Label");
                Blue2TempFontSize = 22;
                LineBreakMode = LineBreakMode.TailTruncation;
                //if (!BLEManager.SharedInstance.adapter.IsBluetoothEnabled)
                //{              // bluetooth in phone is disabled

                //   CustomProbDescription = string.Empty;
                //    MessagingCenter.Send<Blue2PlaceHolderVisibility>(new Blue2PlaceHolderVisibility(true), HACCPConstant.BLUE2_PLACEHOLDER_VISIBILITY);
                //     await page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("TurnONBluetoothonyourdevice"));
                //  }
                //  else
                //   {                                  // connection to the blue2 device is disabled


                // CustomProbDescription = string.Empty;


                MessagingCenter.Send(new Blue2PlaceHolderVisibility(true), HaccpConstant.Blue2PlaceholderVisibility);
                //if (BLEManager.SharedInstance.adapter.IsScanning)
                //  await page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("Blue2TurnOnMessage"));
                //   }
            }

            OnPropertyChanged("CustomProbDescription");
        }

        /// <summary>
        ///     Displaies the temperature reading.
        /// </summary>
        /// <param name="temperature">Temperature.</param>
        private void DisplayTemperatureReading(double temperature)
        {
            var diff = Math.Round(temperature - prevtemp, 1);
            if (diff > 2.8)
                diff = 2.8;
            else if (diff < -2.8)
                diff = -2.8;

            Rotation = HACCPUtil.GetSlope(diff);

            prevtemp = temperature;
            short tempScale;
            if (HaccpAppSettings.SharedInstance.IsWindows)
                tempScale = WindowsBLEManager.SharedInstance.Settings.Scale;
            else
                tempScale = BLEManager.SharedInstance.Settings.Scale;

            if (TemperatureUnit == TemperatureUnit.Celcius && tempScale != 0)
            {
                temperature = HACCPUtil.ConvertFahrenheitToCelsius(temperature);
            }
            else if (TemperatureUnit == TemperatureUnit.Fahrenheit && tempScale != 1)
            {
                temperature = HACCPUtil.ConvertCelsiusToFahrenheit(temperature);
            }
            CurrentTemperature = temperature;
            Blue2TempFontSize = 32;
            LineBreakMode = LineBreakMode.NoWrap;
        }

        private async Task<bool> getLocationPermission()
        {
            if (Device.OS != TargetPlatform.Android)
            {
                return true;
            }
            else
            {
                try 
                {
                    PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Location);
                    if (status != PermissionStatus.Granted)
                    {
                        if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Plugin.Permissions.Abstractions.Permission.Location))
                        {
                            await Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("AndroidrequiresLocationpermissionforBluetoothLEscanning"));  //Android requires \"Location\" permission for Bluetooth LE scanning.                           
                        }

                        var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Plugin.Permissions.Abstractions.Permission.Location });
                        status = results[Plugin.Permissions.Abstractions.Permission.Location];
                    }

                    if (status != PermissionStatus.Granted)
                    {
                        await Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("CannotscanforBlue2withoutLocationpermission")); //Cannot scan for Blue2 without \"Location\" permission.
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    await Page.ShowAlert(string.Empty, ex.ToString());
                    return false;
                }
            }
        }

        #endregion
    }
}