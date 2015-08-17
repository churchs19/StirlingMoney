using System;
using AutoMapper;
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
        [Indexed(Name = "ixUserEmail", Unique = true)]
		public string UserEmail { get; set; }
        [Indexed(Unique = true)]
    	public string UserId { get; set; }
        [Indexed]
		public DateTimeOffset EditDateTime { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsSyncOwner { get; set; }

        public static AppSyncUser FromCore(Core.Data.AppSyncUser toMap)
        {
            return Mapper.Map<Core.Data.AppSyncUser, AppSyncUser>(toMap);
        }

        public Core.Data.AppSyncUser ToCore()
        {
            return Mapper.Map<AppSyncUser, Core.Data.AppSyncUser>(this);
        }
    }
}
