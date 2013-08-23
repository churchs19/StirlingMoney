using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;


namespace Shane.Church.StirlingMoney.Data.v3
{
	[Table]
	public class Account : ChangeTrackingObject
	{
		public Account()
		{
			_transactions = new EntitySet<Transaction>(
				new Action<Transaction>(this.attachTransaction),
				new Action<Transaction>(this.detachTransaction)
				);
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
