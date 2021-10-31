using RGB.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGB.NET.Devices.NZXT.NZXT
{
    public class NZXTDeviceType
    {
        public byte Value { get; set; }
        public string Label { get; set; }
        public int LedCount { get; set; }

        public NZXTDeviceType()
        {
            Label = "NZXT Device";
        }
    }
}
