using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.Utility.Core.Command;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class TransactionListItemViewModel : ObservableObject, IComparable
	{
		private IRepository<Transaction, Guid> _transactionRepository;
		private IRepository<Category, Guid> _categoryRepository;
		private INavigationService _navService;
		private TransactionListViewModel _parent;

		public TransactionListItemViewModel(IRepository<Transaction, Guid> transactionRepository, IRepository<Category, Guid> categoryRepository, INavigationService navService, TransactionListViewModel parent)
		{
			if (transactionRepository == null) throw new ArgumentNullException("transactionRepository");
			_transactionRepository = transactionRepository;
			if (categoryRepository == null) throw new ArgumentNullException("categoryRepository");
			_categoryRepository = categoryRepository;
			if (navService == null) throw new ArgumentNullException("navService");
			_navService = navService;
			if (parent == null) throw new ArgumentNullException("parent");
			_parent = parent;

			EditCommand = new RelayCommand(Edit);
			DeleteCommand = new AsyncRelayCommand(o => Delete());
		}

		private Guid _accountId;

		private Guid _transactionId;
		public Guid TransactionId
		{
			get { return _transactionId; }
			set
			{
				Set(() => TransactionId, ref _transactionId, value);
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

		private double _amount;
		public double Amount
		{
			get { return _amount; }
			set
			{
				Set(() => Amount, ref _amount, value);
			}
		}

		private long _checkNumber;
		public long CheckNumber
		{
			get { return _checkNumber; }
			set
			{
				Set(() => CheckNumber, ref _checkNumber, value);
			}
		}

		private DateTimeOffset _transactionDate;
		public DateTimeOffset TransactionDate
		{
			get { return _transactionDate; }
			set
			{
				Set(() => TransactionDate, ref _transactionDate, value);
			}
		}

		public delegate void PostedChangedHandler();
		public event PostedChangedHandler PostedChanged;

		private bool _posted;
		public bool Posted
		{
			get { return _posted; }
			set
			{
				if (Set(() => Posted, ref _posted, value))
				{
					try
					{
						var t = _transactionRepository.GetEntryAsync(TransactionId).Result;
						if (t != null)
						{
							t.Posted = Posted;
							t = _transactionRepository.AddOrUpdateEntryAsync(t).Result;
						}
						if (PostedChanged != null)
							PostedChanged();
					}
					catch
					{
						Set(() => Posted, ref _posted, !Posted);
					}
				}
			}
		}

		private string _memo;
		public string Memo
		{
			get { return _memo; }
			set
			{
				Set(() => Memo, ref _memo, value);
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

		private DateTimeOffset _editDate;
		public DateTimeOffset EditDate
		{
			get { return _editDate; }
			set
			{
				Set(() => EditDate, ref _editDate, value);
			}
		}

		public bool IsCheck
		{
			get
			{
				return CheckNumber > 0;
			}
		}

		public async Task LoadData(Guid transactionId)
		{
			var t = await _transactionRepository.GetEntryAsync(transactionId);
			if (t != null)
			{
				LoadData(t);
			}
		}

		public void LoadData(Transaction transaction)
		{
			TransactionId = transaction.TransactionId;
			TransactionDate = transaction.TransactionDate;
			Location = transaction.Location;
			Amount = transaction.Amount;
			CheckNumber = transaction.CheckNumber;
			_posted = transaction.Posted;
			Memo = transaction.Note;
			Category = _categoryRepository.GetAllIndexKeys<string>("CategoryName").Where(it => it.Key == transaction.CategoryId).Select(it => it.Value).FirstOrDefault();
			_accountId = transaction.AccountId;
			EditDate = transaction.EditDateTime;
		}

		public ICommand EditCommand { get; private set; }

		public void Edit()
		{
			AddEditTransactionParams param = new AddEditTransactionParams() { AccountId = this._accountId, TransactionId = this.TransactionId };
			_navService.Navigate<AddEditTransactionViewModel>(param);
		}

		public ICommand DeleteCommand { get; private set; }

		public async Task Delete()
		{
			await _transactionRepository.DeleteEntryAsync(TransactionId);
			_parent.Transactions.Remove(this);
			_parent.CurrentRow--;
			_parent.TotalRows--;
		}

		public int CompareTo(object obj)
		{
			if (obj is TransactionListItemViewModel)
			{
				var t = (TransactionListItemViewModel)obj;
				if (this.TransactionDate.Date < t.TransactionDate.Date)
				{
					return -1;
				}
				else if (this.TransactionDate.Date > t.TransactionDate.Date)
				{
					return 1;
				}
				else
				{
					var diff = this.EditDate - t.EditDate;
					return diff.CompareTo(new TimeSpan(0));
				}
			}
			else
			{
				return int.MinValue;
			}
		}
	}
}
