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
        [Indexed(Unique = true)]
		public string BudgetName { get; set; }
		public double BudgetAmount { get; set; }
		public Guid CategoryId { get; set; }
		public Core.Data.PeriodType BudgetPeriod { get; set; }
		public DateTimeOffset StartDate { get; set; }
		public DateTimeOffset? EndDate { get; set; }
        [Indexed]
        public DateTimeOffset EditDateTime { get; set; }
		public bool IsDeleted { get; set; }


        public static Budget FromCore(Core.Data.Budget toMap)
        {
            return new Budget()
            {
                BudgetId = toMap.BudgetId,
                BudgetName = toMap.BudgetName,
                BudgetAmount = toMap.BudgetAmount,
                CategoryId = toMap.CategoryId.HasValue ? toMap.CategoryId.Value : Guid.Empty,
                BudgetPeriod = toMap.BudgetPeriod,
                StartDate = toMap.StartDate,
                EndDate = toMap.EndDate,
                EditDateTime = toMap.EditDateTime,
                IsDeleted = toMap.IsDeleted
            };
//            return Mapper.Map<Core.Data.Budget, Budget>(toMap);
        }

        public Core.Data.Budget ToCore()
        {
            return new Core.Data.Budget()
            {
                BudgetId = this.BudgetId,
                BudgetName = this.BudgetName,
                BudgetAmount = this.BudgetAmount,
                CategoryId = this.CategoryId != Guid.Empty ? this.CategoryId : (Guid?)null,
                BudgetPeriod = this.BudgetPeriod,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                EditDateTime = this.EditDateTime,
                IsDeleted = this.IsDeleted
            };
            //return Mapper.Map<Budget, Core.Data.Budget>(this);
        }
    }
}
