using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HACCP.Core
{
    public class WindowsBLEManager
    {
        private static volatile WindowsBLEManager _instance;

        /// <summary>
        /// WindowsBLEManager Constructor
        /// </summary>
        private WindowsBLEManager()
        {
        }

        #region Properties

        public static WindowsBLEManager SharedInstance
        {
            get { return _instance ?? (_instance = new WindowsBLEManager()); }
        }

        public bool HasAnyPairedDevice { get; set; }

        public IGlobalSettings GlobalSettings { get; set; }

        public BleSettings Settings { get; set; }

        public double LastReading { get; set; }

        public TemperatureUnit LastUnit { get; set; }

        public string ReadingSlope { get; set; }

        public bool IsSleeping { get; set; }

        public bool IsHigh { get; set; }

        public bool IsLow { get; set; }

        public bool IsBatteryLow { get; set; }

        public bool ReadingBlue2Data { get; set; }

        public bool GotTemperartureReading { get; set; }

        public bool OpenedBluetoothSettings { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Method for populating connected devices
        /// </summary>
        public void PopulateDeviceList()
        {
            GlobalSettings.PopulateDeviceListAsync();
        }

        /// <summary>
        /// Register Events
        /// </summary>
        public void RegisterEvents()
        {
            GlobalSettings.PairedDeviceStatusUpdated += GlobalSettings_PairedDeviceStatusUpdated;
            GlobalSettings.BLESettingsUpdated += GlobalSettings_BLESettingsUpdated;
            GlobalSettings.GotTemperatureReading += GlobalSettings_GotTemperatureReading;
            GlobalSettings.ScanningProcess += GlobalSettings_ScanningProcess;
        }

        /// <summary>
        /// GlobalSettings_ScanningProcess
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlobalSettings_ScanningProcess(object sender, bool e)
        {
            MessagingCenter.Send(new WindowsScanningStatusMessage { IsScanning = e },
                HaccpConstant.WindowsScanningStatus);
        }

        /// <summary>
        /// UnRegister Events
        /// </summary>
        public void UnRegisterEvents()
        {
            GlobalSettings.UnRegisterDevice();
        }

        /// <summary>
        /// GlobalSettings_PairedDeviceStatusUpdated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlobalSettings_PairedDeviceStatusUpdated(object sender, DeviceStatusUpdated e)
        {
            HasAnyPairedDevice = e.Status;
           
          //  if (!HasAnyPairedDevice)
         //   {
                IsSleeping = IsLow = IsHigh = IsBatteryLow = GotTemperartureReading = false;
                Settings = null;
          //  }
            MessagingCenter.Send(new WindowsBleConnectionStatusMessage(), HaccpConstant.BleconnectionStatus);
        }

        /// <summary>
        /// GlobalSettings_BLESettingsUpdated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlobalSettings_BLESettingsUpdated(object sender, BLESettingsUpdated e)
        {
            Settings = e.Settings;
            MessagingCenter.Send(new BLEBlue2SettingsUpdatedMessage(), HaccpConstant.Bleblue2SettingsUpdate);
        }

        /// <summary>
        /// Update BLE Settings
        /// </summary>
        /// <param name="blechar"></param>
        /// <returns></returns>
        public async Task UpdateBLESettings(BLEChar blechar)
        {
            if (Settings == null)
                return;

            switch (blechar)
            {
                case BLEChar.Scale:
                    await
                        GlobalSettings.UpdateBLESettings(blechar,
                            SharedInstance.Settings.Scale == (short)0 ? "67" : "70");
                    break;
                case BLEChar.Time:
                    await GlobalSettings.UpdateBLESettings(blechar, SharedInstance.Settings.MeasurementLevel.ToString());
                    break;
                case BLEChar.AutoOff:
                    await GlobalSettings.UpdateBLESettings(blechar, SharedInstance.Settings.AutoOff.ToString());
                    break;
                case BLEChar.Sleep:
                    await GlobalSettings.UpdateBLESettings(blechar, SharedInstance.Settings.Sleep.ToString());
                    break;
                case BLEChar.Prob:
                    await GlobalSettings.UpdateBLESettings(blechar, SharedInstance.Settings.Prob);
                    break;
            }
        }

        /// <summary>
        /// NavigateBluetoothSettingsPage
        /// </summary>
        public void NavigateBluetoothSettingsPage()
        {
            if (!GlobalSettings.IsBusy)
            {
                OpenedBluetoothSettings = true;
                GlobalSettings.OpenBlueToothSettings();
            }
        }

        /// <summary>
        ///     Method for disconnecting ble device
        /// </summary>
        public void DisConnectDevice()
        {
            if (!GlobalSettings.IsBusy)
                GlobalSettings.DisConnectDevice();
        }

        /// <summary>
        /// ResetBlue2AutoOff
        /// </summary>
        public async Task ResetBlue2AutoOff()
        {
            await GlobalSettings.ResetBlue2AutoOff();
            IsSleeping = false;
        }

        /// <summary>
        /// GlobalSettings_GotTemperatureReading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="val"></param>
        private void GlobalSettings_GotTemperatureReading(object sender, string val)
        {
            try
            {
                if (!HasAnyPairedDevice)
                    return;

                var temperatureString = val.Replace(" ", string.Empty); //eg value: 28.4�C
                IsHigh = IsLow = IsSleeping = IsBatteryLow = false;
                Debug.WriteLine("Temperature String:{0}", temperatureString);

                if (!string.IsNullOrEmpty(temperatureString))
                {
                    if (temperatureString.ToLower() == HaccpConstant.Blue2SleepStateValue)
                    {
                        IsSleeping = true;
                        MessagingCenter.Send(new BleTemperatureReadingMessage
                        {
                            IsSleeping = true
                        }, HaccpConstant.BletemperatureReading);
                        Debug.WriteLine("Blue2 device in light sleep mode...");
                    }
                    else if (temperatureString.ToLower().Contains(HaccpConstant.Blue2HighStateValue))
                    {
                        IsHigh = true;
                        MessagingCenter.Send(new BleTemperatureReadingMessage
                        {
                            IsHigh = true
                        }, HaccpConstant.BletemperatureReading);
                        Debug.WriteLine("Blue2 temperarture reading is high...");
                    }
                    else if (temperatureString.ToLower().Contains(HaccpConstant.Blue2LowStateValue))
                    {
                        IsLow = true;
                        MessagingCenter.Send(new BleTemperatureReadingMessage
                        {
                            IsLow = true
                        }, HaccpConstant.BletemperatureReading);
                        Debug.WriteLine("Blue2 temperarture reading is low...");
                    }
                    else if (temperatureString.ToLower().Contains(HaccpConstant.Blue2BatteryLowState))
                    {
                        IsBatteryLow = true;
                        MessagingCenter.Send(new BleTemperatureReadingMessage
                        {
                            IsBatteryLow = true
                        }, HaccpConstant.BletemperatureReading);
                        Debug.WriteLine("Blue2 device battery is low...");
                    }
                    else if (temperatureString.Length > 2)
                    {
                        IsHigh = IsLow = IsSleeping = IsBatteryLow = false;
                        var lastchar = temperatureString[temperatureString.Length - 1];
                        //last character will be 'S' if blue2 button press
                        string temperature;
                        short unit;
                        bool shouldRecord;

                        if (lastchar == 'S')
                        {
                            // if last char is 'S' the temperature must be automatically record
                            temperature = temperatureString.Substring(0, temperatureString.Length - 3);
                            // remove the last 2 character to obtain the actual value
                            unit = temperatureString[temperatureString.Length - 2].ToString().ToUpper() == "C"
                                ? (short)0
                                : (short)1;
                            shouldRecord = true;
                        }
                        else
                        {
                            temperature = temperatureString.Substring(0, temperatureString.Length - 2);
                            // remove the last 2 character to obtain the actual value
                            unit = temperatureString[temperatureString.Length - 1].ToString().ToUpper() == "C"
                                ? (short)0
                                : (short)1;
                            shouldRecord = false;
                        }


                        var doublevalue = HACCPUtil.ConvertToDouble(temperature);

                        LastReading = doublevalue;
                        LastUnit = unit == (short)0 ? TemperatureUnit.Celcius : TemperatureUnit.Fahrenheit;

                        MessagingCenter.Send(new BleTemperatureReadingMessage
                        {
                            TempUnit = unit,
                            TempValue = doublevalue,
                            ShouldRecord = shouldRecord
                        }, HaccpConstant.BletemperatureReading);

                        GotTemperartureReading = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error occured while reading temperature characteristics {0}", ex.Message);
            }
        }

        #endregion
    }
}