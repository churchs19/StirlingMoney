using AutoMapper;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;

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
        [ForeignKey(typeof(Data.Category))]
		public Guid CategoryId { get; set; }
		public Core.Data.PeriodType BudgetPeriod { get; set; }
		public DateTimeOffset StartDate { get; set; }
		public DateTimeOffset? EndDate { get; set; }
        [Indexed]
        public DateTimeOffset EditDateTime { get; set; }
		public bool IsDeleted { get; set; }

        [ManyToOne]
        public Category Category { get; set; }

        public static Budget FromCore(Core.Data.Budget toMap)
        {
            return Mapper.Map<Core.Data.Budget, Budget>(toMap);
        }

        public Core.Data.Budget ToCore()
        {
            return Mapper.Map<Budget, Core.Data.Budget>(this);
        }
    }
}
