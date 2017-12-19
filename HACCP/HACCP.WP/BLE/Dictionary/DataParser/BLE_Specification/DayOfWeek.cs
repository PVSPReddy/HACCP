using System;
using Windows.Storage.Streams;

namespace HACCP.WP.BLE.Dictionary.DataParser.BLE_Specification
{
    public static class DayOfWeek
    {
        // From
        // https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.day_of_week.xml
        public enum Status
        {
            Unknown = 0,
            Monday = 1,
            Tuseday = 2,
            Wednesday = 3,
            Thursday = 4,
            Friday = 5,
            Saturday = 6,
            Sunday = 7
        }

        public static string ParseBuffer(IBuffer buffer)
        {
            var reader = DataReader.FromBuffer(buffer);
            var result = reader.ReadByte();
            var categoryName = result < Enum.GetNames(typeof(Status)).Length ? ((Status) result).ToString() : "Reserved for future use";
            return string.Format("{0} ({1})", result, categoryName);
        }
    }
}