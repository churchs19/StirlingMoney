using Microsoft.Phone.Marketplace;
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
using Telerik.Windows.Controls;
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
			KernelService.Kernel.Rebind<IRepository<Core.Data.Setting, string>>().To<SettingRepository>();
			KernelService.Kernel.Rebind<IRepository<Core.Data.Tombstone, string>>().To<TombstoneRepository>();
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
			KernelService.Kernel.Rebind<LicenseInformation>().ToSelf().InSingletonScope();
#if !AGENT
			KernelService.Kernel.Rebind<RadTrialApplicationReminder>().ToMethod<RadTrialApplicationReminder>(it =>
			{

				//Creates an instance of the RadTrialApplicationReminder component.
				var trialReminder = new RadTrialApplicationReminder();
				trialReminder.TrialReminderMessageBoxInfo.Title = Shane.Church.StirlingMoney.Strings.Resources.AppTrialReminder_MessageBox_Title;
				trialReminder.TrialReminderMessageBoxInfo.Content = Shane.Church.StirlingMoney.Strings.Resources.AppTrialReminder_MessageBox_Content;
				trialReminder.TrialReminderMessageBoxInfo.SkipFurtherRemindersMessage = Shane.Church.StirlingMoney.Strings.Resources.AppTrialReminder_MessageBox_SkipFurtherRemindersMessage;
				trialReminder.TrialExpiredMessageBoxInfo.Title = Shane.Church.StirlingMoney.Strings.Resources.AppTrialEnd_MessageBox_Title;
				trialReminder.TrialExpiredMessageBoxInfo.Content = Shane.Church.StirlingMoney.Strings.Resources.AppTrialEnd_MessageBox_Content;
				trialReminder.TrialExpiredMessageBoxInfo.SkipFurtherRemindersMessage = Shane.Church.StirlingMoney.Strings.Resources.AppTrialEnd_MessageBox_SkipFurtherRemindersMessage;

				//Sets the length of the trial period.
				trialReminder.AllowedTrialPeriod = TimeSpan.MaxValue;

#if DEBUG_TRIAL
				//The reminder is shown only if the application is in trial mode. When this property is set to true the application will simulate that it is in trial mode.
				trialReminder.SimulateTrialForTests = true;

				trialReminder.OccurrenceUsageCount = 1;
#else
				trialReminder.FreePeriod = TimeSpan.FromDays(7);

				//Sets how often the trial reminder is displayed.
				trialReminder.OccurrencePeriod = TimeSpan.FromDays(7);
#endif
				trialReminder.AllowUsersToSkipFurtherReminders = true;

				return trialReminder;
			}).InSingletonScope();
			KernelService.Kernel.Rebind<RadTrialFeatureReminder>().ToMethod<RadTrialFeatureReminder>(it =>
			{
				var syncReminder = new RadTrialFeatureReminder();
				syncReminder.AllowUsersToSkipFurtherReminders = false;
				syncReminder.AllowedTrialPeriod = TimeSpan.FromDays(30);
#if DEBUG_TRIAL
				//The reminder is shown only if the application is in trial mode. When this property is set to true the application will simulate that it is in trial mode.
				syncReminder.SimulateTrialForTests = true;

				syncReminder.OccurrenceUsageCount = 1;
#else
				syncReminder.OccurrencePeriod = TimeSpan.FromDays(3);
#endif
				syncReminder.TrialExpiredMessageBoxInfo.Title = Shane.Church.StirlingMoney.Strings.Resources.WP7SyncTrialReminderExpired_Title;
				syncReminder.TrialExpiredMessageBoxInfo.Content = Shane.Church.StirlingMoney.Strings.Resources.WP7SyncTrialReminderExpired_Content;
				syncReminder.TrialReminderMessageBoxInfo.Title = Shane.Church.StirlingMoney.Strings.Resources.WP7SyncTrialReminder_Title;
				syncReminder.TrialReminderMessageBoxInfo.Content = Shane.Church.StirlingMoney.Strings.Resources.WP7SyncTrialReminder_Content;

				return syncReminder;
			}).InSingletonScope();
			KernelService.Kernel.Rebind<ILicensingService>().To<WP7LicensingService>().InSingletonScope();
			KernelService.Kernel.Rebind<IBackupService>().To<WP7BackupService>();
			KernelService.Kernel.Rebind<IUpgradeDBService>().To<PhoneUpgradeDBService>();
#else
			KernelService.Kernel.Rebind<ILicensingService>().To<WP7AgentLicensingService>().InSingletonScope();
#endif
		}
	}
}
