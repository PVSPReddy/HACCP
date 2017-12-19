﻿using Windows.Storage.Streams;

namespace HACCP.WP.BLE.Dictionary.DataParser.BLE_Specification
{
    public static class AlertStatus
    {
        // From
        // https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.alert_status.xml
        public enum Status
        {
            RingerStateActive = 1 << 0,
            VibrateStateActive = 1 << 1,
            DisplayAlertStatus = 1 << 2
        }

        public static string ParseBuffer(IBuffer buffer)
        {
            var reader = DataReader.FromBuffer(buffer);
            var result = reader.ReadByte();
            return BasicParsers.FlagsSetInByte(typeof(Status), result);
        }
    }
}