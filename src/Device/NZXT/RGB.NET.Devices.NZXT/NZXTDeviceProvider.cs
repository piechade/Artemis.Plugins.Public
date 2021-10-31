using System;
using System.Collections.Generic;
using RGB.NET.Core;
using RGB.NET.Devices.NZXT.Device;
using RGB.NET.Devices.NZXT.NZXT;

namespace RGB.NET.Devices.NZXT
{
    public class NZXTDeviceProvider : AbstractRGBDeviceProvider
    {
        private static NZXTDeviceProvider? _instance;

        public static NZXTDeviceProvider Instance => _instance ?? new NZXTDeviceProvider();

        public NZXTDeviceProvider()
        {
            if (_instance != null)
            {
                throw new InvalidOperationException($"There can be only one instance of type {nameof(NZXTDeviceProvider)}");
            }

            _instance = this;
        }
        protected override IEnumerable<IRGBDevice> LoadDevices()
        {
            NZXTLoader.InitializeAsync().Wait();

            foreach (var item in NZXTLoader.GetDevices())
            {
                if (item.NZXTDevice.IsInitialized)
                {
                    NZXTUpdateQueue matUpdateQueue = new(GetUpdateTrigger(0), item);
                    yield return new NZXTRgbDevice(new NZXTRgbDeviceInfo(item.NZXTDevice.Label, item.NZXTDevice.Type), matUpdateQueue, item.NZXTDevice.GetLedCount());
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            NZXTLoader.FreeDevices();

            GC.SuppressFinalize(this);
        }
        protected override void InitializeSDK() { }
    }
}
