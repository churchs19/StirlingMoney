using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Data;

namespace Shane.Church.StirlingMoney.WP.Helpers
{
    public class SearchImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var isLightTheme = ((Visibility)Application.Current.Resources["PhoneLightThemeVisibility"]) == Visibility.Visible;

            var isSearchVisible = false;
            var isInverted = false;
            if (value != null && value is bool && (bool)value == true) { isSearchVisible = true; }
            if (parameter != null && parameter is bool && (bool)parameter == true) { isInverted = true; }

            if ((isLightTheme && !isInverted) || (!isLightTheme && isInverted))
            {
                // Is light theme
                if (isSearchVisible)
                {
                    return new BitmapImage(new Uri("/Images/Search.dark.active.png", UriKind.Relative));
                }
                else
                {
                    return new BitmapImage(new Uri("/Images/Search.dark.png", UriKind.Relative));
                }
            }
            else
            {
                // Is dark theme
                if (isSearchVisible)
                {
                    return new BitmapImage(new Uri("/Images/Search.light.active.png", UriKind.Relative));
                }
                else
                {
                    return new BitmapImage(new Uri("/Images/Search.light.png", UriKind.Relative));
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
