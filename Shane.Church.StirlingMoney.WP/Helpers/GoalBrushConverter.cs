using Shane.Church.StirlingMoney.Core.ViewModels;
using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Shane.Church.StirlingMoney.WP.Helpers
{
	public class GoalBrushConverter : IValueConverter
	{
		//#FF29C329
		private SolidColorBrush _onPace = new SolidColorBrush(Color.FromArgb(255, 41, 195, 41));
		private SolidColorBrush _behind = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
		private SolidColorBrush _lagging = new SolidColorBrush(Color.FromArgb(255, 255, 246, 19));
		private SolidColorBrush _indeterminate = new SolidColorBrush(Color.FromArgb(255, 255, 164, 0)); //#FFA400

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is GoalSummaryViewModel)
			{
				var model = (GoalSummaryViewModel)value;

				if (model.DaysRemaining <= 0)
				{
					return _behind;
				}

				var totalDays = model.TargetDate - model.StartDate;
				var totalAmount = model.GoalAmount - model.InitialBalance;
				var pacePerDay = totalAmount / totalDays.Days;

				var todaysTargetAmount = totalAmount - (model.DaysRemaining * pacePerDay);

				if (model.CurrentAmount > todaysTargetAmount)
					return _onPace;
				else if (model.CurrentAmount >= (todaysTargetAmount * .95))
					return _lagging;
				else
					return _behind;
			}
			return _indeterminate;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
