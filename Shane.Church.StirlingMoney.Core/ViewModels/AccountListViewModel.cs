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
		private ISettingsService _settings;

		public AccountListViewModel(IRepository<Account> accountRepository, ISettingsService settings)
		{
			if (accountRepository == null) throw new ArgumentNullException("accountRepository");
			_accountRepository = accountRepository;
			if (settings == null) throw new ArgumentNullException("settings");
			_settings = settings;
			_accounts = new ObservableCollection<AccountTileViewModel>();
			_accounts.CollectionChanged += (s, e) =>
			{
				RaisePropertyChanged(() => Accounts);
				RaisePropertyChanged(() => TotalBalance);
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

		public async Task LoadData(bool forceUpdate = false)
		{
			if (!_accountsLoaded || forceUpdate)
			{
				var sort = _settings.LoadSetting<int>("AccountSort");
				var entries = await _accountRepository.GetAllEntriesAsync();
				if (sort == 0)
				{
					entries = entries.OrderBy(it => it.AccountName);
				}
				else
				{
					entries = entries.OrderByDescending(it => it.Transactions.Count()).ThenBy(it => it.AccountName);
				}
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
