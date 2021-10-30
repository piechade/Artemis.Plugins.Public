using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGB.NET.Devices.LogitechCustom.LogitechCustom
{
    class Observable<T>
    {
		private T value;

		public Action<Observable<T>, T, T>? OnChanged;

		public T Value
		{
			get { return value; }
			set
			{
				var oldValue = this.value;
				this.value = value;
                OnChanged?.Invoke(this, oldValue, value);
            }
		}
	}
}
