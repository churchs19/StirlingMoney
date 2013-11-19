using Ninject;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.Data
{
	public class Goal
	{
		public Goal()
		{
			GoalName = "";
		}

		public Guid GoalId { get; set; }
		public string GoalName { get; set; }
		public Guid AccountId { get; set; }
		public double Amount { get; set; }
		public double InitialBalance { get; set; }
		public DateTimeOffset TargetDate { get; set; }
		public DateTimeOffset StartDate { get; set; }
		public DateTimeOffset EditDateTime { get; set; }
		public bool IsDeleted { get; set; }

		public async Task<Account> GetAccount()
		{
			try
			{
				return await KernelService.Kernel.Get<IRepository<Account, Guid>>().GetEntryAsync(this.AccountId);
			}
			catch
			{
				return null;
			}
		}
	}
}
