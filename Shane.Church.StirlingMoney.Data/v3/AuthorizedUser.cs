using Shane.Church.Utility.Core.WP;
using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Shane.Church.StirlingMoney.Data.v3
{
	[Table]
	public class AuthorizedUser : ChangingObservableObject
	{
		private long? _id;
		[Column(CanBeNull = true)]
		public long? Id
		{
			get { return _id; }
			set
			{
				Set(() => Id, ref _id, value);
			}
		}

		private DateTime _editDateTime;
		[Column(CanBeNull = false)]
		public DateTime EditDateTime
		{
			get { return _editDateTime; }
			set
			{
				Set(() => EditDateTime, ref _editDateTime, value);
			}
		}

#pragma warning disable 0169
		[Column(IsVersion = true)]
		private Binary _version;
#pragma warning restore 0169

		private bool? _isDeleted;
		[Column(CanBeNull = true)]
		public bool? IsDeleted
		{
			get { return _isDeleted; }
			set
			{
				Set(() => IsDeleted, ref _isDeleted, value);
			}
		}

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

		[Column]
		internal Guid _accountId;

		private EntityRef<Account> _account;

		[Association(Storage = "_account", ThisKey = "_accountId", OtherKey = "AccountId", IsForeignKey = true)]
		public Account Account
		{
			get { return _account.Entity; }
			set
			{
				RaisePropertyChanging(() => Account);
				_account.Entity = value;

				if (value != null)
				{
					_accountId = value.AccountId;
				}

				RaisePropertyChanged(() => Account);
			}
		}
	}
}
