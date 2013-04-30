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
	public class GoalSummaryViewModel : INotifyPropertyChanged
	{
		public GoalSummaryViewModel()
		{

		}

		private Guid _goalId;
		public Guid GoalId
		{
			get { return _goalId; }
			set
			{
				if (_goalId != value)
				{
					_goalId = value;
					NotifyPropertyChanged("GoalId");
				}
			}
		}

		private string _goalName;
		public string GoalName
		{
			get { return _goalName; }
			set
			{
				if (_goalName != value)
				{
					_goalName = value;
					NotifyPropertyChanged("GoalName");
				}
			}
		}

		private double _goalAmount;
		public double GoalAmount
		{
			get { return _goalAmount; }
			set
			{
				if (_goalAmount != value)
				{
					_goalAmount = value;
					NotifyPropertyChanged("GoalAmount");
					NotifyPropertyChanged("AmountRemaining");
					NotifyPropertyChanged("AmountRemainingText");
					NotifyPropertyChanged("MaximumChartValue");
					NotifyPropertyChanged("CurrentAmountWidth");
					NotifyPropertyChanged("GoalLineLocation");
					NotifyPropertyChanged("GoalLineBrush");
				}
			}
		}

		private double _currentAmount;
		public double CurrentAmount
		{
			get { return _currentAmount; }
			set
			{
				_currentAmount = value;
				NotifyPropertyChanged("CurrentAmount");
				NotifyPropertyChanged("CurrentAmountBrush");
				NotifyPropertyChanged("AmountRemaining");
				NotifyPropertyChanged("AmountRemainingText");
				NotifyPropertyChanged("MaximumChartValue");
				NotifyPropertyChanged("MinimumChartValue");
				NotifyPropertyChanged("CurrentAmountWidth");
				NotifyPropertyChanged("GoalLineLocation");
				NotifyPropertyChanged("GoalLineBrush");
			}
		}

		private double _initialBalance;
		public double InitialBalance
		{
			get { return _initialBalance; }
			set
			{
				_initialBalance = value;
				NotifyPropertyChanged("InitialBalance");
				NotifyPropertyChanged("MaximumChartValue");
				NotifyPropertyChanged("MinimumChartValue");
				NotifyPropertyChanged("CurrentAmountWidth");
				NotifyPropertyChanged("GoalLineLocation");
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
					return string.Format(Resources.ViewModelResources.AmountRemaining, AmountRemaining.ToString("C"));
				}
				else
				{
					return Resources.ViewModelResources.GoalAchieved;
				}
			}
		}

		private DateTime _targetDate;
		public DateTime TargetDate
		{
			get { return _targetDate; }
			set
			{
				if (_targetDate != value)
				{
					_targetDate = value;
					NotifyPropertyChanged("TargetDate");
					NotifyPropertyChanged("DaysRemaining");
					NotifyPropertyChanged("DaysRemainingText");
				}
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
						return String.Format(Resources.ViewModelResources.DaysRemaining, DaysRemaining.ToString());
					else
						return String.Format(Resources.ViewModelResources.DayRemaining, DaysRemaining.ToString());
				}
				else
				{
					if (DaysRemaining != -1)
						return string.Format(Resources.ViewModelResources.DaysOverdue, (-DaysRemaining).ToString());
					else
						return string.Format(Resources.ViewModelResources.DayOverdue, (-DaysRemaining).ToString());
				}
			}
		}

		public double MaximumChartValue
		{
			get
			{
				var maxAmount = InitialBalance + 1.2 * Math.Abs(GoalAmount - InitialBalance);
				if(CurrentAmount >= maxAmount)
				{
					return CurrentAmount;
				}
				else
				{
					return maxAmount;
				}
			}
		}

		public double MinimumChartValue
		{
			get { return Math.Min(InitialBalance, CurrentAmount); }
		}

		public double CurrentAmountWidth
		{
			get
			{
				return 456 * (Math.Abs(CurrentAmount - MinimumChartValue) / Math.Abs(MaximumChartValue - MinimumChartValue));
			}
		}

		public Brush CurrentAmountBrush
		{
			get
			{
				if (AmountRemaining <= 0)
					return new SolidColorBrush(Color.FromArgb(255, 51, 153, 51));
				else
				{
					if (DaysRemaining > 0)
					{
						return new SolidColorBrush(Color.FromArgb(255, 240, 150, 9));
					}
					else
					{
						return new SolidColorBrush(Color.FromArgb(255, 229, 20, 0));
					}
				}
			}
		}

		public double GoalLineLocation
		{
			get
			{
				return 456 * (Math.Abs(GoalAmount - MinimumChartValue) / Math.Abs(MaximumChartValue - MinimumChartValue));
			}
		}

		public Brush GoalLineBrush
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
				if (TileUtility.TileExists(GoalId))
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

		public void LoadData(Guid goalId)
		{
#if !PERSONAL
			using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
				Goal g = (from gi in _context.Goals
						  where gi.GoalId == goalId
						  select gi).FirstOrDefault();
#else
				Goal g = (from gi in ContextInstance.Context.GoalCollection
						  where gi.GoalId == goalId
						  select gi).FirstOrDefault();			
#endif
				if (g != null)
				{
					GoalId = g.GoalId;
					GoalName = g.GoalName;
					GoalAmount = g.Amount;
					TargetDate = g.TargetDate;
#if !PERSONAL
					CurrentAmount = g.Account.AccountBalance;
#else
					CurrentAmount = (from a in ContextInstance.Context.AccountCollection
									 where a.AccountId == g._accountId
									 select a.AccountBalance).FirstOrDefault();
#endif
					InitialBalance = g.InitialBalance;
				}
#if !PERSONAL
			}
#endif
		}
	}
}
