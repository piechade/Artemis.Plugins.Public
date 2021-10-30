using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Device.Net;

namespace RGB.NET.Devices.NZXT.NZXT
{
    public class NZXTController : IDisposable
    {
        private readonly byte[] UsbBuf;
        private readonly DeviceDefinition DeviceDefinition;
        private readonly IDevice NZXTDevice;

        public NZXTController(DeviceDefinition device, IDevice NZXTDevice)
        {
            GetAttachedDevices();


        }

        public void SetColor(Color color, byte zone)
        {
            if (UsbBuf != null)
            {
                UsbBuf[0x04] = zone;
                UsbBuf[0x06] = color.R;
                UsbBuf[0x07] = color.G;
                UsbBuf[0x08] = color.B;
            }
        }


        public void GetAttachedDevices()
        {
            byte[] OutData = new byte[3];

            OutData[0] = 0x20;
            OutData[1] = 0x03;
            OutData[2] = 0x00;


            var result = NZXTDevice.WriteAndReadAsync(OutData).ConfigureAwait(true);
            //Debug.WriteLine("Received hid frame: " + BitConverter.ToString(result.));
        }

        public void Dispose()
        {
            NZXTDevice.Close();
            NZXTDevice.Dispose();
        }
    }
}