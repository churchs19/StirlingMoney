using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;

namespace Shane.Church.StirlingMoney.Core.Sqlite.Data
{
    public class Goal
	{
		public Goal()
		{
			GoalName = "";
		}

        [PrimaryKey]
		public Guid GoalId { get; set; }
        [Indexed(Unique = true)]
        public string GoalName { get; set; }
        [Indexed]
		public Guid AccountId { get; set; }
		public double Amount { get; set; }
		public double InitialBalance { get; set; }
		public DateTimeOffset TargetDate { get; set; }
		public DateTimeOffset StartDate { get; set; }
        [Indexed]
        public DateTimeOffset EditDateTime { get; set; }
		public bool IsDeleted { get; set; }

        public static Goal FromCore(Core.Data.Goal toMap)
        {
            return new Goal()
            {
                GoalId = toMap.GoalId,
                GoalName = toMap.GoalName,
                AccountId = toMap.AccountId,
                Amount = toMap.Amount,
                InitialBalance = toMap.InitialBalance,
                TargetDate = toMap.TargetDate,
                StartDate = toMap.StartDate,
                EditDateTime = toMap.EditDateTime,
                IsDeleted = toMap.IsDeleted
            };
        }

        public Core.Data.Goal ToCore()
        {
            return new Core.Data.Goal()
            {
                GoalId = this.GoalId,
                GoalName = this.GoalName,
                AccountId = this.AccountId,
                Amount = this.Amount,
                InitialBalance = this.InitialBalance,
                TargetDate = this.TargetDate,
                StartDate = this.StartDate,
                EditDateTime = this.EditDateTime,
                IsDeleted = this.IsDeleted
            };
        }
    }
}
