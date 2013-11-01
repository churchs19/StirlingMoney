using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Strings;
using System;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class BudgetSummaryViewModel : ObservableObject
	{
		private INavigationService _navService;
		private IRepository<Budget> _budgetRepository;

		public BudgetSummaryViewModel(INavigationService navService, IRepository<Budget> budgetRepository)
		{
			if (navService == null) throw new ArgumentNullException("navService");
			_navService = navService;
			if (budgetRepository == null) throw new ArgumentNullException("budgetRepository");
			_budgetRepository = budgetRepository;
			EditCommand = new RelayCommand(NavigateToEdit);
			DeleteCommand = new RelayCommand(Delete);
		}

		public void LoadData(Budget b)
		{
			BudgetId = b.BudgetId;
			BudgetName = b.BudgetName;
			TotalAmount = b.BudgetAmount;
			AmountSpent = b.AmountSpent;
			StartDate = b.CurrentPeriodStart;
			EndDate = b.CurrentPeriodEnd;
		}

		private Guid _budgetId;
		public Guid BudgetId
		{
			get { return _budgetId; }
			set
			{
				Set(() => BudgetId, ref _budgetId, value);
			}
		}

		private string _budgetName;
		public string BudgetName
		{
			get { return _budgetName; }
			set
			{
				Set(() => BudgetName, ref _budgetName, value);
			}
		}

		private double _totalAmount;
		public double TotalAmount
		{
			get { return _totalAmount; }
			set
			{
				if (Set(() => TotalAmount, ref _totalAmount, value))
				{
					RaisePropertyChanged(() => AmountRemaining);
					RaisePropertyChanged(() => AmountRemainingText);
				}
			}
		}

		private double _amountSpent;
		public double AmountSpent
		{
			get { return _amountSpent; }
			set
			{
				if (Set(() => AmountSpent, ref _amountSpent, value))
				{
					RaisePropertyChanged(() => AmountRemaining);
					RaisePropertyChanged(() => AmountRemainingText);
					RaisePropertyChanged(() => MaxValue);
				}
			}
		}

		public double AmountRemaining
		{
			get
			{
				return TotalAmount - AmountSpent;
			}
		}

		public string AmountRemainingText
		{
			get
			{
				return string.Format(Resources.AmountRemaining, AmountRemaining.ToString("C"));
			}
		}

		public double MaxValue
		{
			get
			{
				var baseVal = Math.Round(1.2 * TotalAmount);
				if (baseVal % 10 != 0)
				{
					baseVal = baseVal + (10 - (baseVal % 10));
				}
				if (AmountSpent > baseVal)
				{
					return AmountSpent;
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
				return (MaxValue / TotalAmount) - 1;
			}
		}

		public double SpendingRatio
		{
			get
			{
				return AmountSpent / TotalAmount;
			}
		}

		private DateTime _startDate;
		public DateTime StartDate
		{
			get { return _startDate; }
			set
			{
				if (Set(() => StartDate, ref _startDate, value))
				{
					RaisePropertyChanged(() => DaysRemaining);
					RaisePropertyChanged(() => DaysRemainingText);
				}
			}
		}

		private DateTime _endDate;
		public DateTime EndDate
		{
			get { return _endDate; }
			set
			{
				if (Set(() => EndDate, ref _endDate, value))
				{
					RaisePropertyChanged(() => DaysRemaining);
					RaisePropertyChanged(() => DaysRemainingText);
				}
			}
		}

		public int DaysRemaining
		{
			get
			{
				TimeSpan days = EndDate.Subtract(DateTime.Today);
				return (days.Days >= 0) ? days.Days : 0;
			}
		}

		public string DaysRemainingText
		{
			get
			{
				if (DaysRemaining != 1)
					return String.Format(Resources.DaysRemaining, DaysRemaining.ToString());
				else
					return String.Format(Resources.DayRemaining, DaysRemaining.ToString());
			}
		}

		public string PinMenuText
		{
			get
			{
				//if (!TileUtility.TileExists(BudgetId))
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
			_navService.Navigate<AddEditBudgetViewModel>(this.BudgetId);
		}

		public delegate void ItemDeletedHandler(object sender);
		public event ItemDeletedHandler ItemDeleted;

		public ICommand DeleteCommand { get; private set; }

		public void Delete()
		{
			Budget b = KernelService.Kernel.Get<Budget>();
			b.BudgetId = this.BudgetId;
			_budgetRepository.DeleteEntry(b);
			if (ItemDeleted != null)
				ItemDeleted(this);
		}
	}
}
