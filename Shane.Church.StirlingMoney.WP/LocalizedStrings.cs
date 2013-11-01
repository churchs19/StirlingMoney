using Shane.Church.StirlingMoney.Strings;

namespace Shane.Church.StirlingMoney.WP
{
	public class LocalizedStrings
	{
		public LocalizedStrings()
		{
		}

		private static Resources localizedResources = new Shane.Church.StirlingMoney.Strings.Resources();

		public Resources LocalizedResources { get { return localizedResources; } }
	}
}
