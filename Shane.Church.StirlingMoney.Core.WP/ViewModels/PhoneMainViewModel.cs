using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Core.WP.Commands;

namespace Shane.Church.StirlingMoney.Core.WP.ViewModels
{
	public class PhoneMainViewModel : MainViewModel
	{
		public PhoneMainViewModel(IRepository<Budget> budgetRepository, IRepository<Goal> goalRepository)
			: base(budgetRepository, goalRepository)
		{
			_rateCommand = new RateThisAppCommand();
		}
	}
}
