﻿using Windows.Storage.Streams;

namespace HACCP.WP.BLE.Dictionary.DataParser.BLE_Specification
{
    public static class AlertLevel
    {
        // From
        // https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.alert_level.xml
        public enum AlertLevelEnum
        {
            NoAlert = 0,
            MildAlert = 1,
            HighAlert = 2
        }

        public static string ParseBuffer(IBuffer buffer)
        {
            var reader = DataReader.FromBuffer(buffer);
            var result = reader.ReadByte();
            string categoryName;
            if (result < 3)
            {
                categoryName = ((AlertLevelEnum) result).ToString();
            }
            else
            {
                categoryName = "Reserved for future use";
            }
            return string.Format("{0} ({1})", result, categoryName);
        }
    }
}