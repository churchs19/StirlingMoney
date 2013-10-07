using GalaSoft.MvvmLight;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Properties;
using System;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class BudgetSummaryViewModel : ObservableObject
	{
		public BudgetSummaryViewModel()
		{

		}

		public BudgetSummaryViewModel(Budget b)
		{
			BudgetId = b.BudgetId;
			BudgetName = b.BudgetName;
			TotalAmount = b.BudgetAmount;
			AmountSpent = 0;
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
					return baseVal + (10 - (baseVal % 10));
				}
				else
				{
					return baseVal;
				}
			}
		}

		public double TickValue
		{
			get
			{
				var baseVal = Math.Round(TotalAmount / 5);
				if (baseVal % 10 != 0)
				{
					return baseVal + (10 - (baseVal % 10));
				}
				else
				{
					return baseVal;
				}
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

	}
}
