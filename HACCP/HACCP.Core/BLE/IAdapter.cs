using System;
using System.Collections.Generic;

namespace HACCP.Core
{
    public interface IAdapter
    {
        // properties
        bool IsScanning { get; }

        IList<IDevice> DiscoveredDevices { get; }

        IList<IDevice> ConnectedDevices { get; }

        bool IsBluetoothEnabled { get; }
        // events
        event EventHandler<DeviceDiscoveredEventArgs> DeviceDiscovered;
        event EventHandler<DeviceConnectionEventArgs> DeviceConnected;
        event EventHandler<DeviceConnectionEventArgs> DeviceDisconnected;
        event EventHandler BluetoothStateChanged;
        //TODO: add this
        event EventHandler<DeviceConnectionEventArgs> DeviceFailedToConnect;
        event EventHandler ScanTimeoutElapsed;

        //TODO: add this
        event EventHandler ConnectTimeoutElapsed;

        // methods
        void StartScanningForDevices();

        void StartScanningForDevices(Guid serviceUuid);

        void StopScanningForDevices();

        void ConnectToDevice(IDevice device);

        void DisconnectDevice(IDevice device);

        void CancelScanning();
    }
}