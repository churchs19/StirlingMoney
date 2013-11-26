using Shane.Church.StirlingMoney.Core.Services;
using System;
using Telerik.Windows.Controls;

namespace Shane.Church.StirlingMoney.Core.WP8.Services
{
	public class WP8LicensingService : ILicensingService
	{
		private RadTrialApplicationReminder _appReminder;
		private RadTrialFeatureReminder _syncReminder;
		private ISettingsService _settings;

		public WP8LicensingService(RadTrialApplicationReminder reminder, RadTrialFeatureReminder syncReminder, ISettingsService settings)
		{
			if (reminder == null) throw new ArgumentNullException("reminder");
			_appReminder = reminder;
			if (syncReminder == null) throw new ArgumentNullException("syncReminder");
			_syncReminder = syncReminder;
			if (settings == null) throw new ArgumentNullException("settings");
			_settings = settings;
		}

		public bool IsSyncLicensed()
		{
#if PERSONAL
			return true;
#else
			return true;
			//if (!_appReminder.IsTrialMode())
			//	return true;
			//else
			//{
			//	if (_settings.LoadSetting<bool>("SyncEnabled"))
			//	{
			//		if (_syncReminder.FirstUsageDate.HasValue)
			//		{
			//			return !_syncReminder.IsTrialExpired;
			//		}
			//		else
			//		{
			//			_syncReminder.FirstUsageDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
			//			return true;
			//		}
			//	}
			//	else
			//		return false;
			//}
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
