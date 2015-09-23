using System;
using SQLite.Net.Attributes;

namespace Shane.Church.StirlingMoney.Core.Sqlite.Data
{
    public class Category
	{
		public Category()
		{
			CategoryName = "";
		}

        [PrimaryKey]
		public Guid CategoryId { get; set; }
        [Indexed(Unique = true)]
		public string CategoryName { get; set; }
        [Indexed]
        public DateTimeOffset EditDateTime { get; set; }
        [Indexed]
        public bool IsDeleted { get; set; }

        public static Category FromCore(Core.Data.Category toMap)
        {
            return new Category()
            {
                CategoryId = toMap.CategoryId,
                CategoryName = toMap.CategoryName,
                EditDateTime = toMap.EditDateTime,
                IsDeleted = toMap.IsDeleted
            };
        }

        public Core.Data.Category ToCore()
        {
            return new Core.Data.Category()
            {
                CategoryId = this.CategoryId,
                CategoryName = this.CategoryName,
                EditDateTime = this.EditDateTime,
                IsDeleted = this.IsDeleted
            };
        }
    }
}
