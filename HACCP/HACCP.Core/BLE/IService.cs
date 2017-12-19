using System;
using System.Collections.Generic;

namespace HACCP.Core
{
    public interface IService
    {
        Guid ID { get; }
        string Name { get; }
        bool IsPrimary { get; } // iOS only?
        IList<ICharacteristic> Characteristics { get; }
        event EventHandler CharacteristicsDiscovered;


        ICharacteristic FindCharacteristic(KnownCharacteristic characteristic);
        //IDictionary<Guid, ICharacteristic> Characteristics { get; }
        void DiscoverCharacteristics();
    }
}