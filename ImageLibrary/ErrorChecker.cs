using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ImageLibrary
{
    internal static class ErrorChecker
    {
        internal static void ImagesAny<T>(IEnumerable<IImage<T>> images)
            where T : struct, IEquatable<T>
        {
            if (images == null)
            {
                throw new ArgumentNullException(nameof(images), "Passed in images are null");
            }

            if (!images.Any())
            {
                throw new ArgumentException("No Images in Enumeration", nameof(images));
            }
        }

        internal static void CheckSavePath(string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                throw new ArgumentException("The specified directory does not exist", nameof(path));
            }
        }

        internal static void CheckImagePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("The specified path is null or empty", nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new ArgumentException("The specified path does not exist", nameof(path));
            }
        }

        internal static T[] CheckWidthHeightData<T>(int width, int height, IEnumerable<T> data)
        {
            CheckWidthHeight(width, height);

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Data enumeration is null");
            }

            T[] dataArray = data.ToArray();

            if (dataArray.Length != width * height)
            {
                throw new ArgumentException("Data array size does not match specified width and height", nameof(data));
            }

            return dataArray;
        }

        internal static void CheckWidthHeightData<T>(int width, int height, T[] data)
        {
            CheckWidthHeight(width, height);

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Data array is null");
            }

            if (data.Length != width * height)
            {
                throw new ArgumentException("Data array size does not match specified width and height", nameof(data));
            }
        }

        internal static void CheckWidthHeight(int width, int height)
        {
            // (-inf, -1]
            if (width < 0)
            {
                throw new ArgumentException("Width must be positive", nameof(width));
            }

            // (-inf, -1]
            if (height < 0)
            {
                throw new ArgumentException("Height must be positive", nameof(height));
            }

            // 0
            if (width == 0)
            {
                throw new ArgumentException("Width cannot be zero", nameof(width));
            }

            // 0
            if (height == 0)
            {
                throw new ArgumentException("Height cannot be zero", nameof(height));
            }
        }
    }
}
