using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HACCP.Core
{
    public class Blue2SettingsViewModel : BaseViewModel
    {
        #region Member Variables

        private readonly string btnDeSelectImage = "yellowBtnDeselect.png";
        private readonly string btnSelectImage = "yellowBtn.png";
        private int _autoOffInterval = 1;
        private Style _blue2ButtonStyle;
        private Command _blue2Command;
        private string _blue2Id;
        private bool _blue2IdVisible;
        private string _celciusButtonBackground;
        private Color _celciusButtonForegroundColor;
        private string _connectionStatus;
        private string _customProbDescription = string.Empty;
        private bool _enableControls;
        private string _fahrenheitButtonBackground;
        private Color _fahrenheitButtonForegroundColor;
        private string _interval = "1 " + HACCPUtil.GetResourceString("Minute");
        private Command _intervalCommand;
        private bool _isAlertActive;
        private bool _isIntervalMinusEnabled = true;
        private bool _isIntervalPlusEnabled = true;
        private bool _isSleepMinusEnabled = true;
        private bool _isSleepPlusEnabled = true;
        private bool _isTimingMinusEnabled = true;
        private bool _isTimingPlusEnabled = true;
        private int _measurementTiming = 1;
        private string _probeDescriptionPlaceHolder;
        private string _sleep = "1 " + HACCPUtil.GetResourceString("Minute");
        private Command _sleepCommand;
        private int _sleepTime = 1;
        private TemperatureUnit _temperatureUnit;
        private string _timing = "1 " + HACCPUtil.GetResourceString("sec");
        private Command _timingCommand;
        private Command _toggleTemperatureUnitCommand;

        #endregion

        /// <summary>
        /// Blue2Settings ViewModel
        /// </summary>
        /// <param name="page"></param>
        public Blue2SettingsViewModel(IPage page)
            : base(page)
        {         
            _probeDescriptionPlaceHolder = HACCPUtil.GetResourceString("EnterCustomProbeDescription");
            TemperatureUnit = TemperatureUnit.Fahrenheit;

            MessagingCenter.Subscribe<WindowsScanningStatusMessage>(this, HaccpConstant.WindowsScanningStatus, sender =>
            {
                if (sender.IsScanning && ConnectionStatus != HACCPUtil.GetResourceString("ConnectionStatusDisconnect"))
                    ConnectionStatus = HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("Connecting"), false);
                else
                    ConnectionStatus =
                        HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("ConnectionStatusScanBlue2"), false);
            });

            MessagingCenter.Subscribe<WindowsBleConnectionStatusMessage>(this, HaccpConstant.BleconnectionStatus,
                sender =>
                {
                    if (!WindowsBLEManager.SharedInstance.HasAnyPairedDevice)
                    {
                        Blue2Id = string.Empty;
                        Blue2IdVisible = false;
                        EnableControls = false;
                        CustomProbDescription = string.Empty;
                        ConnectionStatus =
                            HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("ConnectionStatusScanBlue2"),
                                false);
                        Blue2ButtonStyle = Application.Current.Resources["Blue2ButtonNotActiveStyle"] as Style;
                        IsTimingPlusEnabled =
                            IsTimingMinusEnabled =
                                IsIntervalPlusEnabled =
                                    IsIntervalMinusEnabled = IsSleepPlusEnabled = IsSleepMinusEnabled = false;
                        TemperatureUnit = TemperatureUnit.Fahrenheit;
                        MessagingCenter.Send(new Blue2PlaceHolderVisibility(true),
                            HaccpConstant.Blue2PlaceholderVisibility);
                    }
                });


            MessagingCenter.Subscribe<BleConnectionStatusMessage>(this, HaccpConstant.BleconnectionStatus,
                async sender =>
                {
                    if (!BLEManager.SharedInstance.Adapter.IsBluetoothEnabled)
                    {
                        // bluetooth in phone is disabled
                        Blue2IdVisible = false;
                        Blue2Id = string.Empty;
                        EnableControls = false;
                        CustomProbDescription = string.Empty;
                        ConnectionStatus =
                            HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("ConnectionStatusScanBlue2"),
                                false);
                        Blue2ButtonStyle = Application.Current.Resources["Blue2ButtonNotActiveStyle"] as Style;
                        IsTimingPlusEnabled =
                            IsTimingMinusEnabled =
                                IsIntervalPlusEnabled =
                                    IsIntervalMinusEnabled = IsSleepPlusEnabled = IsSleepMinusEnabled = false;
                        TemperatureUnit = TemperatureUnit.Fahrenheit;
                        MessagingCenter.Send(new Blue2PlaceHolderVisibility(true),
                            HaccpConstant.Blue2PlaceholderVisibility);
                        if (!_isAlertActive)
                        {
                            _isAlertActive = true;
                            await
                                page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("TurnONBluetoothonyourdevice"));
                            _isAlertActive = false;
                        }
                    }
                    else if (!BLEManager.SharedInstance.IsConnected)
                    {
                        // connection to the blue2 device is disabled
                        Blue2Id = string.Empty;
                        Blue2IdVisible = false;
                        EnableControls = false;
                        CustomProbDescription = string.Empty;
                        ConnectionStatus =
                            HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("ConnectionStatusScanBlue2"),
                                false);
                        Blue2ButtonStyle = Application.Current.Resources["Blue2ButtonNotActiveStyle"] as Style;
                        IsTimingPlusEnabled =
                            IsTimingMinusEnabled =
                                IsIntervalPlusEnabled =
                                    IsIntervalMinusEnabled = IsSleepPlusEnabled = IsSleepMinusEnabled = false;
                        TemperatureUnit = TemperatureUnit.Fahrenheit;
                        MessagingCenter.Send(new Blue2PlaceHolderVisibility(true),
                            HaccpConstant.Blue2PlaceholderVisibility);
                    }
                    else if (BLEManager.SharedInstance.IsConnected)
                    {
                        ConnectionStatus = HACCPUtil.GetResourceString("ConnectionStatusDisconnect");
                        Blue2ButtonStyle = Application.Current.Resources["Blue2ButtonActiveStyle"] as Style;
                    }
                });

            MessagingCenter.Subscribe<BLEBlue2SettingsUpdatedMessage>(this, HaccpConstant.Bleblue2SettingsUpdate,
                sender =>
                {
                    if (HaccpAppSettings.SharedInstance.IsWindows)
                        LoadWindowsBlue2Settings();
                    else if (BLEManager.SharedInstance.Settings != null)
                        LoadBlue2Settings();
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
        ///     Gets or sets the temperature unit.
        /// </summary>
        /// <value>The temperature unit.</value>
        public TemperatureUnit TemperatureUnit
        {
            get { return _temperatureUnit; }
            set
            {
                _temperatureUnit = value;
                if (_temperatureUnit == TemperatureUnit.Fahrenheit)
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
        ///     Gets or sets the timing.
        /// </summary>
        /// <value>The timing.</value>
        public string Timing
        {
            get { return _timing; }
            set { SetProperty(ref _timing, value); }
        }


        /// <summary>
        ///     Gets or sets the interval.
        /// </summary>
        /// <value>The interval.</value>
        public string Interval
        {
            get { return _interval; }
            set { SetProperty(ref _interval, value); }
        }

        /// <summary>
        ///     Gets or sets the sleep.
        /// </summary>
        /// <value>The sleep.</value>
        public string Sleep
        {
            get { return _sleep; }
            set { SetProperty(ref _sleep, value); }
        }

        /// <summary>
        ///     Gets or sets the fahrenheit button background.
        /// </summary>
        /// <value>The fahrenheit button background.</value>
        public string FahrenheitButtonBackground
        {
            get { return _fahrenheitButtonBackground; }
            set { SetProperty(ref _fahrenheitButtonBackground, value); }
        }

        /// <summary>
        ///     Gets or sets the celcius button background.
        /// </summary>
        /// <value>The celcius button background.</value>
        public string CelciusButtonBackground
        {
            get { return _celciusButtonBackground; }
            set { SetProperty(ref _celciusButtonBackground, value); }
        }

        /// <summary>
        ///     Gets or sets the color of the fahrenheit button foreground.
        /// </summary>
        /// <value>The color of the fahrenheit button foreground.</value>
        public Color FahrenheitButtonForegroundColor
        {
            get { return _fahrenheitButtonForegroundColor; }
            set { SetProperty(ref _fahrenheitButtonForegroundColor, value); }
        }

        /// <summary>
        ///     Gets or sets the color of the celcius button foreground.
        /// </summary>
        /// <value>The color of the celcius button foreground.</value>
        public Color CelciusButtonForegroundColor
        {
            get { return _celciusButtonForegroundColor; }
            set { SetProperty(ref _celciusButtonForegroundColor, value); }
        }


        /// <summary>
        ///     Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
        public bool IsTimingPlusEnabled
        {
            get { return _isTimingPlusEnabled; }
            set { SetProperty(ref _isTimingPlusEnabled, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is timing minus enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is timing minus enabled; otherwise, <c>false</c>.</value>
        public bool IsTimingMinusEnabled
        {
            get { return _isTimingMinusEnabled; }
            set { SetProperty(ref _isTimingMinusEnabled, value); }
        }


        /// <summary>
        ///     Gets or sets a value indicating whether this instance is interval plus enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is interval plus enabled; otherwise, <c>false</c>.</value>
        public bool IsIntervalPlusEnabled
        {
            get { return _isIntervalPlusEnabled; }
            set { SetProperty(ref _isIntervalPlusEnabled, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is interval minus enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is interval minus enabled; otherwise, <c>false</c>.</value>
        public bool IsIntervalMinusEnabled
        {
            get { return _isIntervalMinusEnabled; }
            set { SetProperty(ref _isIntervalMinusEnabled, value); }
        }


        /// <summary>
        ///     Gets or sets a value indicating whether this instance is sleep plus enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is sleep plus enabled; otherwise, <c>false</c>.</value>
        public bool IsSleepPlusEnabled
        {
            get { return _isSleepPlusEnabled; }
            set { SetProperty(ref _isSleepPlusEnabled, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is sleep minus enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is sleep minus enabled; otherwise, <c>false</c>.</value>
        public bool IsSleepMinusEnabled
        {
            get { return _isSleepMinusEnabled; }
            set { SetProperty(ref _isSleepMinusEnabled, value); }
        }


        /// <summary>
        ///     Gets or sets the custom prob description.
        /// </summary>
        /// <value>The custom prob description.</value>
        public string CustomProbDescription
        {
            get { return _customProbDescription; }
            set { SetProperty(ref _customProbDescription, value); }
        }

        /// <summary>
        ///     Gets or sets the probe description place holder.
        /// </summary>
        /// <value>The probe description place holder.</value>
        public string EnterCustomProbeDescription
        {
            get { return _probeDescriptionPlaceHolder; }
            set { SetProperty(ref _probeDescriptionPlaceHolder, value); }
        }

        /// <summary>
        ///     Gets or sets the blue2 identifier.
        /// </summary>
        /// <value>The blue2 identifier.</value>
        public string Blue2Id
        {
            get { return _blue2Id; }
            set { SetProperty(ref _blue2Id, value); }
        }


        /// <summary>
        ///     /
        /// </summary>
        /// <value><c>true</c> if blue2 identifier visible; otherwise, <c>false</c>.</value>
        public bool Blue2IdVisible
        {
            get { return _blue2IdVisible; }
            set { SetProperty(ref _blue2IdVisible, value); }
        }

        /// <summary>
        ///     Gets or sets the connection status.
        /// </summary>
        /// <value>The connection status.</value>
        public string ConnectionStatus
        {
            get { return _connectionStatus; }
            set { SetProperty(ref _connectionStatus, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="HACCP.Core.ThermometerModeViewModel" /> enable controls.
        /// </summary>
        /// <value><c>true</c> if enable controls; otherwise, <c>false</c>.</value>
        public bool EnableControls
        {
            get { return _enableControls; }
            set { SetProperty(ref _enableControls, value); }
        }

        /// <summary>
        ///     Gets or sets the blue2 button style.
        /// </summary>
        /// <value>The blue2 button style.</value>
        public Style Blue2ButtonStyle
        {
            get { return _blue2ButtonStyle; }
            set { SetProperty(ref _blue2ButtonStyle, value); }
        }

        #endregion

        #region Commands

        /// <summary>
        ///     Gets the toggle temperature entry command.
        /// </summary>
        /// <value>The toggle temperature entry command.</value>
        public Command ToggleTemperatureUnitCommand
        {
            get
            {
                return _toggleTemperatureUnitCommand ??
                       (_toggleTemperatureUnitCommand = new Command<string>(async parameter =>
                       {
                           if (IsBusy)
                               return;

                           IsBusy = true;

                           if (HaccpAppSettings.SharedInstance.IsWindows)
                           {
                               if (WindowsBLEManager.SharedInstance.Settings == null)
                                   return;

                               TemperatureUnit = parameter == "Celcius"
                                   ? TemperatureUnit.Celcius
                                   : TemperatureUnit.Fahrenheit;

                               WindowsBLEManager.SharedInstance.Settings.Scale = TemperatureUnit ==
                                                                                 TemperatureUnit.Celcius
                                   ? (short) 0
                                   : (short) 1;
                               await WindowsBLEManager.SharedInstance.UpdateBLESettings(BLEChar.Scale);
                           }
                           else
                           {
                               if (BLEManager.SharedInstance.Settings == null)
                                   return;

                               TemperatureUnit = parameter == "Celcius"
                                   ? TemperatureUnit.Celcius
                                   : TemperatureUnit.Fahrenheit;

                               BLEManager.SharedInstance.Settings.Scale = TemperatureUnit == TemperatureUnit.Celcius
                                   ? (short) 0
                                   : (short) 1;
                               BLEManager.SharedInstance.UpdateBLESettings(BLEChar.Scale);
                           }

                           IsBusy = false;
                       }));
            }
        }

        /// <summary>
        ///     Gets the timing command.
        /// </summary>
        /// <value>The timing command.</value>
        public Command TimingCommand
        {
            get
            {
                return _timingCommand ??
                       (_timingCommand = new Command<string>(async parameter =>
                       {
                           if (IsBusy)
                               return;

                           IsBusy = true;

                           if (HaccpAppSettings.SharedInstance.IsWindows &&
                               WindowsBLEManager.SharedInstance.Settings == null)
                               return;
                           if (!HaccpAppSettings.SharedInstance.IsWindows && BLEManager.SharedInstance.Settings == null)
                               return;

                           if (parameter == HaccpConstant.Plusparameter)
                           {
                               if (_measurementTiming >= 4)
                                   IsTimingPlusEnabled = false;
                               if (!IsTimingMinusEnabled)
                                   IsTimingMinusEnabled = true;

                               ++_measurementTiming;
                           }
                           else
                           {
                               if (!IsTimingPlusEnabled)
                                   IsTimingPlusEnabled = true;
                               if (_measurementTiming == 2)
                                   IsTimingMinusEnabled = false;
                               --_measurementTiming;
                           }
                           Timing = string.Format("{0} {1}", _measurementTiming, HACCPUtil.GetResourceString("sec"));

                           if (HaccpAppSettings.SharedInstance.IsWindows)
                           {
                               WindowsBLEManager.SharedInstance.Settings.MeasurementLevel = _measurementTiming;
                               await WindowsBLEManager.SharedInstance.UpdateBLESettings(BLEChar.Time);
                           }
                           else
                           {
                               BLEManager.SharedInstance.Settings.MeasurementLevel = _measurementTiming;
                               BLEManager.SharedInstance.UpdateBLESettings(BLEChar.Time);
                           }

                           IsBusy = false;
                       }));
            }
        }

        /// <summary>
        ///     Gets the interval command.
        /// </summary>
        /// <value>The interval command.</value>
        public Command IntervalCommand
        {
            get
            {
                return _intervalCommand ??
                       (_intervalCommand = new Command<string>(async parameter =>
                       {
                           if (IsBusy)
                               return;

                           IsBusy = true;

                           if (HaccpAppSettings.SharedInstance.IsWindows &&
                               WindowsBLEManager.SharedInstance.Settings == null)
                               return;
                           if (!HaccpAppSettings.SharedInstance.IsWindows && BLEManager.SharedInstance.Settings == null)
                               return;

                           if (parameter == HaccpConstant.Plusparameter)
                           {
                               if (_autoOffInterval >= 29)
                                   IsIntervalPlusEnabled = false;
                               if (!IsIntervalMinusEnabled)
                                   IsIntervalMinusEnabled = true;


                               ++_autoOffInterval;
                           }
                           else
                           {
                               if (!IsIntervalPlusEnabled)
                                   IsIntervalPlusEnabled = true;
                               if (_autoOffInterval == 2)
                                   IsIntervalMinusEnabled = false;

                               --_autoOffInterval;
                           }

                           Interval = string.Format("{0} {1}", _autoOffInterval, HACCPUtil.GetResourceString("Minute"));

                           if (HaccpAppSettings.SharedInstance.IsWindows)
                           {
                               WindowsBLEManager.SharedInstance.Settings.AutoOff = _autoOffInterval;
                               await WindowsBLEManager.SharedInstance.UpdateBLESettings(BLEChar.AutoOff);
                           }
                           else
                           {
                               BLEManager.SharedInstance.Settings.AutoOff = _autoOffInterval;
                               BLEManager.SharedInstance.UpdateBLESettings(BLEChar.AutoOff);
                           }

                           IsBusy = false;
                       }));
            }
        }


        /// <summary>
        ///     Gets the interval command.
        /// </summary>
        /// <value>The interval command.</value>
        public Command SleepCommand
        {
            get
            {
                return _sleepCommand ??
                       (_sleepCommand = new Command<string>(async parameter =>
                       {
                           if (IsBusy)
                               return;

                           IsBusy = true;

                           if (HaccpAppSettings.SharedInstance.IsWindows &&
                               WindowsBLEManager.SharedInstance.Settings == null)
                               return;
                           if (!HaccpAppSettings.SharedInstance.IsWindows && BLEManager.SharedInstance.Settings == null)
                               return;

                           if (parameter == HaccpConstant.Plusparameter)
                           {
                               if (_sleepTime >= 29)
                                   IsSleepPlusEnabled = false;
                               if (!IsSleepMinusEnabled)
                                   IsSleepMinusEnabled = true;

                               if (_sleepTime < 30)
                                   ++_sleepTime;
                           }
                           else
                           {
                               if (!IsSleepPlusEnabled)
                                   IsSleepPlusEnabled = true;
                               if (_sleepTime == 2)
                                   IsSleepMinusEnabled = false;

                               if (_sleepTime > 0)
                                   --_sleepTime;
                           }

                           Sleep = string.Format("{0} {1}", _sleepTime, HACCPUtil.GetResourceString("Minute"));

                           if (HaccpAppSettings.SharedInstance.IsWindows)
                           {
                               WindowsBLEManager.SharedInstance.Settings.Sleep = _sleepTime;
                               await WindowsBLEManager.SharedInstance.UpdateBLESettings(BLEChar.Sleep);
                           }
                           else
                           {
                               BLEManager.SharedInstance.Settings.Sleep = _sleepTime;
                               BLEManager.SharedInstance.UpdateBLESettings(BLEChar.Sleep);
                           }

                           IsBusy = false;
                       }));
            }
        }

        /// <summary>
        ///     Gets the blue2 command.
        /// </summary>
        /// <value>The blue2 command.</value>
        public Command Blue2Command
        {
            get
            {
                return _blue2Command ??
                       //					(blue2Command = new Command (() => ExecuteBlue2Command (),()=>
                       //						{
                       //							return !IsBusy
                       //						});
                       (_blue2Command = new Command(ExecuteBlue2Command, () => !IsBusy));
            }
        }

        #endregion

        #region Methods

        // <summary>
        /// Executes the blue2 command.
        private async void ExecuteBlue2Command()
        {
            if (IsBusy)
                return;


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

            IsBusy = true;
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

            IsBusy = false;
            Blue2Command.ChangeCanExecute();
        }


        /// <summary>
        /// WindowsBlue2Command
        /// </summary>
        public void WindowsBlue2Command()
        {
            if (WindowsBLEManager.SharedInstance.HasAnyPairedDevice)
                WindowsBLEManager.SharedInstance.DisConnectDevice();
            else
                WindowsBLEManager.SharedInstance.NavigateBluetoothSettingsPage();
            // await Launcher.LaunchUriAsync(new Uri("ms-settings-bluetooth:"));
        }

        /// <summary>
        ///    UpdateProbeDescription
        /// </summary>
        public void UpdateProbeDescription()
        {
            if (HaccpAppSettings.SharedInstance.IsWindows)
            {
                if (WindowsBLEManager.SharedInstance.Settings == null)
                    return;

                WindowsBLEManager.SharedInstance.Settings.Prob = CustomProbDescription != null
                    ? CustomProbDescription.Trim()
                    : string.Empty;
                WindowsBLEManager.SharedInstance.UpdateBLESettings(BLEChar.Prob);
            }
            else if (BLEManager.SharedInstance.Settings != null)
            {
                BLEManager.SharedInstance.Settings.Prob = CustomProbDescription != null
                    ? CustomProbDescription.Trim()
                    : string.Empty;
                BLEManager.SharedInstance.UpdateBLESettings(BLEChar.Prob);
            }
        }


        /// <summary>
        ///     Called when the view disappears.
        /// </summary>
        public override void OnViewDisappearing()
        {
            base.OnViewDisappearing();

            MessagingCenter.Unsubscribe<BleConnectionStatusMessage>(this, HaccpConstant.BleconnectionStatus);
            MessagingCenter.Unsubscribe<BLEBlue2SettingsUpdatedMessage>(this, HaccpConstant.Bleblue2SettingsUpdate);
            MessagingCenter.Unsubscribe<BleConnectionTimeOutMessage>(this, HaccpConstant.BleconnectionTimeout);
            MessagingCenter.Unsubscribe<BleScanCompleteMessage>(this, HaccpConstant.Blue2ScanComplete);
            MessagingCenter.Unsubscribe<WindowsBleConnectionStatusMessage>(this, HaccpConstant.BleconnectionStatus);
            MessagingCenter.Unsubscribe<WindowsScanningStatusMessage>(this, HaccpConstant.WindowsScanningStatus);
			BLEManager.SharedInstance.StopTemperatureReading = false;
			BLEManager.SharedInstance.RestartTemperartureReading ();
        }

        /// <summary>
        /// OnViewAppearing
        /// </summary>
        public override void OnViewAppearing()
        {
            base.OnViewAppearing();
            if (HaccpAppSettings.SharedInstance.IsWindows)
                LoadWindowsBlue2Settings();
            else
                LoadBlue2Settings();					
        }

        /// <summary>
        ///     Method for loading ble settings for windows
        /// </summary>
        public void LoadWindowsBlue2Settings()
        {
            if (WindowsBLEManager.SharedInstance.HasAnyPairedDevice)
            {
                ConnectionStatus = HACCPUtil.GetResourceString("ConnectionStatusDisconnect");
                Blue2ButtonStyle = Application.Current.Resources["Blue2ButtonActiveStyle"] as Style;

                var settings = WindowsBLEManager.SharedInstance.Settings;

                if (settings != null)
                {
                    EnableControls = true;

                    Blue2Id = settings.SNo;
                    if (!string.IsNullOrEmpty(Blue2Id))
                        Blue2IdVisible = true;

                    _measurementTiming = settings.MeasurementLevel;
                    Timing = string.Format("{0} {1}", _measurementTiming, HACCPUtil.GetResourceString("sec"));

                    IsTimingPlusEnabled = IsTimingMinusEnabled = true;
                    if (_measurementTiming == 5)
                        IsTimingPlusEnabled = false;
                    else if (_measurementTiming == 1)
                        IsTimingMinusEnabled = false;


                    TemperatureUnit = settings.Scale == 0 ? TemperatureUnit.Celcius : TemperatureUnit.Fahrenheit;

                    _autoOffInterval = settings.AutoOff;
                    Interval = string.Format("{0} {1}", _autoOffInterval, HACCPUtil.GetResourceString("Minute"));

                    IsIntervalPlusEnabled = IsIntervalMinusEnabled = true;
                    if (_autoOffInterval == 30)
                        IsIntervalPlusEnabled = false;
                    else if (_autoOffInterval == 1)
                        IsIntervalMinusEnabled = false;

                    _sleepTime = settings.Sleep;
                    Sleep = string.Format("{0} {1}", _sleepTime, HACCPUtil.GetResourceString("Minute"));

                    IsSleepPlusEnabled = IsSleepMinusEnabled = true;
                    if (_sleepTime == 30)
                        IsSleepPlusEnabled = false;
                    else if (_sleepTime == 1)
                        IsSleepMinusEnabled = false;

                    CustomProbDescription = settings.Prob;
                    MessagingCenter.Send(new Blue2PlaceHolderVisibility(string.IsNullOrEmpty(CustomProbDescription)),
                        HaccpConstant.Blue2PlaceholderVisibility);
                }
                else
                {
                    EnableControls = false;
                }
            }
            else
            {
                EnableControls = false;
                ConnectionStatus =
                    HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("ConnectionStatusScanBlue2"), false);
                Blue2ButtonStyle = Application.Current.Resources["Blue2ButtonNotActiveStyle"] as Style;
                //if (!BLEManager.SharedInstance.adapter.IsBluetoothEnabled)
                //{              // bluetooth in phone is disabled

                //   CustomProbDescription = string.Empty;
                //    MessagingCenter.Send<Blue2PlaceHolderVisibility>(new Blue2PlaceHolderVisibility(true), HACCPConstant.BLUE2_PLACEHOLDER_VISIBILITY);
                //     await page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("TurnONBluetoothonyourdevice"));
                //  }
                //  else
                //   {                                  // connection to the blue2 device is disabled
                Blue2Id = string.Empty;
                Blue2IdVisible = false;
                CustomProbDescription = string.Empty;
                MessagingCenter.Send(new Blue2PlaceHolderVisibility(true), HaccpConstant.Blue2PlaceholderVisibility);
                //if (BLEManager.SharedInstance.adapter.IsScanning)
                //  await page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("Blue2TurnOnMessage"));
                //   }
            }
        }

        /// <summary>
        ///     Loads the blue2 settings.
        /// </summary>
        public async void LoadBlue2Settings()
        {
            if (BLEManager.SharedInstance.IsConnected)
            {
                EnableControls = true;
                ConnectionStatus = HACCPUtil.GetResourceString("ConnectionStatusDisconnect");
                Blue2ButtonStyle = Application.Current.Resources["Blue2ButtonActiveStyle"] as Style;
                if (BLEManager.SharedInstance.Settings != null)
                {
                    var settings = BLEManager.SharedInstance.Settings;

                    Blue2Id = settings.SNo;
                    if (!string.IsNullOrEmpty(Blue2Id))
                        Blue2IdVisible = true;

                    _measurementTiming = settings.MeasurementLevel;
                    Timing = string.Format("{0} {1}", _measurementTiming, HACCPUtil.GetResourceString("sec"));

                    IsTimingPlusEnabled = IsTimingMinusEnabled = true;
                    if (_measurementTiming == 5)
                        IsTimingPlusEnabled = false;
                    else if (_measurementTiming == 1)
                        IsTimingMinusEnabled = false;


                    TemperatureUnit = settings.Scale == 0 ? TemperatureUnit.Celcius : TemperatureUnit.Fahrenheit;


                    _autoOffInterval = settings.AutoOff;
                    Interval = string.Format("{0} {1}", _autoOffInterval, HACCPUtil.GetResourceString("Minute"));

                    IsIntervalPlusEnabled = IsIntervalMinusEnabled = true;
                    if (_autoOffInterval == 30)
                        IsIntervalPlusEnabled = false;
                    else if (_autoOffInterval == 1)
                        IsIntervalMinusEnabled = false;

                    _sleepTime = settings.Sleep;
                    Sleep = string.Format("{0} {1}", _sleepTime, HACCPUtil.GetResourceString("Minute"));

                    IsSleepPlusEnabled = IsSleepMinusEnabled = true;
                    if (_sleepTime == 30)
                        IsSleepPlusEnabled = false;
                    else if (_sleepTime == 1)
                        IsSleepMinusEnabled = false;

                    CustomProbDescription = settings.Prob;
                    MessagingCenter.Send(new Blue2PlaceHolderVisibility(string.IsNullOrEmpty(CustomProbDescription)),
                        HaccpConstant.Blue2PlaceholderVisibility);
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
                if (!BLEManager.SharedInstance.Adapter.IsBluetoothEnabled)
                {
                    // bluetooth in phone is disabled

                    CustomProbDescription = string.Empty;
                    MessagingCenter.Send(new Blue2PlaceHolderVisibility(true), HaccpConstant.Blue2PlaceholderVisibility);
                    await Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("TurnONBluetoothonyourdevice"));
                }
                else
                {
                    // connection to the blue2 device is disabled
                    Blue2Id = string.Empty;
                    Blue2IdVisible = false;

                    CustomProbDescription = string.Empty;
                    MessagingCenter.Send(new Blue2PlaceHolderVisibility(true), HaccpConstant.Blue2PlaceholderVisibility);
                    if (BLEManager.SharedInstance.Adapter.IsScanning)
                        await Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("Blue2TurnOnMessage"));
                }
            }
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