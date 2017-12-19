using Windows.Storage.Streams;

namespace HACCP.WP.BLE.Dictionary.DataParser.BLE_Specification
{
    public static class BodySensorLocation
    {
        // From
        // https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.body_sensor_location.xml
        public enum Status
        {
            Other = 0,
            Chest = 1,
            Wrist = 2,
            Finger = 3,
            Hand = 4,
            Ear_Lobe = 5,
            Foot = 6
        }

        public static string ParseBuffer(IBuffer buffer)
        {
            var reader = DataReader.FromBuffer(buffer);
            var result = reader.ReadByte();
            var categoryName = result < 7 ? ((Status) result).ToString() : "Reserved for future use";
            return string.Format("{0} ({1})", result, categoryName);
        }
    }
}