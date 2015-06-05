namespace ImageLibrary.Extensions
{
    /// <summary>
    /// Values taken from,
    /// http://www.codeproject.com/Articles/6534/Convolution-of-Bitmaps
    /// by Fred Ackers
    /// </summary>
    public static class Convolution
    {
        public static IImage<double> GaussianBlur(this IImage<double> img)
        {
            return img.Convolve(new double[][]
            {
                new[] { 0.045, 0.122, 0.045 },
                new[] { 0.122, 0.332, 0.122 },
                new[] { 0.045, 0.122, 0.045 }
            });
        }

        public static IImage<double> GaussianBlur2(this IImage<double> img)
        {
            return img.Convolve(new double[][]
            {
                new[] { 1.0, 2, 1 },
                new[] { 2.0, 4, 2 },
                new[] { 1.0, 2, 1 }
            });
        }

        public static IImage<double> GaussianBlur3(this IImage<double> img)
        {
            return img.Convolve(new double[][]
            {
                new[] { 0.0, 1, 0 },
                new[] { 1.0, 1, 1 },
                new[] { 0.0, 1, 0 }
            });
        }

        public static IImage<double> Unsharpen(this IImage<double> img)
        {
            return img.Convolve(new double[][]
            {
                new[] { -1.0, -1, -1 },
                new[] { -1.0, +9, -1 },
                new[] { -1.0, -1, -1 }
            });
        }

        public static IImage<double> Sharpness(this IImage<double> img)
        {
            return img.Convolve(new double[][]
            {
                new[] { +0.0, -1, +0 },
                new[] { -1.0, +5, -1 },
                new[] { +0.0, -1, +0 },
            });
        }

        public static IImage<double> Sharpen(this IImage<double> img)
        {
            return img.Convolve(new double[][]
            {
                new[] { -1.0, -1, -1 },
                new[] { -1.0, 16, -1 },
                new[] { -1.0, -1, -1 }
            });
        }

        public static IImage<double> EdgeDetect(this IImage<double> img)
        {
            return img.Convolve(new double[][]
            {
                new[] { -0.125, -0.125, -0.125 },
                new[] { -0.125, +1.000, -0.125 },
                new[] { -0.125, -0.125, -0.125 }
            });
        }

        public static IImage<double> EdgeDetect2(this IImage<double> img)
        {
            return img.Convolve(new double[][]
            {
                new[] { -1.0, -1, -1, },
                new[] { -1.0, +8, -1, },
                new[] { -1.0, -1, -1, },
            });
        }

        public static IImage<double> EdgeDetect3(this IImage<double> img)
        {
            return img.Convolve(new double[][]
            {
                new[] { -5.0, 0, 0, },
                new[] { +0.0, 0, 0, },
                new[] { +0.0, 0, 5, },
            });
        }

        public static IImage<double> EdgeDetect4(this IImage<double> img)
        {
            return img.Convolve(new double[][]
            {
                new[] { -1.0, -1, -1, },
                new[] { +0.0, +0, +0, },
                new[] { +1.0, +1, +1, },
            });
        }

        public static IImage<double> EdgeDetect5(this IImage<double> img)
        {
            return img.Convolve(new double[][]
            {
                new[] { -1.0, -1, -1 },
                new[] { +2.0, +2, +2 },
                new[] { -1.0, -1, -1 },
            });
        }

        public static IImage<double> EdgeDetect6(this IImage<double> img)
        {
            return img.Convolve(new double[][]
            {
                new[] { -5.0, -5, -5 },
                new[] { -5.0, 39, -5 },
                new[] { -5.0, -5, -5 },
            });
        }

        public static IImage<double> SobelHorizontal(this IImage<double> img)
        {
            return img.Convolve(new double[][]
            {
                new[] { +1.0, +2, +1 },
                new[] { +0.0, +0, +0 },
                new[] { -1.0, -2, -1 },
            });
        }

        public static IImage<double> SobelVertical(this IImage<double> img)
        {
            return img.Convolve(new double[][]
            {
                new[] { 1.0, 0, -1 },
                new[] { 2.0, 0, -2 },
                new[] { 1.0, 0, -1 },
            });
        }

        public static IImage<double> PrevitHorizontal(this IImage<double> img)
        {
            return img.Convolve(new double[][]
            {
                new[] { +1.0, +1, +1 },
                new[] { +0.0, +0, +0 },
                new[] { -1.0, -1, -1 }
            });
        }

        public static IImage<double> PrevitVertical(this IImage<double> img)
        {
            return img.Convolve(new double[][]
            {
                new[] { 1.0, 0, -1 },
                new[] { 1.0, 0, -1 },
                new[] { 1.0, 0, -1 }
            });
        }

        public static IImage<double> BoxBlur(this IImage<double> img)
        {
            const double oneNinth = 1f / 9f;

            return img.Convolve(new double[][]
            {
                new[] { oneNinth, oneNinth, oneNinth },
                new[] { oneNinth, oneNinth, oneNinth },
                new[] { oneNinth, oneNinth, oneNinth }
            });
        }

        public static IImage<double> TriangleBlur(this IImage<double> img)
        {
            return img.Convolve(new double[][]
            {
                new[] { 0.0625, 0.125, 0.0625 },
                new[] { 0.1250, 0.250, 0.1250 },
                new[] { 0.0625, 0.125, 0.0625 }
            });
        }
    }
}
