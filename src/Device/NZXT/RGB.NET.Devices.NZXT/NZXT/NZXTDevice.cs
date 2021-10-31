using Device.Net;
using RGB.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGB.NET.Devices.NZXT.NZXT
{
    public class NZXTDevice
    {

        public IDevice? Device { get; set; }
        public RGBDeviceType Type { get; set; }
        public string Label { get; set; }
        public List<NZXTDeviceType> Types { get; set; }
        public bool IsInitialized { get; set; }
        public byte Channel { get; set; }

        public NZXTDevice()
        {
            Type = RGBDeviceType.Unknown;
            Label = "NZXT Device";
            Types = new();
            IsInitialized = false;
            Channel = 1;
        }

        public int GetLedCount()
        {
            int count = 0;
            if (Types != null)
            {
                foreach (var item in Types)
                {
                    count += item.LedCount;
                }
            }
            return count;
        }
    }
}
