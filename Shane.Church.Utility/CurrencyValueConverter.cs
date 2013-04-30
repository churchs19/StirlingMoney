using System;
using System.Windows.Data;
using System.Globalization;

namespace Shane.Church.Utility
{
	public class CurrencyValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			try
			{
				double dblValue = System.Convert.ToDouble(value);
				return dblValue.ToString("C", CultureInfo.CurrentUICulture);
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
				double dblValue = double.Parse(value.ToString(), System.Globalization.NumberStyles.Any, CultureInfo.CurrentUICulture);
				return dblValue;
			}
			catch
			{
				return (double)0;
			}
		}
	}
}
