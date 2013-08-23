using System;

namespace Shane.Church.StirlingMoney.Core.Data
{
	public class Category
	{
		public long? Id { get; set; }
		public Guid CategoryId { get; set; }
		public string CategoryName { get; set; }
		public DateTimeOffset EditDateTime { get; set; }
		public bool? IsDeleted { get; set; }
	}
}
