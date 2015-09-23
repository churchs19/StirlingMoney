using SQLite.Net;
using System;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

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
        [Indexed(Unique = true)]
		public string AccountName { get; set; }
		public double InitialBalance { get; set; }
        [Indexed]
        public DateTimeOffset EditDateTime { get; set; }
		public bool IsDeleted { get; set; }
		public string ImageUri { get; set; }

        public static Account FromCore(Core.Data.Account toMap)
        {
            return new Account()
            {
                AccountId = toMap.AccountId,
                AccountName = toMap.AccountName,
                InitialBalance = toMap.InitialBalance,
                EditDateTime = toMap.EditDateTime,
                IsDeleted = toMap.IsDeleted,
                ImageUri = toMap.ImageUri
            };
        }

        public Core.Data.Account ToCore()
        {
            return new Core.Data.Account()
            {
                AccountId = this.AccountId,
                AccountName = this.AccountName,
                InitialBalance = this.InitialBalance,
                EditDateTime = this.EditDateTime,
                IsDeleted = this.IsDeleted,
                ImageUri = this.ImageUri
            };
        }
	}
}
