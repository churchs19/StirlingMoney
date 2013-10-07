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
		private INavigationService _navService;

		public MainViewModel(IRepository<Budget> budgetRepository, IRepository<Goal> goalRepository, INavigationService navService)
		{
			if (budgetRepository == null) throw new ArgumentNullException("budgetRepository");
			_budgetRepository = budgetRepository;
			if (goalRepository == null) throw new ArgumentNullException("goalRepository");
			_goalRepository = goalRepository;
			if (navService == null) throw new ArgumentNullException("navService");
			_navService = navService;
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

			AddAccountCommand = new RelayCommand(NavigateToAddAccount);
			CategoriesCommand = new RelayCommand(NavigateToCategories);
			SyncCommand = new RelayCommand(Sync);
			ReportsCommand = new RelayCommand(NavigateToReports);
			SettingsCommand = new RelayCommand(NavigateToSettings);
			RateCommand = new RelayCommand(RateApp);
			AboutCommand = new RelayCommand(NavigateToAbout);
			AddBudgetCommand = new RelayCommand(NavigateToAddBudget);
			AddGoalCommand = new RelayCommand(NavigateToAddGoal);
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

		public ICommand AddAccountCommand { get; private set; }

		public void NavigateToAddAccount()
		{
			_navService.Navigate<AddEditAccountViewModel>();
		}

		public ICommand CategoriesCommand { get; private set; }

		public void NavigateToCategories()
		{
			_navService.Navigate<CategoryListViewModel>();
		}

		public ICommand SyncCommand { get; private set; }

		public void Sync()
		{
			throw new NotImplementedException();
		}

		public ICommand ReportsCommand { get; private set; }

		public void NavigateToReports()
		{
			throw new NotImplementedException();
		}

		public ICommand SettingsCommand { get; private set; }

		public void NavigateToSettings()
		{
			_navService.Navigate<SettingsViewModel>();
		}

		public ICommand RateCommand { get; protected set; }

		public void RateApp()
		{
			throw new NotImplementedException();
		}

		public ICommand AboutCommand { get; private set; }

		public void NavigateToAbout()
		{
			_navService.Navigate<AboutViewModel>();
		}

		public ICommand AddBudgetCommand { get; private set; }

		public void NavigateToAddBudget()
		{
			_navService.Navigate<AddEditBudgetViewModel>();
		}

		public ICommand AddGoalCommand { get; private set; }

		public void NavigateToAddGoal()
		{
			_navService.Navigate<AddEditGoalViewModel>();
		}
	}
}
