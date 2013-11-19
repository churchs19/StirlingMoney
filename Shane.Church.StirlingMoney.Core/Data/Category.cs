using System;

namespace Shane.Church.StirlingMoney.Core.Data
{
	public class Category
	{
		public Category()
		{
			CategoryName = "";
		}

		public Guid CategoryId { get; set; }
		public string CategoryName { get; set; }
		public DateTimeOffset EditDateTime { get; set; }
		public bool IsDeleted { get; set; }
	}
}
