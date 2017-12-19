using System;
using System.Threading.Tasks;

namespace HACCP.Core
{
    public interface IGlobalSettings
    {
        bool IsBusy { get; set; }

        event EventHandler<BLESettingsUpdated> BLESettingsUpdated;
        event EventHandler<DeviceStatusUpdated> PairedDeviceStatusUpdated;
        event EventHandler<string> GotTemperatureReading;
        event EventHandler<bool> ScanningProcess;
        Task PopulateDeviceListAsync();
        Task UpdateBLESettings(BLEChar chr, string value);
        void OpenBlueToothSettings();
        void UnRegisterDevice();
        void DisConnectDevice();
        bool HasConnectionWithDevice();
        Task ResetBlue2AutoOff();
    }
}