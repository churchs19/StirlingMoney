using System;
using SQLite;

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
        [Indexed(Name = "ixCategoryName", Unique = true)]
		public string CategoryName { get; set; }
        [Indexed]
        public DateTimeOffset EditDateTime { get; set; }
        [Indexed]
        public bool IsDeleted { get; set; }
	}
}
