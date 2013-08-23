using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Shane.Church.StirlingMoney.Core.WP.Converters
{
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public bool IsReversed { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = System.Convert.ToBoolean(value, CultureInfo.InvariantCulture);
            if (this.IsReversed)
            {
                val = !val;
            }
            if (val)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
