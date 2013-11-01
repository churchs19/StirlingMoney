using Newtonsoft.Json.Linq;

namespace Shane.Church.StirlingMoney.Core.Data
{
	public class SyncItem
	{
		public string TableName { get; set; }
		public string KeyField { get; set; }
		public JArray Values { get; set; }
	}
}
