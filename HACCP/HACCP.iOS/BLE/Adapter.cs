using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using HACCP.Core;
#if __UNIFIED__
using CoreBluetooth;
using CoreFoundation;

#else
using MonoTouch.CoreBluetooth;
using MonoTouch.CoreFoundation;
#endif

namespace HACCP.iOS
{
    public class Adapter : IAdapter
    {
        private readonly AutoResetEvent stateChanged = new AutoResetEvent(false);

        protected CBCentralManager _central;

        protected IList<IDevice> _connectedDevices = new List<IDevice>();

        protected IList<IDevice> _discoveredDevices = new List<IDevice>();

        protected bool _isConnecting;


        protected bool _isScanning;

        private bool cancelScan;

        static Adapter()
        {
            Current = new Adapter();
        }


        protected Adapter()
        {
            _central = new CBCentralManager(DispatchQueue.CurrentQueue);

            _central.DiscoveredPeripheral += (sender, e) =>
            {
                try
                {
                    Debug.WriteLine("DiscoveredPeripheral: " + e.Peripheral.Name);
                    if (e.Peripheral != null)
                    {
                        var d = new Device(e.Peripheral);
                        if (!ContainsDevice(_discoveredDevices, e.Peripheral))
                        {
                            _discoveredDevices.Add(d);
                            DeviceDiscovered(this, new DeviceDiscoveredEventArgs {Device = d});
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error!! : DiscoveredPeripheral: " + ex.Message);
                }
            };

            _central.UpdatedState += (sender, e) =>
            {
                try
                {
                    Console.WriteLine("UpdatedState: " + _central.State);


                    IsBluetoothEnabled = _central.State == CBCentralManagerState.PoweredOn;


                    if (BluetoothStateChanged != null)
                        BluetoothStateChanged(this, e);
                    stateChanged.Set();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error!! : UpdatedState: " + ex.Message);
                }
            };


            _central.ConnectedPeripheral += (sender, e) =>
            {
                try
                {
                    Debug.WriteLine("ConnectedPeripheral: " + e.Peripheral.Name);

                    // when a peripheral gets connected, add that peripheral to our running list of connected peripherals
                    if (!ContainsDevice(_connectedDevices, e.Peripheral))
                    {
                        if (e.Peripheral != null)
                        {
                            cancelScan = true;
                            _isConnecting = false;
                            var d = new Device(e.Peripheral);
                            _connectedDevices.Add(new Device(e.Peripheral));
                            // raise our connected event
                            DeviceConnected(sender, new DeviceConnectionEventArgs {Device = d});
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error!! : ConnectedPeripheral: " + ex.Message);
                }
            };

            _central.DisconnectedPeripheral += (sender, e) =>
            {
                try
                {
                    Debug.WriteLine("DisconnectedPeripheral: " + e.Peripheral.Name);

                    // when a peripheral disconnects, remove it from our running list.
                    IDevice foundDevice = null;
                    foreach (var d in _connectedDevices)
                    {
                        if (d.ID == Guid.ParseExact(e.Peripheral.Identifier.AsString(), "d"))
                            foundDevice = d;
                    }
                    if (foundDevice != null)
                        _connectedDevices.Remove(foundDevice);

                    // raise our disconnected event
                    DeviceDisconnected(sender, new DeviceConnectionEventArgs {Device = new Device(e.Peripheral)});
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error!! : DisconnectedPeripheral: " + ex.Message);
                }
            };

            _central.FailedToConnectPeripheral += (sender, e) =>
            {
                try
                {
                    // raise the failed to connect event
                    if (e.Peripheral != null)
                    {
                        DeviceFailedToConnect(this, new DeviceConnectionEventArgs
                        {
                            Device = new Device(e.Peripheral),
                            ErrorMessage = e.Error.Description
                        });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error!! : FailedToConnectPeripheral: " + ex.Message);
                }
            };
        }

        public CBCentralManager Central
        {
            get { return _central; }
        }

        public bool IsConnecting
        {
            get { return _isConnecting; }
        }

        public static Adapter Current { get; private set; }

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

        public void StartScanningForDevices()
        {
            StartScanningForDevices(Guid.Empty);
        }

        public async void StartScanningForDevices(Guid serviceUuid)
        {
            //
            // Wait for the PoweredOn state
            //
            await WaitForState(CBCentralManagerState.PoweredOn);

            Debug.WriteLine("Adapter: Starting a scan for devices.");

            CBUUID[] serviceUuids = null; // TODO: convert to list so multiple Uuids can be detected
            if (serviceUuid != Guid.Empty)
            {
                var suuid = CBUUID.FromString(serviceUuid.ToString());
                serviceUuids = new[] {suuid};
                Debug.WriteLine("Adapter: Scanning for " + suuid);
            }

            // clear out the list
            _discoveredDevices = new List<IDevice>();

            // start scanning
            _isScanning = true;

            _central.ScanForPeripherals(serviceUuids);


            // in 20 seconds, stop the scan
            //	await Task.Delay (20000, source.Token);

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
                Console.WriteLine("BluetoothLEManager: Scan timeout has elapsed.");
                _isScanning = false;
                _central.StopScan();
                ScanTimeoutElapsed(this, new EventArgs());
            }
        }


        public void CancelScanning()
        {
            cancelScan = true;
        }


        public void StopScanningForDevices()
        {
            Console.WriteLine("Adapter: Stopping the scan for devices.");
            _isScanning = false;
            _central.StopScan();
        }

        public async void ConnectToDevice(IDevice device)
        {
            _isConnecting = true;
            //TODO: if it doesn't connect after 10 seconds, cancel the operation
            // (follow the same model we do for scanning).
            _central.ConnectPeripheral(device.NativeDevice as CBPeripheral, new PeripheralConnectionOptions());

            //			// in 10 seconds, stop the connection
            await Task.Delay(5000);
            //
            //			// if we're still trying to connect
            if (_isConnecting)
            {
                Console.WriteLine("BluetoothLEManager: Connect timeout has elapsed.");

                ConnectTimeoutElapsed(this, new EventArgs());
            }
        }

        public void DisconnectDevice(IDevice device)
        {
            if (device.State == DeviceState.Connected)
                _central.CancelPeripheralConnection(device.NativeDevice as CBPeripheral);
            else
                Debug.WriteLine("Not connected.. Still trying to disconnect..");
        }

        private async Task WaitForState(CBCentralManagerState state)
        {
            Debug.WriteLine("Adapter: Waiting for state: " + state);

            while (_central.State != state)
            {
                await Task.Run(() => stateChanged.WaitOne());
            }
        }

        // util
        protected bool ContainsDevice(IEnumerable<IDevice> list, CBPeripheral device)
        {
            foreach (var d in list)
            {
                if (Guid.ParseExact(device.Identifier.AsString(), "d") == d.ID)
                    return true;
            }
            return false;
        }
    }
}