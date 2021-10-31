using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Device.Net;
using Hid.Net.Windows;
using RGB.NET.Core;
using Microsoft.Extensions.Logging;

namespace RGB.NET.Devices.LogitechCustom.LogitechCustom
{
    public static class LogitechCustomLoader
    {
        public const int VENDOR_ID = 0x046D;
        public static readonly List<LogitechCustomController> controller = new();

        public static async Task InitializeAsync()
        {

            List<DeviceDefinition> devices = new()
            {
                new DeviceDefinition()
                {
                    Pid = 0xC53A,
                    UsagePage = 0xFF00,
                    Page = 2,
                    Label = "PowerPlay",
                    Type = RGBDeviceType.Mousepad,
                    UsbBuf = new byte[] { 0x11, 0x07, 0x0B, 0x3E, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                    IsInitialized = false,
                    Reconnect = false,
                    Zones = 1
                },

                new DeviceDefinition()
                {
                    Pid = 0x0ab5,
                    UsagePage = 0xFF43,
                    Page = 514,
                    Label = "G733",
                    Type = RGBDeviceType.Headset,
                    UsbBuf = new byte[] { 0x11, 0xFF, 0x04, 0x3E, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                    IsInitialized = false,
                    Reconnect = true,
                    Zones = 2
                }
            };


            foreach (var item in devices)
            {
                var test = await GetDeviceAsync(vendorId: VENDOR_ID, item);
                if (test != null)
                {

                    controller.Add(test);
                }
            }
        }

        public static void FreeDevices()
        {

            foreach (var item in controller)
            {
                item.Dispose();
                item.deviceDefinition.IsInitialized = false;
            }
        }

        public static List<LogitechCustomController> GetDevices()
        {
            return controller;
        }

        private static async Task<LogitechCustomController?> GetDeviceAsync(uint vendorId, DeviceDefinition device)
        {
            var loggerFactory = LoggerFactory.Create((builder) =>
            {
                _ = builder.AddDebug().SetMinimumLevel(LogLevel.Trace);
            });


            var hidFactory = new FilterDeviceDefinition(vendorId: vendorId, productId: device.Pid, usagePage: device.UsagePage, label: device.Label).CreateWindowsHidDeviceFactory(loggerFactory);

            var LogitechCustomDeviceDefinition = (await hidFactory.GetConnectedDeviceDefinitionsAsync().ConfigureAwait(false)).FirstOrDefault(d => d.Usage == device.Page);
            IDevice? LogitechCustomDevice = null;
            device.IsInitialized = false;


            if (LogitechCustomDeviceDefinition != null)
            {
                LogitechCustomDevice = await hidFactory.GetDeviceAsync(LogitechCustomDeviceDefinition).ConfigureAwait(false);

                if (LogitechCustomDevice != null)
                {
                    LogitechCustomDevice.InitializeAsync().Wait();
                    var controller = new LogitechCustomController(device, LogitechCustomDevice);
                    controller.deviceDefinition.IsInitialized = true;
                    return controller;
                }
            }

            return null;
        }
    }
}