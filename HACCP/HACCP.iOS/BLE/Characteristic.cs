using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HACCP.Core;
#if __UNIFIED__
using CoreBluetooth;
using Foundation;

#else
using MonoTouch.CoreBluetooth;
using MonoTouch.Foundation;
#endif

namespace HACCP.iOS
{
    public class Characteristic : ICharacteristic
    {
        protected IList<IDescriptor> _descriptors;

        protected CBCharacteristic _nativeCharacteristic;
        private readonly CBPeripheral _parentDevice;

        public Characteristic(CBCharacteristic nativeCharacteristic, CBPeripheral parentDevice)
        {
            _nativeCharacteristic = nativeCharacteristic;
            _parentDevice = parentDevice;
        }

        public event EventHandler<CharacteristicReadEventArgs> ValueUpdated = delegate { };

        public string Uuid
        {
            get { return _nativeCharacteristic.UUID.ToString(); }
        }

        public Guid ID
        {
            get { return CharacteristicUuidToGuid(_nativeCharacteristic.UUID); }
        }

        public byte[] Value
        {
            get
            {
                if (_nativeCharacteristic.Value == null)
                    return null;
                return _nativeCharacteristic.Value.ToArray();
            }
        }

        public string StringValue
        {
            get
            {
                if (Value == null)
                    return string.Empty;
                var stringByes = Value;
                var s1 = Encoding.UTF8.GetString(stringByes);
                //var s2 = System.Text.Encoding.ASCII.GetString (stringByes);
                return s1;
            }
        }

        public string Name
        {
            get { return KnownCharacteristics.Lookup(ID).Name; }
        }

        public CharacteristicPropertyType Properties
        {
            get { return (CharacteristicPropertyType) (int) _nativeCharacteristic.Properties; }
        }

        public IList<IDescriptor> Descriptors
        {
            get
            {
                // if we haven't converted them to our xplat objects
                if (_descriptors != null)
                {
                    _descriptors = new List<IDescriptor>();
                    // convert the internal list of them to the xplat ones
                    foreach (var item in _nativeCharacteristic.Descriptors)
                    {
                        _descriptors.Add(new Descriptor(item));
                    }
                }
                return _descriptors;
            }
        }

        public object NativeCharacteristic
        {
            get { return _nativeCharacteristic; }
        }

        public bool CanRead
        {
            get { return (Properties & CharacteristicPropertyType.Read) != 0; }
        }

        public bool CanUpdate
        {
            get { return (Properties & CharacteristicPropertyType.Notify) != 0; }
        }

        public bool CanWrite
        {
            get
            {
                return (Properties &
                        (CharacteristicPropertyType.WriteWithoutResponse |
                         CharacteristicPropertyType.AppleWriteWithoutResponse)) != 0;
            }
        }

        public Task<ICharacteristic> ReadAsync()
        {
            var tcs = new TaskCompletionSource<ICharacteristic>();

            if (!CanRead)
            {
                throw new InvalidOperationException("Characteristic does not support READ");
            }
            EventHandler<CBCharacteristicEventArgs> updated = null;
            updated = (sender, e) =>
            {
                Console.WriteLine(".....UpdatedCharacterteristicValue");
                var c = new Characteristic(e.Characteristic, _parentDevice);
                tcs.SetResult(c);
                _parentDevice.UpdatedCharacterteristicValue -= updated;
            };

            _parentDevice.UpdatedCharacterteristicValue += updated;
            Console.WriteLine(".....ReadAsync");
            _parentDevice.ReadValue(_nativeCharacteristic);

            return tcs.Task;
        }

        public void Write(byte[] data)
        {
            if (!CanWrite)
            {
                throw new InvalidOperationException("Characteristic does not support WRITE");
            }
            var nsdata = NSData.FromArray(data);
            var descriptor = _nativeCharacteristic;

            var t = (Properties & CharacteristicPropertyType.AppleWriteWithoutResponse) != 0
                ? CBCharacteristicWriteType.WithoutResponse
                : CBCharacteristicWriteType.WithResponse;

            _parentDevice.WriteValue(nsdata, descriptor, t);


            //			Console.WriteLine ("** Characteristic.Write, Type = " + t + ", Data = " + BitConverter.ToString (data));
        }

        public void StartUpdates()
        {
            // TODO: should be bool RequestValue? compare iOS API for commonality
            var successful = false;
            if (CanRead)
            {
                Console.WriteLine("** Characteristic.RequestValue, PropertyType = Read, requesting read");
                _parentDevice.UpdatedCharacterteristicValue += UpdatedRead;

                _parentDevice.ReadValue(_nativeCharacteristic);

                successful = true;
            }
            if (CanUpdate)
            {
                Console.WriteLine("** Characteristic.RequestValue, PropertyType = Notify, requesting updates");
                _parentDevice.UpdatedCharacterteristicValue += UpdatedNotify;

                _parentDevice.SetNotifyValue(true, _nativeCharacteristic);

                successful = true;
            }

            Console.WriteLine("** RequestValue, Succesful: " + successful);
        }

        public void StopUpdates()
        {
            //bool successful = false;
            if (CanUpdate)
            {
                _parentDevice.SetNotifyValue(false, _nativeCharacteristic);
                Console.WriteLine("** Characteristic.RequestValue, PropertyType = Notify, STOP updates");
            }
        }

        // removes listener after first response received
        private void UpdatedRead(object sender, CBCharacteristicEventArgs e)
        {
            ValueUpdated(this, new CharacteristicReadEventArgs
            {
                Characteristic = new Characteristic(e.Characteristic, _parentDevice)
            });
            _parentDevice.UpdatedCharacterteristicValue -= UpdatedRead;
        }

        // continues to listen indefinitely
        private void UpdatedNotify(object sender, CBCharacteristicEventArgs e)
        {
            ValueUpdated(this, new CharacteristicReadEventArgs
            {
                Characteristic = new Characteristic(e.Characteristic, _parentDevice)
            });
        }

        //TODO: this is the exact same as ServiceUuid i think
        public static Guid CharacteristicUuidToGuid(CBUUID uuid)
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