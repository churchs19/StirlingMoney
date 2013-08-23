using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class AccountListViewModel : ObservableObject
	{
		public AccountListViewModel()
		{
			_accounts = new ObservableCollection<AccountTileViewModel>();
			_accounts.CollectionChanged += (s, e) =>
			{
				RaisePropertyChanged(() => Accounts);
				RaisePropertyChanged(() => TotalBalance);
				RaisePropertyChanged(() => HasData);
			};
		}

		private ObservableCollection<AccountTileViewModel> _accounts;
		public ObservableCollection<AccountTileViewModel> Accounts
		{
			get { return _accounts; }
		}

		public double TotalBalance
		{
			get
			{
				try
				{
					var total = Accounts.Sum(m => m.AccountBalance);
					return total;
				}
				catch
				{
					return 0;
				}
			}
		}

		public bool HasData
		{
			get
			{
				return Accounts.Count > 0;
			}
		}

		public void LoadData()
		{
		}
	}
}
