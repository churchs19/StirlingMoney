using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Strings
{
	public class LocalizedStrings
	{
		public LocalizedStrings()
		{
		}

		private static Resources localizedResources = new Resources();

		public Resources LocalizedResources { get { return localizedResources; } }
	}
}
