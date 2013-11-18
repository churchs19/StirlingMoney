using System;

namespace Shane.Church.StirlingMoney.Core.Data
{
	public class Setting
	{
		public string Key { get; set; }
		public object Value { get; set; }
		public DateTimeOffset EditDateTime { get; set; }
		public bool IsDeleted { get; set; }
	}
}
