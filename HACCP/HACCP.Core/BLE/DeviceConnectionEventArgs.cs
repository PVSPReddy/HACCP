using System;

namespace HACCP.Core
{
    public class DeviceConnectionEventArgs : EventArgs
    {
        public IDevice Device;
        public string ErrorMessage;
    }
}