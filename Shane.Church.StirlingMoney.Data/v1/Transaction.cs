using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Shane.Church.StirlingMoney.Data.v1
{
	[Table]
	public class Transaction : INotifyPropertyChanged, INotifyPropertyChanging
	{
		private long _transactionId;
		[Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "BIGINT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
		public long TransactionId
		{
			get { return _transactionId; }
			set
			{
				if (_transactionId != value)
				{
					NotifyPropertyChanging("TransactionId");
					_transactionId = value;
					NotifyPropertyChanged("TransactionId");
				}
			}
		}

		private DateTime _transDate;
		[Column(CanBeNull = false)]
		public DateTime TransactionDate
		{
			get { return _transDate; }
			set
			{
				if (_transDate != value)
				{
					NotifyPropertyChanging("TransactionDate");
					_transDate = value;
					NotifyPropertyChanged("TransactionDate");
				}
			}
		}

		private double _amount;
		[Column(CanBeNull = false)]
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

		private string _location;
		[Column]
		public string Location
		{
			get { return _location; }
			set
			{
				if (_location != value)
				{
					NotifyPropertyChanging("Location");
					_location = value;
					NotifyPropertyChanged("Location");
				}
			}
		}

		private string _note;
		[Column]
		public string Note
		{
			get { return _note; }
			set
			{
				if (_note != value)
				{
					NotifyPropertyChanging("Note");
					_note = value;
					NotifyPropertyChanged("Note");
				}
			}
		}

		private bool _posted;
		[Column(CanBeNull = false)]
		public bool Posted
		{
			get { return _posted; }
			set
			{
				if (_posted != value)
				{
					NotifyPropertyChanging("Posted");
					_posted = value;
					NotifyPropertyChanged("Posted");
				}
			}
		}

		private long? _checkNumber;
		[Column]
		public long? CheckNumber
		{
			get { return _checkNumber; }
			set
			{
				if (_checkNumber != value)
				{
					NotifyPropertyChanging("CheckNumber");
					_checkNumber = value;
					NotifyPropertyChanged("CheckNumber");
				}
			}
		}

		[Column]
		internal long _accountId;

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

		private long? _categoryId;
		[Column]
		public long? CategoryId
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

#pragma warning disable 0169
		// Version column aids update performance.
		[Column(IsVersion = true)]
		private Binary _version;
#pragma warning restore 0169

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
