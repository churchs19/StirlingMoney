using System;
using System.IO;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Data;

namespace Shane.Church.StirlingMoney.Core.Win8.Converters
{
    public sealed class ByteToImageSourceValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                MemoryStream s = new MemoryStream((byte[])value, true);                

                BitmapImage source = new BitmapImage();
                source.SetSource(s.AsRandomAccessStream());

                return source;
            }
            catch { return new BitmapImage(); }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
