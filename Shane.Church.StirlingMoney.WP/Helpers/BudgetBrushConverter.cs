using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Shane.Church.StirlingMoney.WP.Helpers
{
	public class BudgetBrushConverter : IValueConverter
	{
		//#FF29C329
		private SolidColorBrush _standard = new SolidColorBrush(Color.FromArgb(255, 41, 195, 41));
		private SolidColorBrush _over = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
		private SolidColorBrush _approaching = new SolidColorBrush(Color.FromArgb(255, 255, 246, 19));
		private SolidColorBrush _indeterminate = new SolidColorBrush(Color.FromArgb(255, 255, 164, 0)); //#FFA400

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is double)
			{
				var dbl = (double)value;
				if (dbl <= 0.9)
					return _standard;
				else if (dbl > 0.9 && dbl < 1.0)
					return _approaching;
				else
					return _over;
			}
			else
			{
				return _indeterminate;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
