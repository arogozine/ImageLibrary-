using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageLibrary
{
    /// <summary>
    /// Shared internal IImage`T code regardless of underlying implementation.
    /// </summary>
    public static class ImageBase
    {
        #region DownSample

        public static IImage<Y> DownsampleRows<Y>(IImage<Y> img, Func<int, int, Y[], IImage<Y>> func)
            where Y : struct, IEquatable<Y>
        {
            int newHeight = img.Height >> 1;
            var newData = new Y[newHeight * img.Width];

            for (int i = 0, j = 0, y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    newData[i++] = img[j++];
                }

                j += img.Width;
            }

            return func(img.Width, newHeight, newData);
        }

        public static IImage<Y> DownsampleCols<Y>(IImage<Y> img, Func<int, int, Y[], IImage<Y>> func)
            where Y : struct, IEquatable<Y>
        {
            int newWidth = img.Width >> 1;
            int height = img.Height;
            Y[] newData = new Y[newWidth * height];
            for (int i = 0, j = 0, y = 0; y < height; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    newData[i++] = img[j];
                    j += 2;
                }
            }

            return func(newWidth, height, newData);
        }

        public static IImage<Y> Downsample<Y>(IImage<Y> img, Func<int, int, Y[], IImage<Y>> func)
            where Y : struct, IEquatable<Y>
        {
            int width = img.Width;
            int newWidth = width >> 1;
            int newHeight = img.Height >> 1;
            int remnant = width % 2;

            Y[] newData = new Y[newWidth * newHeight];
            for (int i = 0, j = 0, y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    newData[i++] = img[j];
                    j += 2;
                }
                j += width + remnant;
            }

            return func(newWidth, newHeight, newData);
        }

        #endregion

        #region Upsample

        public static IImage<Y> UpsampleRows<Y>(IImage<Y> img, Func<int, int, Y[], IImage<Y>> func)
            where Y : struct, IEquatable<Y>
        {
            Y[] newData = new Y[(img.Height << 1) * img.Width];

            for (int y = 0, i = 0, j = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    newData[i++] = img[j++];
                }

                i += img.Width;
            }

            return func(img.Width, img.Height << 1, newData);
        }

        public static IImage<Y> UpsampleCols<Y>(IImage<Y> img, Func<int, int, Y[], IImage<Y>> func)
            where Y : struct, IEquatable<Y>
        {
            Y[] newData = new Y[(img.Width << 1) * img.Height];

            for (int y = 0, i = 0, j = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    newData[i] = img.Data[j++];
                    i += 2;
                }
            }

            return func(img.Width << 1, img.Height, newData);
        }

        public static IImage<Y> Upsample<Y>(IImage<Y> img, Func<int, int, Y[], IImage<Y>> func)
            where Y : struct, IEquatable<Y>
        {
            Y[] newData = new Y[(img.Width * 2) * (img.Height * 2)];

            lock (img.SyncRoot)
            {
                for (int y = 0, i = 0, j = 0; y < img.Height; y++)
                {
                    for (int x = 0; x < img.Width; x++)
                    {
                        newData[i] = img[j++];
                        i += 2;
                    }

                    i += img.Width << 1;
                }
            }

            return func(img.Width, img.Height, newData);
        }

        #endregion

        #region Crop

        public static IImage<Y> Crop<Y>(IImage<Y> img, Func<int, int, Y[], IImage<Y>> func, int x1, int y1, int width, int height)
            where Y : struct, IEquatable<Y>
        {
            if (x1 < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(x1), "Negative proportion for Crop detected");
            }

            if (y1 < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(y1), "Negative proportion for Crop detected");
            }

            if (width < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Negative proportion for Crop detected");
            }

            if (height < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Negative proportion for Crop detected");
            }

            Y[] newData = new Y[width * height];

            int diff = img.Width - width - x1;

            for (int y = y1, i = img.Width * y1, j = 0; y < y1 + height; y++)
            {
                i += x1;

                for (int x = x1; x < width + x1; x++)
                {
                    newData[j++] = img[i++];
                }

                i += diff;
            }

            return func(width, height, newData);
        }

        public static IImage<Y> Crop<Y>(IImage<Y> img, Func<int, int, Y[], IImage<Y>> func, Rectangle rect)
            where Y : struct, IEquatable<Y>
        {
            return ImageBase.Crop(img, func, rect.X, rect.Y, rect.Width, rect.Height);
        }

        #endregion

        #region Flip

        public static IImage<Y> FlipX<Y>(IImage<Y> img, Func<int, int, Y[], IImage<Y>> func)
            where Y : struct, IEquatable<Y>
        {
            Y[] newData = new Y[img.Length];
            int width = img.Width;
            int height = img.Height;

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    int x = h * width + w;
                    int x1 = h * width + width - w - 1;

                    newData[x] = img[x1];
                }
            }

            return func(width, height, newData);
        }

        public static IImage<Y> FlipY<Y>(IImage<Y> img, Func<int, int, Y[], IImage<Y>> func)
            where Y : struct, IEquatable<Y>
        {
            Y[] sourceArray = img.Data;
            Y[] newData = new Y[img.Length];
            int width = img.Width;
            int height = img.Height;

            for (int y = 0; y < sourceArray.Length; y += width)
            {
                int destinationIndex = sourceArray.Length - y - width;
                Array.Copy(sourceArray, y, newData, destinationIndex, width);
            }

            return func(width, height, newData);
        }

        public static IImage<Y> FlipXY<Y>(IImage<Y> img, Func<int, int, Y[], IImage<Y>> func)
            where Y : struct, IEquatable<Y>
        {
            Y[] newData = new Y[img.Length];

            for (int i = img.Length - 1, i2 = 0; i >= 0; i--, i2++)
            {
                newData[i2] = img[i];
            }

            return func(img.Width, img.Height, newData);
        }

        #endregion

        public static IImage<Y> Pad<Y>(IImage<Y> img, Func<int, int, Y[], IImage<Y>> func, int width, int height)
            where Y : struct, IEquatable<Y>
        {
            if (width < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Padding Height and Width Must Be Positive");
            }

            if (height < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Padding Height and Width Must Be Positive");
            }

            Y[] newData = new Y[height * width];

            int diff = width - img.Width;

            for (int y = 0, i = 0, j = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    newData[j++] = img[i++];
                }

                j += diff;
            }

            return func(width, height, newData);
        }

        public static Y[] Transpose<Y>(Y[] img, int width, int height)
            where Y : struct, IEquatable<Y>
        {
            int m = width;
            int n = height;

            Y[] newData = new Y[img.Length];
            Array.Copy(img, newData, newData.Length);

            if (n == m)
            {
                // Square Matricies Only
                for (int i = 0, _in = 0; i < n - 1; i++, _in += n)
                {
                    for (int j = i + 1, _nj = n * j; j < n; j++, _nj += n)
                    {
                        Y temp = newData[_in + j];
                        newData[_in + j] = newData[i + _nj];
                        newData[i + _nj] = temp;
                    }
                }
            }
            else
            {
                // Non Square Matricies
                int start, next, i;
                Y tmp;

                for (start = 0; start <= m * n - 1; start++)
                {
                    next = start;
                    i = 0;
                    do
                    {
                        i++;
                        next = (next % n) * m + next / n;
                    }
                    while (next > start);

                    if (next < start || i == 1) continue;

                    tmp = newData[next = start];
                    do
                    {
                        i = (next % n) * m + next / n;
                        newData[next] = (i == start) ? tmp : newData[i];
                        next = i;
                    } while (next > start);
                }
            }

            return newData;
        }

        public static IImage<Y> Transpose<Y>(IImage<Y> img, Func<int, int, Y[], IImage<Y>> func)
            where Y : struct, IEquatable<Y>
        {
            int m = img.Width;
            int n = img.Height;

            Y[] newData = new Y[img.Length];
            Array.Copy(img.Data, newData, newData.Length);

            if (n == m)
            {
                // Square Matricies Only
                for (int i = 0, _in = 0; i < n - 1; i++, _in += n)
                {
                    for (int j = i + 1, _nj = n * j; j < n; j++, _nj += n)
                    {
                        Y temp = newData[_in + j];
                        newData[_in + j] = newData[i + _nj];
                        newData[i + _nj] = temp;
                    }
                }
            }
            else
            {
                // Non Square Matricies
                int start, next, i;
                Y tmp;

                for (start = 0; start <= m * n - 1; start++)
                {
                    next = start;
                    i = 0;
                    do
                    {
                        i++;
                        next = (next % n) * m + next / n;
                    } 
                    while (next > start);

                    if (next < start || i == 1) continue;

                    tmp = newData[next = start];
                    do
                    {
                        i = (next % n) * m + next / n;
                        newData[next] = (i == start) ? tmp : newData[i];
                        next = i;
                    } while (next > start);
                }
            }

            return func(img.Height, img.Width, newData);
        }

        public static byte[] ToImage<T>(this IImage<T> img)
            where T : struct, IEquatable<T>
        {
            return ToImage(img, ImageFormat.Png);
        }

        public static byte[] ToImage<T>(this IImage<T> img, ImageFormat format)
            where T : struct, IEquatable<T>
        {
            using (Bitmap image = img.ToBitmap())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, format);
                    return ms.ToArray();
                }
            }
        }

        public static Bitmap ToBitmap<T>(this IImage<T> img)
            where T : struct, IEquatable<T>
        {
            byte[] rgba = img.ToBGRA();
            int width = img.Width;
            int height = img.Height;

            Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            BitmapData binaryImg = image.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite,
                image.PixelFormat);

            Marshal.Copy(rgba, 0, binaryImg.Scan0, rgba.Length);

            image.UnlockBits(binaryImg);

            return image;
        }

        public static void WriteImage<T>(this IImage<T> img, string fullPath)
            where T : struct, IEquatable<T>
        {
            WriteImage(img, fullPath, ImageFormat.Png);
        }

        public static void WriteImage<T>(this IImage<T> img, string imagePath, string dirPath)
            where T : struct, IEquatable<T>
        {
            WriteImage(img, Path.Combine(imagePath, dirPath));
        }

        public static void WriteImage<T>(this IImage<T> img, string fullPath, ImageFormat format)
            where T : struct, IEquatable<T>
        {
            byte[] data = img.ToImage(format);

            // Check that full path end with proper extension
            String extension = "." + format.ToString().ToLower(CultureInfo.CurrentCulture);
            fullPath = fullPath.EndsWith(extension, StringComparison.CurrentCultureIgnoreCase) ?
                fullPath : fullPath + extension;

            // Write
            File.WriteAllBytes(fullPath, data);
        }

        #region Map

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IImage<Y> MapBase<Y>(IImage<Y> img, Func<int, Y> func, bool parallel)
            where Y : struct, IEquatable<Y>
        {
            if (parallel)
            {
                Parallel.For(0, img.Length, i => img[i] = func(i));
            }
            else
            {
                for (int i = 0; i < img.Length; i++)
                {
                    img[i] = func(i);
                }
            }

            return img;
        }

        public static IImage<Y> MapIndex<Y>(this IImage<Y> img, Func<int, Y> func)
            where Y : struct, IEquatable<Y>
        {
            return MapBase(img, func, true);
        }

        public static IImage<Y> MapIndex<Y>(this IImage<Y> img, Func<int, Y> func, bool parallel)
            where Y : struct, IEquatable<Y>
        {
            return MapBase(img, func, parallel);
        }

        public static IImage<Y> MapValue<Y>(this IImage<Y> img, Func<Y, Y> func)
            where Y : struct, IEquatable<Y>
        {
            return MapBase(img, i => func(img[i]), true);
        }

        public static IImage<Y> MapValue<Y>(this IImage<Y> img, Func<Y, Y> func, bool parallel)
            where Y : struct, IEquatable<Y>
        {
            return MapBase(img, i => func(img[i]), parallel);
        }

        public static IImage<Y> MapLocation<Y>(this IImage<Y> img, Func<int, int, Y> func)
            where Y : struct, IEquatable<Y>
        {
            return MapBase(img, i => img[i] = func(i % img.Width, i / img.Height), true);
        }

        public static IImage<Y> MapLocation<Y>(this IImage<Y> img, Func<int, int, Y> func, bool parallel)
            where Y : struct, IEquatable<Y>
        {
            return MapBase(img, i => img[i] = func(i % img.Width, i / img.Height), parallel);
        }

        public static IImage<Y> MapValueIndex<Y>(this IImage<Y> img, Func<int, Y, Y> func)
            where Y : struct, IEquatable<Y>
        {
            return MapBase(img, i => img[i] = func(i, img[i]), true);
        }

        public static IImage<Y> MapValueIndex<Y>(this IImage<Y> img, Func<int, Y, Y> func, bool parallel)
            where Y : struct, IEquatable<Y>
        {
            return MapBase(img, i => img[i] = func(i, img[i]), parallel);
        }

        public static IImage<Y> MapLocationValue<Y>(this IImage<Y> img, Func<int, int, Y, Y> func)
            where Y : struct, IEquatable<Y>
        {
            return MapBase(img, i => img[i] = func(i % img.Width, i / img.Height, img[i]), true);
        }

        public static IImage<Y> MapLocationValue<Y>(this IImage<Y> img, Func<int, int, Y, Y> func, bool parallel)
            where Y : struct, IEquatable<Y>
        {
            return MapBase(img, i => img[i] = func(i % img.Width, i / img.Height, img[i]), parallel);
        }

        #endregion

        public static byte[] ToRGBA<T>(IImage<T> img)
            where T : struct, IEquatable<T>
        {
            byte[] rgba = new byte[img.Length * 4];
            img.ToIndexedBgra((i, pixel) =>
            {
                int j = i * 4;
                rgba[j++] = pixel.B;
                rgba[j++] = pixel.G;
                rgba[j++] = pixel.R;
                rgba[j] = pixel.A;
            });

            return rgba;
        }

        public static byte[] ToRGB<T>(IImage<T> img)
            where T : struct, IEquatable<T>
        {
            byte[] rgb = new byte[img.Length * 3];
            img.ToIndexedBgra((i, pixel) =>
            {
                int j = i * 3;
                rgb[j++] = pixel.B;
                rgb[j++] = pixel.G;
                rgb[j] = pixel.R;
            });

            return rgb;
        }

        public static BGRA[] ToPixelColor<T>(IImage<T> img)
            where T : struct, IEquatable<T>
        {
            BGRA[] rgba = new BGRA[img.Length];
            img.ToIndexedBgra((i, pixel) => { rgba[i] = pixel; });
            return rgba;
        }

        public static void CopyTo<T>(IImage<T> img, Array array, int arrayIndex)
            where T : struct, IEquatable<T>
        {
            for (int i = 0, j = arrayIndex; i < img.Length; i++, j++)
            {
                array.SetValue(img[i], j);
            }
        }

        public static IEnumerator<T> GetEnumerator<T>(IImage<T> img)
            where T : struct, IEquatable<T>
        {
            for (int i = 0; i < img.Length; i++)
            {
                yield return img[i];
            }
        }

        public static bool Contains<T>(IImage<T> img, T obj)
            where T : struct, IEquatable<T>
        {
            return img.Contains(obj);
        }
    }
}
