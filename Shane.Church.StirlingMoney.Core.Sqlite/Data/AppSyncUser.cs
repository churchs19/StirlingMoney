using System;
using SQLite.Net.Attributes;

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
        [Indexed(Unique = true)]
		public string UserEmail { get; set; }
        [Indexed(Unique = true)]
    	public string UserId { get; set; }
        [Indexed]
		public DateTimeOffset EditDateTime { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsSyncOwner { get; set; }

        public static AppSyncUser FromCore(Core.Data.AppSyncUser toMap)
        {
            return new AppSyncUser()
            {
                AppSyncId = toMap.AppSyncId,
                UserEmail = toMap.UserEmail,
                UserId = toMap.UserId,
                EditDateTime = toMap.EditDateTime,
                IsDeleted = toMap.IsDeleted,
                IsSyncOwner = toMap.IsSyncOwner
            };
        }

        public Core.Data.AppSyncUser ToCore()
        {
            return new Core.Data.AppSyncUser()
            {
                AppSyncId = this.AppSyncId,
                UserEmail = this.UserEmail,
                UserId = this.UserId,
                EditDateTime = this.EditDateTime,
                IsDeleted = this.IsDeleted,
                IsSyncOwner = this.IsSyncOwner
            };
        }
    }
}
