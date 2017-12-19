﻿using Windows.Storage.Streams;

namespace HACCP.WP.BLE.Dictionary.DataParser.BLE_Specification
{
    public static class AlertCategoryId
    {
        // From
        // https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.alert_category_id.xml

        public enum CategoryId
        {
            SimpleAlert = 0,
            Email = 1,
            News = 2,
            Call = 3,
            MissedCall = 4,
            SMS_MMS = 5,
            VoiceMail = 6,
            Schedule = 7,
            HighPrioritizedAlert = 8,
            InstantMessage = 9
        }

        public static string ParseBuffer(IBuffer buffer)
        {
            var reader = DataReader.FromBuffer(buffer);
            var result = reader.ReadByte();
            return GetAlertCategoryIdFromByte(result);
        }

        // Gets a string represeting the Alert Category.
        // AlertNotificationControlPoint class needs to call this function. 
        public static string GetAlertCategoryIdFromByte(byte b)
        {
            string categoryName;
            if (b <= 9)
            {
                categoryName = ((CategoryId) b).ToString();
            }
            else if (b >= 251)
            {
                categoryName = "Defined by service specification";
            }
            else
            {
                categoryName = "Reserved for future use";
            }
            return string.Format("{0} ({1})", b, categoryName);
        }
    }
}