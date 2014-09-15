using System;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Windows.Foundation;
using Windows.UI.Core;

namespace Shane.Church.Utility.Core.WP
{
    public static class Imaging
    {
        public enum ImageType
        {
            Png,
            Jpeg
        }

        public static async Task SaveImageAsync(WriteableBitmap wbm, string path, ImageType type)
        {
            await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    //ensure directory exists    
                    String sDirectory = System.IO.Path.GetDirectoryName(path);

                    if (!appStorage.DirectoryExists(sDirectory))
                    {
                        appStorage.CreateDirectory(sDirectory);
                    }

                    if (appStorage.FileExists(path))
                    {
                        appStorage.DeleteFile(path);
                    }

                    //CompensateForRender(wbm.Pixels);

                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(path, System.IO.FileMode.Create, appStorage))
                    {
                        if (type == ImageType.Png)
                        {
                            wbm.WritePNG(stream);
                        }
                        else
                        {
                            wbm.SaveJpeg(stream, wbm.PixelWidth, wbm.PixelHeight, 0, 90);
                        }
                    }
                }
            });
        }

        public static async Task<WriteableBitmap> LoadImageAsync(string path)
        {
            WriteableBitmap bmp = null;
            await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                try
                {
                    using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(path, System.IO.FileMode.Open, appStorage))
                        {
                            bmp = new WriteableBitmap(173, 173);
                            bmp.LoadJpeg(stream);

                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex is System.OutOfMemoryException)
                    {
                        //                    DebugUtility.DebugOutputMemoryUsage("Error Loading Image");
                    }
                    bmp = null;
                }
            });
            return bmp;
        }

        private static void CompensateForRender(int[] bitmapPixels)
        {
            if (bitmapPixels.Length == 0) return;

            for (int i = 0; i < bitmapPixels.Length; i++)
            {
                uint pixel = unchecked((uint)bitmapPixels[i]);

                double a = (pixel >> 24) & 255;
                if ((a == 255) || (a == 0)) continue;

                double r = (pixel >> 16) & 255;
                double g = (pixel >> 8) & 255;
                double b = (pixel) & 255;

                double factor = 255 / a;
                uint newR = (uint)Math.Round(r * factor);
                uint newG = (uint)Math.Round(g * factor);
                uint newB = (uint)Math.Round(b * factor);

                // compose
                bitmapPixels[i] = unchecked((int)((pixel & 0xFF000000) | (newR << 16) | (newG << 8) | newB));
            }
        }
    }
}
