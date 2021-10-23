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

            parts = new List<DeviceDefinition>();

            parts.Add(new DeviceDefinition()
            {
                Pid = 0x2006,
                Label = "NZXT Smart Device V2 1",
            });

            parts.Add(new DeviceDefinition()
            {
                Pid = 0x200d,
                Label = "NZXT Smart Device V2 2",
            });

            parts.Add(new DeviceDefinition()
            {
                Pid = 0x200f,
                Label = "NZXT Smart Device V2 3",
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
            var hidFactory = new FilterDeviceDefinition(vendorId: VENDOR_ID, productId: device.Pid, label: device.Label).CreateWindowsHidDeviceFactory();

            var NZXTDeviceDefinition = (await hidFactory.GetConnectedDeviceDefinitionsAsync().ConfigureAwait(false));

            Debug.WriteLine(NZXTDeviceDefinition);
        }
    }
}