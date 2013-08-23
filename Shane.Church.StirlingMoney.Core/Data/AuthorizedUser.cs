using System;

namespace Shane.Church.StirlingMoney.Core.Data
{
	public class AuthorizedUser
	{
		public long? Id { get; set; }
		public Guid AuthorizedUserId { get; set; }
		public string MicrosoftAccountEmail { get; set; }
		public string StirlingMoneyAccountId { get; set; }
		public DateTimeOffset EditDateTime { get; set; }
		public bool? IsDeleted { get; set; }
	}
}
