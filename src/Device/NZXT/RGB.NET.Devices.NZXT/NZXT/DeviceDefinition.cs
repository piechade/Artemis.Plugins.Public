using RGB.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGB.NET.Devices.NZXT.NZXT
{
    public class DeviceDefinition
    {
        public uint VendorId { get; set; }
        public uint ProductId { get; set; }
        public string? Label { get; set; }
        public int Usage { get; set; }
        public int RgbChannels { get; set; }
        public int FanChannels { get; set; }
        public List<DeviceTypeDefinition>? Types { get; set; }
        public bool IsInitialized { get; internal set; }
    }
}
