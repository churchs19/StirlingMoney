using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Strings;
using Shane.Church.Utility.Core.Command;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class GoalSummaryViewModel : ObservableObject
	{
		private INavigationService _navService;
		private IRepository<Goal, Guid> _goalRepository;

		public GoalSummaryViewModel(INavigationService navService, IRepository<Goal, Guid> goalRepository)
		{
			if (navService == null) throw new ArgumentNullException("navService");
			_navService = navService;
			if (goalRepository == null) throw new ArgumentNullException("goalRepository");
			_goalRepository = goalRepository;

			EditCommand = new RelayCommand(NavigateToEdit);
			DeleteCommand = new AsyncRelayCommand(o => Delete());
		}

		public async Task LoadData(Guid goalId)
		{
			var g = await _goalRepository.GetEntryAsync(goalId);
			LoadData(g);
		}

		public void LoadData(Goal g)
		{
			GoalId = g.GoalId;
			GoalName = g.GoalName;
			GoalAmount = g.Amount;
			InitialBalance = g.InitialBalance;
			TargetDate = g.TargetDate;
			StartDate = g.StartDate;
			CurrentAmount = Account.GetAccountBalance(g.AccountId);
		}

		private Guid _goalId;
		public Guid GoalId
		{
			get { return _goalId; }
			set
			{
				Set(() => GoalId, ref _goalId, value);
			}
		}

		private string _goalName;
		public string GoalName
		{
			get { return _goalName; }
			set
			{
				Set(() => GoalName, ref _goalName, value);
			}
		}

		private double _goalAmount;
		public double GoalAmount
		{
			get { return _goalAmount; }
			set
			{
				if (Set(() => GoalAmount, ref _goalAmount, value))
				{
					RaisePropertyChanged(() => AmountRemaining);
					RaisePropertyChanged(() => AmountRemainingText);
				}
			}
		}

		private double _currentAmount;
		public double CurrentAmount
		{
			get { return _currentAmount; }
			set
			{
				if (Set(() => CurrentAmount, ref _currentAmount, value))
				{
					RaisePropertyChanged(() => AmountRemaining);
					RaisePropertyChanged(() => AmountRemainingText);
				}
			}
		}

		private double _initialBalance;
		public double InitialBalance
		{
			get { return _initialBalance; }
			set
			{
				Set(() => InitialBalance, ref _initialBalance, value);
			}
		}

		public double AmountRemaining
		{
			get
			{
				return GoalAmount - CurrentAmount;
			}
		}

		public string AmountRemainingText
		{
			get
			{
				if (AmountRemaining > 0)
				{
					return string.Format(Resources.AmountRemaining, AmountRemaining.ToString("C"));
				}
				else
				{
					return Resources.GoalAchieved;
				}
			}
		}

		private DateTimeOffset _targetDate;
		public DateTimeOffset TargetDate
		{
			get { return _targetDate; }
			set
			{
				if (Set(() => TargetDate, ref _targetDate, value))
				{
					RaisePropertyChanged(() => DaysRemaining);
					RaisePropertyChanged(() => DaysRemainingText);
				}
			}
		}

		private DateTimeOffset _startDate;
		public DateTimeOffset StartDate
		{
			get { return _startDate; }
			set
			{
				Set(() => StartDate, ref _startDate, value);
			}
		}

		public int DaysRemaining
		{
			get
			{
				TimeSpan days = TargetDate.Subtract(DateTime.Today);
				return days.Days;
			}
		}

		public string DaysRemainingText
		{
			get
			{
				if (DaysRemaining >= 0)
				{
					if (DaysRemaining != 1)
						return String.Format(Resources.DaysRemaining, DaysRemaining.ToString());
					else
						return String.Format(Resources.DayRemaining, DaysRemaining.ToString());
				}
				else
				{
					if (DaysRemaining != -1)
						return string.Format(Resources.DaysOverdue, (-DaysRemaining).ToString());
					else
						return string.Format(Resources.DayOverdue, (-DaysRemaining).ToString());
				}
			}
		}

		public double MinValue
		{
			get
			{
				return Math.Min(InitialBalance, CurrentAmount);
			}
		}

		public double MaxValue
		{
			get
			{
				var baseVal = Math.Round(1.2 * GoalAmount);
				if (baseVal % 10 != 0)
				{
					baseVal = baseVal + (10 - (baseVal % 10));
				}
				if (GoalAmount > baseVal)
				{
					return GoalAmount;
				}
				else
				{
					return baseVal;
				}
			}
		}

		public double OveragePercentage
		{
			get
			{
				return (MaxValue / GoalAmount) - 1;
			}
		}

		public string PinMenuText
		{
			get
			{
				//if (TileUtility.TileExists(GoalId))
				//{
				//	return Resources.ViewModelResources.PinToStart;
				//}
				//else
				//{
				//	return Resources.ViewModelResources.UnpinFromStart;
				//}
				return "";
			}
		}

		public ICommand EditCommand { get; private set; }

		public void NavigateToEdit()
		{
			_navService.Navigate<AddEditBudgetViewModel>(this.GoalId);
		}

		public delegate void ItemDeletedHandler(object sender);
		public event ItemDeletedHandler ItemDeleted;

		public ICommand DeleteCommand { get; private set; }

		public async Task Delete()
		{
			Goal g = KernelService.Kernel.Get<Goal>();
			g.GoalId = this.GoalId;
			await _goalRepository.DeleteEntryAsync(g);
			if (ItemDeleted != null)
				ItemDeleted(this);
		}
	}
}
