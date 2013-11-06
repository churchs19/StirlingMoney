using Newtonsoft.Json;
using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.Data
{
	public class Account
	{
		public long? Id { get; set; }
		public Guid AccountId { get; set; }
		public string AccountName { get; set; }
		public double InitialBalance { get; set; }
		public DateTimeOffset EditDateTime { get; set; }
		public bool? IsDeleted { get; set; }
		public string ImageUri { get; set; }
		[JsonIgnore]
		public double AccountBalance { get; set; }
		[JsonIgnore]
		public double PostedBalance { get; set; }

		[JsonIgnore]
		public double LiveAccountBalance
		{
			get
			{
				try
				{
					return InitialBalance + KernelService.Kernel.Get<ITransactionSum>().GetSumByAccount(AccountId);
				}
				catch { return 0; }
			}
		}

		[JsonIgnore]
		public double LivePostedBalance
		{
			get
			{
				try
				{
					return InitialBalance + KernelService.Kernel.Get<ITransactionSum>().GetPostedSumByAccount(AccountId);
				}
				catch { return 0; }
			}
		}

		[JsonIgnore]
		public IQueryable<Transaction> Transactions
		{
			get
			{
				var repo = KernelService.Kernel.Get<IRepository<Transaction>>();
				return repo.GetFilteredEntries((t) => t.AccountId == this.AccountId);
			}
		}

		public async Task UpdateBalances()
		{
			var repo = KernelService.Kernel.Get<IRepository<Account>>();
			var acct = await repo.GetFilteredEntriesAsync(it => it.AccountId == this.AccountId);
			var a = acct.FirstOrDefault();
			if (a != null)
			{
				a.AccountBalance = a.LiveAccountBalance;
				a.PostedBalance = a.LivePostedBalance;
				await repo.AddOrUpdateEntryAsync(a);
			}
		}
	}
}
