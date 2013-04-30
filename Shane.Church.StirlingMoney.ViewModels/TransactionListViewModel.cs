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
using System.Collections.ObjectModel;
#if !PERSONAL
using System.Data.Linq;
using Shane.Church.StirlingMoney.Data.v2;
#else
using Shane.Church.StirlingMoney.Data.Sync;
#endif
using System.Collections.Generic;
using System.Windows.Data;

namespace Shane.Church.StirlingMoney.ViewModels
{
	public class TransactionListViewModel : INotifyPropertyChanged
	{
		public TransactionListViewModel()
		{
			TransactionCollection = new ObservableCollection<Group<TransactionListItemViewModel>>();
			_transactions = new CollectionViewSource();
			_transactions.Source = TransactionCollection;
			_transactions.View.Filter = (x) => FilterTransaction(x);
			_transactions.View.CollectionChanged += (sender, args) =>
			{
				NotifyPropertyChanged("NoDataVisibility");
			};
			_transactions.View.CurrentChanged += (sender, args) =>
			{
				NotifyPropertyChanged("NoDataVisibility");
			};
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
					NotifyPropertyChanged("AccountName");
					NotifyPropertyChanged("AvailableBalance");
					NotifyPropertyChanged("PostedBalance");
				}
			}
		}

		public string AccountName
		{
			get 
			{
				try
				{
#if !PERSONAL
					using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
					{
						return (from a in _context.Accounts
								where a.AccountId == AccountId
								select a.AccountName).FirstOrDefault();
					}
#else
					return (from a in ContextInstance.Context.AccountCollection
							where a.AccountId == AccountId
							select a.AccountName).FirstOrDefault();
#endif
				}
				finally
				{
				}
			}
		}

		private ObservableCollection<Group<TransactionListItemViewModel>> _transactionCollection;
		public ObservableCollection<Group<TransactionListItemViewModel>> TransactionCollection
		{
			get { return _transactionCollection; }
			set
			{
				if (_transactionCollection != value)
				{
					_transactionCollection = value;
					if (_transactionCollection != null)
					{
						_transactionCollection.CollectionChanged += delegate
						{
							NotifyPropertyChanged("TransactionCollection");
							NotifyPropertyChanged("Transactions");
							NotifyPropertyChanged("AvailableBalance");
							NotifyPropertyChanged("PostedBalance");
							NotifyPropertyChanged("NoDataVisibility");
							if (_transactions != null)
							{
								_transactions.View.MoveCurrentToFirst();
							}
						};
					}

					NotifyPropertyChanged("TransactionCollection");
					NotifyPropertyChanged("Transactions");
					NotifyPropertyChanged("AvailableBalance");
					NotifyPropertyChanged("PostedBalance");
					NotifyPropertyChanged("NoDataVisibility");
					if (_transactions != null)
					{
						_transactions.Source = TransactionCollection;
						_transactions.View.Filter = (x) => FilterTransaction(x);
						_transactions.View.MoveCurrentToFirst();
						_transactions.View.Refresh();
					}
				}
			}
		}

		private CollectionViewSource _transactions;
		public ICollectionView Transactions
		{
			get { return _transactions.View; }
		}

		private bool FilterTransaction(object item)
		{
			var testItem = item as Group<TransactionListItemViewModel>;
			if (!string.IsNullOrWhiteSpace(SearchText) && testItem != null)
			{
				return !testItem.Items.IsEmpty;
			}
			else
			{
				return true;
			}
		}

		private bool _isSearchVisible;
		public bool IsSearchVisible
		{
			get { return _isSearchVisible; }
			set
			{
				if (_isSearchVisible != value)
				{
					_isSearchVisible = value;
					if (!_isSearchVisible)
						SearchText = "";
					NotifyPropertyChanged("IsSearchVisible");
					NotifyPropertyChanged("SearchBoxVisibility");
					NotifyPropertyChanged("SearchImagePath");
				}
			}
		}

		public Visibility SearchBoxVisibility
		{
			get
			{
				if (IsSearchVisible)
					return Visibility.Visible;
				else
					return Visibility.Collapsed;
			}
		}

		public string SearchImagePath
		{
			get
			{
				Visibility darkBackgroundVisibility = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"];
				if (!IsSearchVisible)
				{
					if (darkBackgroundVisibility == Visibility.Visible)
						return "/Images/Search.light.png";
					else
						return "/Images/Search.dark.png";
				}
				else
				{
					if (darkBackgroundVisibility == Visibility.Visible)
						return "/Images/Search.light.active.png";
					else
						return "/Images/Search.dark.active.png";
				}
			}
		}

		private string _searchText;
		public string SearchText
		{
			get { return _searchText; }
			set
			{
				if (_searchText != value)
				{
					_searchText = value;
					foreach (Group<TransactionListItemViewModel> g in TransactionCollection)
					{
						g.Filter = value;
					}
					if (_transactions != null)
					{
						_transactions.View.Refresh();
					}
					NotifyPropertyChanged("SearchText");
					NotifyPropertyChanged("Transactions");
					NotifyPropertyChanged("NoDataVisibility");
				}
			}
		}

		public double PostedBalance
		{
			get
			{
				try
				{
#if !PERSONAL
					using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
					{
						return (from a in _context.Accounts
								where a.AccountId == AccountId
								select a.PostedBalance).FirstOrDefault();
					}
#else
					return (from a in ContextInstance.Context.AccountCollection
							where a.AccountId == AccountId
							select a.PostedBalance).FirstOrDefault();
#endif
				}
				finally
				{
				}
			}
		}

		public double AvailableBalance
		{
			get
			{
				try
				{
#if !PERSONAL
					using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
					{
						return (from a in _context.Accounts
								where a.AccountId == AccountId
								select a.AccountBalance).FirstOrDefault();
					}
#else
					return (from a in ContextInstance.Context.AccountCollection
							where a.AccountId == AccountId
							select a.AccountBalance).FirstOrDefault();
#endif
				}
				finally
				{
				}
			}
		}

		public Visibility NoDataVisibility
		{
			get
			{
				if (Transactions.IsEmpty)
				{
					return Visibility.Visible;
				}
				else
				{
					return Visibility.Collapsed;
				}
			}
		}

		public Visibility LoadingVisibility
		{
			get
			{
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

		public delegate void LoadDataComplete(object sender, EventArgs e);
		public event LoadDataComplete LoadDataCompleted;

		public void LoadData(Guid accountId)
		{
#if !PERSONAL
			using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
#endif
				this.AccountId = accountId;
#if !PERSONAL
				var transactions = (from t in _context.Transactions
									where t.Account.AccountId == accountId
#else
				var transactions = (from t in ContextInstance.Context.TransactionCollection
									where t._accountId == accountId
#endif
									orderby t.TransactionDate ascending
									select t);
				List<TransactionListItemViewModel> items = new List<TransactionListItemViewModel>();
				foreach (var t in transactions)
				{
					if (t != null)
					{
						var categoryName = "";
						if (t.CategoryId.HasValue)
						{
#if !PERSONAL
							categoryName = (from c in _context.Categories
#else
							categoryName = (from c in ContextInstance.Context.CategoryCollection
#endif
											where c.CategoryId == t.CategoryId.Value
											select c.CategoryName).FirstOrDefault();
							if (categoryName == null)
								categoryName = "";
						}
						TransactionListItemViewModel item = new TransactionListItemViewModel()
						{
							Location = t.Location,
							Amount = t.Amount,
							TransactionDate = t.TransactionDate,
							Posted = t.Posted,
							TransactionId = t.TransactionId,
							Memo = t.Note,
							Category = categoryName,
							CheckNumber = t.CheckNumber
						};
						item.PropertyChanged += new PropertyChangedEventHandler((sender, args) =>
						{
							if (args.PropertyName.ToLower() == "posted")
							{
								NotifyPropertyChanged("PostedBalance");
							}
						});
						items.Add(item);
					}
				}

				var groupings = (from t in items
								 group t by t.TransactionDate.Date into g
								 orderby g.Key descending
								 select new Group<TransactionListItemViewModel>(g.Key.ToLongDateString(), g.OrderByDescending(t => t.TransactionId))).ToList();
				try
				{
					Deployment.Current.Dispatcher.BeginInvoke(() =>
					{
						try
						{
							this.TransactionCollection.Clear();
							foreach (var g in groupings)
							{
								if (!string.IsNullOrEmpty(SearchText))
									g.Filter = SearchText;
								this.TransactionCollection.Add(g);
							}
							this.Transactions.Refresh();
							NotifyPropertyChanged("NoDataVisibility");
							if (LoadDataCompleted != null)
							{
								LoadDataCompleted(this, new EventArgs());
							}
						}
						catch /*(Exception ex) */
						{

						}
					});
				}
				catch /*(Exception ex)*/
				{

				}
#if !PERSONAL
			}
#endif
		}
	}
}
