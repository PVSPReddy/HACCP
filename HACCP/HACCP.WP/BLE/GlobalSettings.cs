using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Networking.Proximity;
using Windows.Storage;
using Windows.System;
using HACCP.Core;
using HACCP.WP.BLE.Dictionary;
using HACCP.WP.BLE.Dictionary.DataParser;
using HACCP.WP.BLE.Models;

namespace HACCP.WP.BLE
{
    /// <summary>
    ///     Global settings for the app.
    ///     Acts as a singleton for the entire app, containing the list of all devices,
    ///     storing details of the currently displayed pages, as well as maintaining our
    ///     list of custom service and characteristic names.
    ///     Several settings and the currently registered gatt triggers are also tracked
    ///     here.
    /// </summary>
    public class GlobalSettings : IGlobalSettings
    {
        #region Native Methods

        /// <summary>
        ///     Opens bluetooth settings page
        /// </summary>
        public async void OpenBlueToothSettings()
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings-bluetooth:"));
        }

        #endregion

        /// <summary>
        ///     Initializes the GlobalSettings class.  Also reads the stored cache mode setting.
        /// </summary>
        public static void Initialize()
        {
            // Create objects for the objects that we need. 
            PairedDevices = new List<BEDeviceModel>();

            ParserLookupTable = new CharacteristicParserLookupTable();
            ServiceDictionaryUnknown = new ServiceDictionary();
            ServiceDictionaryConstant = new ServiceDictionary();
            CharacteristicDictionaryUnknown = new CharacteristicDictionary();
            CharacteristicDictionaryConstant = new CharacteristicDictionary();
            ServiceDictionaryConstant.InitAsConstant();
            CharacteristicDictionaryConstant.InitAsConstant();
            DictionariesCleared = false;
            CharacteristicsWithActiveToast = new List<BECharacteristicModel>();

            _useCachedMode = ApplicationData.Current.LocalSettings.Values.ContainsKey(USE_CACHED_MODE);


            PeerFinder.TriggeredConnectionStateChanged += PeerFinder_TriggeredConnectionStateChanged;
        }

        private static void PeerFinder_TriggeredConnectionStateChanged(object sender,
            TriggeredConnectionStateChangedEventArgs args)
        {
            var s = args;
        }

        /// <summary>
        ///     Async initialization function that loads our characteristic dictionaries
        /// </summary>
        /// <returns></returns>
        public static async Task InitializeDictionariesAsync()
        {
            await ServiceDictionaryUnknown.LoadDictionaryAsync();
            await CharacteristicDictionaryUnknown.LoadDictionaryAsync();
        }

        #region ----------------------------- Dictionary cleanup -------------------------

        /// <summary>
        ///     Clear out all custom names in the dictionaries.
        /// </summary>
        /// <returns></returns>
        public static async Task ClearDictionariesAsync()
        {
            await ServiceDictionaryUnknown.ClearDictionaryAsync();
            await CharacteristicDictionaryUnknown.ClearDictionaryAsync();
            DictionariesCleared = true;
        }

        #endregion // Dictionary cleanup

        #region ----------------------------- Other settings -------------------------

        /// <summary>
        ///     Sets and persists whether to use cached mode or not
        /// </summary>
        /// <param name="value"></param>
        private static void SetUseCachedMode(bool value)
        {
            if (value)
            {
                ApplicationData.Current.LocalSettings.Values[USE_CACHED_MODE] = "1";
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values.Remove(USE_CACHED_MODE);
            }
            _useCachedMode = value;
        }

        #endregion // Other settings

        #region ----------------------------- Variables --------------------------

        // For navigation through pages
        public static BEDeviceModel SelectedDevice;
        public static BEServiceModel SelectedService;
        public static BECharacteristicModel SelectedCharacteristic;

        // Dictionaries for keeping track of objects
        public static ServiceDictionary ServiceDictionaryConstant;
        public static ServiceDictionary ServiceDictionaryUnknown;
        public static CharacteristicDictionary CharacteristicDictionaryConstant;
        public static CharacteristicDictionary CharacteristicDictionaryUnknown;
        public static CharacteristicParserLookupTable ParserLookupTable;
        public static bool DictionariesCleared { get; private set; }

        // List of active devices
        public static List<BEDeviceModel> PairedDevices;

        // Toast and background related objects
        public static bool BackgroundAccessRequested;
        public static List<BECharacteristicModel> CharacteristicsWithActiveToast;

        // List of settings
        private const string USE_CACHED_MODE = "UseCachedMode";
        private static bool _useCachedMode;


        public event EventHandler<BLESettingsUpdated> BLESettingsUpdated;
        public event EventHandler<DeviceStatusUpdated> PairedDeviceStatusUpdated;
        public event EventHandler<string> GotTemperatureReading;
        public event EventHandler<bool> ScanningProcess;


        private BECharacteristicModel autooffChar;
        private BECharacteristicModel sleepChar;
        private BECharacteristicModel measurementScaleChar;
        private BECharacteristicModel measurementTimeChar;
        private BECharacteristicModel probChar;
        private BECharacteristicModel disConnectPeripheralChar;
        private BECharacteristicModel resetAutoOffChar;

        private Timer timer;

        public bool IsBusy { get; set; }

        public static bool UseCachedMode
        {
            get { return _useCachedMode; }
            set { SetUseCachedMode(value); }
        }

        #endregion // variables

        #region ----------------------------- Populate Device List -------------------------

        /// <summary>
        ///     Populates the device list and initializes all the various models.
        ///     Waiting for the async calls to complete takes a while, so we want to call this
        ///     function somewhat sparingly.
        /// </summary>
        /// <returns></returns>
        public async Task PopulateDeviceListAsync()
        {
            Debug.WriteLine("PopulateDeviceListAsync Started...");


            Utilities.RunFuncAsTask(async () =>
            {
                try
                {
                    if (IsBusy)
                        return;


                    ScanningProcess(this, true);

                    IsBusy = true;
                    // Remove all devices and start from scratch
                    PairedDevices.Clear();

                    // Asynchronously get all paired bluetooth devices. 
                    var infoCollection = await DeviceInformation.FindAllAsync(BluetoothLEDevice.GetDeviceSelector());

                    // Re-add devices
                    foreach (var info in infoCollection)
                    {
                        // Make sure we don't initialize duplicates
                        if (PairedDevices.FindIndex(device => device.DeviceId == info.Id) >= 0)
                        {
                            continue;
                        }
                        var WRTDevice = await BluetoothLEDevice.FromIdAsync(info.Id);
                        var deviceM = new BEDeviceModel();
                        deviceM.Initialize(WRTDevice, info);
                        if (deviceM.Name.ToLower() == HaccpConstant.Blue2DeviceName)
                        {
                            if (deviceM.Connected)
                                PairedDevices.Add(deviceM);
                        }
                    }

                    Debug.WriteLine("PairedDevices Count...{0}", PairedDevices.Count);

                    PairedDeviceStatusUpdated(this, new DeviceStatusUpdated(PairedDevices.Any()));

                    if (PairedDevices.Any())
                    {
                        SelectedDevice = PairedDevices[0];
                        SelectedDevice.DeviceConnectionStatusChanged -= SelectedDevice_DeviceConnectionStatusChanged;
                        SelectedDevice.DeviceConnectionStatusChanged += SelectedDevice_DeviceConnectionStatusChanged;
                        ReadCharacteristics();
                    }
                    else
                    {
                        IsBusy = false;
                        Debug.WriteLine("No device in the list");
                    }
                }

                catch (Exception ex)
                {
                    Debug.WriteLine("Error PopulateDeviceListAsync : {0}", ex.Message);
                    IsBusy = false;

                    ScanningProcess(this, false);
                }
            });
        }

        private void SelectedDevice_DeviceConnectionStatusChanged(object sender, bool status)
        {
            PairedDeviceStatusUpdated(this, new DeviceStatusUpdated(status));
            // BLESettingsUpdated(this, new BLESettingsUpdated(null));
            IsBusy = false;
            if (!status)
                StopTemperatureReading();
            else if (!PairedDevices.Any())
                PopulateDeviceListAsync();
        }


        public async void ReadCharacteristics()
        {
            try
            {
                Debug.WriteLine("Started ReadCharacteristics...");

                var services = SelectedDevice.ServiceModels;

                var deviceservice = services.FirstOrDefault(x => x.Uuid.ToString() == HaccpConstant.DeviceServiceUuid);
                deviceservice.InitializeCharacteristics();
                await deviceservice.ReadCharacteristicsAsync();

                var batteryService =
                    services.FirstOrDefault(x => x.Name.ToLower().Contains(HaccpConstant.BatteryServiceName));
                batteryService.InitializeCharacteristics();
                await batteryService.ReadCharacteristicsAsync();

                var tempservice = services.FirstOrDefault(x => x.Uuid.ToString() == HaccpConstant.TemperatureServiceUuid);

                SelectedService = tempservice;

                tempservice.InitializeCharacteristics();
                await tempservice.ReadCharacteristicsAsync();


                var settings = new BleSettings();

                if (deviceservice != null && deviceservice.CharacteristicModels != null)
                {
                    var serialNumChar =
                        deviceservice.CharacteristicModels.FirstOrDefault(
                            x => x.Uuid.ToString().Contains(HaccpConstant.SerialNumberUuid));
                    if (serialNumChar != null)
                        settings.SNo = serialNumChar.CharacteristicValue;

                    Debug.WriteLine("settings.SNo {0}", settings.SNo);
                }

                if (batteryService != null && batteryService.CharacteristicModels != null)
                {
                    var batteryChar = batteryService.CharacteristicModels.FirstOrDefault();
                    if (batteryChar != null)
                        settings.BatteryLevel = Convert.ToInt32(batteryChar.CharacteristicValue);

                    Debug.WriteLine("settings.BatteryLevel {0}", settings.BatteryLevel);
                }


                if (tempservice != null && tempservice.CharacteristicModels != null)
                {
                    probChar =
                        tempservice.CharacteristicModels.FirstOrDefault(
                            x => x.Uuid.ToString() == HaccpConstant.ProbeUuid);
                    if (probChar != null && !string.IsNullOrEmpty(probChar.CharacteristicValue))
                    {
                        await probChar.ReadUTF8Value();
                        settings.Prob = probChar.CharacteristicValue;

                        Debug.WriteLine("settings.Prob {0}", settings.Prob);
                    }

                    measurementTimeChar =
                        tempservice.CharacteristicModels.FirstOrDefault(
                            x => x.Uuid.ToString() == HaccpConstant.MeasurementtimingCharacteristicsUuid);
                    if (measurementTimeChar != null && !string.IsNullOrEmpty(measurementTimeChar.CharacteristicValue))
                        settings.MeasurementLevel = Convert.ToInt32(measurementTimeChar.CharacteristicValue);

                    Debug.WriteLine("settings.MeasurementLevel {0}", settings.MeasurementLevel);

                    measurementScaleChar =
                        tempservice.CharacteristicModels.FirstOrDefault(
                            x => x.Uuid.ToString() == HaccpConstant.MeasurementScaleUuid);
                    if (measurementScaleChar != null)
                        settings.Scale = measurementScaleChar.CharacteristicValue == "67" ? (short)0 : (short)1;

                    Debug.WriteLine("settings.Scale {0}", settings.Scale);

                    autooffChar =
                        tempservice.CharacteristicModels.FirstOrDefault(
                            x => x.Uuid.ToString() == HaccpConstant.AutoOffIntervalUuid);
                    if (autooffChar != null && !string.IsNullOrEmpty(autooffChar.CharacteristicValue))
                        settings.AutoOff = Convert.ToInt32(autooffChar.CharacteristicValue);

                    Debug.WriteLine("settings.AutoOff {0}", settings.AutoOff);

                    sleepChar =
                        tempservice.CharacteristicModels.FirstOrDefault(
                            x => x.Uuid.ToString() == HaccpConstant.SleepTimeUuid);
                    if (sleepChar != null && !string.IsNullOrEmpty(sleepChar.CharacteristicValue))
                        settings.Sleep = Convert.ToInt32(sleepChar.CharacteristicValue);


                    Debug.WriteLine("settings.Sleep {0}", settings.Sleep);

                    disConnectPeripheralChar =
                        tempservice.CharacteristicModels.FirstOrDefault(
                            x => x.Uuid.ToString().Contains(HaccpConstant.DiscoonectPeripheralUuid));
                    resetAutoOffChar =
                        tempservice.CharacteristicModels.FirstOrDefault(
                            x => x.Uuid.ToString().Contains(HaccpConstant.ResetAutooffUuid));

                    var aschiChar =
                        tempservice.CharacteristicModels.FirstOrDefault(
                            x => x.Uuid.ToString().Contains(HaccpConstant.AsciiTemperatureUuid));

                    ReadTemperature(aschiChar, settings.MeasurementLevel);
                }

                IsBusy = false;

                BLESettingsUpdated(this, new BLESettingsUpdated(settings));
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Debug.WriteLine("Error reading characteristics {0}", ex.Message);
            }
        }

        /// <summary>
        ///     Method for updating ble settings
        /// </summary>
        /// <param name="chr"></param>
        /// <param name="value"></param>
        public async Task UpdateBLESettings(BLEChar chr, string value)
        {
            try
            {
                switch (chr)
                {
                    case BLEChar.Scale:
                        if (measurementScaleChar != null)
                            await measurementScaleChar.WriteMessageAsync(value);
                        break;
                    case BLEChar.Time:
                        if (measurementTimeChar != null)
                            await measurementTimeChar.WriteMessageAsync(value);
                        break;
                    case BLEChar.AutoOff:
                        if (autooffChar != null)
                            await autooffChar.WriteMessageAsync(value);
                        break;
                    case BLEChar.Sleep:
                        if (sleepChar != null)
                            await sleepChar.WriteMessageAsync(value);
                        break;
                    case BLEChar.Prob:
                        if (probChar != null)
                            probChar.UpdateProbe(value);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error UpdateBLESettings : {0}", ex.Message);
            }
        }


        /// <summary>
        ///     Method for disconnecting device
        /// </summary>
        public void DisConnectDevice()
        {
            try
            {
                if (disConnectPeripheralChar != null)
                    disConnectPeripheralChar.WriteMessageAsync("1");

                StopTemperatureReading();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error on DisConnectDevice : {0}", ex.Message);
            }
        }

        /// <summary>
        ///     Method for waking up ble device
        /// </summary>
        public async Task ResetBlue2AutoOff()
        {
            try
            {
                if (resetAutoOffChar != null)
                    await resetAutoOffChar.WriteMessageAsync("1");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error on ResetBlue2AutoOff : {0}", ex.Message);
            }
        }

        /// <summary>
        ///     Method for un registering device connection event
        /// </summary>
        public void UnRegisterDevice()
        {
            if (SelectedDevice != null)
            {
                SelectedDevice.DeviceConnectionStatusChanged -= SelectedDevice_DeviceConnectionStatusChanged;
            }
        }


        public bool HasConnectionWithDevice()
        {
            return SelectedDevice != null && SelectedDevice.Connected;
        }

        public void ReadTemperature(BECharacteristicModel asciChar, int interval)
        {
            timer = new Timer(async x =>
            {
                await asciChar.ReadUTF8Value();
                var val = asciChar.CharacteristicValue;

                GotTemperatureReading(this, val);
            }, null, 0, interval * 1000);
        }

        public void StopTemperatureReading()
        {
            if (timer != null)
                timer.Dispose();
        }

        #endregion // Populate device list

        #region ----------------------------- Notifications -------------------------

        /// <summary>
        ///     Unregisters all notifications by all characteristics on all paired devices.
        ///     Saves battery on the devices.
        /// </summary>
        /// <returns></returns>
        public static async Task UnregisterAllNotificationsAsync()
        {
            try
            {
                foreach (var deviceModel in PairedDevices)
                {
                    await deviceModel.UnregisterNotificationsAsync();
                }
            }
            catch (Exception ex)
            {
                //  Utilities.OnExceptionWithMessage(ex, "Hit an exception unregistering all notifications.");
                Debug.WriteLine("Hit an exception unregistering all notifications.{0}", ex.Message);
            }
        }

        /// <summary>
        ///     Registers for all notifications by all characteristics on all paired devices.
        ///     Allows us to receive characteristic value updates, and acts as a secondary
        ///     mechanism to inform the OS to auto-connect to the paired devices if they begin
        ///     advertising.
        /// </summary>
        /// <returns></returns>
        public static async Task RegisterAllNotificationsAsync()
        {
            try
            {
                foreach (var deviceModel in PairedDevices)
                {
                    await deviceModel.RegisterNotificationsAsync();
                }
            }
            catch (Exception ex)
            {
                //Utilities.OnExceptionWithMessage(ex, "Hit an exception Registering for all notifications.");
                Debug.WriteLine("Hit an exception unregistering all notifications.{0}", ex.Message);
            }
        }

        #endregion // Notifications

        #region ----------------------------- Toasts and Background -------------------------

        /// <summary>
        ///     Request background access so that we can register toasts.
        ///     Do this once to begin with so that we don't have to do this everywhere.
        /// </summary>
        /// <returns></returns>
        public static async Task RequestBackgroundAccessAsync()
        {
            if (BackgroundAccessRequested)
            {
                return;
            }

            // Request access to run toast in background
            var result = await BackgroundExecutionManager.RequestAccessAsync();

            // Check if got access
            if (result == BackgroundAccessStatus.Denied)
            {
                BackgroundAccessRequested = false;
            }
            else
            {
                BackgroundAccessRequested = true;
            }
        }

        /// <summary>
        ///     An unregister all function, for those times when the toasts are getting annoying.
        /// </summary>
        /// <returns></returns>
        public static async Task UnregisterAllToastsAsync()
        {
            // Unregister all toasts from current session
            foreach (var cm in CharacteristicsWithActiveToast)
            {
                await cm.TaskUnregisterInsideListAsync();
            }

            // Unregister all toasts from the past. 
            foreach (var key in ApplicationData.Current.LocalSettings.Containers.Keys)
            {
                if (key.StartsWith(BECharacteristicModel.TOAST_STRING_PREFIX))
                {
                    ApplicationData.Current.LocalSettings.Values.Remove(key);
                }
            }
            ToastClear();
        }

        /// <summary>
        ///     Simple helper to add a characteristic to be tracked as an active toast
        ///     with a GattCharacteristicNotificationTrigger
        /// </summary>
        /// <param name="cm">Model representing the characteristic</param>
        public static void AddToast(BECharacteristicModel cm)
        {
            foreach (var model in CharacteristicsWithActiveToast)
            {
                if (cm.ToastEquals(model))
                {
                    return;
                }
            }
            CharacteristicsWithActiveToast.Add(cm);
        }

        /// <summary>
        ///     Removes a characteristic from our list of characteristics with active toasts
        /// </summary>
        /// <param name="cm">Model representing the characteristic</param>
        public static void RemoveToast(BECharacteristicModel cm)
        {
            CharacteristicsWithActiveToast.Remove(cm);
        }

        /// <summary>
        ///     Clears out the list of characteristics with active toasts
        /// </summary>
        private static void ToastClear()
        {
            CharacteristicsWithActiveToast.Clear();
        }

        #endregion // Toasts
    }
}