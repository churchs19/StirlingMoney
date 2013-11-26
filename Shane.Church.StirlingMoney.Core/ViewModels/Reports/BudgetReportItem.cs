
using GalaSoft.MvvmLight;
namespace Shane.Church.StirlingMoney.Core.ViewModels.Reports
{
	public class BudgetReportItem : ObservableObject
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
				Set(() => Label, ref _label, value);
			}
		}

		private double _target;
		public double Target
		{
			get { return _target; }
			set
			{
				Set(() => Target, ref _target, value);
			}
		}

		private double _actual;
		public double Actual
		{
			get { return _actual; }
			set
			{
				Set(() => Actual, ref _actual, value);
			}
		}
	}
}
