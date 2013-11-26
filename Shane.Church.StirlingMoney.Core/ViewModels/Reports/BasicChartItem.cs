
using GalaSoft.MvvmLight;
namespace Shane.Church.StirlingMoney.Core.ViewModels.Reports
{
	public class BasicChartItem : ObservableObject
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
				Set(() => Title, ref _title, value);
			}
		}

		private double _value;
		public double Value
		{
			get { return _value; }
			set
			{
				Set(() => Value, ref _value, value);
			}
		}
	}
}
