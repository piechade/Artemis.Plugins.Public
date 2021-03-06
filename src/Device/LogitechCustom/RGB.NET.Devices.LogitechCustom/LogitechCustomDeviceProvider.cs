using System;
using System.Collections.Generic;
using RGB.NET.Core;
using RGB.NET.Devices.LogitechCustom.Device;
using RGB.NET.Devices.LogitechCustom.LogitechCustom;

namespace RGB.NET.Devices.LogitechCustom
{
    public class LogitechCustomDeviceProvider : AbstractRGBDeviceProvider
    {
        private static LogitechCustomDeviceProvider? _instance;

        public static LogitechCustomDeviceProvider Instance => _instance ?? new LogitechCustomDeviceProvider();

        public LogitechCustomDeviceProvider()
        {
            if (_instance != null)
            {
                throw new InvalidOperationException($"There can be only one instance of type {nameof(LogitechCustomDeviceProvider)}");
            }

            _instance = this;
        }
        protected override IEnumerable<IRGBDevice> LoadDevices()
        {
            LogitechCustomLoader.InitializeAsync().Wait();

            foreach (var item in LogitechCustomLoader.GetDevices())
            {
                if (item.deviceDefinition.IsInitialized)
                {
                    LogitechCustomUpdateQueue logitechUpdateQueue = new(GetUpdateTrigger(0), item);
                    yield return new LogitechCustomRgbDevice(new LogitechCustomRgbDeviceInfo(item.deviceDefinition.Label, item.deviceDefinition.Type), logitechUpdateQueue, 1);
                }
            }

        }

        public override void Dispose()
        {
            base.Dispose();
            LogitechCustomLoader.FreeDevices();

            GC.SuppressFinalize(this);
        }
        protected override void InitializeSDK() { }
    }
}
