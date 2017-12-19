using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HACCP.Core
{
    public class RecordTempViewModel : RecordViewModel
    {
        private readonly IDataStore dataStore;
        private Style blue2ButtonStyle;
        private Command blue2Command;
        private string Blue2Id;
        private double blue2Temperature;
        private string connectionStatus;

        private string displayBlue2Temperature;
        private string displayManualEntryTemperature;
        private bool highTemperatureIndicator = true;
        private Command homeCommand;
        private bool isAlertActive;
        private bool isManualEntryOn;
        private Command keyPadToggleCommand;
        private LineBreakMode lineBreakMode;
        private bool lowTemperatureIndicator = true;
        private double manualEntryTemperature;
        private string manualTemperature;
        private string maximumTemperature;
        private string minimumTemperature;
        private string noteText;
        private bool onceLoaded;
        private bool optimumTemperatureIndicator = true;
        private double prevtemp;
        private Command recordCommand;
        private double rotation;
        private LocationMenuItem selectedItem;
        private TemperatureUnit temperatureUnit;
        private Command wakeProbeCommand;

        /// <summary>
        ///     Initializes a new instance of the <see /> class.
        /// </summary>
        /// <param name="page">Page.</param>
        /// <param name="item"></param>
        public RecordTempViewModel(IPage page, LocationMenuItem item)
            : base(page)
        {
            dataStore = new SQLiteDataStore();
            SelectedItem = item;

            IsManualEntryOn = Settings.RecordingMode == RecordingMode.Manual;

            TemperatureUnit = HaccpAppSettings.SharedInstance.DeviceSettings.TempScale == 0
                ? TemperatureUnit.Fahrenheit
                : TemperatureUnit.Celcius;

            var min = HACCPUtil.ConvertToDouble(SelectedItem.Min);
            var max = HACCPUtil.ConvertToDouble(SelectedItem.Max);

            MinimumTemperature = TemperatureUnit == TemperatureUnit.Celcius
                ? Math.Round(HACCPUtil.ConvertFahrenheitToCelsius(min)).ToString()
                : SelectedItem.Min;
            MaximumTemperature = TemperatureUnit == TemperatureUnit.Celcius
                ? Math.Round(HACCPUtil.ConvertFahrenheitToCelsius(max)).ToString()
                : SelectedItem.Max;

            MessagingCenter.Subscribe<WindowsScanningStatusMessage>(this, HaccpConstant.WindowsScanningStatus, sender =>
            {
                if (sender.IsScanning && ConnectionStatus != HACCPUtil.GetResourceString("ConnectionStatusDisconnect"))
                    ConnectionStatus = HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("Connecting"), false);
                else
                    ConnectionStatus =
                        HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("ConnectionStatusScanBlue2"), false);
            });

            MessagingCenter.Subscribe<BleTemperatureReadingMessage>(this, HaccpConstant.BletemperatureReading, sender =>
            {
                bool connected;

                connected = HaccpAppSettings.SharedInstance.IsWindows
                    ? WindowsBLEManager.SharedInstance.HasAnyPairedDevice
                    : BLEManager.SharedInstance.IsConnected;

                if (connected)
                {
                    var msg = sender;
                    if (msg != null)
                    {
                        OnPropertyChanged("IsWakeButtonVisible");
                        if (msg.IsSleeping)
                        {
                            DisplayBlue2Temperature = HACCPUtil.GetResourceString("Blue2SleepString");
                            LineBreakMode = LineBreakMode.TailTruncation;
                        }
                        else if (msg.IsHigh)
                        {
                            DisplayBlue2Temperature = HACCPUtil.GetResourceString("Blue2HighString");
                            LineBreakMode = LineBreakMode.TailTruncation;
                        }
                        else if (msg.IsLow)
                        {
                            DisplayBlue2Temperature = HACCPUtil.GetResourceString("Blue2LowString");
                            LineBreakMode = LineBreakMode.TailTruncation;
                        }
                        else if (msg.IsBatteryLow)
                        {
                            DisplayBlue2Temperature = HACCPUtil.GetResourceString("Blue2BatteryString");
                            LineBreakMode = LineBreakMode.TailTruncation;
                        }
                        else
                        {
                            var temp = msg.TempValue;
                            var unit = msg.TempUnit;
                            if (TemperatureUnit == TemperatureUnit.Celcius && unit != 0)
                                temp = HACCPUtil.ConvertFahrenheitToCelsius(temp);
                            else if (TemperatureUnit == TemperatureUnit.Fahrenheit && unit != 1)
                                temp = HACCPUtil.ConvertCelsiusToFahrenheit(temp);

                            DisplayTemperatureReading(temp, msg.ShouldRecord);
                        }
                    }
                }
            });


            MessagingCenter.Subscribe<WindowsBleConnectionStatusMessage>(this, HaccpConstant.BleconnectionStatus,
                sender =>
                {
                    OnPropertyChanged("IsWakeButtonVisible");

                    if (!WindowsBLEManager.SharedInstance.HasAnyPairedDevice)
                    {
                        ConnectionStatus =
                            HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("ConnectionStatusScanBlue2"),
                                false);
                        Blue2ButtonStyle = Application.Current.Resources["Blue2ButtonNotActiveStyle"] as Style;
                        Blue2Id = string.Empty;
                        DisplayBlue2Temperature = HACCPUtil.GetResourceString("ConnectBlue2Label");
                        LineBreakMode = LineBreakMode.TailTruncation;
                    }
                    //else
                    //{
                    //    ConnectionStatus = HACCPUtil.GetResourceString("ConnectionStatusDisconnect");
                    //    Blue2ButtonStyle = Application.Current.Resources["Blue2ButtonActiveStyle"] as Style;
                    //    DisplayBlue2Temperature = string.Empty;
                    //    LineBreakMode = LineBreakMode.NoWrap;
                    //}
                });


            MessagingCenter.Subscribe<BleConnectionStatusMessage>(this, HaccpConstant.BleconnectionStatus,
                async sender =>
                {
                    OnPropertyChanged("IsWakeButtonVisible");
                    if (BLEManager.SharedInstance.IsConnected)
                    {
                        ConnectionStatus = HACCPUtil.GetResourceString("ConnectionStatusDisconnect");
                        Blue2ButtonStyle = Application.Current.Resources["Blue2ButtonActiveStyle"] as Style;
                        DisplayBlue2Temperature = string.Empty;
                        LineBreakMode = LineBreakMode.NoWrap;
                    }
                    else
                    {
                        ConnectionStatus =
                            HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("ConnectionStatusScanBlue2"),
                                false);
                        Blue2ButtonStyle = Application.Current.Resources["Blue2ButtonNotActiveStyle"] as Style;
                        Blue2Id = string.Empty;
                        DisplayBlue2Temperature = HACCPUtil.GetResourceString("ConnectBlue2Label");
                        LineBreakMode = LineBreakMode.TailTruncation;

                        if (!BLEManager.SharedInstance.Adapter.IsBluetoothEnabled && !IsManualEntryOn)
                        {
                            // bluetooth in phone is disabled
                            if (!isAlertActive)
                            {
                                isAlertActive = true;
                                await
                                    page.ShowAlert(string.Empty,
                                        HACCPUtil.GetResourceString("TurnONBluetoothonyourdevice"));
                                isAlertActive = false;
                            }
                        }
                    }
                });

            MessagingCenter.Subscribe<BLEBlue2SettingsUpdatedMessage>(this, HaccpConstant.Bleblue2SettingsUpdate,
                sender =>
                {
                    if (HaccpAppSettings.SharedInstance.IsWindows && WindowsBLEManager.SharedInstance.Settings != null)
                        LoadWindowsBlue2Settings();

                    else if (BLEManager.SharedInstance.Settings != null)
                        Blue2Id = BLEManager.SharedInstance.Settings.SNo;
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
            get { return temperatureUnit; }
            set
            {
                temperatureUnit = value;
                UnitString = value == TemperatureUnit.Fahrenheit ? "°F" : "°C";
            }
        }

        /// <summary>
        ///     Gets or sets the unit string.
        /// </summary>
        /// <value>The unit string.</value>
        public string UnitString { get; set; }

        /// <summary>
        ///     Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public LocationMenuItem SelectedItem
        {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value); }
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
        ///     Gets or sets the minimum temperature.
        /// </summary>
        /// <value>The minimum temperature.</value>
        public string MinimumTemperature
        {
            get
            {
                //return string.Format ("{0}{1}", Math.Round (Convert.ToDouble (SelectedItem.MIN), 2), UnitString);
                return minimumTemperature;
            }
            set
            {
                value = string.Format("{0}{1}", Math.Round(HACCPUtil.ConvertToDouble(value), 2), UnitString);
                SetProperty(ref minimumTemperature, value);
            }
        }


        /// <summary>
        ///     Gets or sets the maximum temperature.
        /// </summary>
        /// <value>The maximum temperature.</value>
        public string MaximumTemperature
        {
            get
            {
                //return string.Format ("{0}{1}", Math.Round (Convert.ToDouble (SelectedItem.MAX), 2), UnitString);
                return maximumTemperature;
            }
            set
            {
                value = string.Format("{0}{1}", Math.Round(HACCPUtil.ConvertToDouble(value), 2), UnitString);
                SetProperty(ref maximumTemperature, value);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see /> optimum temperature
        ///     indicator.
        /// </summary>
        /// <value><c>true</c> if optimum temperature indicator; otherwise, <c>false</c>.</value>
        public bool OptimumTemperatureIndicator
        {
            get { return optimumTemperatureIndicator; }
            set { SetProperty(ref optimumTemperatureIndicator, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see /> high temperature
        ///     indicator.
        /// </summary>
        /// <value><c>true</c> if high temperature indicator; otherwise, <c>false</c>.</value>
        public bool HighTemperatureIndicator
        {
            get { return highTemperatureIndicator; }
            set { SetProperty(ref highTemperatureIndicator, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see /> low temperature
        ///     indicator.
        /// </summary>
        /// <value><c>true</c> if low temperature indicator; otherwise, <c>false</c>.</value>
        public bool LowTemperatureIndicator
        {
            get { return lowTemperatureIndicator; }
            set { SetProperty(ref lowTemperatureIndicator, value); }
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
        ///     Gets or sets a value indicating whether this instance is manual entry on.
        /// </summary>
        /// <value><c>true</c> if this instance is manual entry on; otherwise, <c>false</c>.</value>
        public bool IsManualEntryOn
        {
            get { return isManualEntryOn; }
            set { SetProperty(ref isManualEntryOn, value); }
        }


        /// <summary>
        ///     Gets or sets the blue2 temperature.
        /// </summary>
        /// <value>The blue2 temperature.</value>
        public double Blue2Temperature
        {
            get { return blue2Temperature; }
            set { SetProperty(ref blue2Temperature, value); }
        }


        /// <summary>
        ///     Gets or sets the manual entry temperature.
        /// </summary>
        /// <value>The manual entry temperature.</value>
        public double ManualEntryTemperature
        {
            get { return manualEntryTemperature; }
            set { SetProperty(ref manualEntryTemperature, value); }
        }

        /// <summary>
        ///     Gets the display manual entry temperature.
        /// </summary>
        /// <value>The display manual entry temperature.</value>
        public string DisplayManualEntryTemperature
        {
            get { return displayManualEntryTemperature; }
            set { SetProperty(ref displayManualEntryTemperature, value); }
        }


        /// <summary>
        ///     Gets or sets the manual temperature.
        /// </summary>
        /// <value>The manual temperature.</value>
        public string ManualTemperature
        {
            get { return manualTemperature; }
            set
            {
                try
                {
                    var decimalChar = Convert.ToString(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    var falsecharacter = decimalChar == "." ? "," : ".";


                    if (value == decimalChar)
                        DisplayManualEntryTemperature = value.Replace(UnitString, string.Empty) + UnitString;
                    else if (value == "-" || value == string.Format("-{0}", decimalChar))
                    {
                        if (Device.OS == TargetPlatform.Android || HaccpAppSettings.SharedInstance.IsWindows)
                        {
                            if (value == "-")
                            {
                                DisplayManualEntryTemperature = value;
                                return;
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(value))
                    {
                        if (!value.Contains(falsecharacter))
                        {
                            ManualEntryTemperature = HACCPUtil.ConvertToDouble(value);
                            DisplayManualEntryTemperature = value.Replace(UnitString, string.Empty) + UnitString;
                        }
                        else
                            return;
                    }
                    else
                        DisplayManualEntryTemperature = string.Empty;
                    SetProperty(ref manualTemperature, value);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        /// <summary>
        ///     Gets the display blue2 temperature.
        /// </summary>
        /// <value>The display blue2 temperature.</value>
        public string DisplayBlue2Temperature
        {
            get { return displayBlue2Temperature; }
            set { SetProperty(ref displayBlue2Temperature, value); }
        }


        /// <summary>
        ///     Gets the note icon.
        /// </summary>
        /// <value>The note icon.</value>
        public string NoteIcon
        {
            get
            {
                return HaccpAppSettings.SharedInstance.DeviceSettings.AllowTextMemo == 1
                    ? "edit.png"
                    : "noteDisabled.png";
            }
        }

        /// <summary>
        ///     Gets the color of the note hint.
        /// </summary>
        /// <value>The color of the note hint.</value>
        public Color NoteHintColor
        {
            get
            {
                return HaccpAppSettings.SharedInstance.DeviceSettings.AllowTextMemo == 1
                    ? Color.White
                    : Color.FromHex("#a2b9c1");
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is note enable.
        /// </summary>
        /// <value><c>true</c> if this instance is note enable; otherwise, <c>false</c>.</value>
        public bool IsNoteEnable
        {
            get { return HaccpAppSettings.SharedInstance.DeviceSettings.AllowTextMemo == 1; }
        }

        /// <summary>
        ///     Gets or sets the note text.
        /// </summary>
        /// <value>The note text.</value>
        public string NoteText
        {
            get { return noteText; }
            set { SetProperty(ref noteText, value); }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is wake button visible.
        /// </summary>
        /// <value><c>true</c> if this instance is wake button visible; otherwise, <c>false</c>.</value>
        public bool IsWakeButtonVisible
        {
            get
            {
                if (!IsManualEntryOn && HaccpAppSettings.SharedInstance.IsWindows)
                    return WindowsBLEManager.SharedInstance.IsSleeping;

                if (!IsManualEntryOn && BLEManager.SharedInstance.IsSleeping)
                    return true;
                return false;
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
        ///     The record command.
        /// </summary>
        public Command RecordCommand
        {
            get
            {
                return recordCommand ??
                       (recordCommand = new Command(ExecuteRecordCommand, () => !IsBusy));
            }
        }

        /// <summary>
        ///     Gets the toggle temperature entry command.
        /// </summary>
        /// <value>The toggle temperature entry command.</value>
        public Command KeyPadToggleCommand
        {
            get
            {
                return keyPadToggleCommand ??
                       (keyPadToggleCommand = new Command(ExecuteToggleCommand, () => !IsBusy));
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
                return blue2Command ??
                       (blue2Command = new Command(ExecuteBlue2Command));
            }
        }

        /// <summary>
        ///     Gets the home command.
        /// </summary>
        /// <value>The home command.</value>
        public Command HomeCommand
        {
            get { return homeCommand ?? (homeCommand = new Command(ExecuteHomeCommand)); }
        }

        /// <summary>
        ///     Gets the home command.
        /// </summary>
        /// <value>The home command.</value>
        public Command WakeProbeCommand
        {
            get
            {
                return wakeProbeCommand ?? (wakeProbeCommand = new Command(async () =>
                {
                    if (HaccpAppSettings.SharedInstance.IsWindows)
                        await WindowsBLEManager.SharedInstance.ResetBlue2AutoOff();
                    else
                        BLEManager.SharedInstance.ResetBlue2AutoOff();
                    OnPropertyChanged("IsWakeButtonVisible");
                }));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Executes the homa command.
        /// </summary>
        public void ExecuteHomeCommand()
        {
            MessagingCenter.Send(new CleanUpMessage(), HaccpConstant.CleanupMessage);
            Page.ReloadHomePage();
        }

        public void WindowsBlue2Command()
        {
            if (WindowsBLEManager.SharedInstance.HasAnyPairedDevice)
                WindowsBLEManager.SharedInstance.DisConnectDevice();
            else
                WindowsBLEManager.SharedInstance.NavigateBluetoothSettingsPage();
        }

        /// <summary>
        ///     Executes the blue2 command.
        /// </summary>
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


        /// <summary>
        ///     Executes the record command.
        /// </summary>
        private async void ExecuteRecordCommand()
        {
            if (IsBusy)
                return;

            try
            {
                bool connected;
                bool gotTemperartureReading;
                if (HaccpAppSettings.SharedInstance.IsWindows)
                {
                    gotTemperartureReading = WindowsBLEManager.SharedInstance.GotTemperartureReading;
                    connected = WindowsBLEManager.SharedInstance.HasAnyPairedDevice;
                }
                else
                {
                    connected = BLEManager.SharedInstance.IsConnected;
                    gotTemperartureReading = BLEManager.SharedInstance.GotTemperartureReading;
                }

                if (!IsManualEntryOn && !connected)
                {
                    if (HaccpAppSettings.SharedInstance.IsWindows)
                        return;
                    if (!BLEManager.SharedInstance.Adapter.IsBluetoothEnabled)
                    {
                        await Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("TurnONBluetoothonyourdevice"));
                        return;
                    }
                    await Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("Blue2TurnOnMessage"));
                    return;
                }


                if (!IsManualEntryOn && !gotTemperartureReading)
                    return;

                if (IsManualEntryOn && string.IsNullOrEmpty(ManualTemperature))
                {
                    await Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("SpecifyTemperature"));
                    return;
                }
                if (IsManualEntryOn)
                {
                    try
                    {
                        double.Parse(ManualTemperature);
                    }
                    catch
                    {
                        Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("Specifyavalidtemperaturevalue"));
                        return;
                    }
                }
                else if (DisplayBlue2Temperature == HACCPUtil.GetResourceString("ConnectBlue2Label") ||
                         DisplayBlue2Temperature == HACCPUtil.GetResourceString("Blue2SleepString") ||
                         DisplayBlue2Temperature == HACCPUtil.GetResourceString("Blue2LowString") ||
                         DisplayBlue2Temperature == HACCPUtil.GetResourceString("Blue2HighString") &&
                         DisplayBlue2Temperature == HACCPUtil.GetResourceString("Blue2BatteryString"))
                {
                    await Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("InvalidTemperatureAlert"));
                    return;
                }

                var allowedMax = HaccpAppSettings.SharedInstance.DeviceSettings.TempScale == 0 ? 9999.9 : 5537.7;
                var allowedMin = HaccpAppSettings.SharedInstance.DeviceSettings.TempScale == 0 ? -9999.9 : -5537.7;

                if (IsManualEntryOn)
                {
                    if (ManualEntryTemperature > allowedMax || ManualEntryTemperature < allowedMin)
                    {
                        await Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("InvalidTemperatureAlert"));
                        return;
                    }
                }
                else
                {
                    var lastReading = HaccpAppSettings.SharedInstance.IsWindows
                        ? WindowsBLEManager.SharedInstance.LastReading
                        : BLEManager.SharedInstance.LastReading;
                    var lastunit = HaccpAppSettings.SharedInstance.IsWindows
                        ? WindowsBLEManager.SharedInstance.LastUnit
                        : BLEManager.SharedInstance.LastUnit;

                    if (TemperatureUnit == TemperatureUnit.Celcius && lastunit != TemperatureUnit.Celcius)
                        lastReading = HACCPUtil.ConvertFahrenheitToCelsius(lastReading);
                    else if (TemperatureUnit == TemperatureUnit.Fahrenheit && lastunit != TemperatureUnit.Fahrenheit)
                        lastReading = HACCPUtil.ConvertCelsiusToFahrenheit(lastReading);

                    Blue2Temperature = lastReading;
                }

                IsBusy = true;
                RecordCommand.ChangeCanExecute();


                var temp = IsManualEntryOn ? ManualEntryTemperature : Blue2Temperature;

                var minTemp = HACCPUtil.ConvertToDouble(SelectedItem.Min);
                var maxTemp = HACCPUtil.ConvertToDouble(SelectedItem.Max);

                var convertedTemp = temp;
                if (TemperatureUnit == TemperatureUnit.Celcius)
                {
                    minTemp = Math.Round(HACCPUtil.ConvertFahrenheitToCelsius(minTemp));
                    maxTemp = Math.Round(HACCPUtil.ConvertFahrenheitToCelsius(maxTemp));
                    //convertedTemp = HACCPUtil.ConvertFahrenheitToCelsius (temp);
                }


                if (convertedTemp >= minTemp && convertedTemp <= maxTemp &&
                    HaccpAppSettings.SharedInstance.DeviceSettings.SkipRecordPreview)
                {
                    var date = DateTime.Now;
                    long ccpid;
                    long.TryParse(SelectedItem.Ccpid, out ccpid);
                    var location = dataStore.GetLocationById(SelectedItem.LocationId);

                    if (TemperatureUnit == TemperatureUnit.Celcius)
                        convertedTemp = HACCPUtil.ConvertCelsiusToFahrenheit(temp);

                    var itemTemperature = new ItemTemperature
                    {
                        IsManualEntry = IsManualEntryOn ? (short)1 : (short)0,
                        ItemID = SelectedItem.ItemId,
                        Temperature = convertedTemp.ToString("0.0"),
                        ItemName = SelectedItem.Name,
                        Max = SelectedItem.Max,
                        Min = SelectedItem.Min,
                        CorrAction = HACCPUtil.GetResourceString("None"),
                        LocationID = SelectedItem.LocationId,
                        Ccp = SelectedItem.Ccp,
                        CCPID = ccpid,
                        IsNA = 0,
                        Hour = date.Hour.ToString(),
                        Day = date.Day.ToString(),
                        Minute = date.Minute.ToString(),
                        Month = date.Month.ToString(),
                        Sec = date.Second.ToString(),
                        Year = date.Year.ToString(),
                        SiteID = HaccpAppSettings.SharedInstance.SiteSettings.SiteId,
                        UserName = HaccpAppSettings.SharedInstance.UserName,
                        DeviceId = HaccpAppSettings.SharedInstance.DeviceId,
                        MenuID = HaccpAppSettings.SharedInstance.SiteSettings.MenuId,
                        TZID = HaccpAppSettings.SharedInstance.SiteSettings.TimeZoneId,
                        LocName = location.Name,
                        Note = NoteText,
                        Blue2ID = SelectedItem.Blue2Id
                    };
                    dataStore.AddTemperature(itemTemperature);

                    SelectedItem.RecordStatus = 1;
                    dataStore.RecordLocationItem(SelectedItem);
                    if (HaccpAppSettings.SharedInstance.IsWindows)
                        WindowsBLEManager.SharedInstance.ResetBlue2AutoOff();
                    else
                        BLEManager.SharedInstance.ResetBlue2AutoOff();

                    MessagingCenter.Send(SelectedItem, HaccpConstant.RecorditemMessage);
                    dataStore.UpdateLocationItemRecordStatus(SelectedItem.LocationId);
                    MessagingCenter.Send(new MenuLocationId
                    {
                        LocationId = SelectedItem.LocationId
                    }, HaccpConstant.MenulocationMessage);

                    MessagingCenter.Send(new RecordSaveCompleteToast(), HaccpConstant.ToastMessage);
                }
                else
                {
                    SelectedItem.IsManualEntry = IsManualEntryOn;
                    SelectedItem.RecordedTemperature = temp.ToString("0.0");
                    SelectedItem.TemperatureUnit = TemperatureUnit;
                    SelectedItem.Blue2Id = IsManualEntryOn ? string.Empty : Blue2Id;
                    SelectedItem.Note = NoteText;
                    if (HaccpAppSettings.SharedInstance.IsWindows)
                        WindowsBLEManager.SharedInstance.ResetBlue2AutoOff();
                    else
                        BLEManager.SharedInstance.ResetBlue2AutoOff();


                    await Page.NavigateToWithSelectedObject(PageEnum.RecordComplete, true, SelectedItem);

                    IsBusy = false;
                    RecordCommand.ChangeCanExecute();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error on recording {0}", ex.Message);
            }
        }


        /// <summary>
        ///     Autos the advance.
        /// </summary>
        public async void CheckAutoAdvance()
        {
            try
            {
                await Page.PopPage();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                if (HaccpAppSettings.SharedInstance.DeviceSettings.AutoAdvance)
                    MessagingCenter.Send(new AutoAdvanceLocationMessage
                    {
                        CurrentId = SelectedItem.ItemId
                    }, HaccpConstant.AutoadvancelocationMessage);

                IsBusy = false;
                RecordCommand.ChangeCanExecute();
            }
        }


        /// <summary>
        ///     Executes the NA command.
        /// </summary>
        /// <returns>The NA command.</returns>
        protected override async Task ExecuteNACommand()
        {
            if (IsBusy)
                return;

            var result =
                await
                    Page.ShowConfirmAlert(string.Empty,
                        HACCPUtil.GetResourceString("SavedNAasthetemperaturerecordingfortheitem"));

            if (result)
            {
                IsBusy = true;
                NACommand.ChangeCanExecute();

                try
                {
                    var location = dataStore.GetLocationById(SelectedItem.LocationId);
                    var date = DateTime.Now;
                    long ccpid;
                    long.TryParse(SelectedItem.Ccpid, out ccpid);
                    var itemTemperature = new ItemTemperature
                    {
                        IsManualEntry = SelectedItem.IsManualEntry ? (short)1 : (short)0,
                        ItemID = SelectedItem.ItemId,
                        Temperature = "0",
                        ItemName = SelectedItem.Name,
                        Max = SelectedItem.Max,
                        Min = SelectedItem.Min,
                        CorrAction = string.Empty,
                        //SelectedCorrectiveAction != null ? SelectedCorrectiveAction.CorrActionName : "None",
                        LocationID = SelectedItem.LocationId,
                        Ccp = SelectedItem.Ccp,
                        CCPID = ccpid,
                        IsNA = 1,
                        Hour = date.Hour.ToString(),
                        Day = date.Day.ToString(),
                        Minute = date.Minute.ToString(),
                        Month = date.Month.ToString(),
                        Sec = date.Second.ToString(),
                        Year = date.Year.ToString(),
                        SiteID = HaccpAppSettings.SharedInstance.SiteSettings.SiteId,
                        UserName = HaccpAppSettings.SharedInstance.UserName,
                        DeviceId = HaccpAppSettings.SharedInstance.DeviceId,
                        MenuID = HaccpAppSettings.SharedInstance.SiteSettings.MenuId,
                        TZID = HaccpAppSettings.SharedInstance.SiteSettings.TimeZoneId,
                        BatchId = HaccpAppSettings.SharedInstance.SiteSettings.LastBatchNumber,
                        LocName = location.Name,
                        Blue2ID = string.Empty
                    };
                    dataStore.AddTemperature(itemTemperature);

                    SelectedItem.RecordStatus = 1;
                    dataStore.RecordLocationItem(SelectedItem);

                    MessagingCenter.Send(SelectedItem, HaccpConstant.RecorditemMessage);

                    dataStore.UpdateLocationItemRecordStatus(SelectedItem.LocationId);

                    MessagingCenter.Send(new MenuLocationId
                    {
                        LocationId = SelectedItem.LocationId
                    }, HaccpConstant.MenulocationMessage);

                    await Page.PopPage();

                    if (HaccpAppSettings.SharedInstance.DeviceSettings.AutoAdvance)
                        MessagingCenter.Send(new AutoAdvanceLocationMessage
                        {
                            CurrentId = SelectedItem.ItemId
                        }, HaccpConstant.AutoadvancelocationMessage);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error on N/A Command {0}", ex.Message);
                }
                finally
                {
                    IsBusy = false;
                    NACommand.ChangeCanExecute();
                }
            }
        }

        /// <summary>
        ///     Executes the toggle command.
        /// </summary>
        public void ExecuteToggleCommand()
        {
            if (IsBusy)
                return;


            if (HaccpAppSettings.SharedInstance.DeviceSettings.AllowManualTemp == 1)
            {
                IsManualEntryOn = !IsManualEntryOn;
            }
            else
            {
                Page.ShowAlert(string.Empty,
                    HACCPUtil.GetResourceString(
                        "YoudonothavepermissiontoenterthetemperaturemanuallyPleasecontacttheApplicationAdministrator"));
            }
            Settings.RecordingMode = IsManualEntryOn ? RecordingMode.Manual : RecordingMode.BlueTooth;
            OnPropertyChanged("IsWakeButtonVisible");
        }


        /// <summary>
        ///     Displaies the temperature reading.
        /// </summary>
        /// <param name="temperature">Temperature.</param>
        /// <param name="autoRecord"></param>
        private void DisplayTemperatureReading(double temperature, bool autoRecord)
        {
            var mintemp = HACCPUtil.ConvertToDouble(SelectedItem.Min);
            var maxtemp = HACCPUtil.ConvertToDouble(SelectedItem.Max);
            if (TemperatureUnit == TemperatureUnit.Celcius)
            {
                mintemp = HACCPUtil.ConvertFahrenheitToCelsius(mintemp);
                maxtemp = HACCPUtil.ConvertFahrenheitToCelsius(maxtemp);
            }

            if (temperature < mintemp)
            {
                OptimumTemperatureIndicator = false;
                HighTemperatureIndicator = false;
                LowTemperatureIndicator = true;
            }
            else if (temperature > maxtemp)
            {
                OptimumTemperatureIndicator = false;
                HighTemperatureIndicator = true;
                LowTemperatureIndicator = false;
            }
            else
            {
                OptimumTemperatureIndicator = true;
                HighTemperatureIndicator = false;
                LowTemperatureIndicator = false;
            }

            var diff = Math.Round(temperature - prevtemp, 1);
            if (diff > 2.8)
                diff = 2.8;
            else if (diff < -2.8)
                diff = -2.8;

            Rotation = HACCPUtil.GetSlope(diff);

            Blue2Temperature = temperature;
            prevtemp = temperature;

            DisplayBlue2Temperature = string.Format("{0}{1}", Blue2Temperature.ToString("0.0"), UnitString);
            LineBreakMode = LineBreakMode.NoWrap;
            if (autoRecord)
            {
                Device.BeginInvokeOnMainThread(() => { RecordCommand.Execute(null); });
            }
        }


        /// <summary>
        ///     Called when the view appears.
        /// </summary>
        public override void OnViewAppearing()
        {
            base.OnViewAppearing();
            if (!onceLoaded)
            {
                if (HaccpAppSettings.SharedInstance.IsWindows)
                    LoadWindowsBlue2Settings();
                else
                    LoadBlue2Settings();
            }
            onceLoaded = true;
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

        /// <summary>
        ///     Searchs the blue2 devices.
        /// </summary>
        public async void LoadBlue2Settings()
        {
            if (BLEManager.SharedInstance.IsConnected)
            {
                ConnectionStatus = HACCPUtil.GetResourceString("ConnectionStatusDisconnect");
                Blue2ButtonStyle = Application.Current.Resources["Blue2ButtonActiveStyle"] as Style;
            }
            else
            {

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
                DisplayBlue2Temperature = HACCPUtil.GetResourceString("ConnectBlue2Label");
                LineBreakMode = LineBreakMode.TailTruncation;

                if (!BLEManager.SharedInstance.Adapter.IsBluetoothEnabled && !IsManualEntryOn)
                {
                    // bluetooth in phone is disabled
                    await Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("TurnONBluetoothonyourdevice"));
                }
                else if (BLEManager.SharedInstance.Adapter.IsScanning && !IsManualEntryOn)
                {
                    // connection to the blue2 device is disabled					
                    await Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("Blue2TurnOnMessage"));
                }
            }
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
                if (WindowsBLEManager.SharedInstance.Settings != null)
                    Blue2Id = WindowsBLEManager.SharedInstance.Settings.SNo;


                if (WindowsBLEManager.SharedInstance.Settings != null)
                {
                    var temp = WindowsBLEManager.SharedInstance.LastReading;
                    var unit = WindowsBLEManager.SharedInstance.Settings.Scale;
                    if (TemperatureUnit == TemperatureUnit.Celcius && unit != 0)
                        temp = HACCPUtil.ConvertFahrenheitToCelsius(temp);
                    else if (TemperatureUnit == TemperatureUnit.Fahrenheit && unit != 1)
                        temp = HACCPUtil.ConvertCelsiusToFahrenheit(temp);

                    DisplayTemperatureReading(temp, false);
                }
            }
            else
            {
                ConnectionStatus =
                    HACCPUtil.TruncateResourceString(HACCPUtil.GetResourceString("ConnectionStatusScanBlue2"), false);
                Blue2ButtonStyle = Application.Current.Resources["Blue2ButtonNotActiveStyle"] as Style;
                DisplayBlue2Temperature = HACCPUtil.GetResourceString("ConnectBlue2Label");
                LineBreakMode = LineBreakMode.TailTruncation;
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