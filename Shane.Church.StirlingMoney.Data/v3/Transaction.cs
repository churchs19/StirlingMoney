using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Shane.Church.Utility.Core.WP;

namespace Shane.Church.StirlingMoney.Data.v3
{
	[Table]
	public class Transaction : ChangeTrackingObject
	{
		private Guid _transactionId;
		[Column(IsPrimaryKey = true, CanBeNull = false, DbType = "UNIQUEIDENTIFIER NOT NULL")]
		public Guid TransactionId
		{
			get { return _transactionId; }
			set
			{
                Set(() => TransactionId, ref _transactionId, value);
			}
		}

		private DateTime _transDate;
		[Column(CanBeNull = false)]
		public DateTime TransactionDate
		{
			get { return _transDate; }
			set
			{
                Set(() => TransactionDate, ref _transDate, value);
			}
		}

		private double _amount;
		[Column(CanBeNull = false)]
		public double Amount
		{
			get { return _amount; }
			set
			{
                Set(() => Amount, ref _amount, value);
			}
		}

		private string _location;
		[Column]
		public string Location
		{
			get { return _location; }
			set
			{
                Set(() => Location, ref _location, value);
			}
		}

		private string _note;
		[Column]
		public string Note
		{
			get { return _note; }
			set
			{
                Set(() => Note, ref _note, value);
			}
		}

		private bool _posted;
		[Column(CanBeNull = false)]
		public bool Posted
		{
			get { return _posted; }
			set
			{
                Set(() => Posted, ref _posted, value);
			}
		}

		private long? _checkNumber;
		[Column]
		public long? CheckNumber
		{
			get { return _checkNumber; }
			set
			{
                Set(() => CheckNumber, ref _checkNumber, value);
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

		private Guid? _categoryId;
		[Column]
		public Guid? CategoryId
		{
			get { return _categoryId; }
			set
			{
                Set(() => CategoryId, ref _categoryId, value);
			}
		}
	}
}
