using System;
using System.Collections.Generic;
using System.Diagnostics;
using HACCP.Core;
#if __UNIFIED__
using CoreBluetooth;

#else
using MonoTouch.CoreBluetooth;
using MonoTouch.Foundation;
#endif

namespace HACCP.iOS
{
    public class Device : DeviceBase
    {
        protected CBPeripheral _nativeDevice;

        protected int _rssi;

        protected IList<IService> _services = new List<IService>();

        public Device(CBPeripheral nativeDevice)
        {
            _nativeDevice = nativeDevice;

            try
            {
                _nativeDevice.DiscoveredService += (sender, e) =>
                {
                    // why we have to do this check is beyond me. if a service has been discovered, the collection
                    // shouldn't be null, but sometimes it is. le sigh, apple.
                    if (_nativeDevice.Services != null)
                    {
                        foreach (var s in _nativeDevice.Services)
                        {
                            Console.WriteLine("Device.Discovered Service: " + s.Description);
                            if (!ServiceExists(s))
                            {
                                _services.Add(new Service(s, _nativeDevice));
                            }
                        }
                        if (ServicesDiscovered != null) ServicesDiscovered(this, new EventArgs());
                    }
                };


#if __UNIFIED__
                // fixed for Unified https://bugzilla.xamarin.com/show_bug.cgi?id=14893
                _nativeDevice.DiscoveredCharacteristic += (sender, e) =>
                {
#else
    //BUGBUG/TODO: this event is misnamed in our SDK
			this._nativeDevice.DiscoverCharacteristic += (object sender, CBServiceEventArgs e) => {
					#endif
                    Console.WriteLine("Device.Discovered Characteristics.");
                    //loop through each service, and update the characteristics
                    foreach (var srv in ((CBPeripheral) sender).Services)
                    {
                        // if the service has characteristics yet
                        if (srv.Characteristics != null)
                        {
                            // locate the our new service
                            foreach (var item in Services)
                            {
                                // if we found the service
                                if (item.ID == Service.ServiceUuidToGuid(srv.UUID))
                                {
                                    item.Characteristics.Clear();

                                    // add the discovered characteristics to the particular service
                                    foreach (var characteristic in srv.Characteristics)
                                    {
                                        Console.WriteLine("Characteristic: " + characteristic.Description);
                                        var newChar = new Characteristic(characteristic, _nativeDevice);
                                        item.Characteristics.Add(newChar);
                                    }
                                    // inform the service that the characteristics have been discovered
                                    // TODO: really, we shoul just be using a notifying collection.
                                    var service = item as Service;
                                    if (service != null) service.OnCharacteristicsDiscovered();
                                }
                            }
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public override string DeviceGUID
        {
            get { return ID.ToString(); }
        }

        public override Guid ID
        {
            get
            {
                //TODO: not sure if this is right. hell, not even sure if a 
                // device should have a UDDI. iOS BLE peripherals do, though.
                // need to look at the BLE Spec
                // Actually.... deprecated in iOS7!
                // Actually again, Uuid is, but Identifier isn't.
                //return _nativeDevice.Identifier.AsString ();//.ToString();
                if (_nativeDevice != null && _nativeDevice.Identifier != null)
                    return Guid.ParseExact(_nativeDevice.Identifier.AsString(), "d");
                return new Guid();
            }
        }

        public override string Name
        {
            get { return _nativeDevice != null ? _nativeDevice.Name : string.Empty; }
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

        public override IList<IService> Services
        {
            get { return _services; }
        }

        public override event EventHandler ServicesDiscovered = delegate { };

        #region public methods

        public override void DiscoverServices()
        {
            _nativeDevice.DiscoverServices();
        }

        public void Disconnect()
        {
            Adapter.Current.DisconnectDevice(this);
            _nativeDevice.Dispose();
        }

        #endregion

        #region internal methods

        protected DeviceState GetState()
        {
            switch (_nativeDevice.State)
            {
                case CBPeripheralState.Connected:
                    return DeviceState.Connected;
                case CBPeripheralState.Connecting:
                    return DeviceState.Connecting;
                case CBPeripheralState.Disconnected:
                    return DeviceState.Disconnected;
                default:
                    return DeviceState.Disconnected;
            }
        }

        protected bool ServiceExists(CBService service)
        {
            foreach (var s in _services)
            {
                if (s.ID == Service.ServiceUuidToGuid(service.UUID))
                    return true;
            }
            return false;
        }

        #endregion
    }
}