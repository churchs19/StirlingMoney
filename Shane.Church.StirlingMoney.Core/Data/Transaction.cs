using Newtonsoft.Json;
using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Linq;

namespace Shane.Church.StirlingMoney.Core.Data
{
	public class Transaction
	{
		public long? Id { get; set; }
		public Guid TransactionId { get; set; }
		public DateTime TransactionDate { get; set; }
		public double Amount { get; set; }
		public string Location { get; set; }
		public string Note { get; set; }
		public bool Posted { get; set; }
		public long? CheckNumber { get; set; }
		public DateTimeOffset EditDateTime { get; set; }
		public bool? IsDeleted { get; set; }

		public Guid AccountId { get; set; }
		public Guid? CategoryId { get; set; }

		[JsonIgnore]
		public Account Account
		{
			get
			{
				try
				{
					return KernelService.Kernel.Get<IRepository<Account>>().GetFilteredEntries(it => it.AccountId == AccountId).FirstOrDefault();
				}
				catch
				{
					return null;
				}
			}
		}

		[JsonIgnore]
		public Category Category
		{
			get
			{
				try
				{
					if (!CategoryId.HasValue) return null;
					return KernelService.Kernel.Get<IRepository<Category>>().GetFilteredEntries(it => it.CategoryId == CategoryId.Value).FirstOrDefault();
				}
				catch
				{
					return null;
				}
			}
		}
	}
}
