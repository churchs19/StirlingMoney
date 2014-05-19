using System;
using System.Windows.Data;

namespace Shane.Church.StirlingMoney.WP.Helpers
{
	public class IntBusyAnimationConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is int)
			{
				var enumValue = (Telerik.Windows.Controls.AnimationStyle)value;
				return enumValue;
			}
			else
			{
				return Telerik.Windows.Controls.AnimationStyle.AnimationStyle3;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
