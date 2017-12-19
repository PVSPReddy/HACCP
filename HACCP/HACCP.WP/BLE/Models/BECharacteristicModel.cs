﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage;
using Windows.Storage.Streams;
using HACCP.Core;
using HACCP.WP.BLE.Dictionary;
using HACCP.WP.BLE.Dictionary.DataParser;
using HACCP.WP.BLE.ViewModels;

namespace HACCP.WP.BLE.Models
{
    /// <summary>
    ///     A model class to handle data manipulations. Manipulations to this class will push
    ///     changes to the corresponding view model instances, which is bound to the UI.
    ///     This model is a wrapper around a single Gatt Characteristic.
    /// </summary>
    public class BECharacteristicModel : BEGattModelBase<GattCharacteristic>
    {
        #region ----------------------------- Properties -----------------------------

        private GattCharacteristic _characteristic { get; set; }

        protected string _name;

        public string Name
        {
            get { return _name; }
            protected set
            {
                _name = value;
                SignalChanged("Name");
            }
        }

        public Guid Uuid
        {
            get { return _characteristic.Uuid; }
        }

        private string _characteristicValue;

        public string CharacteristicValue
        {
            get { return _characteristicValue; }
            protected set
            {
                _characteristicValue = value;
                SignalChanged("CharacteristicValue");
            }
        }

        private CharacteristicDictionaryEntry.ReadUnknownAsEnum _displayType;

        public CharacteristicDictionaryEntry.ReadUnknownAsEnum DisplayType
        {
            get { return _displayType; }
            private set
            {
                _displayType = value;
                SignalChanged("DisplayType");
            }
        }

        public bool Default { get; private set; }
        public BEServiceModel ServiceM { get; private set; }
        public bool Toastable { get; private set; }
        public bool Writable { get; private set; }
        public bool Readable { get; private set; }

        #endregion Properties

        #region ----------------------------- Constructor/Initialization -----------------------------

        public BECharacteristicModel(bool readable)
        {
            Readable = readable;
            _viewModelInstances = new List<BEGattVMBase<GattCharacteristic>>();
            Name = CharacteristicDictionaryEntry.CHARACTERISTIC_MISSING_STRING;
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        ///     Performs the bare minimum initialization of this class.
        /// </summary>
        /// <param name="serviceM"></param>
        /// <param name="characteristic"></param>
        public void Initialize(BEServiceModel serviceM, GattCharacteristic characteristic)
        {
            if (serviceM == null)
            {
                throw new ArgumentNullException(string.Format(
                    "{0}In BECharacteristicVM, BEServiceModel cannot be null.", "ARG0"));
            }

            ServiceM = serviceM;

            if (characteristic == null)
            {
                throw new ArgumentNullException(
                    string.Format("{0}In BECharacteristicVM, GattCharacteristic cannot be null.", "ARG0"));
            }

            _characteristic = characteristic;
         //   CharacteristicValue = CHARACTERISTIC_VALUE_DEFAULT_STRING;
            GetDictionaryAndUpdateProperties();

            Writable |= (_characteristic.CharacteristicProperties & GattCharacteristicProperties.WriteWithoutResponse) !=
                        0;
            Writable |= (_characteristic.CharacteristicProperties & GattCharacteristicProperties.Write) != 0;

          //  ToastInit();
        }

        #endregion // Constructor/Initialization

        #region ----------------------------- Dictionary -----------------------------

        private CharacteristicDictionaryEntry _dictionaryEntry;

        /// <summary>
        ///     Looks up the dictionary entry corresponding to this characteristic, to determine the
        ///     type of parsers needed, for example.
        /// </summary>
        private void GetDictionaryAndUpdateProperties()
        {
            GetDictionaryEntry();
       //     UpdatePropertiesFromDictionaryEntry();
        }

        /// <summary>
        ///     Gets dictionary entry if it exists. Otherwise, adds to unknown dictionary, then gets.
        /// </summary>
        private void GetDictionaryEntry()
        {
            if (GlobalSettings.CharacteristicDictionaryConstant.ContainsKey(Uuid))
            {
                _dictionaryEntry = GlobalSettings.CharacteristicDictionaryConstant[Uuid];
            }
            else if (GlobalSettings.CharacteristicDictionaryUnknown.ContainsKey(Uuid))
            {
                _dictionaryEntry = GlobalSettings.CharacteristicDictionaryUnknown[Uuid];
            }
            else
            {
                _dictionaryEntry = new CharacteristicDictionaryEntry();
                _dictionaryEntry.Initialize(Uuid);
                GlobalSettings.CharacteristicDictionaryUnknown.Add(Uuid, _dictionaryEntry);
            }
        }

        /// <summary>
        ///     Updates properties on this class based on dictionary entry
        /// </summary>
        private void UpdatePropertiesFromDictionaryEntry()
        {
            if (_dictionaryEntry != null)
            {
                Name = _dictionaryEntry.Name;
                Default = _dictionaryEntry.IsDefault;
                DisplayType = _dictionaryEntry.ReadUnknownAs;
            }
        }

        #endregion // Dictionary

        #region ----------------------------- Changing Dictionary Properties ----------------------------

        /// <summary>
        ///     Updates the name of the characteristic
        /// </summary>
        /// <param name="name"></param>
        public void UpdateName(string name)
        {
            Name = name;
            if (_dictionaryEntry == null)
            {
                throw new NullReferenceException(string.Format(
                    "Dictionary entry not initialized in Characteristic {0}", Name));
            }
            _dictionaryEntry.ChangeFriendlyName(name);
        }

        /// <summary>
        ///     Updates how the data of the value is to be represented in the UI
        /// </summary>
        /// <param name="displayType"></param>
        /// <returns></returns>
        public async Task ChangeDisplayTypeAsync(CharacteristicDictionaryEntry.ReadUnknownAsEnum displayType)
        {
            // Change display type
            _dictionaryEntry.ChangeBufferUnknownType(displayType);
            DisplayType = displayType;

            // Reread value
            await ReadValueAsync();
        }

        public bool DictionaryModelChanged
        {
            get { return _dictionaryEntry.HasChanged(); }
        }

        public bool IsParserTypeKnown
        {
            get { return _dictionaryEntry.IsParserTypeKnown; }
        }

        #endregion // Changing Dictionary Properties

        #region ----------------------------- Notification Utilities -----------------------------

        private const string NOTIFICATION_NOT_REGISTERED_STRING = "No value read; not connected.";
        private const string CHARACTERISTIC_VALUE_DEFAULT_STRING = "No characteristic value retrieved.";
        private const string WAITING_FOR_NOTIFICATION = "Waiting for notification.";
        private bool _notificationRegistered;

        /// <summary>
        ///     Registers for notifications from the characteristic.
        /// </summary>
        /// <returns></returns>
        public async Task RegisterNotificationAsync()
        {
            // Multiple logic paths can cause registration of event handler; make sure not duplicated. 
            if (_notificationRegistered)
            {
                return;
            }

            // Tell device that we want notifications. Register event handler if so.
            // This can be done whether the device is currently connected or not.  One bonus for doing
            // it while the device isn't connected is that the OS' bluetooth stack will actively watch
            // out for the device and connect to it if it is advertising.
            if ((_characteristic.CharacteristicProperties & GattCharacteristicProperties.Notify) != 0)
            {
                CharacteristicValue = WAITING_FOR_NOTIFICATION;
                _characteristic.ValueChanged +=
                    CharacteristicValueChanged_Handler;
                await _characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                    GattClientCharacteristicConfigurationDescriptorValue.Notify);
                _notificationRegistered = true;
            }
        }

        /// <summary>
        ///     Unregisters for notifications from the Gatt characteristics.
        /// </summary>
        /// <returns></returns>
        public async Task UnregisterNotificationAsync()
        {
            if (_notificationRegistered)
            {
                if (_toastRegistered)
                {
                    // If we have toasts active, we want this to run in the background still. 
                    return;
                }
                _characteristic.ValueChanged -= CharacteristicValueChanged_Handler;
                await _characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                    GattClientCharacteristicConfigurationDescriptorValue.None);
                CharacteristicValue = NOTIFICATION_NOT_REGISTERED_STRING;
                _notificationRegistered = false;
            }
        }

        /// <summary>
        ///     Event handler for changed, registered notifications.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="obj"></param>
        private void CharacteristicValueChanged_Handler(GattCharacteristic sender, GattValueChangedEventArgs obj)
        {
            if (_characteristic.Service.Device.ConnectionStatus != BluetoothConnectionStatus.Connected)
            {
                return;
            }
            CharacteristicValue = _dictionaryEntry.ParseReadValue(obj.CharacteristicValue);
        }

        #endregion // Notification Utilities

        #region ----------------------------- Toasts -----------------------------

        public static string TOAST_STRING_PREFIX = "TOASTNAME";
        private string _toastName;
        private bool _toastRegistered;

        public bool ToastRegistered
        {
            get { return _toastRegistered; }
            private set
            {
                if (value != _toastRegistered)
                {
                    _toastRegistered = value;
                    SignalChanged("ToastRegistered");
                }
            }
        }

        private bool _toastButtonActive;

        public BECharacteristicModel()
        {
          //  throw new NotImplementedException();
        }

        public bool ToastButtonActive
        {
            get { return _toastButtonActive; }
            private set
            {
                _toastButtonActive = value;
                SignalChanged("ToastButtonActive");
            }
        }

        /// <summary>
        ///     Initializes the basic properties for adding a gatt characteristic
        ///     notification trigger
        /// </summary>
        private void ToastInit()
        {
            _toastName = string.Format("{0}{1}{2}", TOAST_STRING_PREFIX,
                _characteristic.Service.Device.DeviceId.GetHashCode(), Uuid.GetHashCode());
            Toastable = (_characteristic.CharacteristicProperties & GattCharacteristicProperties.Notify) != 0;
            ToastRegistered = ApplicationData.Current.LocalSettings.Values.ContainsKey(_toastName);
            ToastButtonActive = true;

            if (ToastRegistered)
            {
                // We need to add ourselves to the list of active toasts
                GlobalSettings.AddToast(this);
            }
        }

        /// <summary>
        ///     Test for equality
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool ToastEquals(BECharacteristicModel model)
        {
            return _toastName.Equals(model._toastName);
        }

        /// <summary>
        ///     Handles a click on the button to enable/disable toasts
        /// </summary>
        /// <returns></returns>
        public async Task ToastClickAsync()
        {
            if (!ToastRegistered)
            {
                // RegisterBackgroundTasksForToast();
            }
            else
            {
                await TaskUnregisterForToastAsync();
                ToastRegistered = false;
            }
        }


        /// <summary>
        ///     Async task unregistration
        /// </summary>
        /// <returns></returns>
        public async Task TaskUnregisterForToastAsync()
        {
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == _toastName)
                {
                    // Take some action based on finding the background task.
                    await TaskUnregisterHelperAsync(cur.Value);
                    GlobalSettings.RemoveToast(this);
                }
            }
            ToastRegistered = false;
        }

        public async Task TaskUnregisterInsideListAsync()
        {
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == _toastName)
                {
                    // Take some action based on finding the background task.
                    await TaskUnregisterHelperAsync(cur.Value);
                }
            }
            ToastRegistered = false;
        }

        /// <summary>
        ///     Helper to unregister notification toasts
        /// </summary>
        /// <param name="taskRegistration"></param>
        /// <returns></returns>
        private async Task TaskUnregisterHelperAsync(IBackgroundTaskRegistration taskRegistration)
        {
            // Do the unregistration
            taskRegistration.Unregister(true);

            // Don't need to keep track of the toast name data in local settings anymore. 
            ApplicationData.Current.LocalSettings.Values.Remove(_toastName);

            // If current device isn't active, we don't want to drain the battery by listening
            // for notifications on this device.
            if (GlobalSettings.SelectedDevice != ServiceM.DeviceM)
            {
                await UnregisterNotificationAsync();
            }
        }

        #endregion  // Toasts

        #region ----------------------------- Read Utilities -----------------------------

        /// <summary>
        ///     Reads the value of the characeristic of the realistic.
        /// </summary>
        /// <returns></returns>
        public async Task ReadValueAsync()
        {
            try
            {
                if ((_characteristic.CharacteristicProperties & GattCharacteristicProperties.Read) != 0)
                {
                    // Determine if we want to read the actual or cached values
                    var cacheMode = BluetoothCacheMode.Cached;
                    if (!GlobalSettings.UseCachedMode && ServiceM.DeviceM.Connected)
                    {
                        cacheMode = BluetoothCacheMode.Uncached;
                    }

                    // Get some basic device info. 
                    var readResult = await _characteristic.ReadValueAsync(cacheMode);
                    var gattStatus = readResult.Status;
                    if (gattStatus == GattCommunicationStatus.Success)
                    {
                        CharacteristicValue = _dictionaryEntry.ParseReadValue(readResult.Value);
                    }
                }
            }
            catch (Exception e)
            {
                // Utilities.OnExceptionWithMessage(e, "Failed to read characteristic value");
                Debug.WriteLine("Failed to read characteristic value {0}", e.Message);
            }
        }

        public async Task ReadUTF8Value()
        {
            try
            {
                if ((_characteristic.CharacteristicProperties & GattCharacteristicProperties.Read) != 0)
                {
                    // Determine if we want to read the actual or cached values
                    var cacheMode = BluetoothCacheMode.Cached;
                    if (!GlobalSettings.UseCachedMode && ServiceM.DeviceM.Connected)
                    {
                        cacheMode = BluetoothCacheMode.Uncached;
                    }

                    // Get some basic device info. 
                    var readResult = await _characteristic.ReadValueAsync(cacheMode);
                    var gattStatus = readResult.Status;
                    if (gattStatus == GattCommunicationStatus.Success)
                    {
                        CharacteristicValue = BasicParsers.ParseUtf8(readResult.Value);
                    }
                }
            }
            catch (Exception e)
            {
                // Utilities.OnExceptionWithMessage(e, "Failed to read characteristic value");
                Debug.WriteLine("Failed to read characteristic value {0}", e.Message);
            }
        }

        #endregion

        #region  ----------------------------- Write Utilities -----------------------------

        public async void UpdateProbe(string message)
        {
            if (!Writable)
            {
                throw new InvalidOperationException("Should not be able to write to this characteristic");
            }

            var writer = new DataWriter();

            // This currently only writes as byte. (Therefore, the entire system can only write a byte at once...)
            var parseSuccess = FillDatawriterWithBytes(message, writer);
            if (!parseSuccess)
            {
                return;
            }

            // Write buffer to device
            if (_characteristic.Service.Device.ConnectionStatus == BluetoothConnectionStatus.Connected)
            {
                try
                {
                    if ((_characteristic.CharacteristicProperties & GattCharacteristicProperties.WriteWithoutResponse) !=
                        0)
                    {
                        await _characteristic.WriteValueAsync(
                            writer.DetachBuffer(), GattWriteOption.WriteWithoutResponse);
                    }
                    else
                    {
                        await _characteristic.WriteValueAsync(
                            writer.DetachBuffer(), GattWriteOption.WriteWithResponse);
                        await ReadValueAsync();
                    }
                }
                catch (Exception e)
                {
                    /*
                     * Error code used by device in ErrorResponse    WinRT HResult in Exception                 GATT Definition
                     * 0x00	                                         E0420000                                   Undefined by GATT
                     * 0x01 through 0x11                             80650001 through  80650011                 Defined by GATT
                     * 0x12 through 0x7F                             80651000 (E_BLUETOOTH_ATT_UNKNOWN_ERROR)   Reserved by GATT
                     * 0x7F through 0xFF                             E0420080 through E04200FF                  Application Errors
                     * 
                     * Other errors can be found here:
                     * http://msdn.microsoft.com/en-us/library/windows/hardware/hh450806(v=vs.85).aspx
                     */

                    var hr = (uint) e.HResult;
                    var highBytes = hr >> 16;
                    var lowBytes = hr & 0xFFFF;

                    if (hr == 0x80651000 && hr == 0xE0420000)
                    {
                        Utilities.OnExceptionWithMessage(e, "Write failed, unknown error :(");
                    }
                    else if (highBytes == 0xE042 ||
                             (highBytes == 0x8065 && lowBytes > 0 && lowBytes < 12))
                    {
                        Utilities.OnExceptionWithMessage(
                            e,
                            "Write failed, device ErrorResponse: 0x" + lowBytes.ToString("x"));
                    }
                    else
                    {
                        Utilities.OnException(e);
                    }
                }
            }
            else
            {
                // if device isn't connected, throw an exception. 
                Utilities.OnException(new InvalidOperationException("Cannot write to device while disconnected!"));
            }
        }

        /// <summary>
        ///     Writes some bytes to the connected BTLE device
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task WriteMessageAsync(string message)
        {
            if (!Writable)
            {
                throw new InvalidOperationException("Should not be able to write to this characteristic");
            }

            var writer = new DataWriter();

            // This currently only writes as byte. (Therefore, the entire system can only write a byte at once...)
            var parseSuccess = FillDatawriterWithByte(message, writer);
            if (!parseSuccess)
            {
                return;
            }

            // Write buffer to device
            if (_characteristic.Service.Device.ConnectionStatus == BluetoothConnectionStatus.Connected)
            {
                try
                {
                    if ((_characteristic.CharacteristicProperties & GattCharacteristicProperties.WriteWithoutResponse) !=
                        0)
                    {
                        await _characteristic.WriteValueAsync(
                            writer.DetachBuffer(), GattWriteOption.WriteWithoutResponse);
                    }
                    else
                    {
                        await _characteristic.WriteValueAsync(
                            writer.DetachBuffer(), GattWriteOption.WriteWithResponse);
                        await ReadValueAsync();
                    }
                }
                catch (Exception e)
                {
                    /*
                     * Error code used by device in ErrorResponse    WinRT HResult in Exception                 GATT Definition
                     * 0x00	                                         E0420000                                   Undefined by GATT
                     * 0x01 through 0x11                             80650001 through  80650011                 Defined by GATT
                     * 0x12 through 0x7F                             80651000 (E_BLUETOOTH_ATT_UNKNOWN_ERROR)   Reserved by GATT
                     * 0x7F through 0xFF                             E0420080 through E04200FF                  Application Errors
                     * 
                     * Other errors can be found here:
                     * http://msdn.microsoft.com/en-us/library/windows/hardware/hh450806(v=vs.85).aspx
                     */

                    var hr = (uint) e.HResult;
                    var highBytes = hr >> 16;
                    var lowBytes = hr & 0xFFFF;

                    if (hr == 0x80651000 && hr == 0xE0420000)
                    {
                        //  Utilities.OnExceptionWithMessage(e, "Write failed, unknown error :(");
                        Debug.WriteLine("Write failed, unknown error");
                    }
                    else if (highBytes == 0xE042 ||
                             (highBytes == 0x8065 && lowBytes > 0 && lowBytes < 12))
                    {
                        // Utilities.OnExceptionWithMessage(e,"Write failed, device ErrorResponse: 0x" + lowBytes.ToString("x"));
                        Debug.WriteLine("Write failed, device ErrorResponse: 0x");
                    }
                    else
                    {
                        // Utilities.OnException(e);
                        Debug.WriteLine(e.Message);
                    }
                }
            }
            else
            {
                // if device isn't connected, throw an exception. 
                // Utilities.OnException(new InvalidOperationException("Cannot write to device while disconnected!"));
                Debug.WriteLine("Cannot write to device while disconnected!");
            }
        }

        /// <summary>
        ///     Helper function that converts a string to a byte, and writes it to the DataWriter
        /// </summary>
        /// <param name="message"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        private bool FillDatawriterWithByte(string message, DataWriter writer)
        {
            byte writeme;
            try
            {
                writeme = Convert.ToByte(message);
            }
            catch (Exception)
            {
                // Utilities.OnExceptionWithMessage(e, "Needs to be a decimal from 0-255.");
                Debug.WriteLine("Needs to be a decimal from 0-255.");
                return false;
            }
            writer.WriteByte(writeme);
            return true;
        }


        /// <summary>
        ///     Helper function that converts a string to a byte, and writes it to the DataWriter
        /// </summary>
        /// <param name="message"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        private bool FillDatawriterWithBytes(string message, DataWriter writer)
        {
            byte[] bytes;
            try
            {
                bytes = HACCPUtil.GetBytesFromString(message);
            }
            catch (Exception)
            {
                // Utilities.OnExceptionWithMessage(e, "Needs to be a decimal from 0-255.");
                Debug.WriteLine("Needs to be a decimal from 0-255.");
                return false;
            }

            writer.WriteBytes(bytes);
            return true;
        }

        #endregion // Write utilities
    }
}