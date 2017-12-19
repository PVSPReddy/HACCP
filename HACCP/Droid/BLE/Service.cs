using System;
using System.Collections.Generic;
using Android.Bluetooth;
using HACCP.Core;

namespace HACCP.Droid
{
    public class Service : IService
    {
        protected IList<ICharacteristic> _characteristics;

        /// <summary>
        ///     we have to keep a reference to this because Android's api is weird and requires
        ///     the GattServer in order to do nearly anything, including enumerating services
        /// </summary>
        protected BluetoothGatt _gatt;

        /// <summary>
        ///     we also track this because of gogole's weird API. the gatt callback is where
        ///     we'll get notified when services are enumerated
        /// </summary>
        protected GattCallback _gattCallback;

        protected string _name;
        protected BluetoothGattService _nativeService;

        public Service(BluetoothGattService nativeService, BluetoothGatt gatt, GattCallback _gattCallback)
        {
            _nativeService = nativeService;
            _gatt = gatt;
            this._gattCallback = _gattCallback;
        }

        public Guid ID
        {
            get
            {
                //				return this._nativeService.Uuid.ToString ();
                return Guid.ParseExact(_nativeService.Uuid.ToString(), "d");
            }
        }

        public string Name
        {
            get { return _name ?? (_name = KnownServices.Lookup(ID).Name); }
        }

        public bool IsPrimary
        {
            get { return _nativeService.Type == GattServiceType.Primary; }
        }

        //TODO: i think this implictly requests charactersitics.
        // 
        public IList<ICharacteristic> Characteristics
        {
            get
            {
                // if it hasn't been populated yet, populate it
                if (_characteristics == null)
                {
                    _characteristics = new List<ICharacteristic>();
                    foreach (var item in _nativeService.Characteristics)
                    {
                        _characteristics.Add(new Characteristic(item, _gatt, _gattCallback));
                    }
                }
                return _characteristics;
            }
        }

        public ICharacteristic FindCharacteristic(KnownCharacteristic characteristic)
        {
            //TODO: why don't we look in the internal list _chacateristics?
            foreach (var item in _nativeService.Characteristics)
            {
                if (string.Equals(item.Uuid.ToString(), characteristic.ID.ToString()))
                {
                    return new Characteristic(item, _gatt, _gattCallback);
                }
            }
            return null;
        }

        public event EventHandler CharacteristicsDiscovered = delegate { }; // not implemented

        public void DiscoverCharacteristics()
        {
            //throw new NotImplementedException ("This is only in iOS right now, needs to be added to Android");
            CharacteristicsDiscovered(this, new EventArgs());
        }
    }
}