using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Grace;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
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
		internal TransactionListViewModel _parent;

		public TransactionListItemViewModel()
			: this(ContainerService.Container.Locate<IRepository<Transaction, Guid>>(),
					ContainerService.Container.Locate<IRepository<Category, Guid>>(),
					ContainerService.Container.Locate<INavigationService>())
		{

		}

		public TransactionListItemViewModel(IRepository<Transaction, Guid> transactionRepository, IRepository<Category, Guid> categoryRepository, INavigationService navService, TransactionListViewModel parent = null)
		{
			if (transactionRepository == null) throw new ArgumentNullException("transactionRepository");
			_transactionRepository = transactionRepository;
			if (categoryRepository == null) throw new ArgumentNullException("categoryRepository");
			_categoryRepository = categoryRepository;
			if (navService == null) throw new ArgumentNullException("navService");
			_navService = navService;

			EditCommand = new RelayCommand(Edit);
			DeleteCommand = new RelayCommand(Delete);
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

		public delegate void PostedChangedHandler(TransactionListItemViewModel sender);
		public event PostedChangedHandler PostedChanged;

		internal bool _posted;
		public bool Posted
		{
			get { return _posted; }
			set
			{
				var oldValue = _posted;
				if (Set(() => Posted, ref _posted, value))
				{
					if (PostedChanged != null)
						PostedChanged(this);
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

		public delegate void ItemDeletedHandler(TransactionListItemViewModel sender);
		public event ItemDeletedHandler ItemDeleted;

		public ICommand DeleteCommand { get; private set; }

		public void Delete()
		{
			//await _transactionRepository.DeleteEntryAsync(TransactionId);
			//if (this._parent != null)
			//{
			//	_parent.Transactions.Remove(this);
			//	_parent.CurrentRow--;
			//	_parent.TotalRows--;
			//}
			if (ItemDeleted != null)
				ItemDeleted(this);
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
