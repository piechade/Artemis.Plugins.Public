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
                if (item.IsInitialized)
                {
                    NZXTUpdateQueue matUpdateQueue = new NZXTUpdateQueue(GetUpdateTrigger(0), item.Controller);
                    yield return new NZXTRgbDevice(new NZXTRgbDeviceInfo(item.Label, item.Type), matUpdateQueue, 1);
                }
            }

        }

        public override void Dispose()
        {
            base.Dispose();
            NZXTLoader.FreeDevices();
        }
        protected override void InitializeSDK() { }
    }
}
