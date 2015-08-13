using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Grace;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels.Reports;
using Shane.Church.Utility.Core.Command;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class MainViewModel : ObservableObject
	{
		private IDataRepository<Budget, Guid> _budgetRepository;
		private IDataRepository<Goal, Guid> _goalRepository;
		private INavigationService _navService;
		private SyncService _syncService;
		private ILoggingService _logService;
		private ISettingsService _settingsService;

		public MainViewModel(IDataRepository<Budget, Guid> budgetRepository, IDataRepository<Goal, Guid> goalRepository, INavigationService navService, SyncService syncService, ILoggingService logService, ISettingsService settingsService)
		{
			if (budgetRepository == null) throw new ArgumentNullException("budgetRepository");
			_budgetRepository = budgetRepository;
			if (goalRepository == null) throw new ArgumentNullException("goalRepository");
			_goalRepository = goalRepository;
			if (navService == null) throw new ArgumentNullException("navService");
			_navService = navService;
			if (syncService == null) throw new ArgumentNullException("syncService");
			_syncService = syncService;
			_syncService.SyncCompleted += _syncService_SyncCompleted;
			if (logService == null) throw new ArgumentNullException("logService");
			_logService = logService;
			if (settingsService == null) throw new ArgumentNullException("settingsService");
			_settingsService = settingsService;
			_accounts = ContainerService.Container.Locate<AccountListViewModel>();
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
			SyncCommand = new AsyncRelayCommand(action: it => Sync(it), completed: () => SyncTaskCompleted(), error: it => SyncError(it));
			ReportsCommand = new RelayCommand(NavigateToReports);
			SettingsCommand = new RelayCommand(NavigateToSettings);
			RateCommand = new RelayCommand(RateApp);
			AboutCommand = new RelayCommand(NavigateToAbout);
			AddBudgetCommand = new RelayCommand(NavigateToAddBudget);
			AddGoalCommand = new RelayCommand(NavigateToAddGoal);
			BackupCommand = new RelayCommand(NavigateToBackup);
		}

		public virtual Task Initialize()
		{
			return Task.Run(() => { });
		}

		public delegate void SyncCompletedHandler();
		public event SyncCompletedHandler SyncCompleted;

		void _syncService_SyncCompleted()
		{
			if (SyncCompleted != null)
				SyncCompleted();
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
			if (BusyChanged != null)
			{
				BusyChanged(this, new BusyEventArgs() { AnimationType = 2, IsBusy = true, Message = Shane.Church.StirlingMoney.Strings.Resources.ProgressBarText });
			}

			await Task.Yield();

			await Accounts.LoadData(forceUpdate);

			if (BusyChanged != null)
			{
				BusyChanged(this, new BusyEventArgs() { IsBusy = false });
			}
		}

		public async Task LoadBudgets(bool forceUpdate = false)
		{
			if (!_budgetsLoaded || forceUpdate)
			{
				if (BusyChanged != null)
				{
					BusyChanged(this, new BusyEventArgs() { AnimationType = 2, IsBusy = true, Message = Shane.Church.StirlingMoney.Strings.Resources.ProgressBarText });
				}

				await Task.Yield();

                var budgets = await _budgetRepository.GetAllEntriesAsync();
                var keys = budgets.Select(it => it.BudgetId).ToList();
                foreach (var b in Budgets.Where(it => !keys.Contains(it.BudgetId)))
					Budgets.Remove(b);
				foreach (var b in budgets)
				{
					BudgetSummaryViewModel budgetModel = Budgets.Where(it => it.BudgetId == b.BudgetId).FirstOrDefault();
					if(budgetModel == null)
					{
						budgetModel = ContainerService.Container.Locate<BudgetSummaryViewModel>();
						budgetModel.ItemDeleted += budgetModel_ItemDeleted;
						Budgets.Add(budgetModel);
					}
					budgetModel.LoadData(b);
				}
				_budgetsLoaded = true;
				if (BusyChanged != null)
				{
					BusyChanged(this, new BusyEventArgs() { IsBusy = false });
				}
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
				if (BusyChanged != null)
				{
					BusyChanged(this, new BusyEventArgs() { AnimationType = 2, IsBusy = true, Message = Shane.Church.StirlingMoney.Strings.Resources.ProgressBarText });
				}

				await Task.Yield();

                var goals = await _goalRepository.GetAllEntriesAsync();
                var keys = goals.Select(it => it.GoalId).ToList();
				foreach (var g in Goals.Where(it => !keys.Contains(it.GoalId)))
					Goals.Remove(g);
				foreach (var g in goals)
				{
					GoalSummaryViewModel goalModel = Goals.Where(it => it.GoalId == g.GoalId).FirstOrDefault();
					if(goalModel == null)
					{
						goalModel = ContainerService.Container.Locate<GoalSummaryViewModel>();
						goalModel.ItemDeleted += goalModel_ItemDeleted;
						Goals.Add(goalModel);
					}
					goalModel.LoadData(g);
				}
				_goalsLoaded = true;
				if (BusyChanged != null)
				{
					BusyChanged(this, new BusyEventArgs() { IsBusy = false });
				}
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

		public async Task Sync(object param)
		{
			if (BusyChanged != null)
			{
				BusyChanged(this, new BusyEventArgs() { AnimationType = 7, IsBusy = true, Message = Shane.Church.StirlingMoney.Strings.Resources.ProgressBarSyncText });
			}
			await Task.Yield();

			await _syncService.Sync();
		}

		public event EventHandler<BusyEventArgs> BusyChanged;

		protected virtual void OnBusyChanged(BusyEventArgs e)
		{
			// Make a temporary copy of the event to avoid possibility of 
			// a race condition if the last subscriber unsubscribes 
			// immediately after the null check and before the event is raised.
			EventHandler<BusyEventArgs> handler = BusyChanged;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		public Task SyncTaskCompleted()
		{
			return Task.Factory.StartNew(() =>
			{
				if (BusyChanged != null)
				{
					BusyChanged(this, new BusyEventArgs() { IsBusy = false });
				}
			});
		}

		public void SyncError(Exception ex)
		{
			if (SyncCompleted != null)
			{
				SyncCompleted();
			}
			if (BusyChanged != null)
			{
				BusyChanged(this, new BusyEventArgs() { IsBusy = false, IsError = true, Error = ex });
			}
		}

		public ICommand ReportsCommand { get; private set; }

		public void NavigateToReports()
		{
			_navService.Navigate<ReportsViewModel>();
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

		public ICommand BackupCommand { get; private set; }

		public void NavigateToBackup()
		{
			_navService.Navigate<BackupViewModel>();
		}
	}
}
