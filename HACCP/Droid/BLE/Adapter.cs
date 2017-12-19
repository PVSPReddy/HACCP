using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Bluetooth;
using BluetoothToggleWidget;
using HACCP.Core;
using Xamarin.Forms;
using Application = Android.App.Application;
using Object = Java.Lang.Object;
using Android.Support.V4.App;
using Android;
using Android.Content.PM;
using Plugin.CurrentActivity;
using Plugin.Permissions.Abstractions;
using Plugin.Permissions;

namespace HACCP.Droid
{
    /// <summary>
    ///     TODO: this really should be a singleton.
    /// </summary>
    public class Adapter : Object, BluetoothAdapter.ILeScanCallback, IAdapter
    {
        public BluetoothAdapter _adapter;
        protected BTToggleWidget _bTToggleWidget;

        protected IList<IDevice> _connectedDevices = new List<IDevice>();

        protected IList<IDevice> _discoveredDevices = new List<IDevice>();
        protected GattCallback _gattCallback;

        protected bool _isScanning;

        // class members
        protected BluetoothManager _manager;

        private bool cancelScan;

        public Adapter()
        {
            var appContext = Application.Context;
            // get a reference to the bluetooth system service
            _manager = (BluetoothManager) appContext.GetSystemService("bluetooth");


            _adapter = _manager.Adapter;

            IsBluetoothEnabled = _adapter.State == State.On;


            _gattCallback = new GattCallback(this);


            _gattCallback.DeviceConnected += (sender, e) =>
            {
                if (!IsScanning)
                {
                    DisconnectDevice(e.Device);
                    return;
                }
                cancelScan = true;

//				if (e.Device.State != DeviceState.Connected)
                if (!ContaisDevice(_connectedDevices, e.Device))
                {
                    _connectedDevices.Add(e.Device);
                    DeviceConnected(this, e);
                }
            };

            _gattCallback.DeviceDisconnected += (sender, e) =>
            {
                // TODO: remove the disconnected device from the _connectedDevices list
                // i don't think this will actually work, because i'm created a new underlying device here.
                //if(this._connectedDevices.Contains(
                var device = e.Device;
                foreach (var item in _connectedDevices)
                {
                    if (item.DeviceGUID == e.Device.DeviceGUID)
                        device = item;
                }

//				if (e.Device.State != DeviceState.Connected) {
                if (e.Device.DeviceGUID == BLEManager.SharedInstance.SelectedDevice.DeviceGUID)
                {
                    _connectedDevices.Remove(device);
                    DeviceDisconnected(this, e);
                    StopScanningForDevices();
                    DiscoveredDevices.Clear();
                }
            };

            MessagingCenter.Subscribe<AndroidBluetoothStatusMessage>(this, DroidConstant.DEVICE_BLUETOOTH_ONOFF_MESSAGE,
                sender =>
                {
                    var msg = sender;
                    IsBluetoothEnabled = msg.IsEnabled;
                    BluetoothStateChanged(this, null);
                });
        }

        // events
        public event EventHandler<DeviceDiscoveredEventArgs> DeviceDiscovered = delegate { };
        public event EventHandler<DeviceConnectionEventArgs> DeviceConnected = delegate { };
        public event EventHandler<DeviceConnectionEventArgs> DeviceDisconnected = delegate { };
        public event EventHandler<DeviceConnectionEventArgs> DeviceFailedToConnect = delegate { };
        public event EventHandler ScanTimeoutElapsed = delegate { };
        public event EventHandler ConnectTimeoutElapsed = delegate { };
        public event EventHandler BluetoothStateChanged = delegate { };

        public bool IsBluetoothEnabled { get; set; }

        public bool IsScanning
        {
            get { return _isScanning; }
        }

        public IList<IDevice> DiscoveredDevices
        {
            get { return _discoveredDevices; }
        }

        public IList<IDevice> ConnectedDevices
        {
            get { return _connectedDevices; }
        }


        //TODO: scan for specific service type eg. HeartRateMonitor
        public void StartScanningForDevices(Guid serviceUuid)
        {
            StartScanningForDevices();
            //			throw new NotImplementedException ("Not implemented on Android yet, look at _adapter.StartLeScan() overload");
        }

        public async void StartScanningForDevices()
        {
            Console.WriteLine(@"Adapter: Starting a scan for devices.");

            // clear out the list
            _discoveredDevices = new List<IDevice>();

            // start scanning
            _isScanning = true;
            //this._adapter.Enable ();
            _adapter.StartLeScan(this);

            cancelScan = false;

            for (var i = 0; i < 20; i++)
            {
                if (!cancelScan)
                    await Task.Delay(1000);
                else
                    break;
            }


            // if we're still scanning
            if (_isScanning)
            {
                Console.WriteLine(@"BluetoothLEManager: Scan timeout has elapsed.");
                _isScanning = false;
                _adapter.StopLeScan(this);
                ScanTimeoutElapsed(this, new EventArgs());
            }
        }

        public void StopScanningForDevices()
        {
            Console.WriteLine(@"Adapter: Stopping the scan for devices.");
            _isScanning = false;
            _adapter.StopLeScan(this);
        }


        public void CancelScanning()
        {
            cancelScan = true;
        }


        public void ConnectToDevice(IDevice device)
        {
            // returns the BluetoothGatt, which is the API for BLE stuff
            // TERRIBLE API design on the part of google here.

            try
            {
                ((BluetoothDevice) device.NativeDevice).ConnectGatt(Application.Context, false, _gattCallback);
            }
            catch (Exception)
            {
                //var s = ex;
            }
        }

        public void DisconnectDevice(IDevice device)
        {
            ((Device) device).Disconnect();
        }


        public void OnLeScan(BluetoothDevice bleDevice, int rssi, byte[] scanRecord)
        {
            Console.WriteLine(@"Adapter.LeScanCallback: " + bleDevice.Name);
            // TODO: for some reason, this doesn't work, even though they have the same pointer,
            // it thinks that the item doesn't exist. so i had to write my own implementation
            //			if(!this._discoveredDevices.Contains(device) ) {
            //				this._discoveredDevices.Add (device );
            //			}

            if (!DeviceExistsInDiscoveredList(bleDevice))
            {
                var device = new Device(bleDevice, null, null, rssi);

                _discoveredDevices.Add(device);
                // TODO: in the cross platform API, cache the RSSI
                // TODO: shouldn't i only raise this if it's not already in the list?
                DeviceDiscovered(this, new DeviceDiscoveredEventArgs {Device = device});
            }
        }


        public bool ContaisDevice(IList<IDevice> list, IDevice device)
        {
            foreach (var item in list)
            {
                if (item.DeviceGUID == device.DeviceGUID)
                    return true;
            }
            return false;
        }

        protected bool DeviceExistsInDiscoveredList(BluetoothDevice device)
        {
            foreach (var d in _discoveredDevices)
            {
                // TODO: verify that address is unique
                if (device.Address == ((BluetoothDevice) d.NativeDevice).Address)
                    return true;
            }
            return false;
        }
    }
}