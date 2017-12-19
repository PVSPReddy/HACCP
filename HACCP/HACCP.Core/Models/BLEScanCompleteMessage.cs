using System.Collections.Generic;

namespace HACCP.Core
{
    public class BleScanCompleteMessage
    {
        public BleScanCompleteMessage(IList<IDevice> devices)
        {
            Devices = devices;
        }

        public IList<IDevice> Devices { get; set; }
    }
}