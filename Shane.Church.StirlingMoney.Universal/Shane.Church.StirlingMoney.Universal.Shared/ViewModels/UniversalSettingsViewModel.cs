using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Strings;
using Shane.Church.StirlingMoney.Universal.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shane.Church.StirlingMoney.Universal.ViewModels
{
    public class UniversalSettingsViewModel : SettingsViewModel
    {
        public UniversalSettingsViewModel(ISettingsService settings, INavigationService navService, IRepository<AppSyncUser, string> userRepository, SyncService syncService)
            : base(settings, navService, userRepository, syncService)
        {
            SyncFeedbackCommand = new RelayCommand(SendFeedback);
        }

        public async void SendFeedback()
        {
            var mailto = new Uri(String.Format("mailto:?to=shane@s-church.net&subject={0}", Resources.SynchronizationFeedbackEmailSubject));
            await Windows.System.Launcher.LaunchUriAsync(mailto);
        }
    }
}
