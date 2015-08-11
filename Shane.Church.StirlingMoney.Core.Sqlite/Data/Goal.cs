using SQLite;
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
        [Indexed(Name = "ixGoalName", Unique = true)]
        public string GoalName { get; set; }
		public Guid AccountId { get; set; }
		public double Amount { get; set; }
		public double InitialBalance { get; set; }
		public DateTimeOffset TargetDate { get; set; }
		public DateTimeOffset StartDate { get; set; }
        [Indexed]
        public DateTimeOffset EditDateTime { get; set; }
		public bool IsDeleted { get; set; }
	}
}
