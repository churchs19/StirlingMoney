using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.ObjectModel;
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

		private bool _isLoading = false;
		public bool IsLoading
		{
			get { return _isLoading; }
			set
			{
				Set(() => IsLoading, ref _isLoading, value);
			}
		}

		public async Task LoadAccounts(bool forceUpdate = false)
		{
			IsLoading = true;
			await Accounts.LoadData(forceUpdate);
			IsLoading = false;
		}

		public async Task LoadBudgets(bool forceUpdate = false)
		{
			if (!_budgetsLoaded || forceUpdate)
			{
				IsLoading = true;
				var budgets = await _budgetRepository.GetAllEntriesAsync();
				foreach (var b in budgets)
				{
					var budgetModel = KernelService.Kernel.Get<BudgetSummaryViewModel>();
					budgetModel.LoadData(b);
					budgetModel.ItemDeleted += budgetModel_ItemDeleted;
					Budgets.Add(budgetModel);
				}
				_budgetsLoaded = true;
				IsLoading = false;
			}
		}

		void budgetModel_ItemDeleted(object sender)
		{
			var item = sender as BudgetSummaryViewModel;
			if (item != null)
			{
				item.ItemDeleted -= budgetModel_ItemDeleted;
				Budgets.Remove(item);
			}
		}

		public async Task LoadGoals(bool forceUpdate = false)
		{
			if (!_goalsLoaded || forceUpdate)
			{
				IsLoading = true;
				var goals = await _goalRepository.GetAllEntriesAsync();
				foreach (var g in goals)
				{
					var goalModel = KernelService.Kernel.Get<GoalSummaryViewModel>();
					goalModel.LoadData(g);
					goalModel.ItemDeleted += goalModel_ItemDeleted;
					Goals.Add(goalModel);
				}
				_goalsLoaded = true;
				IsLoading = false;
			}
		}

		void goalModel_ItemDeleted(object sender)
		{
			var item = sender as GoalSummaryViewModel;
			if (item != null)
			{
				item.ItemDeleted -= goalModel_ItemDeleted;
				Goals.Remove(item);
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
