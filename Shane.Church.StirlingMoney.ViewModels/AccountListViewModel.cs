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
using System.Collections.ObjectModel;
using System.ComponentModel;
#if !PERSONAL
using System.Data.Linq;
using Shane.Church.StirlingMoney.Data.v2;
#else
using Shane.Church.StirlingMoney.Data.Sync;
#endif
using Shane.Church.Utility;

namespace Shane.Church.StirlingMoney.ViewModels
{
	public class AccountListViewModel : INotifyPropertyChanged
	{
		public AccountListViewModel()
		{
			Accounts = new ObservableCollection<AccountViewModel>();
		}

		private ObservableCollection<AccountViewModel> _accounts;
		public ObservableCollection<AccountViewModel> Accounts
		{
			get { return _accounts; }
			set
			{
				if (_accounts != value)
				{
					_accounts = value;
					if (_accounts != null)
					{
						_accounts.CollectionChanged += delegate
						{
							NotifyPropertyChanged("Accounts");
							NotifyPropertyChanged("TotalBalance");
							NotifyPropertyChanged("NoDataVisibility");
						};
					}
					NotifyPropertyChanged("Accounts");
					NotifyPropertyChanged("TotalBalance");
					NotifyPropertyChanged("NoDataVisibility");
				}
			}
		}

		public double TotalBalance
		{
			get
			{
				try
				{
					if (Accounts != null)
					{
						var total = Accounts.Sum(m => m.AccountBalance);
						return total;
					}
					else
					{
						return 0;
					}
				}
				catch
				{
					return 0;
				}
			}
		}

		public Visibility NoDataVisibility
		{
			get
			{
				if (Accounts.Count == 0)
				{
					return Visibility.Visible;
				}
				else
				{
					return Visibility.Collapsed;
				}
			}
		}

		public void LoadData()
		{
			this.Accounts.Clear();

#if !PERSONAL
			using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
#endif

			AppSettings settings = new AppSettings();

			if (settings.AccountSort == 1)
			{
#if !PERSONAL
					var accounts = (from a in _context.Accounts
									let count = a.Transactions.Count()
									orderby count descending, a.AccountName ascending
									select a);
#else
				var accountSort = (from a in ContextInstance.Context.AccountCollection
								   join tx in ContextInstance.Context.TransactionCollection on a.AccountId equals tx._accountId into j1
								   from j2 in j1.DefaultIfEmpty()
								   group j2 by a.AccountId into grouped
								   select new { AccountId = grouped.Key, Count = grouped.Count(t => t != null && t.TransactionId != null) });
				var accounts = (from a in ContextInstance.Context.AccountCollection
								join a2 in accountSort on a.AccountId equals a2.AccountId
								orderby a2.Count descending, a.AccountName
								select a);
#endif
				foreach (Account a in accounts)
				{
					this.Accounts.Add(new AccountViewModel(a));
				}
			}
			else
			{
#if !PERSONAL
					foreach (Account a in _context.Accounts.OrderBy(m => m.AccountName))
#else
				foreach (Account a in ContextInstance.Context.AccountCollection.OrderBy(m => m.AccountName))
#endif
				{
					this.Accounts.Add(new AccountViewModel() { AccountId = a.AccountId, AccountName = a.AccountName, AccountBalance = a.AccountBalance });
				}
			}
#if !PERSONAL
			}
#endif
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
