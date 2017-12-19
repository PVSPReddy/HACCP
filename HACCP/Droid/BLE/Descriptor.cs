using System;
using Android.Bluetooth;
using HACCP.Core;

namespace HACCP.Droid
{
    public class Descriptor : IDescriptor
    {
        protected string _name;
        protected BluetoothGattDescriptor _nativeDescriptor;

        public Descriptor(BluetoothGattDescriptor nativeDescriptor)
        {
            _nativeDescriptor = nativeDescriptor;
        }

        public /*BluetoothGattDescriptor*/ object NativeDescriptor
        {
            get { return _nativeDescriptor; }
        }

        public Guid ID
        {
            get
            {
                return Guid.ParseExact(_nativeDescriptor.Uuid.ToString(), "d");
                //return this._nativeDescriptor.Uuid.ToString ();
            }
        }

        public string Name
        {
            get { return _name ?? (_name = KnownDescriptors.Lookup(ID).Name); }
        }
    }
}