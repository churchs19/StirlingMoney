using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Shane.Church.StirlingMoney.Data.v2
{
	[Table]
	public class Budget
	{
		public Budget()
		{

		}

		private Guid _budgetId;
		[Column(IsPrimaryKey = true, CanBeNull = false, DbType = "UNIQUEIDENTIFIER NOT NULL")]
		public Guid BudgetId
		{
			get { return _budgetId; }
			set
			{
				if (_budgetId != value)
				{
					NotifyPropertyChanging("BudgetId");
					_budgetId = value;
					NotifyPropertyChanged("BudgetId");
				}
			}
		}

		private string _budgetName;
		[Column(CanBeNull=false)]
		public string BudgetName
		{
			get { return _budgetName; }
			set
			{
				if (_budgetName != value)
				{
					NotifyPropertyChanging("BudgetName");
					_budgetName = value;
					NotifyPropertyChanged("BudgetName");
				}
			}
		}

		private double _budgetAmount;
		[Column(CanBeNull=false)]
		public double BudgetAmount
		{
			get { return _budgetAmount; }
			set
			{
				if (_budgetAmount != value)
				{
					NotifyPropertyChanging("BudgetAmount");
					_budgetAmount = value;
					NotifyPropertyChanged("BudgetAmount");
				}
			}
		}

		private Guid? _categoryId;
		[Column]
		public Guid? CategoryId
		{
			get { return _categoryId; }
			set
			{
				if (_categoryId != value)
				{
					NotifyPropertyChanging("CategoryId");
					_categoryId = value;
					NotifyPropertyChanged("CategoryId");
				}
			}
		}

		private long _budgetPeriod;
		/// <summary>
		/// 0 = Weekly
		/// 1 = Monthly
		/// 2 = Yearly
		/// 3 = Custom
		/// </summary>
		[Column(CanBeNull=false)]
		public long BudgetPeriod
		{
			get { return _budgetPeriod; }
			set
			{
				if (_budgetPeriod != value)
				{
					NotifyPropertyChanging("BudgetPeriod");
					_budgetPeriod = value;
					NotifyPropertyChanged("BudgetPeriod");
				}
			}
		}

		private DateTime _startDate;
		[Column(CanBeNull=false)]
		public DateTime StartDate
		{
			get { return _startDate; }
			set
			{
				if (_startDate != value)
				{
					NotifyPropertyChanging("StartDate");
					_startDate = value;
					NotifyPropertyChanged("StartDate");
				}
			}
		}

		private DateTime? _endDate;
		[Column(CanBeNull=true)]
		public DateTime? EndDate
		{
			get { return _endDate; }
			set
			{
				if (_endDate != value)
				{
					NotifyPropertyChanging("EndDate");
					_endDate = value;
					NotifyPropertyChanged("EndDate");
				}
			}
		}

		// Version column aids update performance.
		[Column(IsVersion = true)]
		private Binary _version;

		public DateTime CurrentPeriodStart
		{
			get
			{
				if (BudgetPeriod == 3)
				{
					return StartDate;
				}
				else
				{
					int day = StartDate.Day;
					DateTime start = DateTime.Today;
					if (start.Day < day)
					{
						start = start.AddMonths(-1);
					}
					if (start.Month == 2 && start.Month > 28)
					{
						if (DateTime.IsLeapYear(start.Year))
						{
							return new DateTime(start.Year, start.Month, 29);
						}
						else
						{
							return new DateTime(start.Year, start.Month, 28);
						}
					}
					else if ((start.Month == 4 || start.Month == 6 || start.Month == 9 || start.Month == 11) && day == 31)
					{
						return new DateTime(start.Year, start.Month, 30);
					}
					else
					{
						return new DateTime(start.Year, start.Month, day);
					}
				}
			}
		}

		public DateTime CurrentPeriodEnd
		{
			get
			{
				switch (BudgetPeriod)
				{
					case 0:
						return CurrentPeriodStart.AddDays(7);
					case 1:
					default:
						return CurrentPeriodStart.AddMonths(1);
					case 2:
						return CurrentPeriodStart.AddYears(1);
				}
			}
		}


		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		// Used to notify that a property changed
		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion

		#region INotifyPropertyChanging Members

		public event PropertyChangingEventHandler PropertyChanging;

		// Used to notify that a property is about to change
		private void NotifyPropertyChanging(string propertyName)
		{
			if (PropertyChanging != null)
			{
				PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
			}
		}

		#endregion
	}
}
