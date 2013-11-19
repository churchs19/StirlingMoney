using GalaSoft.MvvmLight;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class AccountListViewModel : ObservableObject
	{
		private IRepository<Account, Guid> _accountRepository;
		private ISettingsService _settings;

		public AccountListViewModel()
			: this(KernelService.Kernel.Get<IRepository<Account, Guid>>(), KernelService.Kernel.Get<ISettingsService>())
		{

		}

		public AccountListViewModel(IRepository<Account, Guid> accountRepository, ISettingsService settings)
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
				var entriesList = entries.ToList();
				List<Account> sortedEntries;
				if (sort == 0)
				{
					sortedEntries = entriesList.OrderBy(it => it.AccountName).ToList();
				}
				else
				{
					sortedEntries = entriesList.OrderByDescending(it => it.TransactionCount).ThenBy(it => it.AccountName).ToList();
				}
				foreach (var a in Accounts.Where(it => !sortedEntries.Select(e => e.AccountId).Contains(it.AccountId)))
					Accounts.Remove(a);
				foreach (var a in sortedEntries)
				{
					var acctTileQuery = Accounts.Where(it => it.AccountId == a.AccountId);
					AccountTileViewModel tile;
					if (acctTileQuery.Any())
					{
						tile = acctTileQuery.FirstOrDefault();
					}
					else
					{
						tile = KernelService.Kernel.Get<AccountTileViewModel>();
						tile.AccountDeleted += tile_AccountDeleted;
						Accounts.Add(tile);
					}
					tile.LoadData(a, true);
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
