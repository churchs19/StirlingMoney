using System;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Shane.Church.StirlingMoney.Core.WP.Converters
{
    public class ByteToImageSourceValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
        {
            try
            {
                MemoryStream s = new MemoryStream((byte[])value, true);

                BitmapImage source = new BitmapImage();
                source.SetSource(s);

                return source;
            }
            catch { return new BitmapImage(); }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
        {
            throw new NotSupportedException();
        }
    }
}
