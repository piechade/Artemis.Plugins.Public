using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Device.Net;
using Hid.Net.Windows;
using RGB.NET.Core;
// ReSharper disable AsyncConverter.AsyncWait

namespace RGB.NET.Devices.LogitechCustom.LogitechCustom
{
    public static class LogitechCustomLoader
    {
        public const int VENDOR_ID = 0x046D;
        static List<DeviceDefinition> parts = new List<DeviceDefinition>();

        public static async Task InitializeAsync()
        {

            parts = new List<DeviceDefinition>();

            parts.Add(new DeviceDefinition()
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
            });

            parts.Add(new DeviceDefinition()
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
            });


            foreach (var item in parts)
            {
                await GetDeviceAsync(vendorId: VENDOR_ID, item);
            }
        }

        public static void FreeDevices()
        {

            foreach (var item in parts)
            {
                item.Controller?.Dispose();
                item.IsInitialized = false;
            }
        }

        public static List<DeviceDefinition> GetDevices()
        {
            return parts;
        }

        private static async Task GetDeviceAsync(int vendorId, DeviceDefinition device)
        {
            var hidFactory = new FilterDeviceDefinition(vendorId: VENDOR_ID, productId: device.Pid, usagePage: device.UsagePage, label: device.Label).CreateWindowsHidDeviceFactory();

            var LogitechCustomDeviceDefinition = (await hidFactory.GetConnectedDeviceDefinitionsAsync().ConfigureAwait(false)).FirstOrDefault(d => d.Usage == device.Page);
            IDevice? LogitechCustomDevice = null;
            device.IsInitialized = false;


            if (LogitechCustomDeviceDefinition != null)
            {
                LogitechCustomDevice = await hidFactory.GetDeviceAsync(LogitechCustomDeviceDefinition).ConfigureAwait(false);

                if (LogitechCustomDevice != null)
                {
                    LogitechCustomDevice.InitializeAsync().Wait();
                    device.Controller = new LogitechCustomController(device, LogitechCustomDevice);
                    device.IsInitialized = true;
                }
            }

        }
    }
}