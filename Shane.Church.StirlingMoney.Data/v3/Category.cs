using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Shane.Church.StirlingMoney.Data.v3
{
	[Table]
	public class Category : ChangeTrackingObject
	{
		public Category()
		{
		}

		private Guid _categoryId;
		[Column(IsPrimaryKey = true, CanBeNull = false, DbType = "UNIQUEIDENTIFIER NOT NULL")]
		public Guid CategoryId
		{
			get { return _categoryId; }
			set
			{
                Set(() => CategoryId, ref _categoryId, value);
			}
		}

		private string _categoryName;
		[Column(CanBeNull = false)]
		public string CategoryName
		{
			get { return _categoryName; }
			set
			{
                Set(() => CategoryName, ref _categoryName, value);
			}
		}
	}
}
