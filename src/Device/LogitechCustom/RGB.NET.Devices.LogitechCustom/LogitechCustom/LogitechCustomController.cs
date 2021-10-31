using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
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

        public readonly DeviceDefinition deviceDefinition;

        private readonly byte[] UsbBuf;
        private readonly IDevice LogitechCustomDevice;
        private readonly Thread thread;
        private readonly ConcurrentQueue<byte[]> _jobs = new();
        private readonly byte[][] lastvalue = new byte[5][];
        private readonly IObservable<TransferResult> readDeviceObservable;

        private int voltage = 0;
        private bool run = false;


        public LogitechCustomController(DeviceDefinition device, IDevice logitechCustomDevice)
        {
            deviceDefinition = new DeviceDefinition();
            LogitechCustomDevice = logitechCustomDevice;
            UsbBuf = new byte[20];

            if (device != null && device.UsbBuf != null)
            {
                run = true;
                lastvalue = new byte[device.Zones][];
                deviceDefinition = device;
                UsbBuf = (byte[])device.UsbBuf.Clone();
            }


            thread = new Thread(new ThreadStart(OnStart))
            {
                IsBackground = true
            };
            thread.Start();

            readDeviceObservable = Observable
                .Timer(TimeSpan.Zero, TimeSpan.FromSeconds(.1))
                .SelectMany(_ => Observable.FromAsync(() => LogitechCustomDevice.ReadAsync(new CancellationTokenSource(LogitechWirelessProtocolTimeout).Token)))
                .Distinct();

            _ = readDeviceObservable.Subscribe(sr =>
            {
                HandleRead(sr);
            });

            deviceDefinition.PropertyChanged += (s, e) =>
            {
                var newValue = (bool)((PropertyChangedExtendedEventArgs)e).NewValue;
                var oldValue = (bool)((PropertyChangedExtendedEventArgs)e).OldValue;


                if (s != null)
                {
                    if (newValue && (newValue != oldValue))
                    {
                        SendLastValue(2000);
                    }
                }
            };

        }

        private void SendLastValue(int delay)
        {
            Task.Factory.StartNew(async () =>
            {
                Debug.WriteLine("Last value");
                await Task.Delay(delay);
                foreach (var item in lastvalue)
                {
                    if (item != null)
                    {
                        _jobs.Enqueue(item);
                    }
                }
            });
        }

        private void HandleRead(TransferResult sr)
        {
            Debug.WriteLine("Received hid frame: " + BitConverter.ToString(sr.Data));

            if (sr.Data[2] == 0x08 && ((sr.Data[3] & 0xf0) == 0x00))
            {
                voltage = sr.Data[4] << 8;
                voltage |= sr.Data[5];
                Debug.WriteLine("Battery voltage: " + voltage);

                if (voltage == 0)
                {
                    deviceDefinition.Connect = false;
                }
                else if (deviceDefinition.Connect == false)
                {
                    deviceDefinition.Connect = true;

                }
            }

            if (sr.Data[2] == 0x05)
            {
                SendLastValue(2500);
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
            DoWork(_jobs);
        }

        private void DoWork(ConcurrentQueue<byte[]> queue)
        {
            while (run)
            {
                if (queue.TryDequeue(out var result))
                {
                    _ = LogitechCustomDevice.WriteAsync(result).ConfigureAwait(false);
                    System.Threading.Thread.Sleep(5);
                }
            }
        }

        //private static byte[] Gen_battery_level_update_message()
        //{
        //    byte[] OutData = new byte[20];

        //    for (int index = 0; index < (20); ++index)
        //    {
        //        OutData[index] = 0x00;
        //    }

        //    OutData[0] = 0x11;
        //    OutData[1] = 0xff;

        //    OutData[2] = 0x08;
        //    OutData[3] = 0x0f;

        //    return OutData;
        //}

        public void Dispose()
        {
            run = false;
            thread.Interrupt();
            LogitechCustomDevice.Close();
            LogitechCustomDevice.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}