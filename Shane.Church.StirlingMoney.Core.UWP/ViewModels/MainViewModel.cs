using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Services;

namespace Shane.Church.StirlingMoney.Core.UWP.ViewModels
{
    public class MainViewModel : Core.ViewModels.MainViewModel
    {
        public MainViewModel(IDataRepository<Budget, Guid> budgetRepository, IDataRepository<Goal, Guid> goalRepository, INavigationService navService, SyncService syncService, ILoggingService logService, ISettingsService settingsService)
			: base(budgetRepository, goalRepository, navService, syncService, logService, settingsService)
		{
//            RateCommand = new RateThisAppCommand();
        }

        public override Task Initialize()
        {
            return base.Initialize();
        }
    }
}
