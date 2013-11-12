using Microsoft.WindowsAzure.MobileServices;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.SterlingDb;
using Shane.Church.StirlingMoney.Core.SterlingDb.Repositories;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Core.WP;
using Shane.Church.StirlingMoney.Core.WP.Services;
using Shane.Church.StirlingMoney.Core.WP.ViewModels;
#if WP8
#else
using Shane.Church.StirlingMoney.Core.WP7.Services;
#endif
using System;
using System.Linq;
using Wintellect.Sterling.Core;
using Ninject;

namespace Shane.Church.StirlingMoney.Core.WP7
{
	public static class NinjectBootstrapper
	{
		public static void Bootstrap()
		{
			if (KernelService.Kernel == null)
				KernelService.Kernel = new StandardKernel();
#if !AGENT
			KernelService.Kernel.Rebind<IAgentManagementService>().To<PhoneAgentManagementService>().InSingletonScope();
			KernelService.Kernel.Rebind<IWebNavigationService>().To<PhoneWebNavigationService>().InSingletonScope();
			KernelService.Kernel.Rebind<MainViewModel>().To<PhoneMainViewModel>();
			KernelService.Kernel.Rebind<AboutViewModel>().To<PhoneAboutViewModel>();
			KernelService.Kernel.Rebind<AddEditAccountViewModel>().To<PhoneAddEditAccountViewModel>();
			KernelService.Kernel.Rebind<SettingsViewModel>().To<PhoneSettingsViewModel>();
			KernelService.Kernel.Rebind<ILoggingService>().To<PhoneLoggingService>();
#else
			KernelService.Kernel.Rebind<ILoggingService>().To<Shane.Church.StirlingMoney.Core.WP7.Agent.Services.AgentLoggingService>();
#endif
			KernelService.Kernel.Rebind<INavigationService>().To<PhoneNavigationService>().InSingletonScope();
			KernelService.Kernel.Rebind<ISettingsService>().To<PhoneSettingsService>().InSingletonScope();
			KernelService.Kernel.Rebind<IRepository<Core.Data.Account, Guid>>().To<AccountRepository>();
			KernelService.Kernel.Rebind<IRepository<Core.Data.AppSyncUser, string>>().To<AppSyncUserRepository>();
			KernelService.Kernel.Rebind<IRepository<Core.Data.Budget, Guid>>().To<BudgetRepository>();
			KernelService.Kernel.Rebind<IRepository<Core.Data.Category, Guid>>().To<CategoryRepository>();
			KernelService.Kernel.Rebind<IRepository<Core.Data.Goal, Guid>>().To<GoalRepository>();
			KernelService.Kernel.Rebind<IRepository<Core.Data.Transaction, Guid>>().To<TransactionRepository>();
			KernelService.Kernel.Rebind<ITransactionSum>().To<TransactionRepository>();
#if WP8
#else
			KernelService.Kernel.Rebind<ITileService<Core.Data.Account, Guid>>().To<WP7AccountTileService>().InSingletonScope();
#endif
			KernelService.Kernel.Rebind<AccountTileViewModel>().To<PhoneAccountTileViewModel>();
			KernelService.Kernel.Rebind<IMobileServiceClient>().ToMethod<MobileServiceClient>(it =>
			{
				var client = new MobileServiceClient(
					MobileServiceConfig.Uri,
					MobileServiceConfig.Key
				);
				client.SerializerSettings.DateParseHandling = Newtonsoft.Json.DateParseHandling.DateTimeOffset;
				client.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.RoundtripKind;
				client.SerializerSettings.CamelCasePropertyNames = true;
				client.SerializerSettings.Converters.Remove(client.SerializerSettings.Converters.Where(its => its is Microsoft.WindowsAzure.MobileServices.MobileServiceIsoDateTimeConverter).FirstOrDefault());
				return client;
			});
			KernelService.Kernel.Rebind<SyncService>().To<WP7SyncService>();
			KernelService.Kernel.Rebind<ISterlingPlatformAdapter>().To<Wintellect.Sterling.WP7.PlatformAdapter>();
			KernelService.Kernel.Rebind<ISterlingDriver>().To<Wintellect.Sterling.WP7.IsolatedStorage.IsolatedStorageDriver>();
			KernelService.Kernel.Rebind<ISterlingDatabaseInstance>().To<StirlingMoneyDatabaseInstance>();
			KernelService.Kernel.Rebind<SterlingEngine>().ToSelf().InSingletonScope();
		}
	}
}
