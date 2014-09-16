using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Shane.Church.StirlingMoney.Universal.Converters
{
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public bool IsReversed { get; set; }
        public object Convert(object value, Type targetType, object parameter, string language)
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
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
