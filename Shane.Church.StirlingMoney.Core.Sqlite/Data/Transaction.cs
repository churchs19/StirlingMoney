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
        [Indexed]
		public Guid CategoryId { get; set; }

        public static Transaction FromCore(Core.Data.Transaction toMap)
        {
            return new Transaction()
            {
                TransactionId = toMap.TransactionId,
                TransactionDate = toMap.TransactionDate,
                Amount = toMap.Amount,
                Location = toMap.Location,
                Note = toMap.Note,
                Posted = toMap.Posted,
                CheckNumber = toMap.CheckNumber,
                EditDateTime = toMap.EditDateTime,
                IsDeleted = toMap.IsDeleted,
                AccountId = toMap.AccountId,
                CategoryId = toMap.CategoryId
            };
        }

        public Core.Data.Transaction ToCore()
        {
            return new Core.Data.Transaction()
            {
                TransactionId = this.TransactionId,
                TransactionDate = this.TransactionDate,
                Amount = this.Amount,
                Location = this.Location,
                Note = this.Note,
                Posted = this.Posted,
                CheckNumber = this.CheckNumber,
                EditDateTime = this.EditDateTime,
                IsDeleted = this.IsDeleted,
                AccountId = this.AccountId,
                CategoryId = this.CategoryId
            };
        }
    }
}
