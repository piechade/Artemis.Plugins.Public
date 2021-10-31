using RGB.NET.Core;

namespace RGB.NET.Devices.NZXT.Device
{
    public class NZXTRgbDeviceInfo : IRGBDeviceInfo
    {
        #region Properties & Fields

        public string DeviceName { get; }

        public RGBDeviceType DeviceType { get; }

        public string Manufacturer => "NZXT";

        public string Model { get; }

        public object? LayoutMetadata { get; set; }

        #endregion

        #region Constructors

        public NZXTRgbDeviceInfo(string model, RGBDeviceType deviceType)
        {
            Model = model;
            DeviceType = deviceType;
            DeviceName = DeviceHelper.CreateDeviceName(Manufacturer, model);
        }

        #endregion
    }
}
