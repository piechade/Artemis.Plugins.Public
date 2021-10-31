using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Device.Net;
using Hid.Net.Windows;
using Microsoft.Extensions.Logging;
using RGB.NET.Core;
// ReSharper disable AsyncConverter.AsyncWait

namespace RGB.NET.Devices.NZXT.NZXT
{
    public static class NZXTLoader
    {
        private static ILoggerFactory? loggerFactory;
        public static List<NZXTController> NZXTController = new();

        public static async Task InitializeAsync()
        {

            loggerFactory = LoggerFactory.Create((builder) =>
            {
                _ = builder.AddDebug().SetMinimumLevel(LogLevel.Trace);
            });


            List<DeviceTypeDefinition> devicetypes = new()
            {
                new DeviceTypeDefinition()
                {
                    Label = "Hue Plus LED Strip",
                    Value = 0x01,
                    LeedCount = 10,
                    Type = RGBDeviceType.LedStripe
                },
                new DeviceTypeDefinition()
                {
                    Label = "Aer 1 RGB Fan",
                    Value = 0x02,
                    LeedCount = 8,
                    Type = RGBDeviceType.Fan
                },
                new DeviceTypeDefinition()
                {
                    Label = "Hue 2 LED Strip",
                    Value = 0x04,
                    LeedCount = 10,
                    Type = RGBDeviceType.LedStripe
                },
                new DeviceTypeDefinition()
                {
                    Label = "Aer 2 RGB Fan(120mmn",
                    Value = 0x0B,
                    LeedCount = 8,
                    Type = RGBDeviceType.Fan
                },
                new DeviceTypeDefinition()
                {
                    Label = "Aer 2 RGB Fan (140mm)",
                    Value = 0x0C,
                    LeedCount = 8,
                    Type = RGBDeviceType.Fan
                }
            };


            List<DeviceDefinition> deviceDefinitions = new()
            {
                new DeviceDefinition()
                {
                    VendorId = 0x1E71,
                    ProductId = 0x2006,
                    Label = "NZXT Smart Device V2",
                    Usage = 1,
                    RgbChannels = 2,
                    FanChannels = 3,
                    Types = devicetypes
                }
            };

            NZXTController = new();


            foreach (var deviceDefinition in deviceDefinitions)
            {
                IDevice device2 = await GetDeviceAsync(vendorId: deviceDefinition.VendorId, productId: deviceDefinition.ProductId, label: deviceDefinition.Label, usage: deviceDefinition.Usage);
                await SendFirmwareRequest(device2);
                NZXTController = await GetAttachedDevicesAsync(deviceDefinition, device2);
            }

        }

        private static async Task SendFirmwareRequest(IDevice device)
        {
            byte[] UsbBuf = new byte[64];

            UsbBuf[0x00] = 0x10;
            UsbBuf[0x01] = 0x01;

            _ = await device.WriteAsync(UsbBuf);

            var result = await device.ReadAsync();

            Debug.WriteLine("Firmware: {0}.{1}.{2}", result.Data[0x11], result.Data[0x12], result.Data[0x13]);

        }

        public static void FreeDevices()
        {

            foreach (var item in NZXTController)
            {
                item.Dispose();
                item.NZXTDevice.IsInitialized = false;
            }
        }

        public static List<NZXTController> GetDevices()
        {
            return NZXTController;
        }

        private static async Task<IDevice> GetDeviceAsync(uint vendorId, uint productId, string label, int usage)
        {
            var hidFactory = new FilterDeviceDefinition(vendorId: vendorId, productId: productId, label: label).CreateWindowsHidDeviceFactory();
            var NZXTDeviceDefinition = (await hidFactory.GetConnectedDeviceDefinitionsAsync().ConfigureAwait(false)).FirstOrDefault(d => d.Usage == usage);
            var NZXTDevice = await hidFactory.GetDeviceAsync(NZXTDeviceDefinition).ConfigureAwait(false);
            NZXTDevice.InitializeAsync().Wait();
            return NZXTDevice;
        }


        public static async Task<List<NZXTController>> GetAttachedDevicesAsync(DeviceDefinition deviceDefinition, IDevice device)
        {
            byte[] outData = new byte[64];
            outData[0x00] = 0x20;
            outData[0x01] = 0x03;
            outData[0x02] = 0x00;


            Debug.WriteLine("Send hid frame: " + BitConverter.ToString(outData));
            _ = await device.WriteAsync(outData);

            List<NZXTController> NZXTController = new();

            TransferResult result;

            do
            {
                result = await device.ReadAsync();
            } while ((result.BytesTransferred != 64) || (result.Data[0] != 0x21) || (result.Data[1] != 0x03));


            Debug.WriteLine("Device result: {0}", result);

            uint numRgbChannels = (uint)deviceDefinition.RgbChannels;
            var channelLeds = new uint[numRgbChannels];

            for (uint chan = 0; chan < numRgbChannels; chan++)
            {
                uint start = 0x0F + (6 * chan);

                NZXTDevice NZXTDevice = new NZXTDevice
                {
                    Device = device,
                    Channel = (byte)(chan + 1),
                    Types = new List<NZXTDeviceType>(),
                    IsInitialized = true
                };

                for (int dev = 0; dev < 6; dev++)
                {
                    Debug.Write("{0:D2}, ", result.Data[start + dev].ToString());

                    var test = deviceDefinition.Types.Find(x => x.Value == result.Data[start + dev]);

                    if (test != null)
                    {
                        NZXTDevice.Types.Add(new NZXTDeviceType
                        {
                            Label = test.Label,
                            LedCount = test.LeedCount
                        });

                        if (NZXTDevice.Type == RGBDeviceType.Unknown)
                        {
                            NZXTDevice.Type = test.Type;
                        }
                        else if (NZXTDevice.Type != test.Type)
                        {
                            NZXTDevice.Type = RGBDeviceType.All;
                        }

                        NZXTDevice.Label = test.Label;
                    }

                }
                var controller = new NZXTController(NZXTDevice);
                NZXTController.Add(controller);

            }

            return NZXTController;
        }

        private static NZXTDevice NewMethod(DeviceDefinition device, IDevice nZXTDevice, byte channel)
        {
            NZXTDevice part = new()
            {
                Device = nZXTDevice,
                Label = device.Label,
                Channel = channel,
                IsInitialized = true
            };
            part.Types = new List<NZXTDeviceType>();
            return part;
        }
    }
}