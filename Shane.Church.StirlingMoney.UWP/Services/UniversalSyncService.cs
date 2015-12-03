using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shane.Church.StirlingMoney.Core.Services;
using Microsoft.WindowsAzure.MobileServices;
using Shane.Church.StirlingMoney.Core.Repositories;

namespace Shane.Church.StirlingMoney.UWP.Services
{
    public class UniversalSyncService : SyncService
    {
        public UniversalSyncService(IMobileServiceClient client,
                            ISettingsService settings,
                            ILoggingService log,
                            IDataRepository<Core.Data.Account, Guid> accounts,
                            IDataRepository<Core.Data.AppSyncUser, string> users,
                            IDataRepository<Core.Data.Budget, Guid> budgets,
                            IDataRepository<Core.Data.Goal, Guid> goals,
                            IDataRepository<Core.Data.Category, Guid> categories,
                            IDataRepository<Core.Data.Transaction, Guid> transactions,
                            ILicensingService licensing)
			: base(client, settings, log, accounts, users, budgets, goals, categories, transactions, licensing)
        {

        }
        public override Task AuthenticateUser()
        {
            throw new NotImplementedException();
        }

        public override Task AuthenticateUserSilent()
        {
            throw new NotImplementedException();
        }

        public override void Disconnect()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> IsNetworkConnected()
        {
            throw new NotImplementedException();
        }
    }
}
