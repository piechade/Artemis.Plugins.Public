using RGB.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGB.NET.Devices.NZXT.NZXT
{
    public class DeviceTypeDefinition
    {
        public byte Value { get; set; }
        public string Label { get; set; }
        public int LeedCount { get; set; }
        public RGBDeviceType Type { get; set; }
    }
}
