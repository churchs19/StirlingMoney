using Newtonsoft.Json;
using SQLite;
using System;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.Sqlite.Data
{
    public class Budget
	{
		public Budget()
		{
			BudgetName = "";
		}

        [PrimaryKey]
		public Guid BudgetId { get; set; }
        [Indexed(Name = "ixBudgetName", Unique = true)]
		public string BudgetName { get; set; }
		public double BudgetAmount { get; set; }
		public Guid? CategoryId { get; set; }
		public Core.Data.PeriodType BudgetPeriod { get; set; }
		public DateTimeOffset StartDate { get; set; }
		public DateTimeOffset? EndDate { get; set; }
        [Indexed]
        public DateTimeOffset EditDateTime { get; set; }
		public bool IsDeleted { get; set; }
	}
}
