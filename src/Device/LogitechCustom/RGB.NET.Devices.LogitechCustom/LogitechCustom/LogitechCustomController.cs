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

namespace RGB.NET.Devices.LogitechCustom.LogitechCustom
{
    public class LogitechCustomController : IDisposable
    {
        public const int LogitechWirelessProtocolTimeout = 300;
        private readonly byte[] UsbBuf;
        private readonly DeviceDefinition? DeviceDefinition;
        private readonly IDevice LogitechCustomDevice;

        private int voltage = 0;

        private Thread thread;
        private Thread thread2;

        private bool run = false;

        private ConcurrentQueue<byte[]> _jobs = new ConcurrentQueue<byte[]>();

        private byte[][] lastvalue = new byte[5][];
        private bool is_headset_connected;

        public LogitechCustomController(DeviceDefinition device, IDevice logitechCustomDevice)
        {
            if (device != null && device.UsbBuf != null)
            {
                UsbBuf = (byte[])device.UsbBuf.Clone();
            }
            else
            {
                UsbBuf = new byte[20];
            }
            DeviceDefinition = device;
            LogitechCustomDevice = logitechCustomDevice;

            run = true;

            if (device != null)
            {
                lastvalue = new byte[device.Zones][];
            }



            thread = new Thread(new ThreadStart(OnStart))
            {
                IsBackground = true
            };
            thread.Start();

            thread2 = new Thread(new ThreadStart(OnStart2))
            {
                IsBackground = true
            };
            thread2.Start();

            if (device != null && device.Reconnect)
            {
                var aTimer = new System.Timers.Timer(1000);
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

                aTimer.Interval = 60000;
                aTimer.Enabled = true;
            }


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
                lastvalue[zone] = (byte[])UsbBuf.Clone();
            }
        }
        private void OnStart()
        {
            while (run)
            {
                if (_jobs.TryDequeue(out var result))
                {
                    _ = LogitechCustomDevice.WriteAsync(result).ConfigureAwait(false);
                    System.Threading.Thread.Sleep(5);
                }

                //if (_jobs.Count > 0)
                //{
                //    Debug.WriteLine(_jobs.Count);
                //}
            }
        }

        private void OnStart2()
        {
            while (run)
            {
                using (var sr = LogitechCustomDevice.ReadAsync())
                {
                    Debug.WriteLine(JsonSerializer.Serialize(sr));
                    Debug.WriteLine("Received hid frame: " + BitConverter.ToString(sr.Result.Data));

                    if (sr.Result.Data[2] == 0x08 && ((sr.Result.Data[3] & 0xf0) == 0x00))
                    {
                        voltage = sr.Result.Data[4] << 8;
                        voltage |= sr.Result.Data[5];
                        Debug.WriteLine("Battery voltage: " + voltage);

                        if (voltage == 0)
                        {
                            is_headset_connected = false;
                        }
                        else if (is_headset_connected == false)
                        {
                            is_headset_connected = true;

                            _jobs.Clear();
                            System.Threading.Thread.Sleep(8000);

                            foreach (var item in lastvalue)
                            {
                                _jobs.Enqueue(item);
                            }
                        }

                    }
                }
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            battery_level_update();
        }

        private void battery_level_update()
        {
            byte[] OutData = new byte[20];

            for (int index = 0; index < (20); ++index)
            {
                OutData[index] = 0x00;
            }

            OutData[0] = 0x11;
            OutData[1] = 0xff;

            OutData[2] = 0x08;
            OutData[3] = 0x0f;

            //Ask for battery voltage
            _ = LogitechCustomDevice.WriteAsync(OutData).ConfigureAwait(false);
        }



        public void Dispose()
        {
            run = false;
            thread.Interrupt();
            thread2.Interrupt();
            LogitechCustomDevice.Close();
            LogitechCustomDevice.Dispose();
        }
    }
}