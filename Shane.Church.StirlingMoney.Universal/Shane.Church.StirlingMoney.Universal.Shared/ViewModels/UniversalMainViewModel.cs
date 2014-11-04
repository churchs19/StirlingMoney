using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Universal.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shane.Church.StirlingMoney.Universal.ViewModels
{
    public class UniversalMainViewModel : MainViewModel
    {
        public UniversalMainViewModel(IRepository<Budget, Guid> budgetRepository, IRepository<Goal, Guid> goalRepository, INavigationService navService, SyncService syncService, ILoggingService logService, ISettingsService settingsService)
            : base(budgetRepository, goalRepository, navService, syncService, logService, settingsService)
        {
            RateCommand = new RateThisAppCommand();
        }

    }
}
