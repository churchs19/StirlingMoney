using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
#if !PERSONAL
using System.Data.Linq;
using Shane.Church.StirlingMoney.Data.v2;
#else
using Shane.Church.StirlingMoney.Data.Sync;
#endif

namespace Shane.Church.StirlingMoney
{
	public class TransactionListItemViewModel : INotifyPropertyChanged
	{
		public TransactionListItemViewModel()
		{
		}

		private Guid _transactionId;
		public Guid TransactionId
		{
			get { return _transactionId; }
			set
			{
				if (_transactionId != value)
				{
					_transactionId = value;
					NotifyPropertyChanged("TransactionId");
				}
			}
		}

		private string _location;
		public string Location
		{
			get { return _location; }
			set
			{
				if (_location != value)
				{
					_location = value;
					NotifyPropertyChanged("Location");
				}
			}
		}

		private double _amount;
		public double Amount
		{
			get { return _amount; }
			set
			{
				if (_amount != value)
				{
					_amount = value;
					NotifyPropertyChanged("Amount");
				}
			}
		}

		private long? _checkNumber;
		public long? CheckNumber
		{
			get { return _checkNumber; }
			set
			{
				if (_checkNumber != value)
				{
					_checkNumber = value;
					NotifyPropertyChanged("CheckNumber");
				}
			}
		}

		private DateTime _transactionDate;
		public DateTime TransactionDate
		{
			get { return _transactionDate; }
			set
			{
				if (_transactionDate != value)
				{
					_transactionDate = value;
					NotifyPropertyChanged("TransactionDate");
				}
			}
		}

		private bool _posted;
		public bool Posted
		{
			get { return _posted; }
			set
			{
				if (_posted != value)
				{
					_posted = value;

					if (TransactionId != Guid.Empty)
					{
#if !PERSONAL
						using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
						{
							var transaction = (from t in _context.Transactions
											   where t.TransactionId == TransactionId
											   select t).FirstOrDefault();
							transaction.Posted = _posted;
							_context.SubmitChanges();
						}
#else
						var transaction = (from t in ContextInstance.Context.TransactionCollection
										   where t.TransactionId == TransactionId
										   select t).FirstOrDefault();
						transaction.Posted = _posted;
						ContextInstance.Context.SaveChanges();
#endif
					}

					NotifyPropertyChanged("Posted");
				}
			}
		}

		private string _memo;
		public string Memo
		{
			get { return _memo; }
			set
			{
				if (_memo != value)
				{
					_memo = value;
					NotifyPropertyChanged("Memo");
				}
			}
		}

		private string _category;
		public string Category
		{
			get { return _category; }
			set
			{
				if (_category != value)
				{
					_category = value;
					NotifyPropertyChanged("Category");
				}
			}
		}

		public Brush TransactionColor
		{
			get
			{
				if (Amount >= 0)
				{
					return new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
				}
				else
				{
					return new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
				}
			}
		}

		public Visibility CheckNumberVisibility
		{
			get
			{
				if (CheckNumber.HasValue)
					return Visibility.Visible;
				else
					return Visibility.Collapsed;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (null != handler)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
