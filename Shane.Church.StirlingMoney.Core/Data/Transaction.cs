using Ninject;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.Data
{
	public class Transaction
	{
		public Guid TransactionId { get; set; }
		public DateTimeOffset TransactionDate { get; set; }
		public double Amount { get; set; }
		public string Location { get; set; }
		public string Note { get; set; }
		public bool Posted { get; set; }
		public long? CheckNumber { get; set; }
		public DateTimeOffset EditDateTime { get; set; }
		public bool IsDeleted { get; set; }

		public Guid AccountId { get; set; }
		public Guid CategoryId { get; set; }

		public async Task<Account> GetAccount()
		{
			try
			{
				return await KernelService.Kernel.Get<IRepository<Account, Guid>>().GetEntryAsync(AccountId);
			}
			catch
			{
				return null;
			}
		}

		public async Task<Category> GetCategory()
		{
			try
			{
				if (!CategoryId.Equals(Guid.Empty)) return null;
				return await KernelService.Kernel.Get<IRepository<Category, Guid>>().GetEntryAsync(CategoryId);
			}
			catch
			{
				return null;
			}
		}
	}
}
