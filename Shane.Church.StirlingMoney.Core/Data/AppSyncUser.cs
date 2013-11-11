using System;

namespace Shane.Church.StirlingMoney.Core.Data
{
	public class AppSyncUser
	{
		public Guid AppSyncId { get; set; }
		public string UserEmail { get; set; }
		public string UserId { get; set; }
		public DateTimeOffset EditDateTime { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsSyncOwner { get; set; }
	}
}
