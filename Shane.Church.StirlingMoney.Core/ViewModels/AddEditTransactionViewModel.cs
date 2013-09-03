using GalaSoft.MvvmLight;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Properties;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class AddEditTransactionViewModel : ObservableObject
	{
		private TransactionType _transactionType = TransactionType.Unknown;

		public AddEditTransactionViewModel()
		{
			_categories = new ObservableCollection<string>();
			_categories.CollectionChanged += (s, e) =>
				{
					RaisePropertyChanged(() => Categories);
				};
			_transferAccounts = new ObservableCollection<string>();
			_transferAccounts.CollectionChanged += (s, e) =>
				{
					RaisePropertyChanged(() => TransferAccounts);
				};
		}

		private Guid _transactionId;
		public Guid TransactionId
		{
			get { return _transactionId; }
			set
			{
				Set(() => TransactionId, ref _transactionId, value);
			}
		}

		private Guid _accountId;
		public Guid AccountId
		{
			get { return _accountId; }
			set
			{
				Set(() => AccountId, ref _accountId, value);
			}
		}

		private DateTime _transactionDate;
		public DateTime TransactionDate
		{
			get { return _transactionDate; }
			set
			{
				Set(() => TransactionDate, ref _transactionDate, value);
			}
		}

		private double _amount;
		public double Amount
		{
			get { return _amount; }
			set
			{
				Set(() => Amount, ref _amount, value);
			}
		}

		private string _location;
		public string Location
		{
			get { return _location; }
			set
			{
				Set(() => Location, ref _location, value);
			}
		}

		private string _note;
		public string Note
		{
			get { return _note; }
			set
			{
				Set(() => Note, ref _note, value);
			}
		}

		private bool _posted;
		public bool Posted
		{
			get { return _posted; }
			set
			{
				Set(() => Posted, ref _posted, value);
			}
		}

		private long? _checkNumber;
		public long? CheckNumber
		{
			get { return _checkNumber; }
			set
			{
				Set(() => CheckNumber, ref _checkNumber, value);
			}
		}

		private string _category;
		public string Category
		{
			get { return _category; }
			set
			{
				Set(() => Category, ref _category, value);
			}
		}

		private ObservableCollection<string> _categories;
		public ObservableCollection<string> Categories
		{
			get { return _categories; }
		}

		private string _transferAccount;
		public string TransferAccount
		{
			get { return _transferAccount; }
			set
			{
				Set(() => TransferAccount, ref _transferAccount, value);
			}
		}

		private ObservableCollection<string> _transferAccounts;
		public ObservableCollection<string> TransferAccounts
		{
			get { return _transferAccounts; }
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
				switch (_transactionType)
				{
					case TransactionType.Check:
						return Resources.CheckTitle;
					case TransactionType.Deposit:
						return Resources.DepositTitle;
					case TransactionType.Transfer:
						return Resources.TransferTitle;
					case TransactionType.Withdrawal:
						return Resources.WithdrawalTitle;
					default:
						return Resources.UnknownTitle;
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

		public bool AccountVisible
		{
			get
			{
				return _transactionType == TransactionType.Transfer && TransactionId == Guid.Empty;
			}
		}

		public bool LocationVisible
		{
			get
			{
				return !AccountVisible;
			}
		}

		public bool CategoryVisible
		{
			get
			{
				return _transactionType != TransactionType.Transfer;
			}
		}

		public void LoadData(Guid accountId, Guid transactionId, TransactionType type = TransactionType.Unknown)
		{
			this._transactionType = type;
			AccountId = accountId;
			var transactionRepository = KernelService.Kernel.Get<IRepository<Transaction>>();
			var categoryRepository = KernelService.Kernel.Get<IRepository<Category>>();
			var accountRepository = KernelService.Kernel.Get<IRepository<Account>>();

			if (transactionId == Guid.Empty)
			{
				TransactionDate = DateTime.Today;
				if (_transactionType == TransactionType.Check)
				{
					var checks = transactionRepository.GetFilteredEntries(it => it.CheckNumber.HasValue).Select(it => it.CheckNumber);
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
				Transaction t = transactionRepository.GetFilteredEntries(it => it.TransactionId == TransactionId).FirstOrDefault();
				if (t != null)
				{
					TransactionId = t.TransactionId;
					TransactionDate = t.TransactionDate;
					Amount = t.Amount;
					Location = t.Location;
					Note = t.Note;
					Posted = t.Posted;
					if (t.CategoryId.HasValue)
					{
						Category = categoryRepository.GetFilteredEntries(it => it.CategoryId == t.CategoryId.Value).Select(it => it.CategoryName).FirstOrDefault();
					}
					else
					{
						Category = "";
					}

					CheckNumber = t.CheckNumber;
				}
			}
			if (type == TransactionType.Unknown)
			{
				Transaction t = transactionRepository.GetFilteredEntries(it => it.TransactionId == transactionId).FirstOrDefault();
				if (t.CheckNumber.HasValue)
					_transactionType = TransactionType.Check;
				else if (t.Location != null && (t.Location.Contains(Resources.TransferFromComparisonString) || t.Location.Contains(Resources.TransferToComparisonString)))
					_transactionType = TransactionType.Transfer;
				else if (t.Amount >= 0)
					_transactionType = TransactionType.Deposit;
				else
					_transactionType = TransactionType.Withdrawal;
			}
			if (_transactionType == TransactionType.Check || _transactionType == TransactionType.Withdrawal)
				Amount = -Amount;

			Categories.Clear();
			foreach (var c in categoryRepository.GetAllEntries().OrderBy(it => it.CategoryName).Select(it => it.CategoryName))
				Categories.Add(c);
			if (Categories.Count > 0)
				Categories.Insert(0, "");

			TransferAccounts.Clear();
			foreach (var a in accountRepository.GetAllEntries().OrderBy(it => it.AccountName).Select(it => it.AccountName))
				TransferAccounts.Add(a);
			if (TransferAccounts.Count > 0)
				TransferAccounts.Insert(0, "");
		}

		public IList<string> Validate()
		{
			List<string> validationErrors = new List<string>();

			if (_transactionType == TransactionType.Transfer)
			{
				if (TransactionId == Guid.Empty)
				{
					if (string.IsNullOrWhiteSpace(TransferAccount))
						validationErrors.Add(Resources.AccountRequiredError);
				}
			}
			else
			{
				if (string.IsNullOrWhiteSpace(Location))
					validationErrors.Add(Resources.LocationRequiredError);
			}

			return validationErrors;
		}

		public void SaveTransaction()
		{
			var transactionRepository = KernelService.Kernel.Get<IRepository<Transaction>>();
			var categoryRepository = KernelService.Kernel.Get<IRepository<Category>>();

			Transaction transaction = new Transaction();
			transaction.TransactionId = TransactionId;
			transaction.TransactionDate = TransactionDate;
			transaction.Note = Note;
			transaction.Posted = Posted;
			transaction.AccountId = AccountId;
			Guid categoryId = Guid.Empty;
			if (!string.IsNullOrEmpty(Category))
			{
				categoryId = categoryRepository.GetFilteredEntries(it => it.CategoryName == Category).Select(it => it.CategoryId).FirstOrDefault();
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
						if (Location.Contains(Resources.TransferFromComparisonString))
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
						transaction.Location = string.Format(Resources.TransferToLocation, TransferAccount);

						if (TransactionId == Guid.Empty)
						{
							transaction = transactionRepository.AddOrUpdateEntry(transaction);
						}

						var accountRepository = KernelService.Kernel.Get<IRepository<Account>>();

						Transaction destTransaction = new Transaction();
						destTransaction.TransactionDate = TransactionDate;
						destTransaction.Note = Note;
						destTransaction.Posted = Posted;
						destTransaction.AccountId = accountRepository.GetFilteredEntries(it => it.AccountName == TransferAccount).Select(it => it.AccountId).FirstOrDefault();
						destTransaction.Amount = Amount;
						var accountName = accountRepository.GetFilteredEntries(it => it.AccountId == AccountId).Select(it => it.AccountName).FirstOrDefault();
						destTransaction.Location = string.Format(Resources.TransferFromLocation, accountName);
						destTransaction.TransactionId = Guid.NewGuid();

						transactionRepository.AddOrUpdateEntry(destTransaction);
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

			if (_transactionType != TransactionType.Transfer && TransactionId != Guid.Empty)
			{
				transactionRepository.AddOrUpdateEntry(transaction);
			}
		}
	}
}
