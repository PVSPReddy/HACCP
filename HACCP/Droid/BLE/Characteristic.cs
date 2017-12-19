using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.Bluetooth;
using HACCP.Core;

namespace HACCP.Droid
{
    public class Characteristic : ICharacteristic
    {
        protected IList<IDescriptor> _descriptors;

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


        protected BluetoothGattCharacteristic _nativeCharacteristic;


        public Characteristic(BluetoothGattCharacteristic nativeCharacteristic, BluetoothGatt gatt,
            GattCallback gattCallback)
        {
            _nativeCharacteristic = nativeCharacteristic;
            _gatt = gatt;
            _gattCallback = gattCallback;

            if (_gattCallback != null)
            {
                // wire up the characteristic value updating on the gattcallback
                _gattCallback.CharacteristicValueUpdated += (sender, e) =>
                {
                    // it may be other characteristics, so we need to test
                    if (e.Characteristic.ID == ID)
                    {
                        // update our underlying characteristic (this one will have a value)
                        //TODO: is this necessary? probably the underlying reference is the same.
                        //this._nativeCharacteristic = e.Characteristic;

                        ValueUpdated(this, e);
                    }
                };
            }
        }

        public event EventHandler<CharacteristicReadEventArgs> ValueUpdated = delegate { };

        public string Uuid
        {
            get { return _nativeCharacteristic.Uuid.ToString(); }
        }

        public Guid ID
        {
            get { return Guid.Parse(_nativeCharacteristic.Uuid.ToString()); }
        }

        public byte[] Value
        {
            get { return _nativeCharacteristic.GetValue(); }
        }

        public string StringValue
        {
            get
            {
                if (Value == null)
                    return string.Empty;
                return Encoding.UTF8.GetString(Value);
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
                if (_descriptors == null)
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

        //NOTE: why this requires Apple, we have no idea. BLE stands for Mystery.
        public bool CanWrite
        {
            get
            {
                return (Properties & CharacteristicPropertyType.WriteWithoutResponse |
                        CharacteristicPropertyType.AppleWriteWithoutResponse) != 0;
            }
        }

        // HACK: UNTESTED - this API has only been tested on iOS
        public void Write(byte[] data)
        {
            if (!CanWrite)
            {
                throw new InvalidOperationException("Characteristic does not support WRITE");
            }

            var c = _nativeCharacteristic;
            c.SetValue(data);
		   _gatt.WriteCharacteristic(c);
            Console.WriteLine(@".....Write");
        }


        // HACK: UNTESTED - this API has only been tested on iOS
        public Task<ICharacteristic> ReadAsync()
        {
            var tcs = new TaskCompletionSource<ICharacteristic>();

            if (!CanRead)
            {
                throw new InvalidOperationException("Characteristic does not support READ");
            }
            EventHandler<CharacteristicReadEventArgs> updated = null;
            updated = (sender, e) =>
            {
                // it may be other characteristics, so we need to test
                var c = e.Characteristic;
                tcs.SetResult(c);
                if (_gattCallback != null)
                {
                    // wire up the characteristic value updating on the gattcallback
                    _gattCallback.CharacteristicValueUpdated -= updated;
                }
            };


            if (_gattCallback != null)
            {
                // wire up the characteristic value updating on the gattcallback
                _gattCallback.CharacteristicValueUpdated += updated;
            }

            Console.WriteLine(@".....ReadAsync");
            _gatt.ReadCharacteristic(_nativeCharacteristic);

            return tcs.Task;
        }

        public void StartUpdates()
        {
            // TODO: should be bool RequestValue? compare iOS API for commonality
            var successful = false;
            if (CanRead)
            {
                Console.WriteLine(@"Characteristic.RequestValue, PropertyType = Read, requesting updates");
                successful = _gatt.ReadCharacteristic(_nativeCharacteristic);
            }
            if (CanUpdate)
            {
                Console.WriteLine(@"Characteristic.RequestValue, PropertyType = Notify, requesting updates");

                successful = _gatt.SetCharacteristicNotification(_nativeCharacteristic, true);

                // [TO20131211@1634] It seems that setting the notification above isn't enough. You have to set the NOTIFY
                // descriptor as well, otherwise the receiver will never get the updates. I just grabbed the first (and only)
                // descriptor that is associated with the characteristic, which is the NOTIFY descriptor. This seems like a really
                // odd way to do things to me, but I'm a Bluetooth newbie. Google has a example here (but ono real explaination as
                // to what is going on):
                // http://developer.android.com/guide/topics/connectivity/bluetooth-le.html#notification
                //
                // HACK: further detail, in the Forms client this only seems to work with a breakpoint on it
                // (ie. it probably needs to wait until the above 'SetCharacteristicNofication' is done before doing this...?????? [CD]
                Thread.Sleep(100);
                    // HACK: did i mention this was a hack?????????? [CD] 50ms was too short, 100ms seems to work

                if (_nativeCharacteristic.Descriptors.Count > 0)
                {
                    var descriptor = _nativeCharacteristic.Descriptors[0];
                    descriptor.SetValue(BluetoothGattDescriptor.EnableNotificationValue.ToArray());
                    _gatt.WriteDescriptor(descriptor);
                }
                else
                {
                    Console.WriteLine(@"RequestValue, FAILED: _nativeCharacteristic.Descriptors was empty, not sure why");
                }
            }

            Console.WriteLine(@"RequestValue, Succesful: " + successful);
        }

        public void StopUpdates()
        {
            if (CanUpdate)
            {
                _gatt.SetCharacteristicNotification(_nativeCharacteristic, false);
                //TODO: determine whether 
                Console.WriteLine(@"Characteristic.RequestValue, PropertyType = Notify, STOP updates");
            }
        }
    }
}