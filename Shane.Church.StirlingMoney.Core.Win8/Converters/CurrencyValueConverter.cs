using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace Shane.Church.StirlingMoney.Core.Win8.Converters
{
    public sealed class CurrencyValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
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

        public object ConvertBack(object value, Type targetType, object parameter, string language)
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
