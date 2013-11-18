using Microsoft.Phone.Marketplace;
using Shane.Church.StirlingMoney.Core.Services;
using System;

namespace Shane.Church.StirlingMoney.Core.WP7.Services
{
	public class WP7AgentLicensingService : ILicensingService
	{
		private LicenseInformation _licenseInfo;

		public WP7AgentLicensingService(LicenseInformation info)
		{
			if (info == null) throw new ArgumentNullException("info");
			_licenseInfo = info;
		}

		public bool IsSyncLicensed()
		{
#if PERSONAL
			return true;
#else
			return true;  //TODO: Address this
#endif
		}

		public bool IsCSVLicensed()
		{
#if PERSONAL
			return true;
#else
			return !_licenseInfo.IsTrial();
#endif
		}

		public bool IsAdvancedReportingLicensed()
		{
#if PERSONAL
			return true;
#else
			return !_licenseInfo.IsTrial();
#endif
		}
	}
}
