using Shane.Church.Utility.Core.WP;
using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Shane.Church.StirlingMoney.Data.v3
{
	[Table]
	public class Category : ChangingObservableObject
	{
		public Category()
		{
		}

		private long? _id;
		[Column(CanBeNull = true)]
		public long? Id
		{
			get { return _id; }
			set
			{
				Set(() => Id, ref _id, value);
			}
		}

		private DateTime _editDateTime;
		[Column(CanBeNull = false)]
		public DateTime EditDateTime
		{
			get { return _editDateTime; }
			set
			{
				Set(() => EditDateTime, ref _editDateTime, value);
			}
		}

#pragma warning disable 0169
		[Column(IsVersion = true)]
		private Binary _version;
#pragma warning restore 0169

		private bool? _isDeleted;
		[Column(CanBeNull = true)]
		public bool? IsDeleted
		{
			get { return _isDeleted; }
			set
			{
				Set(() => IsDeleted, ref _isDeleted, value);
			}
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
