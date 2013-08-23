using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Shane.Church.StirlingMoney.Data.v3
{
	[Table]
	public class Goal : ChangeTrackingObject
	{
		private Guid _goalId;
		[Column(IsPrimaryKey = true, CanBeNull = false, DbType = "UNIQUEIDENTIFIER NOT NULL")]
		public Guid GoalId
		{
			get { return _goalId; }
			set
			{
                Set(() => GoalId, ref _goalId, value);
			}
		}

		private string _goalName;
		[Column(CanBeNull = false)]
		public string GoalName
		{
			get { return _goalName; }
			set
			{
                Set(() => GoalName, ref _goalName, value);
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
                RaisePropertyChanging(() => Account);
				_account.Entity = value;

				if (value != null)
				{
					_accountId = value.AccountId;
				}

                RaisePropertyChanged(() => Account);
			}
		}

		private double _amount;
		[Column]
		public double Amount
		{
			get { return _amount; }
			set
			{
                Set(() => Amount, ref _amount, value);
			}
		}

		private double _initialBalance;
		[Column]
		public double InitialBalance
		{
			get { return _initialBalance; }
			set
			{
                Set(() => InitialBalance, ref _initialBalance, value);
			}
		}

		private DateTime _targetDate;
		[Column]
		public DateTime TargetDate
		{
			get { return _targetDate; }
			set
			{
                Set(() => TargetDate, ref _targetDate, value);
			}
		}
	}
}
