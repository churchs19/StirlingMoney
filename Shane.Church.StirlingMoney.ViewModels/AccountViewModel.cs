using System;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Shane.Church.StirlingMoney.Tiles;
#if !PERSONAL
using System.Data.Linq;
using Shane.Church.StirlingMoney.Data.v2;
#else
using Shane.Church.StirlingMoney.Data.Sync;
#endif


namespace Shane.Church.StirlingMoney
{
	public class AccountViewModel : INotifyPropertyChanged
	{
		public AccountViewModel()
		{

		}

		public AccountViewModel(Account a)
		{
			AccountId = a.AccountId;
			AccountName = a.AccountName;
			AccountBalance = a.AccountBalance;
		}

		private string _accountName;
		public string AccountName
		{
			get
			{
				return _accountName;
			}
			set
			{
				if (value != _accountName)
				{
					_accountName = value;
					NotifyPropertyChanged("AccountName");
				}
			}
		}

		private double _accountBalance;
		public double AccountBalance
		{
			get
			{
				return _accountBalance;
			}
			set
			{
				if (value != _accountBalance)
				{
					_accountBalance = value;
					NotifyPropertyChanged("AccountBalance");
				}
			}
		}

		private Guid _accountId;
		public Guid AccountId
		{
			get
			{
				return _accountId;
			}
			set
			{
				if (value != _accountId)
				{
					_accountId = value;
					NotifyPropertyChanged("AccountId");
				}
			}
		}

		public string PinMenuText
		{
			get
			{
				if (!TileUtility.TileExists(AccountId))
				{
					return Shane.Church.StirlingMoney.ViewModels.Resources.ViewModelResources.PinToStart;
				}
				else
				{
					return Shane.Church.StirlingMoney.ViewModels.Resources.ViewModelResources.UnpinFromStart;
				}
			}
			set
			{
				NotifyPropertyChanged("PinMenuText");
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

		public void LoadData(Guid AccountId)
		{
#if !PERSONAL
			using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
				var account = (from a in _context.Accounts
							   where a.AccountId == AccountId
							   select a).FirstOrDefault();
#else
			var account = (from a in ContextInstance.Context.AccountCollection
						   where a.AccountId == AccountId
						   select a).FirstOrDefault();
#endif
				if (account != null)
				{
					this.AccountId = account.AccountId;
					this.AccountName = account.AccountName;
					this.AccountBalance = account.AccountBalance;
				}
#if !PERSONAL
			}
#endif
		}
	}
}