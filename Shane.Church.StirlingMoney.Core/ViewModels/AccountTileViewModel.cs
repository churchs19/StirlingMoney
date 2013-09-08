using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Properties;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Linq;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class AccountTileViewModel : ObservableObject
	{
		private ITileService<Account> _tileService;
		private IRepository<Account> _accountRepository;
		private INavigationService _navService;

		public AccountTileViewModel(ITileService<Account> tileService, IRepository<Account> accountRepository, INavigationService navService)
		{
			if (tileService == null) throw new ArgumentNullException("tileService");
			_tileService = tileService;
			if (accountRepository == null) throw new ArgumentNullException("accountRepository");
			_accountRepository = accountRepository;
			if (navService == null) throw new ArgumentNullException("navService");
			_navService = navService;

			EditCommand = new RelayCommand(Edit);
			DeleteCommand = new RelayCommand(Delete);
			PinCommand = new RelayCommand(Pin);
			TransactionsCommand = new RelayCommand(Transactions);
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
				Set(() => AccountName, ref _accountName, value);
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
				Set(() => AccountBalance, ref _accountBalance, value);
			}
		}

		private double _postedBalance;
		public double PostedBalance
		{
			get { return _postedBalance; }
			set
			{
				Set(() => PostedBalance, ref _postedBalance, value);
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
				Set(() => AccountId, ref _accountId, value);
			}
		}

		public string PinMenuText
		{
			get
			{
				if (!_tileService.TileExists(AccountId))
				{
					return Resources.PinToStart;
				}
				else
				{
					return Resources.UnpinFromStart;
				}
			}
		}

		public ICommand EditCommand { get; protected set; }

		public void Edit()
		{
			_navService.Navigate<AddEditAccountViewModel>(this.AccountId);
		}

		public ICommand DeleteCommand { get; protected set; }

		public void Delete()
		{

		}

		public ICommand PinCommand { get; protected set; }

		public void Pin()
		{

		}

		public ICommand TransactionsCommand { get; protected set; }

		public void Transactions()
		{
			_navService.Navigate<TransactionListViewModel>(this.AccountId);
		}

		public void LoadData(Guid accountId)
		{
			Account a = _accountRepository.GetFilteredEntries(it => it.AccountId == accountId).FirstOrDefault();
			if (a != null)
			{
				AccountId = accountId;
				AccountName = a.AccountName;
				AccountBalance = a.AccountBalance;
				PostedBalance = a.PostedBalance;
			}
		}

		public void LoadData(Account a)
		{
			AccountId = a.AccountId;
			AccountName = a.AccountName;
			AccountBalance = a.AccountBalance;
			PostedBalance = a.PostedBalance;
		}
	}
}
