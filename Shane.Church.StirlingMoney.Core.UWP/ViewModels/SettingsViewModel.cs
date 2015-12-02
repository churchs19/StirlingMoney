using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;

namespace Shane.Church.StirlingMoney.Core.UWP.ViewModels
{
    public class SettingsViewModel : Core.ViewModels.SettingsViewModel
    {
        public SettingsViewModel(ISettingsService settings, INavigationService navService, IDataRepository<AppSyncUser, string> userRepository, SyncService syncService)
			: base(settings, navService, userRepository, syncService)
		{
//            SyncFeedbackCommand = new RelayCommand(SendFeedback);
        }

    }
}
