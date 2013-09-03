using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class MainViewModel : ObservableObject
	{
		private IRepository<Budget> _budgetRepository;
		private IRepository<Goal> _goalRepository;

		public MainViewModel(IRepository<Budget> budgetRepository, IRepository<Goal> goalRepository)
		{
			if (budgetRepository == null) throw new ArgumentNullException("budgetRepository");
			_budgetRepository = budgetRepository;
			if (goalRepository == null) throw new ArgumentNullException("goalRepository");
			_goalRepository = goalRepository;
			_accounts = KernelService.Kernel.Get<AccountListViewModel>();
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

		private bool _budgetsLoaded = false;
		private ObservableCollection<BudgetSummaryViewModel> _budgets;
		public ObservableCollection<BudgetSummaryViewModel> Budgets
		{
			get { return _budgets; }
		}

		private bool _goalsLoaded = false;
		private ObservableCollection<GoalSummaryViewModel> _goals;
		public ObservableCollection<GoalSummaryViewModel> Goals
		{
			get { return _goals; }
		}

		public async Task LoadAccounts(bool forceUpdate = false)
		{
			await Accounts.LoadData(forceUpdate);
		}

		public async Task LoadBudgets(bool forceUpdate = false)
		{
			if (!_budgetsLoaded || forceUpdate)
			{
				var budgets = await _budgetRepository.GetAllEntriesAsync();
				foreach (var b in budgets.Select(it => new BudgetSummaryViewModel(it)))
				{
					Budgets.Add(b);
				}
				_budgetsLoaded = true;
			}
		}

		public async Task LoadGoals(bool forceUpdate = false)
		{
			if (!_goalsLoaded || forceUpdate)
			{
				var goals = await _goalRepository.GetAllEntriesAsync();
				foreach (var b in goals.Select(it => new GoalSummaryViewModel(it)))
				{
					Goals.Add(b);
				}
				_goalsLoaded = true;
			}
		}

		protected ICommand _addAccountCommand;
		public ICommand AddAccountCommand
		{
			get
			{
				if (_addAccountCommand == null)
				{
					_addAccountCommand = new RelayCommand(() =>
					{
						var navService = KernelService.Kernel.Get<INavigationService>();
						navService.Navigate<AddEditAccountViewModel>();
					});
				}
				return _addAccountCommand;
			}
		}

		protected ICommand _categoriesCommand;
		public ICommand CategoriesCommand
		{
			get
			{
				if (_categoriesCommand == null)
				{
					_categoriesCommand = new RelayCommand(() =>
					{
						var navService = KernelService.Kernel.Get<INavigationService>();
						navService.Navigate<CategoryListViewModel>();
					});
				}
				return _categoriesCommand;
			}
		}

		protected ICommand _syncCommand;
		public ICommand SyncCommand
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		protected ICommand _reportsCommand;
		public ICommand ReportsCommand
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		protected ICommand _settingsCommand;
		public ICommand SettingsCommand
		{
			get
			{
				if (_settingsCommand == null)
				{
					_settingsCommand = new RelayCommand(() =>
					{
						var navService = KernelService.Kernel.Get<INavigationService>();
						navService.Navigate<SettingsViewModel>();
					});
				}
				return _settingsCommand;
			}
		}

		protected ICommand _rateCommand;
		public ICommand RateCommand
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		protected ICommand _aboutCommand;
		public ICommand AboutCommand
		{
			get
			{
				if (_aboutCommand == null)
				{
					_aboutCommand = new RelayCommand(() =>
					{
						var navService = KernelService.Kernel.Get<INavigationService>();
						navService.Navigate<AboutViewModel>();
					});
				}
				return _aboutCommand;
			}
		}
	}
}
