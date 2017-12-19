using System;
using HACCP.Core;
#if __UNIFIED__
using CoreBluetooth;

#else
using MonoTouch.CoreBluetooth;
#endif

namespace HACCP.iOS
{
    public class Descriptor : IDescriptor
    {
        protected string _name;
        protected CBDescriptor _nativeDescriptor;

        public Descriptor(CBDescriptor nativeDescriptor)
        {
            _nativeDescriptor = nativeDescriptor;
        }

        public /*CBDescriptor*/ object NativeDescriptor
        {
            get { return _nativeDescriptor; }
        }

        public Guid ID
        {
            get
            {
                //return this._nativeDescriptor.UUID.ToString ();
                return Guid.ParseExact(_nativeDescriptor.UUID.ToString(), "d");
            }
        }

        public string Name
        {
            get
            {
                if (_name == null)
                    _name = KnownDescriptors.Lookup(ID).Name;
                return _name;
            }
        }
    }
}