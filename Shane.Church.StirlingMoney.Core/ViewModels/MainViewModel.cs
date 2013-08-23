using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class MainViewModel : ObservableObject
	{
		public MainViewModel()
		{
			_accounts = new AccountListViewModel();
			_budgets = new ObservableCollection<BudgetSummaryViewModel>();
			_budgets.CollectionChanged += (s, e) =>
			{
				RaisePropertyChanged(() => Budgets);
			};
			_goals = new ObservableCollection<GoalSummaryViewModel>();
			_goals.CollectionChanged += (s, e) =>
			{
				RaisePropertyChanged(() => Goals);
			};
		}

		private AccountListViewModel _accounts;
		public AccountListViewModel Accounts
		{
			get { return _accounts; }
		}

		private ObservableCollection<BudgetSummaryViewModel> _budgets;
		public ObservableCollection<BudgetSummaryViewModel> Budgets
		{
			get { return _budgets; }
		}

		private ObservableCollection<GoalSummaryViewModel> _goals;
		public ObservableCollection<GoalSummaryViewModel> Goals
		{
			get { return _goals; }
		}


	}
}
