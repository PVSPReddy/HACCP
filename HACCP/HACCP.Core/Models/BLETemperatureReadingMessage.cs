using System;

namespace HACCP.Core
{
	public class BleTemperatureReadingMessage
	{
		public short TempUnit{ get; set; }

		public double TempValue{ get; set; }

		public bool IsSleeping{ get; set; }

		public bool IsHigh{ get; set; }

		public bool IsLow{ get; set; }

		public bool IsBatteryLow{ get; set;}

		public bool ShouldRecord{ get; set; }
	}
}

