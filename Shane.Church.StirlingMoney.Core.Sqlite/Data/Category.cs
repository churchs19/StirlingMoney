using System;
using SQLite;
using AutoMapper;

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

        public static Category FromCore(Core.Data.Category toMap)
        {
            return Mapper.Map<Core.Data.Category, Category>(toMap);
        }

        public Core.Data.Category ToCore()
        {
            return Mapper.Map<Category, Core.Data.Category>(this);
        }
    }
}
