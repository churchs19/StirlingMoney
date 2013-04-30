using System;
using System.ComponentModel;

namespace Shane.Church.StirlingMoney.ViewModels.Reports
{

	public class BasicChartItem : INotifyPropertyChanged
	{
		public BasicChartItem()
		{

		}

		private string _title;
		public string Title
		{
			get { return _title; }
			set
			{
				if (_title != value)
				{
					_title = value;
					NotifyPropertyChanged("Title");
				}
			}
		}

		private double _value;
		public double Value
		{
			get { return _value; }
			set
			{
				if (_value != value)
				{
					_value = value;
					NotifyPropertyChanged("Value");
				}
			}
		}


		#region INotifyPropertyChanged Members
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (null != handler)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}
}
