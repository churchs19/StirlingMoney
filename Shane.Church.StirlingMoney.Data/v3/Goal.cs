using Shane.Church.Utility.Core.WP;
using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Shane.Church.StirlingMoney.Data.v3
{
	[Table]
	public class Goal : ChangingObservableObject
	{
		private long? _id;
		[Column(CanBeNull = true)]
		public long? Id
		{
			get { return _id; }
			set
			{
				Set(() => Id, ref _id, value);
			}
		}

		private DateTime _editDateTime;
		[Column(CanBeNull = false)]
		public DateTime EditDateTime
		{
			get { return _editDateTime; }
			set
			{
				Set(() => EditDateTime, ref _editDateTime, value);
			}
		}

#pragma warning disable 0169
		[Column(IsVersion = true)]
		private Binary _version;
#pragma warning restore 0169

		private bool? _isDeleted;
		[Column(CanBeNull = true)]
		public bool? IsDeleted
		{
			get { return _isDeleted; }
			set
			{
				Set(() => IsDeleted, ref _isDeleted, value);
			}
		}

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
		[Column(CanBeNull = false)]
		public DateTime TargetDate
		{
			get { return _targetDate; }
			set
			{
				Set(() => TargetDate, ref _targetDate, value);
			}
		}

		private DateTime _startDate;
		[Column(CanBeNull = false)]
		public DateTime StartDate
		{
			get { return _startDate; }
			set
			{
				Set(() => StartDate, ref _startDate, value);
			}
		}
	}
}
