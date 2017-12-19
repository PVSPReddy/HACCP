using System;
using System.Collections.Generic;
using HACCP.Core;
#if __UNIFIED__
using CoreBluetooth;

#else
using MonoTouch.CoreBluetooth;
#endif

namespace HACCP.iOS
{
    public class Service : IService
    {
        protected IList<ICharacteristic> _characteristics;
        protected string _name;

        protected CBService _nativeService;
        protected CBPeripheral _parentDevice;

        public Service(CBService nativeService, CBPeripheral parentDevice)
        {
            _nativeService = nativeService;
            _parentDevice = parentDevice;
        }

        public event EventHandler CharacteristicsDiscovered = delegate { };

        public Guid ID
        {
            get { return ServiceUuidToGuid(_nativeService.UUID); }
        }

        public string Name
        {
            get { return _name ?? (_name = KnownServices.Lookup(ID).Name); }
        }

        public bool IsPrimary
        {
            get { return _nativeService.Primary; }
        }

        //TODO: decide how to Interface this, right now it's only in the iOS implementation
        public void DiscoverCharacteristics()
        {
            // TODO: need to raise the event and listen for it.
            _parentDevice.DiscoverCharacteristics(_nativeService);
        }

        public IList<ICharacteristic> Characteristics
        {
            get
            {
                // if it hasn't been populated yet, populate it
                if (_characteristics == null)
                {
                    _characteristics = new List<ICharacteristic>();
                    if (_nativeService.Characteristics != null)
                    {
                        foreach (var item in _nativeService.Characteristics)
                        {
                            _characteristics.Add(new Characteristic(item, _parentDevice));
                        }
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
                if (string.Equals(item.UUID.ToString(), characteristic.ID.ToString()))
                {
                    return new Characteristic(item, _parentDevice);
                }
            }
            return null;
        }

        public void OnCharacteristicsDiscovered()
        {
            CharacteristicsDiscovered(this, new EventArgs());
        }

        public static Guid ServiceUuidToGuid(CBUUID uuid)
        {
            //this sometimes returns only the significant bits, e.g.
            //180d or whatever. so we need to add the full string
            var id = uuid.ToString();
            if (id.Length == 4)
            {
                id = "0000" + id + "-0000-1000-8000-00805f9b34fb";
            }
            return Guid.ParseExact(id, "d");
        }
    }
}