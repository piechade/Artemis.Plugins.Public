using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Device.Net;
// ReSharper disable AsyncConverter.AsyncWait

namespace RGB.NET.Devices.LogitechCustom.LogitechCustom
{
    public class LogitechCustomController : IDisposable
    {
        public const int LogitechWirelessProtocolTimeout = 300;
        private readonly byte[] UsbBuf;
        private readonly DeviceDefinition? DeviceDefinition;
        private readonly IDevice LogitechCustomDevice;

        private ConcurrentQueue<byte[]> _jobs = new ConcurrentQueue<byte[]>();

        public LogitechCustomController(DeviceDefinition device, IDevice logitechCustomDevice)
        {
            if (device != null && device.UsbBuf != null)
            {
                UsbBuf = (byte[])device.UsbBuf.Clone();
            } else
            {
                UsbBuf = new byte[20];
            }
            DeviceDefinition = device;
            LogitechCustomDevice = logitechCustomDevice;

            var thread = new Thread(new ThreadStart(OnStart));
            thread.IsBackground = true;
            thread.Start();
        }

        public void SetColor(Color color, byte zone)
        {
            if (UsbBuf != null)
            {
                UsbBuf[0x04] = zone;
                UsbBuf[0x06] = color.R;
                UsbBuf[0x07] = color.G;
                UsbBuf[0x08] = color.B;
                _jobs.Enqueue((byte[])UsbBuf.Clone());
        
            }
        }
        private void OnStart()
        {
            while (true)
            {
                if (_jobs.TryDequeue(out var result))
                {
                    _ = LogitechCustomDevice.WriteAsync(result).Wait(LogitechWirelessProtocolTimeout);
                    System.Threading.Thread.Sleep(5);
                }

                if (_jobs.Count > 10)
                {
                    Console.WriteLine(10);
                }
            }
        }


        public void Dispose()
        {
            LogitechCustomDevice.Close();
            LogitechCustomDevice.Dispose();
        }
    }
}