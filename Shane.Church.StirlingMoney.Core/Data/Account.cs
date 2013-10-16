using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Linq;

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

		public double AccountBalance
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

		public double PostedBalance
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

		public IQueryable<Transaction> Transactions
		{
			get
			{
				var repo = KernelService.Kernel.Get<IRepository<Transaction>>();
				return repo.GetFilteredEntries((t) => t.AccountId == this.AccountId);
			}
		}
	}
}
