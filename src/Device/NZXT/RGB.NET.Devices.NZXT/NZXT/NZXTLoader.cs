using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Device.Net;
using Hid.Net.Windows;
using RGB.NET.Core;
// ReSharper disable AsyncConverter.AsyncWait

namespace RGB.NET.Devices.NZXT.NZXT
{
    public static class NZXTLoader
    {
        public const int VENDOR_ID = 0x1e71;
        static List<DeviceDefinition> parts = new List<DeviceDefinition>();

        public static async Task InitializeAsync()
        {

            parts = new List<DeviceDefinition>
            {
                new DeviceDefinition()
                {
                    Pid = 0x2006,
                    Page = 1,
                    Label = "NZXT Smart Device V2 1",
                }
            };


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

        private static async Task GetDeviceAsync(uint vendorId, DeviceDefinition device)
        {
            var hidFactory = new FilterDeviceDefinition(vendorId: vendorId, productId: device.Pid, label: device.Label).CreateWindowsHidDeviceFactory();

            var NZXTDeviceDefinition = (await hidFactory.GetConnectedDeviceDefinitionsAsync().ConfigureAwait(false)).FirstOrDefault(d => d.Usage == device.Page);
            IDevice? NZXTDevice = null;
            device.IsInitialized = false;

            if (NZXTDeviceDefinition != null)
            {
                NZXTDevice = await hidFactory.GetDeviceAsync(NZXTDeviceDefinition).ConfigureAwait(false);

                if (NZXTDevice != null)
                {
                    NZXTDevice.InitializeAsync().Wait();
                    device.Controller = new NZXTController(device, NZXTDevice);
                    device.IsInitialized = true;
                }
            }
        }
    }
}