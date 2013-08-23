using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.WP.Data;
using Shane.Church.StirlingMoney.Core.WP.Services;
using Shane.Church.StirlingMoney.Data.v3;

namespace Shane.Church.StirlingMoney.WP
{
	public static class NinjectBootstrapper
	{
		public static void Bootstrap()
		{
			KernelService.Kernel = new StandardKernel();
			KernelService.Kernel.Bind<IAgentManagementService>().To<PhoneAgentManagementService>().InSingletonScope();
			KernelService.Kernel.Bind<INavigationService>().To<PhoneNavigationService>().InSingletonScope();
			KernelService.Kernel.Bind<ISettingsService>().To<PhoneSettingsService>().InSingletonScope();
			KernelService.Kernel.Bind<IWebNavigationService>().To<PhoneWebNavigationService>().InSingletonScope();
			KernelService.Kernel.Bind<StirlingMoneyDataContext>().ToSelf().InSingletonScope();
			KernelService.Kernel.Bind<IRepository<Core.Data.Account>>().To<AccountRepository>();
			KernelService.Kernel.Bind<IRepository<Core.Data.AuthorizedUser>>().To<AuthorizedUserRepository>();
			KernelService.Kernel.Bind<IRepository<Core.Data.Budget>>().To<BudgetRepository>();
			KernelService.Kernel.Bind<IRepository<Core.Data.Category>>().To<CategoryRepository>();
			KernelService.Kernel.Bind<IRepository<Core.Data.Goal>>().To<GoalRepository>();
			KernelService.Kernel.Bind<IRepository<Core.Data.Transaction>>().To<TransactionRepository>();
			KernelService.Kernel.Bind<ITransactionSum>().To<TransactionRepository>();
		}
	}
}
