using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;

namespace Artemis.Plugins.Devices.NZXT
{
    public class NZXTDeviceProvider : DeviceProvider
    {
        private readonly IRgbService _rgbService;

        public NZXTDeviceProvider(IRgbService rgbService) : base(RGB.NET.Devices.NZXT.NZXTDeviceProvider.Instance)
        {
            _rgbService = rgbService;
        }

        public override void Enable()
        {
            _rgbService.AddDeviceProvider(RgbDeviceProvider);
        }

        public override void Disable()
        {
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RgbDeviceProvider.Dispose();
        }
    }
}