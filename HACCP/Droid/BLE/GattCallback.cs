using System;
using System.Collections.Generic;
using Android.Bluetooth;
using HACCP.Core;

namespace HACCP.Droid
{
    public class GattCallback : BluetoothGattCallback
    {
        protected Adapter _adapter;

        public GattCallback(Adapter adapter)
        {
            _adapter = adapter;
        }

        public IList<BluetoothGattService> Services { get; set; }

        public event EventHandler<DeviceConnectionEventArgs> DeviceConnected = delegate { };
        public event EventHandler<DeviceConnectionEventArgs> DeviceDisconnected = delegate { };
        public event EventHandler<ServicesDiscoveredEventArgs> ServicesDiscovered = delegate { };
        public event EventHandler<CharacteristicReadEventArgs> CharacteristicValueUpdated = delegate { };


        public override void OnConnectionStateChange(BluetoothGatt gatt, GattStatus status, ProfileState newState)
        {
            Console.WriteLine(@"OnConnectionStateChange: ");

            base.OnConnectionStateChange(gatt, status, newState);

            //TODO: need to pull the cached RSSI in here, or read it (requires the callback)
            var device = new Device(gatt.Device, gatt, this, 0);

            if (status == GattStatus.Failure)
            {
                BLEManager.SharedInstance.IsBLERefrehing = true;
                device.Close();
                _adapter._adapter.Disable();
                return;
            }
            try
            {
                switch (newState)
                {
                    // disconnected
                    case ProfileState.Disconnected:
                        Console.WriteLine(@"disconnected");

                        DeviceDisconnected(this, new DeviceConnectionEventArgs {Device = device});
                        break;
                    // connecting
                    case ProfileState.Connecting:
                        Console.WriteLine(@"Connecting");
                        break;
                    // connected
                    case ProfileState.Connected:
                        Console.WriteLine(@"Connected");
                        DeviceConnected(this, new DeviceConnectionEventArgs {Device = device});
                        break;
                    // disconnecting
                    case ProfileState.Disconnecting:
                        Console.WriteLine(@"Disconnecting");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Error OnConnectionStateChange {0}", ex.Message);
            }
        }

        public override void OnServicesDiscovered(BluetoothGatt gatt, GattStatus status)
        {
            base.OnServicesDiscovered(gatt, status);
            Services = gatt.Services;

            Console.WriteLine(@"OnServicesDiscovered: " + status);

            ServicesDiscovered(this, new ServicesDiscoveredEventArgs());
        }

        public override void OnDescriptorRead(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, GattStatus status)
        {
            base.OnDescriptorRead(gatt, descriptor, status);

            Console.WriteLine(@"OnDescriptorRead: " + descriptor);
        }

        public override void OnCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic,
            GattStatus status)
        {
            base.OnCharacteristicRead(gatt, characteristic, status);

            Console.WriteLine(@"OnCharacteristicRead: " + characteristic.GetStringValue(0));

            CharacteristicValueUpdated(this, new CharacteristicReadEventArgs
            {
                Characteristic = new Characteristic(characteristic, gatt, this)
            }
                );
        }

        public override void OnCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
        {
            base.OnCharacteristicChanged(gatt, characteristic);

            Console.WriteLine(@"OnCharacteristicChanged: " + characteristic.GetStringValue(0));

            CharacteristicValueUpdated(this, new CharacteristicReadEventArgs
            {
                Characteristic = new Characteristic(characteristic, gatt, this)
            }
                );
        }
    }
}