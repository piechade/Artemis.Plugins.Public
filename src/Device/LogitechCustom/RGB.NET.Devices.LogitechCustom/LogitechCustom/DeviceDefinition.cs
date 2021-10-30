using RGB.NET.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGB.NET.Devices.LogitechCustom.LogitechCustom
{
    public class DeviceDefinition : NotifyObject
    {
        public uint Pid { get; set; }
        public ushort UsagePage { get; set; }
        public int Page { get; set; }

        public string Label = "Logitech";
        public byte[]? UsbBuf { get; set; }
        public LogitechCustomController? Controller { get; set; }
        public RGBDeviceType Type { get; set; }
        public bool IsInitialized { get; set; }
        public bool Reconnect { get; set; }

        private bool _connect;

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool Connect
        {
            get { return _connect; }
            set
            {
                OnPropertyChanged(_connect, value, nameof(Connect));
                _connect = value;
            }
        }



        public int Zones { get; set; }

        private void OnPropertyChanged(bool oldValue, bool newValue, string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedExtendedEventArgs(name, oldValue, newValue));
            }
        }

    }

    public class NotifyObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName, object oldvalue, object newvalue)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedExtendedEventArgs(propertyName, oldvalue, newvalue));
        }
    }

    public class PropertyChangedExtendedEventArgs : PropertyChangedEventArgs
    {
        public virtual object OldValue { get; private set; }
        public virtual object NewValue { get; private set; }

        public PropertyChangedExtendedEventArgs(string propertyName, object oldValue,
               object newValue)
               : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
