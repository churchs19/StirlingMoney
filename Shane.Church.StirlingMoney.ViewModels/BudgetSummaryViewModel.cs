using System;
using System.Net;
using System.Windows;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Shane.Church.StirlingMoney.Tiles;
#if !PERSONAL
using System.Data.Linq;
using Shane.Church.StirlingMoney.Data.v2;
#else
using Shane.Church.StirlingMoney.Data.Sync;
#endif

namespace Shane.Church.StirlingMoney.ViewModels
{
	public class BudgetSummaryViewModel : INotifyPropertyChanged
	{
		public BudgetSummaryViewModel()
		{

		}

		private Guid _budgetId;
		public Guid BudgetId
		{
			get { return _budgetId; }
			set
			{
				if (_budgetId != value)
				{
					_budgetId = value;
					NotifyPropertyChanged("BudgetId");
				}
			}
		}

		private string _budgetName;
		public string BudgetName
		{
			get { return _budgetName; }
			set
			{
				if (_budgetName != value)
				{
					_budgetName = value;
					NotifyPropertyChanged("BudgetName");
				}
			}
		}

		private double _totalAmount;
		public double TotalAmount
		{
			get { return _totalAmount; }
			set
			{
				if (_totalAmount != value)
				{
					_totalAmount = value;
					NotifyPropertyChanged("TotalAmount");
					NotifyPropertyChanged("AmountRemaining");
					NotifyPropertyChanged("AmountRemainingText");
					NotifyPropertyChanged("MaximumChartValue");
					NotifyPropertyChanged("AmountSpentWidth");
					NotifyPropertyChanged("BudgetLineLocation");
					NotifyPropertyChanged("BudgetLineBrush");
				}
			}
		}

		private double _amountSpent;
		public double AmountSpent
		{
			get { return _amountSpent; }
			set
			{
				_amountSpent = value;
				NotifyPropertyChanged("AmountSpent");
				NotifyPropertyChanged("AmountSpentBrush");
				NotifyPropertyChanged("AmountRemaining");
				NotifyPropertyChanged("AmountRemainingText");
				NotifyPropertyChanged("MaximumChartValue");
				NotifyPropertyChanged("AmountSpentWidth");
				NotifyPropertyChanged("BudgetLineLocation");
				NotifyPropertyChanged("BudgetLineBrush");
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
				return string.Format(Resources.ViewModelResources.AmountRemaining, AmountRemaining.ToString("C"));
			}
		}

		private DateTime _startDate;
		public DateTime StartDate
		{
			get { return _startDate; }
			set
			{
				if (_startDate != value)
				{
					_startDate = value;
					NotifyPropertyChanged("StartDate");
					NotifyPropertyChanged("DaysRemaining");
					NotifyPropertyChanged("DaysRemainingText");
				}
			}
		}

		private DateTime _endDate;
		public DateTime EndDate
		{
			get { return _endDate; }
			set
			{
				if (_endDate != value)
				{
					_endDate = value;
					NotifyPropertyChanged("EndDate");
					NotifyPropertyChanged("DaysRemaining");
					NotifyPropertyChanged("DaysRemainingText");
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
					return String.Format(Resources.ViewModelResources.DaysRemaining, DaysRemaining.ToString());
				else
					return String.Format(Resources.ViewModelResources.DayRemaining, DaysRemaining.ToString());
			}
		}

		public double MaximumChartValue
		{
			get
			{
				if (AmountRemaining < 0)
				{
					return AmountSpent;
				}
				else
				{
					return 1.2 * TotalAmount;
				}
			}
		}

		public double AmountSpentWidth
		{
			get
			{
				return 456 * (AmountSpent / MaximumChartValue);
			}
		}

		public Brush AmountSpentBrush
		{
			get
			{
				if (AmountRemaining > 0)
					return new SolidColorBrush(Color.FromArgb(255, 51, 153, 51));
				else
					return new SolidColorBrush(Color.FromArgb(255, 229, 20, 0));
			}
		}

		public double BudgetLineLocation
		{
			get
			{
				return 456 * (TotalAmount / MaximumChartValue);
			}
		}

		public Brush BudgetLineBrush
		{
			get
			{
				if (AmountRemaining > 0)
				{
					return (Brush)Application.Current.Resources["PhoneContrastForegroundBrush"];
				}
				else
				{
					return (Brush)Application.Current.Resources["PhoneContrastBackgroundBrush"];
				}
			}
		}

		public string PinMenuText
		{
			get
			{
				if (!TileUtility.TileExists(BudgetId))
				{
					return Resources.ViewModelResources.PinToStart;
				}
				else
				{
					return Resources.ViewModelResources.UnpinFromStart;
				}
			}
			set
			{
				NotifyPropertyChanged("PinMenuText");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (null != handler)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public void LoadData(Guid budgetId)
		{
#if !PERSONAL
			using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
				Budget b = (from bd in _context.Budgets
							where bd.BudgetId == budgetId
							select bd).FirstOrDefault();
#else
			Budget b = (from bd in ContextInstance.Context.BudgetCollection
						where bd.BudgetId == budgetId
						select bd).FirstOrDefault();
#endif
				if (b != null)
				{
					BudgetId = b.BudgetId;
					BudgetName = b.BudgetName;
					TotalAmount = b.BudgetAmount;
					StartDate = b.CurrentPeriodStart;
					EndDate = b.CurrentPeriodEnd;

					if (!b.CategoryId.HasValue)
					{
#if !PERSONAL
						var amountSpentQuery = (from tx in _context.Transactions
												where tx.TransactionDate >= StartDate &&
												tx.TransactionDate < EndDate &&
												tx.Amount < 0 &&
												!tx.Location.Contains(Resources.ViewModelResources.TransferToComparisonString) &&
												!tx.Location.Contains(Resources.ViewModelResources.TransferFromComparisonString)
												select tx.Amount);
#else
						var amountSpentQuery = (from tx in ContextInstance.Context.TransactionCollection
												where tx.TransactionDate >= StartDate &&
												tx.TransactionDate < EndDate &&
												tx.Amount < 0 &&
												!tx.Location.Contains(Resources.ViewModelResources.TransferToComparisonString) &&
												!tx.Location.Contains(Resources.ViewModelResources.TransferFromComparisonString)
												select tx.Amount);
#endif
						if (amountSpentQuery.Any())
						{
							AmountSpent = -amountSpentQuery.Sum();
						}
						else
						{
							AmountSpent = 0;
						}
					}
					else
					{
#if !PERSONAL
						var amountSpentQuery = (from tx in _context.Transactions
												where tx.TransactionDate >= StartDate &&
												tx.TransactionDate < EndDate &&
												tx.Amount < 0 &&
												!tx.Location.Contains(Resources.ViewModelResources.TransferToComparisonString) &&
												!tx.Location.Contains(Resources.ViewModelResources.TransferFromComparisonString) &&
												tx.CategoryId == b.CategoryId
												select tx.Amount);
#else
						var amountSpentQuery = (from tx in ContextInstance.Context.TransactionCollection
												where tx.TransactionDate >= StartDate &&
												tx.TransactionDate < EndDate &&
												tx.Amount < 0 &&
												!tx.Location.Contains(Resources.ViewModelResources.TransferToComparisonString) &&
												!tx.Location.Contains(Resources.ViewModelResources.TransferFromComparisonString) &&
												tx.CategoryId == b.CategoryId
												select tx.Amount);
#endif
						if (amountSpentQuery.Any())
						{
							AmountSpent = -amountSpentQuery.Sum();
						}
						else
						{
							AmountSpent = 0;
						}
					}
				}
#if !PERSONAL
			}
#endif
		}
	}
}
