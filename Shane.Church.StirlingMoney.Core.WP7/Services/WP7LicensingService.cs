using Shane.Church.StirlingMoney.Core.Services;
using System;
using Telerik.Windows.Controls;

namespace Shane.Church.StirlingMoney.Core.WP7.Services
{
	public class WP7LicensingService : ILicensingService
	{
		private RadTrialApplicationReminder _appReminder;
		private RadTrialFeatureReminder _syncReminder;

		public WP7LicensingService(RadTrialApplicationReminder reminder, RadTrialFeatureReminder syncReminder)
		{
			if (reminder == null) throw new ArgumentNullException("reminder");
			_appReminder = reminder;
			if (syncReminder == null) throw new ArgumentNullException("syncReminder");
			_syncReminder = syncReminder;
		}

		public bool IsSyncLicensed()
		{
#if PERSONAL
			return true;
#else
			if (!_appReminder.IsTrialMode())
				return true;
			else
			{
				if (_syncReminder.FirstUsageDate.HasValue)
				{
					return !_syncReminder.IsTrialExpired;
				}
				else
				{
					_syncReminder.FirstUsageDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
					return true;
				}
			}
#endif
		}

		public bool IsCSVLicensed()
		{
#if PERSONAL
			return true;
#else
			return !_appReminder.IsTrialMode();
#endif
		}

		public bool IsAdvancedReportingLicensed()
		{
#if PERSONAL
			return true;
#else
			return !_appReminder.IsTrialMode();
#endif
		}
	}
}
