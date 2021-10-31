using System;
using RGB.NET.Core;
using RGB.NET.Devices.LogitechCustom.LogitechCustom;

namespace RGB.NET.Devices.LogitechCustom.Device
{
    public class LogitechCustomUpdateQueue : UpdateQueue
    {
        private readonly LogitechCustomController? _LogitechCustomController;

        public LogitechCustomUpdateQueue(IDeviceUpdateTrigger updateTrigger, LogitechCustomController? LogitechCustomController)
            : base(updateTrigger)
        {
            if (LogitechCustomController != null) _LogitechCustomController = LogitechCustomController;
        }

        protected override void Update(in ReadOnlySpan<(object key, Color color)> dataSet)
        {
            foreach (var (key, color) in dataSet)
            {
                var a = color.GetA();
                var r = color.GetR();
                var g = color.GetG();
                var b = color.GetB();
                _LogitechCustomController?.SetColor(System.Drawing.Color.FromArgb(a, r, g, b), (byte)(int)key);
            }
        }
    }
}
