using GalaSoft.MvvmLight;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class GoalSummaryViewModel : ObservableObject
	{
		public GoalSummaryViewModel()
		{

		}

		public GoalSummaryViewModel(Goal g)
		{
			GoalId = g.GoalId;
			GoalName = g.GoalName;
			GoalAmount = g.Amount;
			InitialBalance = g.InitialBalance;
			TargetDate = g.TargetDate;
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

		private DateTime _targetDate;
		public DateTime TargetDate
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
	}
}
