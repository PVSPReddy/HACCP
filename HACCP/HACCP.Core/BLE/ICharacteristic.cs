using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HACCP.Core
{
    public interface ICharacteristic
    {
        // properties
        Guid ID { get; }
        string Uuid { get; }
        byte[] Value { get; }
        string StringValue { get; }
        IList<IDescriptor> Descriptors { get; }
        object NativeCharacteristic { get; }
        string Name { get; }
        CharacteristicPropertyType Properties { get; }

        bool CanRead { get; }
        bool CanUpdate { get; }
        bool CanWrite { get; }
        // events
        event EventHandler<CharacteristicReadEventArgs> ValueUpdated;

        // methods
        //		void EnumerateDescriptors ();

        void StartUpdates();
        void StopUpdates();

        Task<ICharacteristic> ReadAsync();

        void Write(byte[] data);
    }
}