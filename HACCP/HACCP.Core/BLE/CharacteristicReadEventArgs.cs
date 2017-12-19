using System;

namespace HACCP.Core
{
    public class CharacteristicReadEventArgs : EventArgs
    {
        public ICharacteristic Characteristic { get; set; }
    }
}