using Microsoft.WindowsAzure.MobileServices;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wintellect.Sterling.Core;

namespace Shane.Church.StirlingMoney.Universal.Services
{
    public class UniversalSyncService : SyncService
    {
        public UniversalSyncService(IMobileServiceClient client,
							ISettingsService settings,
							ILoggingService log,
							IRepository<Account, Guid> accounts,
							IRepository<AppSyncUser, string> users,
							IRepository<Budget, Guid> budgets,
							IRepository<Goal, Guid> goals,
							IRepository<Category, Guid> categories,
							IRepository<Transaction, Guid> transactions,
							SterlingEngine engine,
							ILicensingService licensing)
			: base(client, settings, log, accounts, users, budgets, goals, categories, transactions, licensing)
		{

        }   

        public override void Disconnect()
        {
            //throw new NotImplementedException();
        }

        public override async Task<bool> IsNetworkConnected()
        {
            return await Task.FromResult(false);
        }

        public override async Task AuthenticateUserSilent()
        {
            
        }

        public override async Task AuthenticateUser()
        {
            
        }
    }
}
