using Newtonsoft.Json;
using Ninject;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
			: this(KernelService.Kernel.Get<IRepository<Account, Guid>>(),
					KernelService.Kernel.Get<IRepository<Transaction, Guid>>(),
					KernelService.Kernel.Get<ITransactionSum>())
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
			var transactionIds = _transactionRepository.GetAllIndexKeys<Guid>("TransactionAccountId").Where(it => it.Value == this.AccountId).Select(it => it.Key);
			return _transactionRepository.GetAllIndexKeys<Tuple<DateTimeOffset, DateTimeOffset>>("TransactionDateEditDateTime").Where(it => transactionIds.Contains(it.Key)).ToDictionary(key => key.Key, val => val.Value);
		}

		public async Task<IQueryable<Transaction>> GetTransactions()
		{
			return await _transactionRepository.GetIndexFilteredEntriesAsync<Guid>("TransactionAccountId", AccountId);
		}

		public static double GetAccountBalance(Guid AccountId)
		{
			var accountRepo = KernelService.Kernel.Get<IRepository<Account, Guid>>();
			var transSum = KernelService.Kernel.Get<ITransactionSum>();
			var initialBalance = accountRepo.GetAllIndexKeys<double>("InitialBalance").Where(it => it.Key == AccountId).Select(it => it.Value).FirstOrDefault();
			return initialBalance + transSum.GetSumByAccount(AccountId);
		}
	}
}
