﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class TransactionListViewModel : ObservableObject, ICommittable, ITombstoneFriendly
	{
		private IRepository<Account, Guid> _accountRepository;
		private IRepository<Transaction, Guid> _transactionRepository;
		private IRepository<Tombstone, string> _tombstoneRepository;
		private INavigationService _navService;
		private DateTimeOffset _refreshTime;

		public TransactionListViewModel(IRepository<Account, Guid> accountRepository, IRepository<Transaction, Guid> transactionRepository, INavigationService navService, IRepository<Tombstone, string> tombstoneRepository)
		{
			if (accountRepository == null) throw new ArgumentNullException("accountRepository");
			_accountRepository = accountRepository;
			if (transactionRepository == null) throw new ArgumentNullException("transactionRepository");
			_transactionRepository = transactionRepository;
			if (navService == null) throw new ArgumentNullException("navService");
			_navService = navService;
			if (tombstoneRepository == null) throw new ArgumentNullException("tombstoneRepository");
			_tombstoneRepository = tombstoneRepository;

			WithdrawCommand = new RelayCommand(Withdraw);
			WriteCheckCommand = new RelayCommand(WriteCheck);
			DepositCommand = new RelayCommand(Deposit);
			TransferCommand = new RelayCommand(Transfer);

			_transactions = new ObservableCollection<TransactionListItemViewModel>();
			_transactions.CollectionChanged += _transactions_CollectionChanged;
		}

		public delegate void BusyChangedHandler(BusyEventArgs args);
		public event BusyChangedHandler BusyChanged;

		void _transactions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			RaisePropertyChanged(() => Transactions);
			RaisePropertyChanged(() => PostedBalance);
			RaisePropertyChanged(() => AvailableBalance);
		}

		private Account _account;
		protected Account Account
		{
			get { return _account; }
			set
			{
				if (Set(() => Account, ref _account, value))
				{
					RaisePropertyChanged(() => AccountId);
					RaisePropertyChanged(() => AccountName);
					RaisePropertyChanged(() => AvailableBalance);
					RaisePropertyChanged(() => PostedBalance);
				}
			}
		}

		public Guid AccountId
		{
			get { return _account.AccountId; }
		}

		public string AccountName
		{
			get { return _account.AccountName; }
		}

		public async Task<Account> GetAccount()
		{
			return await _accountRepository.GetEntryAsync(AccountId);
		}

		private ObservableCollection<TransactionListItemViewModel> _transactions;
		public ObservableCollection<TransactionListItemViewModel> Transactions
		{
			get { return _transactions; }
		}

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
				return _account != null ? _account.PostedBalance : 0;
			}
		}

		public double AvailableBalance
		{
			get
			{
				return _account != null ? _account.AccountBalance : 0;
			}
		}

		public int CurrentRow { get; set; }
		public int TotalRows { get; set; }

		private bool _initialLoadComplete;
		public bool InitialLoadComplete
		{
			get { return _initialLoadComplete; }
			set
			{
				Set(() => InitialLoadComplete, ref _initialLoadComplete, value);
			}
		}

		public async Task LoadNextTransactions(int count = 40)
		{
			if (Account != null && CurrentRow < TotalRows)
			{
				var nextTransactions = Account.GetTransactionKeys().OrderByDescending(it => it.Value.Item1).ThenByDescending(it => it.Value.Item2).Skip(CurrentRow).Take(count).Select(it => it.Key).ToList();
				foreach (var t in nextTransactions)
				{
					var updated = _transactionRepository.GetAllIndexKeys<Tuple<Guid, DateTimeOffset>>("TransactionAccountIdEditDateTime").Where(it => it.Value.Item2 > _refreshTime && it.Value.Item1 == this.AccountId).Select(it => it.Key);
					var updatedList = updated.ToList();
					var listItem = Transactions.Where(it => it.TransactionId == t).FirstOrDefault();
					if (listItem != null)
					{
						await listItem.LoadData(t);
					}
					else
					{
						var item = KernelService.Kernel.Get<TransactionListItemViewModel>(new Ninject.Parameters.ConstructorArgument("parent", this));
						await item.LoadData(t);
						item.PostedChanged += async (s) => await item_PostedChanged(s);
						Transactions.Add(item);
						CurrentRow++;
						TotalRows = this.Account.TransactionCount;
					}
				}
				_refreshTime = DateTimeOffset.UtcNow;
			}
		}

		async Task item_PostedChanged(TransactionListItemViewModel sender)
		{
			var value = _transactionRepository.GetAllIndexKeys<Tuple<DateTimeOffset, bool>>("EditDateTimePosted").Where(it => it.Key == sender.TransactionId).Select(it => it.Value.Item2).FirstOrDefault();
			if (value != sender.Posted)
			{
				var trans = await _transactionRepository.GetEntryAsync(sender.TransactionId);
				trans.Posted = sender.Posted;
				await _transactionRepository.AddOrUpdateEntryAsync(trans);
			}
			RaisePropertyChanged(() => PostedBalance);
			RaisePropertyChanged(() => AvailableBalance);
		}

		void item_ItemDeleted(object sender, EventArgs args)
		{
			var item = sender as TransactionListItemViewModel;
			if (item != null)
			{
				CurrentRow--;
				RaisePropertyChanged(() => PostedBalance);
				RaisePropertyChanged(() => AvailableBalance);
			}
		}

		public async Task LoadData(Guid accountId)
		{
			if (BusyChanged != null)
			{
				BusyChanged(new BusyEventArgs() { AnimationType = 2, IsBusy = true, Message = Shane.Church.StirlingMoney.Strings.Resources.ProgressBarText });
			}

			await TaskEx.Yield();

			this.Account = await _accountRepository.GetEntryAsync(accountId);
			this.CurrentRow = 0;
			this.TotalRows = this.Account != null ? Account.TransactionCount : 0;
			InitialLoadComplete = true;

			await TaskEx.Yield();

			await LoadNextTransactions();

			if (BusyChanged != null)
			{
				BusyChanged(new BusyEventArgs() { IsBusy = false });
			}
		}

		public async Task RefreshData()
		{
			if (BusyChanged != null)
			{
				BusyChanged(new BusyEventArgs() { AnimationType = 2, IsBusy = true, Message = Shane.Church.StirlingMoney.Strings.Resources.ProgressBarText });
			}

			await TaskEx.Yield();

			var updated = _transactionRepository.GetAllIndexKeys<Tuple<Guid, DateTimeOffset>>("TransactionAccountIdEditDateTime").Where(it => it.Value.Item2 > _refreshTime && it.Value.Item1 == this.AccountId).Select(it => it.Key);
			var updatedList = updated.ToList();
			foreach (var t in updatedList)
			{
				var listItem = Transactions.Where(it => it.TransactionId == t).FirstOrDefault();
				if (listItem != null)
				{
					await listItem.LoadData(t);
				}
				else
				{
					var item = KernelService.Kernel.Get<TransactionListItemViewModel>(new Ninject.Parameters.ConstructorArgument("parent", this));
					await item.LoadData(t);
					item.PostedChanged += async (s) => await item_PostedChanged(s);
					Transactions.Add(item);
					CurrentRow++;
					TotalRows = this.Account.TransactionCount;
				}
			}
			_refreshTime = DateTimeOffset.UtcNow;
			RaisePropertyChanged(() => AvailableBalance);
			RaisePropertyChanged(() => PostedBalance);

			await TaskEx.Yield();

			if (BusyChanged != null)
			{
				BusyChanged(new BusyEventArgs() { IsBusy = false });
			}
		}

		public ICommand WithdrawCommand { get; private set; }

		public void Withdraw()
		{
			AddEditTransactionParams param = new AddEditTransactionParams() { Type = Core.Data.TransactionType.Withdrawal, AccountId = AccountId };
			_navService.Navigate<AddEditTransactionViewModel>(param);
		}

		public ICommand WriteCheckCommand { get; private set; }

		public void WriteCheck()
		{
			AddEditTransactionParams param = new AddEditTransactionParams() { Type = Core.Data.TransactionType.Check, AccountId = AccountId };
			_navService.Navigate<AddEditTransactionViewModel>(param);
		}

		public ICommand DepositCommand { get; private set; }

		public void Deposit()
		{
			AddEditTransactionParams param = new AddEditTransactionParams() { Type = Core.Data.TransactionType.Deposit, AccountId = AccountId };
			_navService.Navigate<AddEditTransactionViewModel>(param);
		}

		public ICommand TransferCommand { get; private set; }

		public void Transfer()
		{
			AddEditTransactionParams param = new AddEditTransactionParams() { Type = Core.Data.TransactionType.Transfer, AccountId = AccountId };
			_navService.Navigate<AddEditTransactionViewModel>(param);
		}

		public async Task Commit()
		{
			await _accountRepository.Commit();
		}

		public void Deactivate()
		{
			DeactivateAsync().Wait(2000);
		}

		public void Activate()
		{
			ActivateAsync().Wait(2000);
		}

		public async Task DeactivateAsync()
		{
			try
			{
				Tombstone t = new Tombstone();
				t.Key = typeof(TransactionListViewModel).ToString();
				t.State.Add("Account", this.Account);
				t.State.Add("Transactions", this.Transactions.ToList());
				t.State.Add("CurrentRow", this.CurrentRow);
				t.State.Add("SearchText", this.SearchText);
				t.State.Add("SearchVisible", this.SearchVisible);
				t.State.Add("RefreshTime", this._refreshTime);
				await _tombstoneRepository.AddOrUpdateEntryAsync(t);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			await this.Commit();
		}

		public async Task ActivateAsync()
		{
			var t = await _tombstoneRepository.GetEntryAsync(typeof(TransactionListViewModel).ToString());
			if (t != null)
			{
				this.Account = t.TryGet<Account>("Account", null);
				if (this.Account != null)
				{
					var transactions = t.TryGet<List<TransactionListItemViewModel>>("Transactions", new List<TransactionListItemViewModel>());
					foreach (var tli in transactions)
					{
						if (!Transactions.Where(it => it.TransactionId == tli.TransactionId).Any())
						{
							tli._parent = this;
							tli.PostedChanged += async (s) => await item_PostedChanged(s);
							Transactions.Add(tli);
						}
						else
						{
							var existingItem = Transactions.Where(it => it.TransactionId == tli.TransactionId).First();
							existingItem._parent = this;
							existingItem.PostedChanged += async (s) => await item_PostedChanged(s);
						}
					}
					this.CurrentRow = t.TryGet<int>("CurrentRow", 0);
					this.SearchText = t.TryGet<string>("SearchText", "");
					this.SearchVisible = t.TryGet<bool>("SearchVisible", false);
					this._refreshTime = t.TryGet<DateTimeOffset>("RefreshTime", DateTimeOffset.MinValue);
					await RefreshData();
				}
				await _tombstoneRepository.DeleteEntryAsync(typeof(TransactionListViewModel).ToString());
			}
		}
	}
}
