using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Shane.Church.StirlingMoney.Core.WP.Converters
{
	public class TransactionBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is double)
			{
				if (((double)value) >= 0)
				{
					return new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
				}
				else
				{
					return new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
				}
			}
			else
			{
				return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
