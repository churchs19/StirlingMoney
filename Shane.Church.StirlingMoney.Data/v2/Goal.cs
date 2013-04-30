using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Shane.Church.StirlingMoney.Data.v2
{
	[Table]
	public class Goal
	{
		public Goal()
		{
		
		}

		private Guid _goalId;
		[Column(IsPrimaryKey = true, CanBeNull = false, DbType = "UNIQUEIDENTIFIER NOT NULL")]
		public Guid GoalId
		{
			get { return _goalId; }
			set
			{
				if (_goalId != value)
				{
					NotifyPropertyChanging("GoalId");
					_goalId = value;
					NotifyPropertyChanged("GoalId");
				}
			}
		}

		private string _goalName;
		[Column(CanBeNull = false)]
		public string GoalName
		{
			get { return _goalName; }
			set
			{
				if (_goalName != value)
				{
					NotifyPropertyChanging("GoalName");
					_goalName = value;
					NotifyPropertyChanged("GoalName");
				}
			}
		}

		[Column]
		internal Guid _accountId;

		private EntityRef<Account> _account;

		[Association(Storage = "_account", ThisKey = "_accountId", OtherKey = "AccountId", IsForeignKey = true)]
		public Account Account
		{
			get { return _account.Entity; }
			set
			{
				NotifyPropertyChanging("Account");
				_account.Entity = value;

				if (value != null)
				{
					_accountId = value.AccountId;
				}

				NotifyPropertyChanged("Account");
			}
		}

		private double _amount;
		[Column]
		public double Amount
		{
			get { return _amount; }
			set
			{
				if (_amount != value)
				{
					NotifyPropertyChanging("Amount");
					_amount = value;
					NotifyPropertyChanged("Amount");
				}
			}
		}

		private double _initialBalance;
		[Column]
		public double InitialBalance
		{
			get { return _initialBalance; }
			set
			{
				if (_initialBalance != value)
				{
					NotifyPropertyChanging("InitialBalance");
					_initialBalance = value;
					NotifyPropertyChanged("InitialBalance");
				}
			}
		}

		private DateTime _targetDate;
		[Column]
		public DateTime TargetDate
		{
			get { return _targetDate; }
			set
			{
				if (_targetDate != value)
				{
					NotifyPropertyChanging("TargetDate");
					_targetDate = value;
					NotifyPropertyChanged("TargetDate");
				}
			}
		}

		// Version column aids update performance.
		[Column(IsVersion = true)]
		private Binary _version;

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
