using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Shane.Church.StirlingMoney.Data.v1
{
	[Table]
	public class Category : INotifyPropertyChanged, INotifyPropertyChanging
	{
		public Category()
		{
		}

		private long _categoryId;
		[Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "BIGINT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
		public long CategoryId
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

		// Version column aids update performance.
		[Column(IsVersion = true)]
		private Binary _version;

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
