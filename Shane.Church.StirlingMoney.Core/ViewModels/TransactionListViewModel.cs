using GalaSoft.MvvmLight;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class TransactionListViewModel : ObservableObject
	{
		private IRepository<Account> _accountRepository;
		private IRepository<Transaction> _transactionRepository;
		private IRepository<Category> _categoryRepository;

		public TransactionListViewModel(IRepository<Account> accountRepository, IRepository<Transaction> transactionRepository, IRepository<Category> categoryRepository)
		{
			if (accountRepository == null) throw new ArgumentNullException("accountRepository");
			_accountRepository = accountRepository;
			if (transactionRepository == null) throw new ArgumentNullException("transactionRepository");
			_transactionRepository = transactionRepository;
			if (categoryRepository == null) throw new ArgumentNullException("categoryRepository");
			_categoryRepository = categoryRepository;
		}

		private Guid _accountId;
		public Guid AccountId
		{
			get { return _accountId; }
			set
			{
				if (Set(() => AccountId, ref _accountId, value))
				{
					RaisePropertyChanged(() => AccountName);
					RaisePropertyChanged(() => AvailableBalance);
					RaisePropertyChanged(() => PostedBalance);
				}
			}
		}

		public string AccountName
		{
			get
			{
				return _accountRepository.GetFilteredEntries(it => it.AccountId == AccountId).Select(it => it.AccountName).FirstOrDefault();
			}
		}

		//private ObservableCollection<Group<TransactionListItemViewModel>> _transactionCollection;
		//public ObservableCollection<Group<TransactionListItemViewModel>> TransactionCollection
		//{
		//	get { return _transactionCollection; }
		//	set
		//	{
		//		if (_transactionCollection != value)
		//		{
		//			_transactionCollection = value;
		//			if (_transactionCollection != null)
		//			{
		//				_transactionCollection.CollectionChanged += delegate
		//				{
		//					NotifyPropertyChanged("TransactionCollection");
		//					NotifyPropertyChanged("Transactions");
		//					NotifyPropertyChanged("AvailableBalance");
		//					NotifyPropertyChanged("PostedBalance");
		//					NotifyPropertyChanged("NoDataVisibility");
		//					if (_transactions != null)
		//					{
		//						_transactions.View.MoveCurrentToFirst();
		//					}
		//				};
		//			}

		//			NotifyPropertyChanged("TransactionCollection");
		//			NotifyPropertyChanged("Transactions");
		//			NotifyPropertyChanged("AvailableBalance");
		//			NotifyPropertyChanged("PostedBalance");
		//			NotifyPropertyChanged("NoDataVisibility");
		//			if (_transactions != null)
		//			{
		//				_transactions.Source = TransactionCollection;
		//				_transactions.View.Filter = (x) => FilterTransaction(x);
		//				_transactions.View.MoveCurrentToFirst();
		//				_transactions.View.Refresh();
		//			}
		//		}
		//	}
		//}

		//private CollectionViewSource _transactions;
		//public ICollectionView Transactions
		//{
		//	get { return _transactions.View; }
		//}

		//private bool FilterTransaction(object item)
		//{
		//	var testItem = item as Group<TransactionListItemViewModel>;
		//	if (!string.IsNullOrWhiteSpace(SearchText) && testItem != null)
		//	{
		//		return !testItem.Items.IsEmpty;
		//	}
		//	else
		//	{
		//		return true;
		//	}
		//}

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
				var account = _accountRepository.GetFilteredEntries(it => it.AccountId == AccountId).FirstOrDefault();
				return account != null ? account.PostedBalance : 0;
			}
		}

		public double AvailableBalance
		{
			get
			{
				var account = _accountRepository.GetFilteredEntries(it => it.AccountId == AccountId).FirstOrDefault();
				return account != null ? account.AccountBalance : 0;
			}
		}

		public bool HasData
		{
			get
			{
				return false;
			}
		}

		private bool _isLoading;
		public bool IsLoading
		{
			get { return _isLoading; }
			set
			{
				Set(() => IsLoading, ref _isLoading, value);
			}
		}

		public void LoadData(Guid accountId)
		{
			this.AccountId = accountId;
			//var transactions = _transactionRepository.GetFilteredEntries(it => it.AccountId == AccountId);
			//List<TransactionListItemViewModel> items = new List<TransactionListItemViewModel>();
			//foreach (var t in transactions)
			//{
			//	var categoryName = "";
			//	if (t.CategoryId.HasValue)
			//	{
			//		categoryName = _categoryRepository.GetFilteredEntries(it => it.CategoryId == t.CategoryId.Value).Select(it => it.CategoryName).FirstOrDefault();
			//		if (categoryName == null)
			//			categoryName = "";
			//	}
			//	TransactionListItemViewModel item = KernelService.Kernel.Get<TransactionListItemViewModel>();
			//	item.Location = t.Location;
			//	item.Amount = t.Amount;
			//	item.TransactionDate = t.TransactionDate;
			//	item.Posted = t.Posted;
			//	item.TransactionId = t.TransactionId;
			//	item.Memo = t.Note;
			//	item.Category = categoryName;
			//	item.CheckNumber = t.CheckNumber;
			//	item.PropertyChanged += (sender, args) =>
			//	{
			//		if (args.PropertyName.ToLower() == "posted")
			//		{
			//			RaisePropertyChanged(() => PostedBalance);
			//		}
			//	};
			//	items.Add(item);
			//}

			//var groupings = (from t in items
			//				 group t by t.TransactionDate.Date into g
			//				 orderby g.Key descending
			//				 select new Group<TransactionListItemViewModel>(g.Key.ToLongDateString(), g.OrderByDescending(t => t.TransactionId))).ToList();
			//try
			//{
			//	Deployment.Current.Dispatcher.BeginInvoke(() =>
			//	{
			//		try
			//		{
			//			this.TransactionCollection.Clear();
			//			foreach (var g in groupings)
			//			{
			//				if (!string.IsNullOrEmpty(SearchText))
			//					g.Filter = SearchText;
			//				this.TransactionCollection.Add(g);
			//			}
			//			this.Transactions.Refresh();
			//			NotifyPropertyChanged("NoDataVisibility");
			//			if (LoadDataCompleted != null)
			//			{
			//				LoadDataCompleted(this, new EventArgs());
			//			}
			//		}
			//		catch /*(Exception ex) */
			//		{

			//		}
			//	});
			//}
			//catch /*(Exception ex)*/
			//{

			//}
		}
	}
}
