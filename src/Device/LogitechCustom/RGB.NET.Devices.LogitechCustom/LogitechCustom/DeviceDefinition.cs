using RGB.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGB.NET.Devices.LogitechCustom.LogitechCustom
{
    public class DeviceDefinition
    {
        public uint Pid { get; set; }
        public ushort UsagePage { get; set; }
        public int Page { get; set; }
        public string Label = "Logitech";
        public byte[]? UsbBuf { get; set; }
        public LogitechCustomController? Controller { get; set; }
        public RGBDeviceType Type { get; set; }
        public bool IsInitialized { get; set; }
        public bool Reconnect { get; set; }
        public int Zones { get; set; }
    }
}
