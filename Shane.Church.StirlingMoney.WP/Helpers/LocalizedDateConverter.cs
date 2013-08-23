using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Shane.Church.StirlingMoney.WP.Helpers
{
	public class LocalizedDateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			try
			{
				DateTime dtValue = System.Convert.ToDateTime(value);
				return dtValue.ToString("D", CultureInfo.CurrentUICulture);
			}
			catch
			{
				return value.ToString();
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			try
			{
				DateTime dtValue = DateTime.Parse(value.ToString(), CultureInfo.CurrentUICulture, DateTimeStyles.AssumeLocal);
				return dtValue;
			}
			catch
			{
				return DateTime.MinValue;
			}
		}
	}
}
