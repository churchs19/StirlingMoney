using System;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Collections.ObjectModel;


namespace Shane.Church.StirlingMoney.Data.v1
{
	[Table]
	public class Account : INotifyPropertyChanged, INotifyPropertyChanging
	{
		public Account()
		{
			_transactions = new EntitySet<Transaction>(
				new Action<Transaction>(this.attachTransaction),
				new Action<Transaction>(this.detachTransaction)
				);
		}

		private long _accountId;
		[Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "BIGINT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
		public long AccountId 
		{
			get { return _accountId; }
			set
			{
				if (_accountId != value)
				{
					NotifyPropertyChanging("AccountId");
					_accountId = value;
					NotifyPropertyChanged("AccountId");
				}
			}
		}

		private string _accountName;
		[Column(CanBeNull = false)]
		public string AccountName
		{
			get { return _accountName; }
			set
			{
				if (_accountName != value)
				{
					NotifyPropertyChanging("AccountName");
					_accountName = value;
					NotifyPropertyChanged("AccountName");
				}
			}
		}

		private double _initialBalance;
		[Column(CanBeNull = false)]
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

		// Version column aids update performance.
		[Column(IsVersion = true)]
		private Binary _version;

		private EntitySet<Transaction> _transactions;

		[Association(Storage = "_transactions", OtherKey = "_accountId", ThisKey = "AccountId")]
		public EntitySet<Transaction> Transactions
		{
			get { return this._transactions; }
			set { this._transactions.Assign(value); }
		}

		private void attachTransaction(Transaction trans)
		{
			NotifyPropertyChanging("Transaction");
			trans.Account = this;
		}

		private void detachTransaction(Transaction trans)
		{
			NotifyPropertyChanging("Transaction");
			trans.Account = null;
		}

		public double AccountBalance
		{
			get
			{
				try
				{
					var transactionTotal = (from t in Transactions
											select t.Amount).Sum();
					return InitialBalance + transactionTotal;
				}
				catch { return InitialBalance; }
			}
		}

		public double PostedBalance
		{
			get
			{
				try
				{
					var transactionTotal = (from t in Transactions
											where t.Posted
											select t.Amount).Sum();
					return InitialBalance + transactionTotal;
				}
				catch { return InitialBalance; }
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
