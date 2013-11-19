using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.StirlingMoney.Core.Data
{
	public class Tombstone
	{
		public Tombstone()
		{
			State = new Dictionary<string, object>();
		}

		public Dictionary<string, object> State { get; set; }

		public T TryGet<T>(string key, T defaultValue)
		{
			if (State.ContainsKey(key))
			{
				return (T)State[key];
			}
			return defaultValue;
		}
	}
}
