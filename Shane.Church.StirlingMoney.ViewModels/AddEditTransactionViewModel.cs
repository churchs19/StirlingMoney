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
using System.Collections.Generic;
#if !PERSONAL
using System.Data.Linq;
using Shane.Church.StirlingMoney.Data.v2;
#else
using Shane.Church.StirlingMoney.Data.Sync;
#endif
using System.Collections.ObjectModel;

namespace Shane.Church.StirlingMoney.ViewModels
{
	public class AddEditTransactionViewModel : INotifyPropertyChanged
	{
		private TransactionType _transactionType = TransactionType.Unknown;

		public AddEditTransactionViewModel()
		{
			_categories = new ObservableCollection<string>();
			_transferAccounts = new ObservableCollection<string>();
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

		private Guid _accountId;
		public Guid AccountId
		{
			get { return _accountId; }
			set
			{
				if (_accountId != value)
				{
					_accountId = value;
					NotifyPropertyChanged("AccountId");
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

		private string _note;
		public string Note
		{
			get { return _note; }
			set
			{
				if (_note != value)
				{
					_note = value;
					NotifyPropertyChanged("Note");
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
					NotifyPropertyChanged("Posted");
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

		private ObservableCollection<string> _categories;
		public ObservableCollection<string> Categories
		{
			get { return _categories; }
			set
			{
				if (_categories != value)
				{
					_categories = value;
					if (_categories != null)
					{
						_categories.CollectionChanged += delegate
						{
							NotifyPropertyChanged("Categories");
						};
					}
					NotifyPropertyChanged("Categories");
				}
			}
		}

		private string _transferAccount;
		public string TransferAccount
		{
			get { return _transferAccount; }
			set
			{
				if (_transferAccount != value)
				{
					_transferAccount = value;
					NotifyPropertyChanged("TransferAccount");
				}
			}
		}

		private ObservableCollection<string> _transferAccounts;
		public ObservableCollection<string> TransferAccounts
		{
			get { return _transferAccounts; }
			set
			{
				if (_transferAccounts != value)
				{
					_transferAccounts = value;
					if (_transferAccounts != null)
					{
						_transferAccounts.CollectionChanged += delegate
						{
							NotifyPropertyChanged("TransferAccounts");
						};
					}
					NotifyPropertyChanged("TransferAccounts");
				}
			}
		}

		public bool IsCheck
		{
			get { return _transactionType == TransactionType.Check; }
		}

		public bool IsTransfer
		{
			get { return _transactionType == TransactionType.Transfer; }
		}

		public bool IsDeposit
		{
			get { return _transactionType == TransactionType.Deposit; }
		}

		public string TitleText
		{
			get
			{
				switch(_transactionType)
				{
					case TransactionType.Check:
						return Resources.ViewModelResources.CheckTitle;
					case TransactionType.Deposit:
						return Resources.ViewModelResources.DepositTitle;
					case TransactionType.Transfer:
						return Resources.ViewModelResources.TransferTitle;
					case TransactionType.Withdrawal:
						return Resources.ViewModelResources.WithdrawalTitle;
					default:
						return Resources.ViewModelResources.UnknownTitle;
				}
			}
		}

		public bool IsLocationReadOnly
		{
			get
			{
				return (_transactionType == TransactionType.Transfer && TransactionId != Guid.Empty);
			}
		}

		public Visibility AccountVisibility
		{
			get
			{
				if (_transactionType == TransactionType.Transfer && TransactionId == Guid.Empty)
				{
					return Visibility.Visible;
				}
				else
				{
					return Visibility.Collapsed;
				}
			}
		}

		public Visibility LocationVisibility
		{
			get
			{
				if (AccountVisibility == Visibility.Collapsed)
				{
					return Visibility.Visible;
				}
				else
				{
					return Visibility.Collapsed;
				}
			}
		}

		public Visibility CheckVisibility
		{
			get
			{
				if (IsCheck)
					return Visibility.Visible;
				else
					return Visibility.Collapsed;
			}
		}

		public Visibility CategoryVisibility
		{
			get
			{
				if (_transactionType == TransactionType.Transfer)
				{
					return Visibility.Collapsed;
				}
				else
				{
					return Visibility.Visible;
				}
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

		public void LoadData(Guid accountId, Guid transactionId, TransactionType type = TransactionType.Unknown)
		{
#if !PERSONAL
			using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
#endif
				_transactionType = type;
				AccountId = accountId;

				if (transactionId == Guid.Empty)
				{
					TransactionDate = DateTime.Today;
					if (_transactionType == TransactionType.Check)
					{
#if !PERSONAL
						var checks = (from t in _context.Transactions
									  where (t.Account.AccountId == accountId) && (t.CheckNumber.HasValue)
#else
						var checks = (from t in ContextInstance.Context.TransactionCollection
									  where (t._accountId == accountId) && (t.CheckNumber.HasValue)
#endif
									  select t.CheckNumber.Value);
						if (checks.Any())
						{
							CheckNumber = checks.Max() + 1;
						}
						else
						{
							CheckNumber = 1;
						}
					}
				}
				else
				{
#if !PERSONAL
					Transaction t = (from tx in _context.Transactions
#else
					Transaction t = (from tx in ContextInstance.Context.TransactionCollection
#endif
									 where tx.TransactionId == transactionId
									 select tx).FirstOrDefault();
					TransactionId = t.TransactionId;
					TransactionDate = t.TransactionDate;
					Amount = t.Amount;
					Location = t.Location;
					Note = t.Note;
					Posted = t.Posted;
					if (t.CategoryId.HasValue)
					{
#if !PERSONAL
						Category = (from c in _context.Categories
#else
						Category = (from c in ContextInstance.Context.CategoryCollection
#endif
									where c.CategoryId == t.CategoryId.Value
									select c.CategoryName).FirstOrDefault();
					}
					else
					{
						Category = "";
					}

					CheckNumber = t.CheckNumber;
				}
				if (type == TransactionType.Unknown)
				{
#if !PERSONAL
					Transaction t = (from tx in _context.Transactions
#else
					Transaction t = (from tx in ContextInstance.Context.TransactionCollection
#endif
									 where tx.TransactionId == transactionId
									 select tx).FirstOrDefault();
					if (t.CheckNumber.HasValue)
						_transactionType = TransactionType.Check;
					else if (t.Location != null && (t.Location.Contains(Resources.ViewModelResources.TransferFromComparisonString) || t.Location.Contains(Resources.ViewModelResources.TransferToComparisonString)))
						_transactionType = TransactionType.Transfer;
					else if (t.Amount >= 0)
						_transactionType = TransactionType.Deposit;
					else
						_transactionType = TransactionType.Withdrawal;
				}
				if (_transactionType == TransactionType.Check || _transactionType == TransactionType.Withdrawal)
					Amount = -Amount;

				Categories.Clear();
#if !PERSONAL
				foreach (Category c in _context.Categories.OrderBy(m => m.CategoryName))
#else
				foreach (Category c in ContextInstance.Context.CategoryCollection.OrderBy(m => m.CategoryName))
#endif
				{
					Categories.Add(c.CategoryName);
				}
				if (Categories.Count > 0)
				{
					Categories.Insert(0, "");
				}

				TransferAccounts.Clear();
#if !PERSONAL
				var transferAccounts = (from a in _context.Accounts
#else
				var transferAccounts = (from a in ContextInstance.Context.AccountCollection
#endif
										where a.AccountId != AccountId
										orderby a.AccountName
										select a);
				foreach (Account a in transferAccounts)
				{
					TransferAccounts.Add(a.AccountName);
				}
				if (TransferAccounts.Count > 0)
				{
					TransferAccounts.Insert(0, "");
				}
#if !PERSONAL
			}
#endif
		}

		public IList<string> Validate()
		{
			List<string> validationErrors = new List<string>();

			if (_transactionType == TransactionType.Transfer)
			{
				if (TransactionId == Guid.Empty)
				{
					if (string.IsNullOrWhiteSpace(TransferAccount))
						validationErrors.Add(Shane.Church.StirlingMoney.ViewModels.Resources.ViewModelResources.AccountRequiredError);
				}
			}
			else
			{
				if (string.IsNullOrWhiteSpace(Location))
					validationErrors.Add(Shane.Church.StirlingMoney.ViewModels.Resources.ViewModelResources.LocationRequiredError);
			}

			return validationErrors;
		}

		public void SaveTransaction()
		{
#if !PERSONAL
			using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
#endif
				var transaction = new Transaction();
				if (TransactionId != Guid.Empty)
				{
#if !PERSONAL
					transaction = (from t in _context.Transactions
#else
					transaction = (from t in ContextInstance.Context.TransactionCollection
#endif
								   where t.TransactionId == TransactionId
								   select t).FirstOrDefault();
				}
				transaction.TransactionDate = TransactionDate;
				transaction.Note = Note;
				transaction.Posted = Posted;
#if !PERSONAL
				transaction.Account = (from a in _context.Accounts
									   where a.AccountId == AccountId
									   select a).FirstOrDefault();
#else
				transaction._accountId = AccountId;
#endif
				Guid categoryId = Guid.Empty;
				if (!string.IsNullOrEmpty(Category))
				{
#if !PERSONAL
					categoryId = (from c in _context.Categories
#else
					categoryId = (from c in ContextInstance.Context.CategoryCollection
#endif
								  where c.CategoryName == Category
								  select c.CategoryId).FirstOrDefault();
				}
				switch (_transactionType)
				{
					case TransactionType.Check:
						transaction.Location = Location;
						transaction.CategoryId = categoryId;
						transaction.CheckNumber = CheckNumber;
						transaction.Amount = -Amount;
						break;
					case TransactionType.Deposit:
						transaction.Location = Location;
						transaction.CategoryId = categoryId;
						transaction.Amount = Amount;
						break;
					case TransactionType.Transfer:
						if (TransactionId != Guid.Empty)
						{
							//Editing an existing transfer
							if (Location.Contains(Resources.ViewModelResources.TransferFromComparisonString))
							{
								transaction.Amount = Math.Abs(Amount);
							}
							else
							{
								transaction.Amount = -Math.Abs(Amount);
							}
						}
						else
						{
							//New transfer
							transaction.Amount = -Amount;
							transaction.Location = string.Format(Resources.ViewModelResources.TransferToLocation, TransferAccount);

							if (TransactionId == Guid.Empty)
							{
								transaction.TransactionId = Guid.NewGuid();
#if !PERSONAL
								_context.Transactions.InsertOnSubmit(transaction);
#else
								ContextInstance.Context.AddTransaction(transaction);
#endif
							}

							Transaction destTransaction = new Transaction();
							destTransaction.TransactionDate = TransactionDate;
							destTransaction.Note = Note;
							destTransaction.Posted = Posted;
#if !PERSONAL
							destTransaction.Account = (from a in _context.Accounts
														  where a.AccountName == TransferAccount
														  select a).FirstOrDefault();
#else
							destTransaction._accountId = (from a in ContextInstance.Context.AccountCollection
														  where a.AccountName == TransferAccount
														  select a.AccountId).FirstOrDefault();
#endif
							destTransaction.Amount = Amount;
#if !PERSONAL
							var accountName = (from a in _context.Accounts
											   where a.AccountId == transaction.Account.AccountId
#else
							var accountName = (from a in ContextInstance.Context.AccountCollection
											   where a.AccountId == transaction._accountId
#endif
											   select a.AccountName).FirstOrDefault();
							destTransaction.Location = string.Format(Resources.ViewModelResources.TransferFromLocation, accountName);
							destTransaction.TransactionId = Guid.NewGuid();

#if !PERSONAL
							_context.Transactions.InsertOnSubmit(destTransaction);
#else
							ContextInstance.Context.AddTransaction(destTransaction);
#endif
						}
						break;
					case TransactionType.Withdrawal:
						transaction.Location = Location;
						transaction.CategoryId = categoryId;
						transaction.Amount = -Amount;
						break;
					default:
						break;
				}

				if (TransactionId == Guid.Empty && _transactionType != TransactionType.Transfer)
				{
					transaction.TransactionId = Guid.NewGuid();
#if !PERSONAL
					_context.Transactions.InsertOnSubmit(transaction);
#else
					ContextInstance.Context.AddTransaction(transaction);
#endif
				}
#if !PERSONAL
				_context.SubmitChanges();
			}
#else
			ContextInstance.Context.SaveChanges();
#endif
		}
	}
}
