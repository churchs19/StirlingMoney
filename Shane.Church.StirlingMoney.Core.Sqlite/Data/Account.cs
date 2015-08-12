using SQLite.Net;
using System;
using AutoMapper;
using SQLite.Net.Attributes;

namespace Shane.Church.StirlingMoney.Core.Sqlite.Data
{
    public class Account
	{

		public Account()
		{
            AccountName = "";
		}

        [PrimaryKey]
		public Guid AccountId { get; set; }
        [Indexed(Name = "ixAccountName", Unique = true)]
		public string AccountName { get; set; }
		public double InitialBalance { get; set; }
        [Indexed]
        public DateTimeOffset EditDateTime { get; set; }
		public bool IsDeleted { get; set; }
		public string ImageUri { get; set; }

        public static Account FromCore(Core.Data.Account accountToMap)
        {
            return Mapper.Map<Core.Data.Account, Account>(accountToMap);
        }

        public Core.Data.Account ToCore()
        {
            return Mapper.Map<Account, Core.Data.Account>(this);
        }
	}
}
