using System;
using SQLite;

namespace Shane.Church.StirlingMoney.Core.Sqlite.Data
{
    public class AppSyncUser
	{
		public AppSyncUser()
		{
			UserEmail = "";
		}

        [PrimaryKey]
		public Guid AppSyncId { get; set; }
        [Indexed(Name = "ixUserEmail", Unique = true)]
		public string UserEmail { get; set; }
		public string UserId { get; set; }
        [Indexed]
		public DateTimeOffset EditDateTime { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsSyncOwner { get; set; }
	}
}
