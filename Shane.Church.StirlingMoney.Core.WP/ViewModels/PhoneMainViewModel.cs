using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Core.WP.Commands;
using System;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.WP.ViewModels
{
	public class PhoneMainViewModel : MainViewModel
	{
		public PhoneMainViewModel(IRepository<Budget, Guid> budgetRepository, IRepository<Goal, Guid> goalRepository, INavigationService navService, SyncService syncService, ILoggingService logService, ISettingsService settingsService)
			: base(budgetRepository, goalRepository, navService, syncService, logService, settingsService)
		{
			RateCommand = new RateThisAppCommand();
		}

		public override async Task Initialize()
		{
			try
			{
				using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
				{
					var dbVersion = 3;
					var dbFile = "";
					if (store.FileExists(StirlingMoney.Data.v2.StirlingMoneyDataContext.DBFileName))
					{
						dbVersion = 2;
						dbFile = StirlingMoney.Data.v2.StirlingMoneyDataContext.DBFileName;
					}
					else if (store.FileExists(StirlingMoney.Data.v1.StirlingMoneyDataContext.DBFileName))
					{
						dbVersion = 1;
						dbFile = StirlingMoney.Data.v1.StirlingMoneyDataContext.DBFileName;
					}

					if (dbVersion != 3)
					{
						IUpgradeDBService upgrade = KernelService.Kernel.Get<IUpgradeDBService>();

						OnBusyChanged(new BusyEventArgs() { AnimationType = 2, IsBusy = true, Message = Shane.Church.StirlingMoney.Strings.Resources.ProgressBarText });

						await Task.Yield();

						await upgrade.UpgradeDatabase(dbVersion, dbFile);

						OnBusyChanged(new BusyEventArgs() { AnimationType = 2, IsBusy = false });
					}
				}
			}
			catch (Exception ex)
			{
				OnBusyChanged(new BusyEventArgs() { AnimationType = 2, IsBusy = false, IsError = true, Error = ex });
			}
		}
	}
}

