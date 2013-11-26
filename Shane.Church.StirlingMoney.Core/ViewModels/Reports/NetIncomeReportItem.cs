
using GalaSoft.MvvmLight;
namespace Shane.Church.StirlingMoney.Core.ViewModels.Reports
{
	public class NetIncomeReportItem : ObservableObject
	{
		private string _label;
		public string Label
		{
			get { return _label; }
			set
			{
				Set(() => Label, ref _label, value);
			}
		}

		private double _income;
		public double Income
		{
			get { return _income; }
			set
			{
				Set(() => Income, ref _income, value);
			}
		}

		private double _expenses;
		public double Expenses
		{
			get { return _expenses; }
			set
			{
				Set(() => Expenses, ref _expenses, value);
			}
		}
	}
}
