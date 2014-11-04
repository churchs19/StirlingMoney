using Newtonsoft.Json;
using Grace;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Wintellect.Sterling.Core.Serialization;

namespace Shane.Church.StirlingMoney.Core.Data
{
	public class Account
	{
		private IRepository<Account, Guid> _accountRepository;
		private IRepository<Transaction, Guid> _transactionRepository;
		private ITransactionSum _transactionSum;

		public Account(IRepository<Account, Guid> accountRepo, IRepository<Transaction, Guid> transactionRepo, ITransactionSum transSum)
		{
			if (accountRepo == null) throw new ArgumentNullException("accountRepo");
			_accountRepository = accountRepo;
			if (transactionRepo == null) throw new ArgumentNullException("transactionRepo");
			_transactionRepository = transactionRepo;
			if (transSum == null) throw new ArgumentNullException("transSum");
			_transactionSum = transSum;

			AccountName = "";
		}

		public Account()
			: this(ContainerService.Container.Locate<IRepository<Account, Guid>>(),
					ContainerService.Container.Locate<IRepository<Transaction, Guid>>(),
					ContainerService.Container.Locate<ITransactionSum>())
		{

		}

		public Guid AccountId { get; set; }
		public string AccountName { get; set; }
		public double InitialBalance { get; set; }
		public DateTimeOffset EditDateTime { get; set; }
		public bool IsDeleted { get; set; }
		public string ImageUri { get; set; }

		[JsonIgnore]
		[SterlingIgnore]
		public int TransactionCount
		{
			get
			{
				return _transactionRepository.GetIndexFilteredEntriesCount<Guid>("TransactionAccountId", AccountId);
			}
		}

		[JsonIgnore]
		[SterlingIgnore]
		public double AccountBalance
		{
			get
			{
				try
				{
					return InitialBalance + _transactionSum.GetSumByAccount(AccountId);
				}
				catch { return 0; }
			}
		}

		[JsonIgnore]
		[SterlingIgnore]
		public double PostedBalance
		{
			get
			{
				try
				{
					return InitialBalance + _transactionSum.GetPostedSumByAccount(AccountId);
				}
				catch { return 0; }
			}
		}

		public Dictionary<Guid, Tuple<DateTimeOffset, DateTimeOffset>> GetTransactionKeys()
		{
			var transactionIds = _transactionRepository.GetAllIndexKeys<Tuple<Guid, double>>("TransactionAccountIdAmount").Where(it => it.Value.Item1 == this.AccountId).Select(it => it.Key);
			return _transactionRepository.GetAllIndexKeys<Tuple<DateTimeOffset, DateTimeOffset>>("TransactionDateEditDateTime").Where(it => transactionIds.Contains(it.Key)).ToDictionary(key => key.Key, val => val.Value);
		}

		public static double GetAccountBalance(Guid AccountId)
		{
			var accountRepo = ContainerService.Container.Locate<IRepository<Account, Guid>>();
			var transSum = ContainerService.Container.Locate<ITransactionSum>();
			var initialBalance = accountRepo.GetAllIndexKeys<double>("InitialBalance").Where(it => it.Key == AccountId).Select(it => it.Value).FirstOrDefault();
			return initialBalance + transSum.GetSumByAccount(AccountId);
		}
	}
}
