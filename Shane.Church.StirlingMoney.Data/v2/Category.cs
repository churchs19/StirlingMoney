using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Shane.Church.StirlingMoney.Data.v2
{
	[Table]
	public class Category : INotifyPropertyChanged, INotifyPropertyChanging
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
				if (_categoryId != value)
				{
					NotifyPropertyChanging("CategoryId");
					_categoryId = value;
					NotifyPropertyChanged("CategoryId");
				}
			}
		}

		private string _categoryName;
		[Column(CanBeNull = false)]
		public string CategoryName
		{
			get { return _categoryName; }
			set
			{
				if (_categoryName != value)
				{
					NotifyPropertyChanging("CategoryName");
					_categoryName = value;
					NotifyPropertyChanged("CategoryName");
				}
			}
		}

#pragma warning disable 0169
		// Version column aids update performance.
		[Column(IsVersion = true)]
		private Binary _version;
#pragma warning restore 0169

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		// Used to notify that a property changed
		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion

		#region INotifyPropertyChanging Members

		public event PropertyChangingEventHandler PropertyChanging;

		// Used to notify that a property is about to change
		private void NotifyPropertyChanging(string propertyName)
		{
			if (PropertyChanging != null)
			{
				PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
			}
		}

		#endregion
	}
}
