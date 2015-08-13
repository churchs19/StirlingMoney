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
		private IDataRepository<Account, Guid> _accountRepository;
		private IDataRepository<Transaction, Guid> _transactionRepository;
		private ITransactionSum _transactionSum;

		public Account(IDataRepository<Account, Guid> accountRepo, IDataRepository<Transaction, Guid> transactionRepo, ITransactionSum transSum)
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
			: this(ContainerService.Container.Locate<IDataRepository<Account, Guid>>(),
					ContainerService.Container.Locate<IDataRepository<Transaction, Guid>>(),
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
                return _transactionRepository.GetFilteredEntriesCount(t => t.AccountId == AccountId);
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

		public IQueryable<Transaction> GetTransactions(int currentRow = 0, int? pageSize = null)
		{
            return _transactionRepository.GetFilteredEntries(t => t.AccountId == AccountId, false, currentRow, pageSize);
		}

		public static double GetAccountBalance(Guid AccountId)
		{
			var accountRepo = ContainerService.Container.Locate<IDataRepository<Account, Guid>>();
			var transSum = ContainerService.Container.Locate<ITransactionSum>();
            var account = accountRepo.GetEntry(AccountId);
            double initialBalance = 0;
            if (account != null) initialBalance = account.InitialBalance;
			return initialBalance + transSum.GetSumByAccount(AccountId);
		}
	}
}
