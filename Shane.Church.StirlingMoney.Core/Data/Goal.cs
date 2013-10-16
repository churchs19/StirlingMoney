using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Linq;

namespace Shane.Church.StirlingMoney.Core.Data
{
	public class Goal
	{
		public long? Id { get; set; }
		public Guid GoalId { get; set; }
		public string GoalName { get; set; }
		public Guid AccountId { get; set; }
		public double Amount { get; set; }
		public double InitialBalance { get; set; }
		public DateTime TargetDate { get; set; }
		public DateTime StartDate { get; set; }
		public DateTimeOffset EditDateTime { get; set; }
		public bool? IsDeleted { get; set; }

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
	}
}
