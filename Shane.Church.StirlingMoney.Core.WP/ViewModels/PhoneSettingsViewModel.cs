using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Strings;
using System;

namespace Shane.Church.StirlingMoney.Core.WP.ViewModels
{
	public class PhoneSettingsViewModel : SettingsViewModel
	{
		public PhoneSettingsViewModel(ISettingsService settings, INavigationService navService, IDataRepository<AppSyncUser, string> userRepository, SyncService syncService)
			: base(settings, navService, userRepository, syncService)
		{
			SyncFeedbackCommand = new RelayCommand(SendFeedback);
		}

		public void SendFeedback()
		{
			EmailComposeTask emailTask = new EmailComposeTask();
			emailTask.To = "shane@s-church.net";
			emailTask.Subject = Resources.SynchronizationFeedbackEmailSubject;
			emailTask.Show();
		}
	}
}
