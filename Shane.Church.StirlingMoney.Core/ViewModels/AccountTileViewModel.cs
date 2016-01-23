using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Strings;
using Shane.Church.Utility.Core.Command;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class AccountTileViewModel : ObservableObject
	{
		private ITileService<Account, Guid> _tileService;
		private IDataRepository<Account, Guid> _accountRepository;
		private INavigationService _navService;
		protected string _imageUri;

		public AccountTileViewModel(ITileService<Account, Guid> tileService, IDataRepository<Account, Guid> accountRepository, INavigationService navService)
		{
			if (tileService == null) throw new ArgumentNullException("tileService");
			_tileService = tileService;
			if (accountRepository == null) throw new ArgumentNullException("accountRepository");
			_accountRepository = accountRepository;
			if (navService == null) throw new ArgumentNullException("navService");
			_navService = navService;

			EditCommand = new RelayCommand(Edit);
			DeleteCommand = new AsyncRelayCommand(o => Delete());
			PinCommand = new RelayCommand(Pin);
			TransactionsCommand = new RelayCommand(Transactions);

			var rnd = new Random();
			UpdateInterval = TimeSpan.FromSeconds(Convert.ToDouble(rnd.Next(3, 6)) + rnd.NextDouble());
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

		private byte[] _image;
		public byte[] Image
		{
			get { return _image; }
			set
			{
				Set(() => Image, ref _image, value);
			}
		}

		private TimeSpan _updateInterval;
		public TimeSpan UpdateInterval
		{
			get { return _updateInterval; }
			private set
			{
				Set(() => UpdateInterval, ref _updateInterval, value);
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
			set
			{
				RaisePropertyChanged(() => PinMenuText);
			}
		}

		public ICommand EditCommand { get; protected set; }

		public void Edit()
		{
			_navService.Navigate<AddEditAccountViewModel>(this.AccountId);
		}

		public ICommand DeleteCommand { get; protected set; }

		public delegate void AccountDeletedHandler(object sender);
		public event AccountDeletedHandler AccountDeleted;

		public async Task Delete()
		{
			await _accountRepository.DeleteEntryAsync(this.AccountId);
			if (AccountDeleted != null)
				AccountDeleted(this);
		}

		public ICommand PinCommand { get; protected set; }

		public void Pin()
		{
			if (_tileService.TileExists(AccountId))
			{
				_tileService.DeleteTile(AccountId);
			}
			else
			{
				_tileService.AddTile(AccountId);
			}
		}

		public ICommand TransactionsCommand { get; protected set; }

		public void Transactions()
		{
			_navService.Navigate<TransactionListViewModel>(new TransactionListParams() { Id = AccountId, PinnedTile = false });
		}

		public delegate void AccountUpdatedHandler(object sender);
		public event AccountUpdatedHandler AccountUpdated;

		public virtual async Task LoadData(Guid accountId, bool updateTile = false)
		{
			Account a = await _accountRepository.GetEntryAsync(accountId);
			if (a != null)
			{
				LoadData(a, updateTile);
			}
		}

        public virtual void LoadDataSync(Guid accountId, bool updateTile = false)
        {
            Account a = _accountRepository.GetEntry(accountId);
            if(a!= null)
            {
                LoadData(a, updateTile);
            }
        }

		public virtual void LoadData(Account a, bool updateTile = false)
		{
			AccountId = a.AccountId;
			AccountName = a.AccountName;
			AccountBalance = a.AccountBalance;
			PostedBalance = a.PostedBalance;
			_imageUri = a.ImageUri;
			if (updateTile)
				_tileService.UpdateTile(AccountId);
			if (AccountUpdated != null)
				AccountUpdated(this);
		}
	}
}
