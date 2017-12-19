namespace HACCP.Core
{
    /// <summary>
    /// BLESettingsUpdated Class
    /// </summary>
    public class BLESettingsUpdated
    {
        public BleSettings Settings;

        /// <summary>
        /// BLESettingsUpdated  Constructor
        /// </summary>
        /// <param name="settings"></param>
        public BLESettingsUpdated(BleSettings settings)
        {
            Settings = settings;
        }
    }

    /// <summary>
    /// DeviceStatusUpdated Class
    /// </summary>
    public class DeviceStatusUpdated
    {
        public DeviceStatusUpdated(bool status)
        {
            Status = status;
        }

        public bool Status { get; set; }
    }
}