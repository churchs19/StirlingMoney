using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class TransactionListViewModel : ObservableObject, ICommittable
	{
		private IRepository<Account, Guid> _accountRepository;
		private IRepository<Transaction, Guid> _transactionRepository;
		private INavigationService _navService;
		private DateTimeOffset _refreshTime;

		public TransactionListViewModel(IRepository<Account, Guid> accountRepository, IRepository<Transaction, Guid> transactionRepository, INavigationService navService)
		{
			if (accountRepository == null) throw new ArgumentNullException("accountRepository");
			_accountRepository = accountRepository;
			if (transactionRepository == null) throw new ArgumentNullException("transactionRepository");
			_transactionRepository = transactionRepository;
			if (navService == null) throw new ArgumentNullException("navService");
			_navService = navService;

			WithdrawCommand = new RelayCommand(Withdraw);
			WriteCheckCommand = new RelayCommand(WriteCheck);
			DepositCommand = new RelayCommand(Deposit);
			TransferCommand = new RelayCommand(Transfer);

			_transactions = new ObservableCollection<TransactionListItemViewModel>();
			_transactions.CollectionChanged += _transactions_CollectionChanged;
		}

		public delegate void BusyChangedHandler(BusyEventArgs args);
		public event BusyChangedHandler BusyChanged;

		void _transactions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			RaisePropertyChanged(() => Transactions);
			RaisePropertyChanged(() => PostedBalance);
			RaisePropertyChanged(() => AvailableBalance);
		}

		private Account _account;
		protected Account Account
		{
			get { return _account; }
			set
			{
				if (Set(() => Account, ref _account, value))
				{
					RaisePropertyChanged(() => AccountId);
					RaisePropertyChanged(() => AccountName);
					RaisePropertyChanged(() => AvailableBalance);
					RaisePropertyChanged(() => PostedBalance);
				}
			}
		}

		public Guid AccountId
		{
			get { return _account.AccountId; }
		}

		public string AccountName
		{
			get { return _account.AccountName; }
		}

		public async Task<Account> GetAccount()
		{
			return await _accountRepository.GetEntryAsync(AccountId);
		}

		private ObservableCollection<TransactionListItemViewModel> _transactions;
		public ObservableCollection<TransactionListItemViewModel> Transactions
		{
			get { return _transactions; }
		}

		private bool _searchVisible;
		public bool SearchVisible
		{
			get { return _searchVisible; }
			set
			{
				if (Set(() => SearchVisible, ref _searchVisible, value))
				{
					if (!SearchVisible) SearchText = "";
				}
			}
		}

		private string _searchText;
		public string SearchText
		{
			get { return _searchText; }
			set
			{
				if (Set(() => SearchText, ref _searchText, value))
				{
					//if (_searchText != value)
					//{
					//	_searchText = value;
					//	foreach (Group<TransactionListItemViewModel> g in TransactionCollection)
					//	{
					//		g.Filter = value;
					//	}
					//	if (_transactions != null)
					//	{
					//		_transactions.View.Refresh();
					//	}
					//	NotifyPropertyChanged("SearchText");
					//	NotifyPropertyChanged("Transactions");
					//	NotifyPropertyChanged("NoDataVisibility");
					//}
				}
			}
		}

		public double PostedBalance
		{
			get
			{
				return _account != null ? _account.PostedBalance : 0;
			}
		}

		public double AvailableBalance
		{
			get
			{
				return _account != null ? _account.AccountBalance : 0;
			}
		}

		public int CurrentRow { get; set; }
		public int TotalRows { get; set; }

		private bool _initialLoadComplete;
		public bool InitialLoadComplete
		{
			get { return _initialLoadComplete; }
			set
			{
				Set(() => InitialLoadComplete, ref _initialLoadComplete, value);
			}
		}

		public async Task LoadNextTransactions(int count = 40)
		{
			if (Account != null && CurrentRow < TotalRows)
			{
				var nextTransactions = Account.GetTransactionKeys().OrderByDescending(it => it.Value.Item1).ThenByDescending(it => it.Value.Item2).Skip(CurrentRow).Take(count).Select(it => it.Key).ToList();
				foreach (var t in nextTransactions)
				{
					var item = KernelService.Kernel.Get<TransactionListItemViewModel>(new Ninject.Parameters.ConstructorArgument("parent", this));
					await item.LoadData(t);
					item.PostedChanged += item_PostedChanged;
					Transactions.Add(item);
					CurrentRow++;
				}
				_refreshTime = DateTimeOffset.UtcNow;
			}
		}

		void item_PostedChanged()
		{
			RaisePropertyChanged(() => PostedBalance);
			RaisePropertyChanged(() => AvailableBalance);
		}

		void item_ItemDeleted(object sender, EventArgs args)
		{
			var item = sender as TransactionListItemViewModel;
			if (item != null)
			{
				CurrentRow--;
				RaisePropertyChanged(() => PostedBalance);
				RaisePropertyChanged(() => AvailableBalance);
			}
		}

		public async Task LoadData(Guid accountId)
		{
			if (BusyChanged != null)
			{
				BusyChanged(new BusyEventArgs() { AnimationType = 2, IsBusy = true, Message = Shane.Church.StirlingMoney.Strings.Resources.ProgressBarText });
			}

			await TaskEx.Yield();

			this.Account = await _accountRepository.GetEntryAsync(accountId);
			this.CurrentRow = 0;
			if (this.Account != null)
			{
				this.TotalRows = Account.TransactionCount;
			}
			else
			{
				this.TotalRows = 0;
			}
			InitialLoadComplete = true;

			await TaskEx.Yield();

			await LoadNextTransactions();

			if (BusyChanged != null)
			{
				BusyChanged(new BusyEventArgs() { IsBusy = false });
			}
		}

		public async Task RefreshData()
		{
			if (BusyChanged != null)
			{
				BusyChanged(new BusyEventArgs() { AnimationType = 2, IsBusy = true, Message = Shane.Church.StirlingMoney.Strings.Resources.ProgressBarText });
			}

			await TaskEx.Yield();

			var updated = _transactionRepository.GetAllIndexKeys<DateTimeOffset>("EditDateTime").Where(it => it.Value > _refreshTime).Select(it => it.Key);
			var updatedList = updated.ToList();
			foreach (var t in updatedList)
			{
				var listItem = Transactions.Where(it => it.TransactionId == t).FirstOrDefault();
				if (listItem != null)
				{
					await listItem.LoadData(t);
				}
				else
				{
					var item = KernelService.Kernel.Get<TransactionListItemViewModel>(new Ninject.Parameters.ConstructorArgument("parent", this));
					await item.LoadData(t);
					item.PostedChanged += item_PostedChanged;
					Transactions.Add(item);
					CurrentRow++;
					TotalRows = this.Account.TransactionCount;
				}
			}
			_refreshTime = DateTimeOffset.UtcNow;
			RaisePropertyChanged(() => AvailableBalance);
			RaisePropertyChanged(() => PostedBalance);

			if (BusyChanged != null)
			{
				BusyChanged(new BusyEventArgs() { IsBusy = false });
			}
		}

		public ICommand WithdrawCommand { get; private set; }

		public void Withdraw()
		{
			AddEditTransactionParams param = new AddEditTransactionParams() { Type = Core.Data.TransactionType.Withdrawal, AccountId = AccountId };
			_navService.Navigate<AddEditTransactionViewModel>(param);
		}

		public ICommand WriteCheckCommand { get; private set; }

		public void WriteCheck()
		{
			AddEditTransactionParams param = new AddEditTransactionParams() { Type = Core.Data.TransactionType.Check, AccountId = AccountId };
			_navService.Navigate<AddEditTransactionViewModel>(param);
		}

		public ICommand DepositCommand { get; private set; }

		public void Deposit()
		{
			AddEditTransactionParams param = new AddEditTransactionParams() { Type = Core.Data.TransactionType.Deposit, AccountId = AccountId };
			_navService.Navigate<AddEditTransactionViewModel>(param);
		}

		public ICommand TransferCommand { get; private set; }

		public void Transfer()
		{
			AddEditTransactionParams param = new AddEditTransactionParams() { Type = Core.Data.TransactionType.Transfer, AccountId = AccountId };
			_navService.Navigate<AddEditTransactionViewModel>(param);
		}

		public async Task Commit()
		{
			await _accountRepository.Commit();
		}
	}
}
