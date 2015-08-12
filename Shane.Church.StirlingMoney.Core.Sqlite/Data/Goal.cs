using AutoMapper;
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
        [Indexed(Name = "ixGoalName", Unique = true)]
        public string GoalName { get; set; }
        [ForeignKey(typeof(Data.Account))]
		public Guid AccountId { get; set; }
		public double Amount { get; set; }
		public double InitialBalance { get; set; }
		public DateTimeOffset TargetDate { get; set; }
		public DateTimeOffset StartDate { get; set; }
        [Indexed]
        public DateTimeOffset EditDateTime { get; set; }
		public bool IsDeleted { get; set; }

        [ManyToOne]
        public Account Account { get; set; }

        public static Goal FromCore(Core.Data.Goal toMap)
        {
            return Mapper.Map<Core.Data.Goal, Goal>(toMap);
        }

        public Core.Data.Goal ToCore()
        {
            return Mapper.Map<Goal, Core.Data.Goal>(this);
        }
    }
}
