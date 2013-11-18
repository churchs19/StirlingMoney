using System;

namespace Shane.Church.StirlingMoney.Core.Data
{
	public class ProgressChangedArgs : EventArgs
	{
		public int ProgressPercentage { get; set; }
		public long Bytes { get; set; }
		public long TotalBytes { get; set; }
	}
}
