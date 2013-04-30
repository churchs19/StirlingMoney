using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Reflection;
using Shane.Church.Utility;

namespace Shane.Church.StirlingMoney
{
	public class Group<T> : IEnumerable<T>
	{
		public Group(string name, IEnumerable<T> items)
		{
			this.Title = name;
			try
			{
				this.ItemsCollection = new List<T>(items);
			}
			catch (ArgumentOutOfRangeException)
			{
				this.ItemsCollection = new List<T>();
			}
			_items = new CollectionViewSource();
			_items.Source = ItemsCollection;
			_items.View.Filter = (x) => FilterItem(x);
		}

		public override bool Equals(object obj)
		{
			Group<T> that = obj as Group<T>;

			return (that != null) && (this.Title.Equals(that.Title));
		}

		public string Title
		{
			get;
			set;
		}

		private IList<T> ItemsCollection
		{
			get;
			set;
		}

		private string _filter;
		public string Filter
		{
			get { return _filter; }
			set
			{
				if (_filter != value)
				{
					_filter = value;
					if (_items != null)
						_items.View.Refresh();
				}
			}
		}

		private bool FilterItem(object item)
		{
			if (string.IsNullOrWhiteSpace(Filter) || this.Title.ToLower().Contains(Filter.ToLower()))
			{
				return true;
			}
			else
			{
				try
				{
					var itemT = (T)item;
					var properties = itemT.GetType().GetProperties();
					foreach (PropertyInfo p in properties)
					{
						if(p.GetIndexParameters().Length == 0 && p.PropertyType != typeof(Guid))
						{
							var text = p.GetValue(itemT, null);
							if (text != null && text.ToString().ToLower().Contains(Filter.ToLower()))
								return true;
						}
					}
					return false;
				}
				catch { return true; }
			}
		}

		private CollectionViewSource _items;
		public ICollectionView Items
		{
			get { return _items.View; }
		}

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return new CastEnumerator<T>(this.Items.GetEnumerator());
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.Items.GetEnumerator();
		}

		#endregion
	}
}
