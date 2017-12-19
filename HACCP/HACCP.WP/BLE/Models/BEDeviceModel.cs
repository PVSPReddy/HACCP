using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using HACCP.WP.BLE.ViewModels;

namespace HACCP.WP.BLE.Models
{
    /// <summary>
    ///     A model class to handle data manipulations. Manipulations to this class will push
    ///     changes to the corresponding view model instances, which is bound to the UI.
    ///     This model is a wrapper around the BluetoothLEDevice class.
    /// </summary>
    public class BEDeviceModel : BEGattModelBase<BluetoothLEDevice>
    {
        #region ------------------------------ Properties ------------------------------

        public List<BEServiceModel> ServiceModels { get; private set; }
        private BluetoothLEDevice _device { get; set; }
        public event EventHandler<bool> DeviceConnectionStatusChanged;

        public string Name
        {
            get { return _device.Name.Trim(); }
        } // sanitized to remove spaces

        public ulong BluetoothAddress
        {
            get { return _device.BluetoothAddress; }
        }

        public string DeviceId
        {
            get { return _device.DeviceId; }
        }

        public bool Connected;

        #endregion // Properties

        #region ------------------------------ Constructor/Initialize ------------------------------

        public BEDeviceModel()
        {
            ServiceModels = new List<BEServiceModel>();
            _viewModelInstances = new List<BEGattVMBase<BluetoothLEDevice>>();
        }

        /// <summary>
        ///     Initialization
        /// </summary>
        /// <param name="device"></param>
        /// <param name="deviceInfo"></param>
        public void Initialize(BluetoothLEDevice device, DeviceInformation deviceInfo)
        {
            // Check for valid input
            if (device == null)
            {
                throw new ArgumentNullException(string.Format("Dev{0}ice cannot be null.", "ARG0"));
            }
            if (deviceInfo == null)
            {
                throw new ArgumentNullException(string.Format("{0} DeviceInformation cannot be null.", "In BEDeviceVM,"));
            }

            // Initialize variables
            _device = device;
            if (_device.ConnectionStatus == BluetoothConnectionStatus.Connected)
            {
                Connected = true;
            }

            foreach (var service in _device.GattServices)
            {
                var serviceM = new BEServiceModel();
                serviceM.Initialize(service, this);
                ServiceModels.Add(serviceM);
            }


            // Register event handlers
            _device.ConnectionStatusChanged += OnConnectionStatusChanged;
            _device.NameChanged += OnNameChanged;
            // _device.GattServicesChanged += OnGattervicesChanged;

            // Register for notifications from the device, on a separate thread
            //
            // NOTE:
            // This has the effect of telling the OS that we're interested in
            // these devices, and for it to automatically connect to them when
            // they are advertising.


            Utilities.RunFuncAsTask(RegisterNotificationsAsync);
        }

        #endregion

        #region ---------------------------- Event Handlers ----------------------------

        /// <summary>
        ///     NameChanged event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="obj"></param>
        private void OnNameChanged(BluetoothLEDevice sender, object obj)
        {
            SignalChanged("Name");
        }

        /// <summary>
        ///     ConnectionStatusChanged event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="obj"></param>
        private void OnConnectionStatusChanged(BluetoothLEDevice sender, object obj)
        {
            Debug.WriteLine("OnConnectionStatusChanged {0}:", _device.ConnectionStatus);
            bool value = _device.ConnectionStatus == BluetoothConnectionStatus.Connected;
            if (value != Connected)
            {
                // Change internal boolean and signal UI
                Connected = value;
                SignalChanged("ConnectString");
                SignalChanged("ConnectColor");
            }

            if (DeviceConnectionStatusChanged != null)
                DeviceConnectionStatusChanged(this, value);
        }

        #endregion // event handlers

        #region ---------------------------- Registering Notifications ----------------------------

        /// <summary>
        ///     Registers notifications for all characteristics in all services in this device
        /// </summary>
        private bool _notificationsRegistered;

        public async Task RegisterNotificationsAsync()
        {
            // Don't need to register notifications multiple times. 
            if (_notificationsRegistered)
            {
                return;
            }

            foreach (var serviceM in ServiceModels)
            {
                await serviceM.RegisterNotificationsAsync();
            }

            // Notifications now registered.
            _notificationsRegistered = true;
        }

        /// <summary>
        ///     Unregisters notifications for all characteristics in all services in this devices
        /// </summary>
        /// <returns></returns>
        public async Task UnregisterNotificationsAsync()
        {
            try
            {
                foreach (var serviceM in ServiceModels)
                {
                    await serviceM.UnregisterNotificationsAsync();
                }
            }
            catch (Exception ex)
            {
                // There's a chance the unregister will fail, as the device has been removed.
                // Utilities.OnExceptionWithMessage(ex, "This failure may be expected as we're trying to unregister a device upon removal.");
                Debug.WriteLine(
                    "This failure may be expected as we're trying to unregister a device upon removal.  {0}", ex.Message);
            }

            _notificationsRegistered = false;
        }

        #endregion // registering notifications
    }
}