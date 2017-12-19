using Windows.Storage.Streams;

namespace HACCP.WP.BLE.Dictionary.DataParser.BLE_Specification
{
    public static class AlertNotificationControlPoint
    {
        // From
        // https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.alert_notification_control_point.xml
        public enum CommandID
        {
            EnableNewIncomingAlertNotification = 0,
            EnableUnreadCategoryStatusNotification = 1,
            DisableNewIncomingAlertNotification = 2,
            DisableUnreadCategoryStatusNotification = 3,
            NotifyNewIncomingAlertImmediately = 4,
            NotifyUnreadCategoryStatusImmediately = 5
        }

        public static string ParseBuffer(IBuffer buffer)
        {
            var reader = DataReader.FromBuffer(buffer);
            var current = reader.ReadByte();
            var categoryName = current <= 5 ? ((CommandID) current).ToString() : "Reserved for future use";
            var result = string.Format("\n{0} ({1})\n", current, categoryName);

            current = reader.ReadByte();
            result += AlertCategoryId.GetAlertCategoryIdFromByte(current);

            return result;
        }
    }
}