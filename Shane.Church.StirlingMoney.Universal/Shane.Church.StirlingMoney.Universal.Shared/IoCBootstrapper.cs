using Microsoft.WindowsAzure.MobileServices;
using Grace;
using Grace.DependencyInjection;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.SterlingDb;
using Shane.Church.StirlingMoney.Core.SterlingDb.Repositories;
using Shane.Church.StirlingMoney.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Shane.Church.StirlingMoney.Universal.Services;
using Shane.Church.StirlingMoney.Universal.ViewModels;
using Wintellect.Sterling.Core;
using Windows.ApplicationModel.Store;

namespace Shane.Church.StirlingMoney.Universal
{
    public static class IoCBootstrapper
    {
        public static void Bootstrap()
        {
            ContainerService.Container = new Grace.DependencyInjection.DependencyInjectionContainer();

            ContainerService.Container.Configure(c => c.Export<UniversalAgentManagementService>().As<IAgentManagementService>().Lifestyle.Singleton());
            ContainerService.Container.Configure(c=> c.Export<UniversalWebNavigationService>().As<IWebNavigationService>().Lifestyle.Singleton());
            ContainerService.Container.Configure(c=> c.Export<UniversalMainViewModel>().As<MainViewModel>());
            ContainerService.Container.Configure(c => c.Export<UniversalAboutViewModel>().As<AboutViewModel>());
            ContainerService.Container.Configure(c => c.Export<UniversalSettingsViewModel>().As<SettingsViewModel>());
            ContainerService.Container.Configure(c => c.Export<UniversalLoggingService>().As<ILoggingService>());
            ContainerService.Container.Configure(c => c.Export<UniversalNavigationService>().As<INavigationService>().Lifestyle.Singleton());
            ContainerService.Container.Configure(c => c.Export<UniversalAccountTileViewModel>().As<AccountTileViewModel>());
            ContainerService.Container.Configure(c => c.Export<UniversalAddEditAccountViewModel>().As<AddEditAccountViewModel>());

            ContainerService.Container.Configure(c => c.Export<UniversalSettingsService>().As<ISettingsService>().Lifestyle.Singleton());
            ContainerService.Container.Configure(c => c.Export<AccountRepository>().As<IRepository<Core.Data.Account, Guid>>());
            ContainerService.Container.Configure(c => c.Export<AppSyncUserRepository>().As<IRepository<Core.Data.AppSyncUser, string>>());
            ContainerService.Container.Configure(c => c.Export<BudgetRepository>().As<IRepository<Core.Data.Budget, Guid>>());
            ContainerService.Container.Configure(c => c.Export<CategoryRepository>().As<IRepository<Core.Data.Category, Guid>>());
            ContainerService.Container.Configure(c => c.Export<GoalRepository>().As<IRepository<Core.Data.Goal, Guid>>());
            ContainerService.Container.Configure(c => c.Export<TransactionRepository>().As<IRepository<Core.Data.Transaction, Guid>>().As<ITransactionSum>().As<ITransactionSearch>());
            ContainerService.Container.Configure(c => c.Export<SettingRepository>().As<IRepository<Core.Data.Setting, string>>());
            ContainerService.Container.Configure(c => c.Export<TombstoneRepository>().As<IRepository<Core.Data.Tombstone, string>>());
            ContainerService.Container.Configure(c => c.Export<UniversalAccountTileService>().As<ITileService<Core.Data.Account, Guid>>().Lifestyle.Singleton());
            //			KernelService.Kernel.Rebind<AccountTileViewModel>().As<AccountTileViewModel>());
            ContainerService.Container.Configure(c=> c.ExportInstance<IMobileServiceClient>((scope, context) =>
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
            ContainerService.Container.Configure(c => c.Export<UniversalSyncService>().As<SyncService>());
            ContainerService.Container.Configure(c => c.Export<Wintellect.Sterling.WinRT.PlatformAdapter>().As<ISterlingPlatformAdapter>());
            ContainerService.Container.Configure(c => c.ExportInstance<Wintellect.Sterling.WinRT.WindowsStorage.WindowsStorageDriver>((scope, context) => { return new Wintellect.Sterling.WinRT.WindowsStorage.WindowsStorageDriver(); }).As<ISterlingDriver>());
            ContainerService.Container.Configure(c => c.Export<StirlingMoneyDatabaseInstance>().As<ISterlingDatabaseInstance>());
            ContainerService.Container.Configure(c => c.Export<SterlingEngine>().Lifestyle.Singleton());
//            ContainerService.Container.Configure(c => c.Export<LicenseInformation>().Lifestyle.Singleton());

            ContainerService.Container.Configure(c => c.Export<UniversalLicensingService>().As<ILicensingService>().Lifestyle.Singleton());
            ContainerService.Container.Configure(c => c.Export<UniversalBackupService>().As<IBackupService>());
            ContainerService.Container.Configure(c => c.Export<UniversalUpgradeDBService>().As<IUpgradeDBService>());
        }
    }
}
