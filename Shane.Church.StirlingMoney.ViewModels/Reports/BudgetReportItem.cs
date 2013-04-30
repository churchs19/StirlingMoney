using System;
using System.ComponentModel;

namespace Shane.Church.StirlingMoney.ViewModels.Reports
{
	public class BudgetReportItem : INotifyPropertyChanged
	{
		public BudgetReportItem()
		{

		}

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

		private double _target;
		public double Target
		{
			get { return _target; }
			set
			{
				if (_target != value)
				{
					_target = value;
					NotifyPropertyChanged("Target");
				}
			}
		}

		private double _actual;
		public double Actual
		{
			get { return _actual; }
			set
			{
				if (_actual != value)
				{
					_actual = value;
					NotifyPropertyChanged("Actual");
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
