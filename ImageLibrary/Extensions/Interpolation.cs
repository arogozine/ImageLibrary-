using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageLibrary.Extensions
{
    /// <summary>
    /// Interpolation Method
    /// </summary>
    public enum InterpMethod : int
    {
        /// <summary>
        /// Gets the nearest pixel
        /// </summary>
        NearestNeighbour,

        Bilinear,

        Lanczos3,

        Lanczos4,

        Lanczos6,

        Lanczos12,
    }

    /// <summary>
    /// Interpolation
    /// Adapted from,
    ///     Wikipedia.org
    ///     Stack Overflow Questions
    ///     Image Resampler (Google Code)
    /// </summary>
    public static class Interpolation
    {

        public static RGB Interpolate(this IImage<RGB> img, InterpMethod method, double x, double y)
        {
            switch (method)
            {
                case InterpMethod.NearestNeighbour:
                    return InterpolateNearest(img, x, y);

                case InterpMethod.Bilinear:
                    return InteroplateBilinear(img, x, y, (a, b, ratio) => a * (1.0 - ratio) + b * ratio);
                default:
                    throw new NotImplementedException();
            }
        }

        public static IImage<double> Resize(this IImage<double> img, InterpMethod method, int newHeight, int newWidth)
        {
            double heightDiff = (double)img.Height / (double)newHeight;
            double widthDiff = (double)img.Width / (double)newWidth;

            var resizedImage = new Image(newWidth, newHeight);

            Parallel.For(0, newHeight, y =>
            {
                var relativeY = heightDiff * y;
                double relativeX = 0.0;
                for (int x = 0; x < newWidth; x++)
                {
                    relativeX += widthDiff;
                    resizedImage[y, x] = img.Interpolate(method, relativeX, relativeY);
                }
            });

            return resizedImage;
        }

        public static double Interpolate(this IImage<double> img, InterpMethod method, double x, double y)
        {
            switch (method)
            {
                case InterpMethod.NearestNeighbour:
                    return InterpolateNearest(img, x, y);
                case InterpMethod.Bilinear:
                    return InteroplateBilinear(img, x, y, (a, b, ratio) => a * (1.0 - ratio) + b * ratio);
                case InterpMethod.Lanczos3:
                    return LancInternal(x, y, img, 3);
                case InterpMethod.Lanczos4:
                    return LancInternal(x, y, img, 4);
                case InterpMethod.Lanczos6:
                    return LancInternal(x, y, img, 6);
                case InterpMethod.Lanczos12:
                    return LancInternal(x, y, img, 12);
                default:
                    throw new NotImplementedException();
            }
        }

        private static T InterpolateNearest<T>(IImage<T> img, double x, double y)
            where T : struct, IEquatable<T>
        {
            int i = (int)Filters.NearestNeighbor(x);
            int j = (int)Filters.NearestNeighbor(y);

            // Edge Cases (X)
            if (i >= img.Width - 1)
            {
                i = img.Width - 1;
            }

            // Edge Cases (Y)
            if (j >= img.Height - 1)
            {
                j = img.Height - 1;
            }

            return img[j, i];
        }

        private static T InteroplateBilinear<T>(IImage<T> img, double x, double y, Func<T, T, double, T> lerpf)
            where T : struct, IEquatable<T>
        {
            int width = img.Width;
            int height = img.Height;

            // For Edges
            // width is width - 1 zero indexed [ we want another -1 buffer ]
            x = x > width - 2 ? width - 2 : (x < 0.0 ? 0.0 : x);
            y = y > height - 2 ? height - 2 : (y < 0.0 ? 0.0 : y);

            // http://en.wikipedia.org/wiki/Bilinear_filtering
            int int_x = (int)x;
            int int_y = (int)y;

            double x_diff = x - Math.Floor(x);
            double y_diff = y - Math.Floor(y);

            // Pixels,
            // 00 01
            // 10 11
            int f00i = int_y * width + int_x;
            int f01i = f00i + 1;
            int f10i = f00i + width;
            int f11i = f10i + 1;

            // Get those Pixels
            T f00 = img[f00i];
            T f01 = img[f01i];
            T f10 = img[f10i];
            T f11 = img[f11i];

            // Stackoverflow - Wiki implementation did not work
            T left = lerpf(f00, f10, y_diff);
            T right = lerpf(f01, f11, y_diff);
            return lerpf(left, right, x_diff);
        }

        // http://code.google.com/p/imageresampler/

        public static double LancInternal(double x, double y, IImage<double> img, int count)
        {
            // http://en.wikipedia.org/wiki/Lanczos_resampling
            Func<double, double> lanczosFilter = Filters.Lanczos(count);
            int width = img.Width;
            int height = img.Height;
            int floor_x = (int)x;
            int floor_y = (int)y;

            // SUM
            double value = 0.0;
            for (int i = floor_x - count + 1; i <= floor_x + count; i++)
            {
                // Out of Bounds; i in [0 .. width - 1]
                if (i < 0 || i >= width)
                {
                    continue;
                }

                for (int j = floor_y - count + 1; j <= floor_y + count; j++)
                {
                    // Out of Bounds; j in [0 .. height - 1]
                    if (j < 0 || j >= height)
                    {
                        continue;
                    }

                    value += img[j, i] * lanczosFilter(x - i) * lanczosFilter(y - j);
                }
            }

            return value;
        }
    }
}
