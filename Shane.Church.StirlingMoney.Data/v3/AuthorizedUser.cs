using System;
using System.Data.Linq.Mapping;

namespace Shane.Church.StirlingMoney.Data.v3
{
	[Table]
	public class AuthorizedUser : ChangeTrackingObject
	{
		private Guid _authorizedUserId;
		[Column(IsPrimaryKey = true, CanBeNull = false, DbType = "UNIQUEIDENTIFIER NOT NULL")]
		public Guid AuthorizedUserId
		{
			get { return _authorizedUserId; }
			set
			{
				Set(() => AuthorizedUserId, ref _authorizedUserId, value);
			}
		}

		private string _microsoftAccountEmail;
		[Column(CanBeNull = false)]
		public string MicrosoftAccountEmail
		{
			get { return _microsoftAccountEmail; }
			set
			{
				Set(() => MicrosoftAccountEmail, ref _microsoftAccountEmail, value);
			}
		}

		private string _stirlingMoneyAccountId;
		[Column(CanBeNull = true)]
		public string StirlingMoneyAccountId
		{
			get { return _stirlingMoneyAccountId; }
			set
			{
				Set(() => StirlingMoneyAccountId, ref _stirlingMoneyAccountId, value);
			}
		}
	}
}
