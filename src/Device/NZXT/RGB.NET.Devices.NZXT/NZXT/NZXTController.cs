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
        public readonly NZXTDevice NZXTDevice;

        public NZXTController(NZXTDevice device)
        {
            NZXTDevice = device;
            UsbBuf = new byte[64];
        }

        public void SetColor(Color color, int zone)
        {
            if (UsbBuf != null)
            {
                UsbBuf[0x00] = 0x22;
                UsbBuf[0x01] = 0x10;
                UsbBuf[0x02] = NZXTDevice.Channel;
                UsbBuf[0x03] = 0x00;

                var test1 = 0x04;
                if (zone > 0)
                {
                    test1 += 3 * zone;
                }

                UsbBuf[test1] = color.G;
                UsbBuf[test1+1] = color.R;
                UsbBuf[test1+2] = color.B;

                if (NZXTDevice.Device != null)
                {
                   NZXTDevice.Device.WriteAsync(UsbBuf);
                }

                SendApply();
            }
        }

        public void SendApply()
        {
            byte[] UsbBuf = new byte[64];

            UsbBuf[0x00] = 0x22;
            UsbBuf[0x01] = 0xA0;
            UsbBuf[0x02] = NZXTDevice.Channel;
            UsbBuf[0x04] = 0x01;
            UsbBuf[0x07] = 0x28;
            UsbBuf[0x0A] = 0x80;
            UsbBuf[0x0C] = 0x32;
            UsbBuf[0x0F] = 0x01;
            if (NZXTDevice.Device != null)
            {
                NZXTDevice.Device.WriteAsync(UsbBuf);
            }
        }

        public void Dispose()
        {
            if (NZXTDevice.Device != null)
            {
                NZXTDevice.Device.Close();
                NZXTDevice.Device.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}