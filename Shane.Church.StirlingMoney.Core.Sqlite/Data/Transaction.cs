using AutoMapper;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;

namespace Shane.Church.StirlingMoney.Core.Sqlite.Data
{
	public class Transaction
	{
        [PrimaryKey]
		public Guid TransactionId { get; set; }
        [Indexed]
		public DateTimeOffset TransactionDate { get; set; }
		public double Amount { get; set; }
        [Indexed]
		public string Location { get; set; }
		public string Note { get; set; }
        [Indexed]
        public bool Posted { get; set; }
        [Indexed]
		public long CheckNumber { get; set; }
        [Indexed]
		public DateTimeOffset EditDateTime { get; set; }
		public bool IsDeleted { get; set; }

        [Indexed]
		public Guid AccountId { get; set; }
		public Guid CategoryId { get; set; }

        public static Transaction FromCore(Core.Data.Transaction toMap)
        {
            return Mapper.Map<Core.Data.Transaction, Transaction>(toMap);
        }

        public Core.Data.Transaction ToCore()
        {
            return Mapper.Map<Transaction, Core.Data.Transaction>(this);
        }
    }
}
