using Shane.Church.Utility.Core.WP;
using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;


namespace Shane.Church.StirlingMoney.Data.v3
{
	[Table]
	public class Account : ChangingObservableObject
	{
		public Account()
		{
			_transactions = new EntitySet<Transaction>(
				new Action<Transaction>(this.attachTransaction),
				new Action<Transaction>(this.detachTransaction)
				);
		}

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

		private Guid _accountId;
		[Column(IsPrimaryKey = true, CanBeNull = false, DbType = "UNIQUEIDENTIFIER NOT NULL")]
		public Guid AccountId
		{
			get { return _accountId; }
			set
			{
				Set(() => AccountId, ref _accountId, value);
			}
		}

		private string _accountName;
		[Column(CanBeNull = false)]
		public string AccountName
		{
			get { return _accountName; }
			set
			{
				Set(() => AccountName, ref _accountName, value);
			}
		}

		private double _initialBalance;
		[Column(CanBeNull = false)]
		public double InitialBalance
		{
			get { return _initialBalance; }
			set
			{
				Set(() => InitialBalance, ref _initialBalance, value);
			}
		}

		private string _imageUri;
		[Column]
		public string ImageUri
		{
			get { return _imageUri; }
			set
			{
				Set(() => ImageUri, ref _imageUri, value);
			}
		}

		//private bool _isCreditCard;
		//public bool IsCreditCard
		//{
		//	get { return _isCreditCard; }
		//	set
		//	{
		//		Set(() => IsCreditCard, ref _isCreditCard, value);
		//	}
		//}

		//private double _creditLimit;
		//public double CreditLimit
		//{
		//	get { return _creditLimit; }
		//	set
		//	{
		//		Set(() => CreditLimit, ref _creditLimit, value);
		//	}
		//}

		private double? _archivedBalance;
		[Column(CanBeNull = true)]
		public double? ArchivedBalance
		{
			get { return _archivedBalance; }
			set
			{
				Set(() => ArchivedBalance, ref _archivedBalance, value);
			}
		}

		private double _accountBalance;
		[Column]
		public double AccountBalance
		{
			get { return _accountBalance; }
			set
			{
				Set(() => AccountBalance, ref _accountBalance, value);
			}
		}

		private double _postedBalance;
		[Column]
		public double PostedBalance
		{
			get { return _postedBalance; }
			set
			{
				Set(() => PostedBalance, ref _postedBalance, value);
			}
		}

		private EntitySet<Transaction> _transactions;

		[Association(Storage = "_transactions", OtherKey = "_accountId", ThisKey = "AccountId")]
		public EntitySet<Transaction> Transactions
		{
			get { return this._transactions; }
			set { this._transactions.Assign(value); }
		}

		private void attachTransaction(Transaction trans)
		{
			RaisePropertyChanging(() => Transactions);
			trans.Account = this;
			RaisePropertyChanged(() => Transactions);
		}

		private void detachTransaction(Transaction trans)
		{
			RaisePropertyChanging(() => Transactions);
			trans.Account = null;
			RaisePropertyChanged(() => Transactions);
		}
	}
}
