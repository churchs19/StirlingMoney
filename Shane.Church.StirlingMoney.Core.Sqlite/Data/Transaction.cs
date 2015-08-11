using SQLite;
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
		public long CheckNumber { get; set; }
        [Indexed]
		public DateTimeOffset EditDateTime { get; set; }
		public bool IsDeleted { get; set; }

        [Indexed]
		public Guid AccountId { get; set; }
        [Indexed]
		public Guid CategoryId { get; set; }
	}
}
