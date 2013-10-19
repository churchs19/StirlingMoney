using GalaSoft.MvvmLight;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class AccountListViewModel : ObservableObject
	{
		private IRepository<Account> _accountRepository;

		public AccountListViewModel(IRepository<Account> accountRepository)
		{
			if (accountRepository == null) throw new ArgumentNullException("accountRepository");
			_accountRepository = accountRepository;
			_accounts = new ObservableCollection<AccountTileViewModel>();
			_accounts.CollectionChanged += (s, e) =>
			{
				RaisePropertyChanged(() => Accounts);
				RaisePropertyChanged(() => TotalBalance);
				RaisePropertyChanged(() => HasData);
			};
		}

		private bool _accountsLoaded = false;
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

		public async Task LoadData(bool forceUpdate = false)
		{
			if (!_accountsLoaded || forceUpdate)
			{
				var entries = await _accountRepository.GetAllEntriesAsync();
				foreach (var a in entries)
				{
					var tile = KernelService.Kernel.Get<AccountTileViewModel>();
					tile.LoadData(a);
					tile.AccountDeleted += tile_AccountDeleted;
					Accounts.Add(tile);
				}
				_accountsLoaded = true;
			}
		}

		void tile_AccountDeleted(object sender)
		{
			var item = sender as AccountTileViewModel;
			if (item != null)
			{
				item.AccountDeleted -= tile_AccountDeleted;
				Accounts.Remove(item);
			}
		}
	}
}
