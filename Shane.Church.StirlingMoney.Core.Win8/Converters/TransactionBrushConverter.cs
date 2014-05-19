using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Shane.Church.StirlingMoney.Core.Win8.Converters
{
	public sealed class TransactionBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
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

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
