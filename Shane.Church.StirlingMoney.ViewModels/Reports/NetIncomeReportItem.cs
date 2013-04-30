using System;
using System.ComponentModel;

namespace Shane.Church.StirlingMoney.ViewModels.Reports
{
	public class NetIncomeReportItem : INotifyPropertyChanged
	{
		private string _label;
		public string Label
		{
			get { return _label; }
			set
			{
				if (_label != value)
				{
					_label = value;
					NotifyPropertyChanged("Label");
				}
			}
		}

		private double _income;
		public double Income
		{
			get { return _income; }
			set
			{
				if (_income != value)
				{
					_income = value;
					NotifyPropertyChanged("Income");
				}
			}
		}

		private double _expenses;
		public double Expenses
		{
			get { return _expenses; }
			set
			{
				if (_expenses != value)
				{
					_expenses = value;
					NotifyPropertyChanged("Expenses");
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
