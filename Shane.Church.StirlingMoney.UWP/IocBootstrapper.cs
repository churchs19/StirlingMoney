using Microsoft.WindowsAzure.MobileServices;
using Grace;
using Grace.DependencyInjection;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Core.Sqlite.Repositories;
using System;
using System.Linq;
using Shane.Church.StirlingMoney.UWP.Services;
using Shane.Church.StirlingMoney.UWP;
using Windows.ApplicationModel.Store;

namespace Shane.Church.StirlingMoney.UWP
{
    public static class IoCBootstrapper
    {
        public static void Bootstrap()
        {
            ContainerService.Container = new DependencyInjectionContainer();

            ContainerService.Container.Configure(c => c.Export<AgentManagementService>().As<IAgentManagementService>().UsingLifestyle(new Grace.DependencyInjection.Lifestyle.SingletonLifestyle()));
            ContainerService.Container.Configure(c => c.Export<WebNavigationService>().As<IWebNavigationService>().UsingLifestyle(new Grace.DependencyInjection.Lifestyle.SingletonLifestyle()));
            ContainerService.Container.Configure(c => c.Export<MainViewModel>().As<MainViewModel>());
            ContainerService.Container.Configure(c => c.Export<AboutViewModel>().As<AboutViewModel>());
            ContainerService.Container.Configure(c => c.Export<SettingsViewModel>().As<SettingsViewModel>());
            ContainerService.Container.Configure(c => c.Export<LoggingService>().As<ILoggingService>());
            ContainerService.Container.Configure(c => c.Export<NavigationService>().As<INavigationService>().UsingLifestyle(new Grace.DependencyInjection.Lifestyle.SingletonLifestyle()));
            ContainerService.Container.Configure(c => c.Export<AccountTileViewModel>().As<AccountTileViewModel>());
            ContainerService.Container.Configure(c => c.Export<AddEditAccountViewModel>().As<AddEditAccountViewModel>());

            ContainerService.Container.Configure(c => c.Export<SettingsService>().As<ISettingsService>().UsingLifestyle(new Grace.DependencyInjection.Lifestyle.SingletonLifestyle()));
            ContainerService.Container.Configure(c => c.Export<AccountRepository>().As<IDataRepository<Core.Data.Account, Guid>>());
            ContainerService.Container.Configure(c => c.Export<AppSyncUserRepository>().As<IDataRepository<Core.Data.AppSyncUser, string>>());
            ContainerService.Container.Configure(c => c.Export<BudgetRepository>().As<IDataRepository<Core.Data.Budget, Guid>>());
            ContainerService.Container.Configure(c => c.Export<CategoryRepository>().As<IDataRepository<Core.Data.Category, Guid>>());
            ContainerService.Container.Configure(c => c.Export<GoalRepository>().As<IDataRepository<Core.Data.Goal, Guid>>());
            ContainerService.Container.Configure(c => c.Export<TransactionRepository>().As<IDataRepository<Core.Data.Transaction, Guid>>().As<ITransactionSum>().As<ITransactionSearch>());
            //            ContainerService.Container.Configure(c => c.Export<Core.Sqlite.Repositories.SettingRepository>().As<IDataRepository<Core.Data.Setting, string>>());
            //            ContainerService.Container.Configure(c => c.Export<TombstoneRepository>().As<IDataRepository<Core.Data.Tombstone, string>>());
            ContainerService.Container.Configure(c => c.Export<AccountTileService>().As<ITileService<Core.Data.Account, Guid>>().UsingLifestyle(new Grace.DependencyInjection.Lifestyle.SingletonLifestyle()));
            //			KernelService.Kernel.Rebind<AccountTileViewModel>().As<AccountTileViewModel>());
            ContainerService.Container.Configure(c => c.ExportInstance<IMobileServiceClient>((scope, context) =>
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
            }));
            ContainerService.Container.Configure(c => c.Export<SyncService>().As<SyncService>());
            //ContainerService.Container.Configure(c => c.Export<Wintellect.Sterling.WP8.PlatformAdapter>().As<ISterlingPlatformAdapter>());
            //ContainerService.Container.Configure(c => c.ExportInstance<Wintellect.Sterling.WP8.IsolatedStorage.IsolatedStorageDriver>((scope, context) => { return new Wintellect.Sterling.WP8.IsolatedStorage.IsolatedStorageDriver(); }).As<ISterlingDriver>());
            //ContainerService.Container.Configure(c => c.Export<StirlingMoneyDatabaseInstance>().As<ISterlingDatabaseInstance>());
            //ContainerService.Container.Configure(c => c.Export<SterlingEngine>().UsingLifestyle(new Grace.DependencyInjection.Lifestyle.SingletonLifestyle()));
            //ContainerService.Container.Configure(c => c.Export<LicenseInformation>().UsingLifestyle(new Grace.DependencyInjection.Lifestyle.SingletonLifestyle()));
//#if !AGENT
//            ContainerService.Container.Configure(c => c.ExportInstance<RadTrialApplicationReminder>((scope, context) =>
//            {
//                //Creates an instance of the RadTrialApplicationReminder component
//                var trialReminder = new RadTrialApplicationReminder();
//                trialReminder.TrialReminderMessageBoxInfo.Title = Shane.Church.StirlingMoney.Strings.Resources.AppTrialReminder_MessageBox_Title;
//                trialReminder.TrialReminderMessageBoxInfo.Content = Shane.Church.StirlingMoney.Strings.Resources.AppTrialReminder_MessageBox_Content;
//                trialReminder.TrialReminderMessageBoxInfo.SkipFurtherRemindersMessage = Shane.Church.StirlingMoney.Strings.Resources.AppTrialReminder_MessageBox_SkipFurtherRemindersMessage;
//                trialReminder.TrialExpiredMessageBoxInfo.Title = Shane.Church.StirlingMoney.Strings.Resources.AppTrialEnd_MessageBox_Title;
//                trialReminder.TrialExpiredMessageBoxInfo.Content = Shane.Church.StirlingMoney.Strings.Resources.AppTrialEnd_MessageBox_Content;
//                trialReminder.TrialExpiredMessageBoxInfo.SkipFurtherRemindersMessage = Shane.Church.StirlingMoney.Strings.Resources.AppTrialEnd_MessageBox_SkipFurtherRemindersMessage;

//                //Sets the length of the trial period.
//                trialReminder.AllowedTrialPeriod = TimeSpan.MaxValue;

//#if DEBUG_TRIAL
//                //The reminder is shown only if the application is in trial mode. When this property is set to true the application will simulate that it is in trial mode.
//                trialReminder.SimulateTrialForTests = true;

//                trialReminder.OccurrenceUsageCount = 1;
//#else
//                trialReminder.FreePeriod = TimeSpan.FromDays(7);

//                //Sets how often the trial reminder is displayed.
//                trialReminder.OccurrencePeriod = TimeSpan.FromDays(7);
//#endif
//                trialReminder.AllowUsersToSkipFurtherReminders = true;

//                return trialReminder;
//            }).UsingLifestyle(new Grace.DependencyInjection.Lifestyle.SingletonLifestyle()));
//            ContainerService.Container.Configure(c => c.ExportInstance<RadTrialFeatureReminder>((scope, context) =>
//            {
//                var syncReminder = new RadTrialFeatureReminder();
//                syncReminder.FeatureId = 1;
//                syncReminder.AllowUsersToSkipFurtherReminders = false;
//                syncReminder.AllowedTrialPeriod = TimeSpan.FromDays(30);
//#if DEBUG_TRIAL
//                //The reminder is shown only if the application is in trial mode. When this property is set to true the application will simulate that it is in trial mode.
//                syncReminder.SimulateTrialForTests = true;

//                syncReminder.OccurrenceUsageCount = 1;
//#else
//                syncReminder.OccurrencePeriod = TimeSpan.FromDays(3);
//#endif
//                syncReminder.TrialExpiredMessageBoxInfo.Title = Shane.Church.StirlingMoney.Strings.Resources.WP7SyncTrialReminderExpired_Title;
//                syncReminder.TrialExpiredMessageBoxInfo.Content = Shane.Church.StirlingMoney.Strings.Resources.WP7SyncTrialReminderExpired_Content;
//                syncReminder.TrialReminderMessageBoxInfo.Title = Shane.Church.StirlingMoney.Strings.Resources.WP7SyncTrialReminder_Title;
//                syncReminder.TrialReminderMessageBoxInfo.Content = Shane.Church.StirlingMoney.Strings.Resources.WP7SyncTrialReminder_Content;

//                return syncReminder;
//            }).UsingLifestyle(new Grace.DependencyInjection.Lifestyle.SingletonLifestyle()));
            ContainerService.Container.Configure(c => c.Export<LicensingService>().As<ILicensingService>().UsingLifestyle(new Grace.DependencyInjection.Lifestyle.SingletonLifestyle()));
            //ContainerService.Container.Configure(c => c.Export<WP8BackupService>().As<IBackupService>());
            ContainerService.Container.Configure(c => c.Export<UpgradeDBService>().As<IUpgradeDBService>());
//#else
//            KernelService.Kernel.Rebind<ILicensingService>().As<WP8AgentLicensingService>().SingleInstance());
            //ContainerService.Container.Configure(c => c.Export<WP8AgentLicensingService>().As<ILicensingService>().UsingLifestyle(new Grace.DependencyInjection.Lifestyle.SingletonLifestyle()));
//#endif
        }
    }
}
