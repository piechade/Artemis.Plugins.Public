using System;
using RGB.NET.Core;
using RGB.NET.Devices.NZXT.NZXT;

namespace RGB.NET.Devices.NZXT.Device
{
    public class NZXTUpdateQueue : UpdateQueue
    {
        private readonly NZXTController? _NZXTController;

        public NZXTUpdateQueue(IDeviceUpdateTrigger updateTrigger, NZXTController? NZXTController)
            : base(updateTrigger)
        {
            if (NZXTController != null) _NZXTController = NZXTController;
        }

        protected override void Update(in ReadOnlySpan<(object key, Color color)> dataSet)
        {
            foreach (var d in dataSet)
            {
                var a = d.color.GetA();
                var r = d.color.GetR();
                var g = d.color.GetG();
                var b = d.color.GetB();
                _NZXTController?.SetColor(System.Drawing.Color.FromArgb(a, r, g, b), (byte)(int)d.key);
            }
        }
    }
}
