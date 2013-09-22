using GalaSoft.MvvmLight;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class TransactionListViewModel : ObservableObject
	{
		private IRepository<Account> _accountRepository;
		private IRepository<Transaction> _transactionRepository;

		public TransactionListViewModel(IRepository<Account> accountRepository, IRepository<Transaction> transactionRepository)
		{
			if (accountRepository == null) throw new ArgumentNullException("accountRepository");
			_accountRepository = accountRepository;
			if (transactionRepository == null) throw new ArgumentNullException("transactionRepository");
			_transactionRepository = transactionRepository;

			_transactions = new ObservableCollection<TransactionListItemViewModel>();
			_transactions.CollectionChanged += (s, e) =>
				{
					RaisePropertyChanged(() => Transactions);
					RaisePropertyChanged(() => PostedBalance);
					RaisePropertyChanged(() => AvailableBalance);
				};
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

		public int CurrentRow { get; protected set; }
		public int TotalRows { get; protected set; }

		private bool _isLoading;
		public bool IsLoading
		{
			get { return _isLoading; }
			set
			{
				Set(() => IsLoading, ref _isLoading, value);
			}
		}

		public void LoadNextTransactions(int count = 20)
		{
			if (Account != null && CurrentRow < TotalRows)
			{
				var nextTransactions = Account.Transactions.OrderByDescending(it => it.TransactionDate).ThenByDescending(it => it.EditDateTime).Skip(CurrentRow).Take(count).ToList();
				foreach (var t in nextTransactions)
				{
					var item = KernelService.Kernel.Get<TransactionListItemViewModel>();
					item.LoadData(t);
					Transactions.Add(item);
					CurrentRow++;
				}
			}
		}

		public void LoadData(Guid accountId)
		{
			IsLoading = true;
			this.Account = _accountRepository.GetFilteredEntries(it => it.AccountId == accountId).FirstOrDefault();
			this.CurrentRow = 0;
			this.TotalRows = this.Account != null ? this.Account.Transactions.Count() : 0;

			IsLoading = false;
		}
	}
}
