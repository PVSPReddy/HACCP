using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Android.Bluetooth;
using HACCP.Core;

namespace HACCP.Droid
{
    public class Device : DeviceBase
    {
        /// <summary>
        ///     we also track this because of gogole's weird API. the gatt callback is where
        ///     we'll get notified when services are enumerated
        /// </summary>
        protected GattCallback _gattCallback;

        protected BluetoothDevice _nativeDevice;

        protected int _rssi;

        protected IList<IService> _services = new List<IService>();

        /// <summary>
        ///     we have to keep a reference to this because Android's api is weird and requires
        ///     the GattServer in order to do nearly anything, including enumerating services
        ///     TODO: consider wrapping the Gatt and Callback into a single object and passing that around instead.
        /// </summary>
        private BluetoothGatt ss;

        public Device(BluetoothDevice nativeDevice, BluetoothGatt gatt,
            GattCallback gattCallback, int rssi)
        {
            try
            {
                _nativeDevice = nativeDevice;
                _gatt = gatt;
                _gattCallback = gattCallback;
                _rssi = rssi;

                // when the services are discovered on the gatt callback, cache them here
                if (_gattCallback != null)
                {
                    _gattCallback.ServicesDiscovered += (s, e) =>
                    {
                        if (_gattCallback.Services != null)
                        {
                            var services = _gattCallback.Services;

                            _services = new List<IService>();
                            foreach (var item in services)
                            {
                                _services.Add(new Service(item, _gatt, _gattCallback));
                            }
                            if (ServicesDiscovered != null) ServicesDiscovered(this, e);
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error :{0}", ex.Message);
            }
        }

        protected BluetoothGatt _gatt
        {
            get { return ss; }
            set { ss = value; }
        }

        public override string DeviceGUID
        {
            get { return ID.ToString(); }
        }

        public override Guid ID
        {
            get
            {
                //TODO: verify - fix from Evolve player
                var deviceGuid = new byte[16];
                var macWithoutColons = _nativeDevice.Address.Replace(":", "");
                var macBytes = Enumerable.Range(0, macWithoutColons.Length)
                    .Where(x => x%2 == 0)
                    .Select(x => Convert.ToByte(macWithoutColons.Substring(x, 2), 16))
                    .ToArray();
                macBytes.CopyTo(deviceGuid, 10);
                return new Guid(deviceGuid);
                //return _nativeDevice.Address;
                //return Guid.Empty;
            }
        }

        public override string Name
        {
            get { return _nativeDevice.Name; }
        }

        public override int Rssi
        {
            get { return _rssi; }
        }

        public override object NativeDevice
        {
            get { return _nativeDevice; }
        }

        // TODO: investigate the validity of this. Android API seems to indicate that the
        // bond state is available, rather than the connected state, which are two different
        // things. you can be bonded but not connected.
        public override DeviceState State
        {
            get { return GetState(); }
        }

        //TODO: strongly type IService here
        public override IList<IService> Services
        {
            get { return _services; }
        }

        public override event EventHandler ServicesDiscovered = delegate { };

        #region internal methods

        protected DeviceState GetState()
        {
            switch (_nativeDevice.BondState)
            {
                case Bond.Bonded:
                    return DeviceState.Connected;
                case Bond.Bonding:
                    return DeviceState.Connecting;
                default:
                    return DeviceState.Disconnected;
            }
        }

        #endregion

        #region public methods

        public override void DiscoverServices()
        {
            if (_gatt != null)
                _gatt.DiscoverServices();
        }

        public void Disconnect()
        {
            try
            {
                if (_gatt != null)
                {
                    _gatt.Disconnect();
                    _gatt.Dispose();
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void Close()
        {
            try
            {
                if (_gatt != null)
                {
                    _gatt.Close();
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        #endregion
    }
}