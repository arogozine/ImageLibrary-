using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageLibrary.Extensions
{
    /// <summary>
    /// Kernel convolution usually requires values from pixels outside of the image boundaries. 
    /// There are a variety of methods for handling image edges.
    /// </summary>
    public enum EdgeHandling
    {
        /// <summary>
        /// The nearest border pixels are conceptually extended as far as necessary to provide values for the convolution. 
        /// Corner pixels are extended in 90° wedges.
        /// Other edge pixels are extended in lines.
        /// </summary>
        Extend,

        /// <summary>
        /// The image is conceptually wrapped (or tiled) and values are taken from the opposite edge or corner.
        /// </summary>
        Wrap,

        /// <summary>
        /// Any pixel in the output image which would require values from beyond the edge is skipped.
        /// This method can result in the output image being slightly smaller, with the edges having been cropped.
        /// </summary>
        Crop
    }

    public static class ImageExtensions
    {
        // https://code.google.com/p/2dimagefilter/wiki/ImageScaling

        public static IImage<BGRA> PixelUpscale(this IImage<BGRA> img)
        {
            var upscaled = new BgraImage(img.Width << 1, img.Height << 1, new BGRA[img.Length << 2]);

            for (int x = 0, xu = 0; x < img.Width; x++, xu+=2)
            {
                for (int y = 0, yu = 0; y < img.Height; y++, yu+=2)
                {
                    //   A    --\ 1 2
                    // C P B  --/ 3 4
                    //   D
                    BGRA P = img[y, x];
                    BGRA A = img[y - 1, x];
                    BGRA B = img[y, x + 1];
                    BGRA D = img[y + 1, x];
                    BGRA C = img[y, x - 1];

                    // 1=P; 2=P; 3=P; 4=P;
                    BGRA _1 = P, _2 = P, _3 = P, _4 = P;
                    // IF C==A AND C!=D AND A!=B => 1=A
                    if (C == A && C != D && A != B) _1 = A;

                    // IF A==B AND A!=C AND B!=D => 2=B
                    if (A == B && A != C && B != D) _2 = B;

                    // IF B==D AND B!=A AND D!=C => 4=D
                    if (B == D && B != A && D != C) _4 = D;

                    // IF D==C AND D!=B AND C!=A => 3=C
                    if (D == C && D != B && C != A) _3 = C;

                    upscaled[yu, xu + 0] = _1;
                    upscaled[yu, xu + 1] = _2;
                    ++yu;
                    upscaled[yu, xu + 0] = _3;
                    upscaled[yu, xu + 1] = _4;
                }
            }

            return null;
        }

        public static IImage<double> Invert(this IImage<double> img)
        {
            return img
                .Copy()
                .Normalize()
                .MapValue(val => 1.0 - val);
        }

        public static IImage<T> Insert<T>(this IImage<T> img, IImage<T> insertImg, int x, int y)
            where T : struct, IEquatable<T>
        {
            img = img.Copy();
            
            // No Op Insert
            if (img.Width <= x || img.Height <= y)
                return img;

            int x_diff = img.Width - x;
            int y_diff = img.Height - y;

            int x_max = Math.Min(x_diff, insertImg.Width);
            int y_max = Math.Min(y_diff, insertImg.Height);

            for (int col = x, col_i = 0; col_i < x_max; col++, col_i++)
            {
                for (int row = y, row_i = 0; row_i < y_max ; row++, row_i++)
                {
                    img[col, row] = insertImg[col_i, row_i];
                }
            }

            return img;
        }

        #region Matrix

        public static Matrix<Double> AsMatrix(this IImage<double> img)
        {
            return Matrix<double>
                .Build
                .Dense(img.Cols, img.Rows, img.Data);
        }

        public static Matrix<Complex> AsMatrix(this IImage<Complex> img)
        {
            return Matrix<Complex>
                .Build
                .Dense(img.Cols, img.Rows, img.Data);
        }

        public static IImage<RGB> Multiply(this IImage<RGB> a, IImage<RGB> b)
        {
            var redPart = a.RedPart().Multiply(b.RedPart());
            var greenPart = a.GreenPart().Multiply(b.GreenPart());
            var bluePart = a.BluePart().Multiply(b.BluePart());

            return ImageConversion.ToRgb(redPart, greenPart, bluePart);
        }

        public static IImage<double> Multiply(this IImage<double> a, IImage<double> b)
        {
            var denseMatrixA = Matrix<double>.Build
                .Dense(a.Cols, a.Rows, a.Data);

            var denseMatrixB = Matrix<double>.Build
                .Dense(b.Cols, b.Rows, b.Data);

            var matrixResult = denseMatrixA.Multiply(denseMatrixB);

            return new Image(matrixResult.ColumnCount, matrixResult.RowCount, matrixResult.ToColumnWiseArray());
        }

        public static IImage<Complex> Multiply(this IImage<Complex> a, IImage<Complex> b)
        {
            var denseMatrixA = Matrix<Complex>.Build
                .Dense(a.Cols, a.Rows, a.Data);

            var denseMatrixB = Matrix<Complex>.Build
                .Dense(b.Cols, b.Rows, b.Data);

            var matrixResult = denseMatrixA.Multiply(denseMatrixB);

            return new ComplexImage(matrixResult.ColumnCount, matrixResult.RowCount, matrixResult.ToColumnWiseArray());
        }

        #endregion

        /// <summary>
        /// Produces a new rotated image.
        /// </summary>
        /// <param name="img">Old Image</param>
        /// <param name="degree">Degree to rotate by</param>
        /// <returns>Rotated image</returns>
        public static IImage<Double> Rotate(this IImage<Double> img, double degree)
        {
            int width = img.Width;
            int height = img.Height;

            // Middle
            double center_x = width / 2.0;
            double center_y = height / 2.0;

            // 2*PI / 360 * degree 
            const double twoPi360 = Math.PI * 2 / 360.0;
            double sin_pheta = Math.Sin(twoPi360 * degree);
            double cos_pheta = Math.Cos(twoPi360 * degree);

            // x' = x cos f - y sin f 
            Func<double, double, double> rotx = (x, y) =>
                ((x - center_x) * cos_pheta) -
                ((center_y - y) * sin_pheta)
                + center_x;

            // y' = y cos f + x sin f
            Func<double, double, double> roty = (x, y) =>
                center_y -
                ((center_y - y) * cos_pheta) +
                ((x - center_x) * sin_pheta);

            return new Image(width, height)
                .MapLocation((int x, int y) =>
                {
                    var _x = rotx(x, y);
                    var _y = roty(x, y);

                    // No negative values please
                    _x = _x < 0.0 ? 0.0 : _x;
                    _y = _y < 0.0 ? 0.0 : _y;

                    if ((_x < 0 || _x > width) && (_y < 0 || _y > height))
                    {
                        return 0;
                    }

                    return img.Interpolate(InterpMethod.Bilinear, _x, _y);
                });
        }

        /// <summary>
        /// Creates a histogram of an image.
        /// </summary>
        /// <param name="image">Image input</param>
        /// <returns>Histogram</returns>
        public static int[] Histogram(this IImage<double> image)
        {
            // double 0.3 -> int 0
            int[] floored = image.Data
                .Select(x => (int)Math.Floor(x))
                .ToArray();

            // will hold unique integer counts of values
            int[] histogram = new int[Math.Max(floored.Max() + 1, 256)];

            for (int i = 0; i < floored.Length; i++)
            {
                int pixelValue = floored[i];
                histogram[pixelValue]++;
            }

            return histogram;
        }

        /// <summary>
        /// Cumulative Distribution Function
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static double[] CumulativeDistribution(this IImage<double> image)
        {
            // one = 255.0 / (n*m)
            double one = 255.0 / image.Length;

            double val = 0;
            return image
                .Histogram()
                .Select(x => { val += one * x; return val; })
                .ToArray();
        }

        /// <summary>
        /// Helps contrast
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static IImage<double> Equalize(this IImage<double> image)
        {
            double[] cdf = image.CumulativeDistribution();

            return image
                .Copy()
                .MapValue(x => cdf[(int)Math.Floor(x)]);
        }

        public static IImage<RGB> MapAcross(this IImage<RGB> image, Func<double, double> func)
        {
            return image.MapValue((RGB x) => new RGB(func(x.R), func(x.G), func(x.B)), true);
        }

        public static IImage<RGB> MapAcross(this IImage<RGB> image, Func<double, double> func, bool parallel)
        {
            return image.MapValue((RGB x) => new RGB(func(x.R), func(x.G), func(x.B)), parallel);
        }

        /// <summary>
        /// Noramlizes Floating Point Image to [0 - 255] range for easy convesion to byte
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static IImage<double> Normalize256(this IImage<double> image)
        {
            return image.Normalize().MapValue(x => x * 255.0);
        }

        public static IImage<RGB> Normalize256(this IImage<RGB> image)
        {
            return image.Normalize().MapValue(x => 255.0 * x);
        }

        public static IImage<double> Normalize(this IImage<double> image)
        {
            double max = double.MinValue;
            double min = double.MaxValue;

            foreach (double a in image.Data)
            {
                max = a > max ? a : max;
                min = a < min ? a : min;
            }

            double scale = max - min;

            return image
                .Copy()
                .MapValue(x => (x - min) / scale);
        }

        public static IImage<RGB> Normalize(this IImage<RGB> image)
        {
            Double max = double.MinValue;
            Double min = double.MaxValue;

            foreach (RGB a in image.Data)
            {
                max = a.B > max ? a.B : max;
                max = a.G > max ? a.G : max;
                max = a.R > max ? a.R : max;

                min = a.B < min ? a.B : min;
                min = a.G < min ? a.G : min;
                min = a.R < min ? a.R : min;
            }

            double scale = max - min;

            return image
                .MapAcross(x => (x - min) / scale);
        }

        #region Erode / Dilate / Open / Close

        public static IImage<double> Erode(this IImage<double> img)
        {
            return Erode(img,
                        new double[][] 
                        { 
                            new [] { 1.0, 1.0 },
                            new [] { 1.0, 1.0 }
                        });
        }

        public static IImage<Double> Erode(this IImage<double> img, double[][] conv)
        {
            return img
                .Copy()
                .Convolve(conv)
                .MapValue((double x) => x == 4 ? 1 : 0);
        }

        public static IImage<Double> Dilate(this IImage<Double> img)
        {
            return Dilate(img, new double[][]
                            { 
                                new [] { 1.0, 1.0 },
                                new [] { 1.0, 1.0 }
                            });
        }

        public static IImage<Double> Dilate(this IImage<Double> img, double[][] conv)
        {
            return img
                .Copy()
                .Convolve(conv)
                .MapValue((double x) => x > 0 ? 1 : 0);
        }

        public static IImage<Double> Open(this IImage<Double> img)
        {
            return Open(img, new double[][] 
                { 
                    new [] { 1.0, 1.0 },
                    new [] { 1.0, 1.0 }
                });
        }

        public static IImage<Double> Open(this IImage<Double> img, double[][] conv)
        {
            return img
                .Copy()
                .Convolve(conv)
                .MapValue((double x) => x == 4 ? 1 : 0)
                .Convolve(conv)
                .MapValue((double x) => x > 0 ? 1 : 0);
        }

        public static IImage<Double> Close(this IImage<Double> img)
        {
            return Close(img, new double[][] 
                { 
                    new [] { 1.0, 1.0 },
                    new [] { 1.0, 1.0 }
                });
        }

        public static IImage<Double> Close(this IImage<Double> img, double[][] conv)
        {
            return img
                .Copy()
                .Convolve(conv)
                .MapValue((double x) => x > 0 ? 1 : 0)
                .Convolve(conv)
                .MapValue((double x) => x == 4 ? 1 : 0);
        }

        #endregion

        #region MedianFilter

        /// <summary>
        /// Returns a real image with the same dimensions where each pixel (i, j)
        /// is replaced by a pixel with a median value in the neighborhood size
        /// m times n centered on (i, j)
        /// </summary>
        /// <param name="img">Real IImage<Double></param>
        /// <param name="y">Rows</param>
        /// <param name="x">Cols</param>
        /// <returns></returns>
        public static IImage<double> MedianFilter(this IImage<double> img, int y, int x)
        {
            int dy = y / 2;
            int dx = x / 2;

            int n = x * y; ;

            if (dy < 0 || dx < 0 || n > 81)
            {
                throw new ArgumentException("Illegal window size");
            }

            double[] a = new double[n];
            var sp4 = new Image(img.Width, img.Height);
            double[] data4 = sp4.Data;

            for (int i0 = 0; i0 < img.Height; i0++)
            {
                for (int j0 = 0; j0 < img.Width; j0++)
                {
                    int k = 0;
                    for (int i1 = -dy; i1 <= dy; i1++)
                    {
                        if (i0 + i1 >= 0 && i0 + i1 < img.Height)
                        {
                            for (int j1 = -dx; j1 <= dx; j1++)
                            {
                                if (j0 + j1 >= 0 && j0 + j1 < img.Width)
                                {
                                    a[k++] = img.Data[(i0 + i1) * img.Width + j0 + j1];
                                }
                            }
                        }
                    }
                    QuickSort(a, k);
                    data4[i0 * img.Width + j0] = a[k / 2];
                }
            }
            return sp4;
        }

        /// <summary>
        ///  Adapted from an implementation of Quicksort by Darel Rex Finley
        /// </summary>
        /// <param name="a"></param>
        /// <param name="n"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void QuickSort(double[] a, int n)
        {
            int i, l, r;
            double pivot;
            double[] beg = new double[81];
            double[] end = new double[81];

            i = 0;
            // beg[0] = 0;
            end[0] = n;

            while (i >= 0)
            {
                l = (int)beg[i];
                r = (int)(end[i] - 1);
                if (l < r)
                {
                    pivot = a[l];
                    while (l < r)
                    {
                        while (a[r] >= pivot && l < r)
                            r--;
                        if (l < r)
                            a[l++] = a[r];
                        while (a[l] <= pivot && l < r)
                            l++;
                        if (l < r)
                            a[r--] = a[l];
                    }
                    a[l] = pivot;
                    beg[i + 1] = l + 1;
                    end[i + 1] = end[i];
                    end[i++] = l;
                }
                else
                    i--;
            }
        }

        #endregion

        #region IImage Parts

        public static IImage<double> RealPart(this IImage<Complex> img)
        {
            return new Image(
                img.Width,
                img.Height,
                img.Select(x => x.Real).ToArray());
        }

        public static IImage<double> ImaginaryPart(this IImage<Complex> img)
        {
            return new Image(
                img.Width,
                img.Height,
                img.Select(x => x.Imaginary).ToArray());
        }

        public static IImage<double> RedPart(this IImage<RGB> img)
        {
            return new Image(img.Width, img.Height, img.Data.Select(rgb => rgb.R).ToArray());
        }

        public static IImage<double> BluePart(this IImage<RGB> img)
        {
            return new Image(img.Width, img.Height, img.Data.Select(rgb => rgb.B).ToArray());
        }

        public static IImage<double> GreenPart(this IImage<RGB> img)
        {
            return new Image(img.Width, img.Height, img.Data.Select(rgb => rgb.G).ToArray());
        }

        public static IImage<double> HuePart(this IImage<HSV> img)
        {
            return new Image(img.Width, img.Height, img.Data.Select(x => x.H).ToArray());
        }

        public static IImage<double> SaturationPart(this IImage<HSV> img)
        {
            return new Image(img.Width, img.Height, img.Data.Select(x => x.S).ToArray());
        }

        public static IImage<double> ValuePart(this IImage<HSV> img)
        {
            return new Image(img.Width, img.Height, img.Data.Select(x => x.V).ToArray());
        }

        public static IImage<double> HuePart(this IImage<HSL> img)
        {
            return new Image(img.Width, img.Height, img.Data.Select(x => x.H).ToArray());
        }

        public static IImage<double> SaturationPart(this IImage<HSL> img)
        {
            return new Image(img.Width, img.Height, img.Data.Select(x => x.S).ToArray());
        }

        public static IImage<double> LightnessPart(this IImage<HSL> img)
        {
            return new Image(img.Width, img.Height, img.Data.Select(x => x.L).ToArray());
        }

        #endregion

        #region Make Hot Image

        /// <summary>
        /// Returns a color image with the same dimensions
        /// The R, G, B values of the result image at (i, j) are determined by using the value of image
        /// at (i, j) to index three lookup tables. These lookup tables implement a false coloring scheme 
        /// which maps small values to black, large values to white, and intermediate values to shades of red, orange, and yellow (in that order). 
        /// </summary>
        /// <param name="image">Greyscale IImage<Double></param>
        /// <returns>Color IImage<Double> with a false coloring scheme</returns>
        public static IImage<RGB> MakeHotImage(this IImage<double> image)
        {
            RGBImage sp2 = new RGBImage(image.Width, image.Height);

            double maximum = double.MinValue;
            double minimum = double.MaxValue;

            int n = image.Length;

            for (int i = 0; i < n; i++)
            {
                double value = image[i];
                if (value > maximum) maximum = value;
                if (value < minimum) minimum = value;
            }

            for (int i = 0; i < n; i++)
            {
                double value = image[i];
                double r = rhot(minimum, maximum, value);
                double g = ghot(minimum, maximum, value);
                double b = bhot(minimum, maximum, value);
                sp2.Data[i] = new RGB(r, g, b);
            }

            return sp2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double bhot(double xmin, double xmax, double x)
        {
            const double twoThirds = 2.0 / 3.0;

            x = (x - xmin) / (xmax - xmin);

            if (x < twoThirds)
                return 0f;
            else
                return (x - twoThirds) * 3.0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ghot(double xmin, double xmax, double x)
        {
            const double twoThirds = 2.0 / 3.0;
            const double oneThird = 1.0 / 3.0;

            x = (x - xmin) / (xmax - xmin);

            if (x < oneThird)
                return 0.0;

            if (x < twoThirds)
                return (x - oneThird) * 3.0;

            else
                return 1.0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double rhot(double xmin, double xmax, double x)
        {
            const double oneThird = 1.0 / 3.0;

            x = (x - xmin) / (xmax - xmin);

            return x < oneThird ? x * 3.0 : 1.0;
        }

        #endregion

        public static double[][] CentersOfMass(this IImage<double> sp1)
        {

            int[] r = new int[sp1.Length];
            int[] c = new int[sp1.Length];
            int[] a = new int[sp1.Length];

            int rows = sp1.Height;
            int cols = sp1.Width;
            int n = rows * cols;
            var data = sp1;

            double maximum = 0;

            for (int i = 0; i < n; i++)
                if (data[i] > maximum) maximum = data[i];

            maximum++;

            double[][] sp2 = new double[(int)maximum][];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int index = (int)data[i * cols + j];
                    r[index] += i;
                    c[index] += j;
                    a[index]++;
                }
            }

            for (int i = 0; i < maximum; i++)
            {

                double r_i = r[i];
                double a_i = a[i];
                double c_i = c[i];

                sp2[i] = new double[] { r_i / a_i, c_i / a_i };
            }

            return sp2;
        }

        public static int[][] BoundingBoxes(this IImage<double> sp1)
        {

            int rows = sp1.Height;
            int cols = sp1.Width;
            var data = sp1;
            int n = rows * cols;

            double maximum = 0;
            for (int i = 0; i < n; i++)
                if (data[i] > maximum)
                    maximum = data[i];
            maximum++;

            int[] rmin = new int[(int)maximum];
            int[] rmax = new int[(int)maximum];
            int[] cmin = new int[(int)maximum];
            int[] cmax = new int[(int)maximum];

            int[][] sp2 = new int[(int)maximum][];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int index = (int)data[i * cols + j];
                    rmax[index] = i;
                    cmax[index] = j;
                }
            }

            for (int i = rows - 1; i >= 0; i--)
            {
                for (int j = cols - 1; j >= 0; j--)
                {
                    int index = (int)data[i * cols + j];
                    rmin[index] = i;
                    cmin[index] = j;
                }
            }

            for (int i = 0; i < maximum; i++)
            {

                sp2[i] = new int[] { rmin[i], cmin[i], rmax[i], cmax[i] };
            }

            return sp2;
        }

        public static int[] Areas(this IImage<double> sp1)
        {

            int rows = sp1.Height;
            int cols = sp1.Width;
            double[] data1 = sp1.Data;
            int n = rows * cols;

            double maximum = 0;
            for (int i = 0; i < n; i++)
            {
                if (data1[i] > maximum)
                {
                    maximum = data1[i];
                }
            }

            int[] sp2 = new int[(int)maximum + 1];

            for (int i = 0; i < n; i++)
            {
                int index = (int)data1[i];
                if (index >= 0)
                {
                    sp2[index] = sp2[index] + 1;
                }
            }

            return sp2;
        }

        public static IImage<double> Outline(this IImage<double> image)
        {
            return ImageExtensions.Outline(image, 1, 0);
        }

        /// <summary>
        /// Returns an image where edge pixels are set to the edge value and non-edge
        /// pixels are set to the non-edge value.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="edge"></param>
        /// <param name="non_edge"></param>
        /// <returns></returns>
        public static IImage<double> Outline(this IImage<double> image, int edge, int non_edge)
        {
            int rows = image.Height;
            int cols = image.Width;
            double[] data1 = image.Data;

            var sp4 = new Image(image.Width, image.Height);
            double[] data4 = sp4.Data;

            int index0;
            for (int i = 0; i < rows; i++)
            {
                index0 = i * cols;
                for (int j = 0; j < cols; j++)
                {
                    data4[index0 + j] = non_edge;
                }
            }

            for (int i = 0; i < rows - 1; i++)
            {
                index0 = i * cols;
                int index1 = (i + 1) * cols;
                for (int j = 0; j < cols; j++)
                {
                    if (data1[index0 + j] + data1[index1 + j] == 1)
                    {
                        data4[index0 + j] = edge;
                    }
                }
            }

            index0 = (rows - 1) * cols;
            for (int j = 0; j < cols; j++)
            {
                if (data1[index0 + j] + data1[j] == 1)
                {
                    data4[index0 + j] = edge;
                }
            }

            for (int i = 0; i < rows; i++)
            {
                index0 = i * cols;
                for (int j = 1; j < cols - 1; j++)
                {
                    if (data1[index0 + j] + data1[index0 + j + 1] == 1)
                    {
                        data4[index0 + j] = edge;
                    }
                }
            }

            for (int i = 0; i < rows; i++)
            {
                index0 = i * cols;
                if (data1[index0 + cols - 1] + data1[index0] == 1)
                {
                    data4[index0 + cols - 1] = edge;
                }
            }

            return sp4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Mod(int x, int n)
        {
            return x >= n ? x - n : (x < 0 ? n + x : x);
        }

        public static unsafe IImage<double> DistanceTransform(this IImage<double> sp1)
        {
            double* kdata1 = stackalloc double[5 * 5];
            double* kdata2 = stackalloc double[5 * 5];

            kdata1[0] = kdata2[24] = 2.8284;
            kdata1[1] = kdata2[23] = 2.2;
            kdata1[2] = kdata2[22] = 2.0;
            kdata1[3] = kdata2[21] = 2.2;
            kdata1[4] = kdata2[20] = 2.8284;
            kdata1[5] = kdata2[19] = 2.2;
            kdata1[6] = kdata2[18] = 1.4;
            kdata1[7] = kdata2[17] = 1.0;
            kdata1[8] = kdata2[16] = 1.4;
            kdata1[9] = kdata2[15] = 2.2;
            kdata1[10] = kdata2[14] = 2.0;
            kdata1[11] = kdata2[13] = 1.0;
            kdata1[12] = kdata2[12] = 0.0;

            //
            int rows = sp1.Height;
            int cols = sp1.Width;
            int n = (int)(Math.Sqrt(rows * rows + cols * cols) / 2.0);
            var data1 = sp1;

            IImage<Double> sp2 = sp1.Outline(0, 10 * 10 ^ 7 /* 10e7 */);
            var data2 = sp2;

            Parallel.For(0, n, m =>
            {

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        double sum = 10e7;
                        for (int k = 2; k < 5; k++)
                        {
                            int index0 = cols * Mod(i + k - 2, rows);
                            int index1 = 5 * k;
                            for (int l = 0; l < 5; l++)
                            {
                                if (index1 + l < 12)
                                    continue;

                                double x = data2[index0 + Mod(j + l - 2, cols)] + kdata2[index1 + l];
                                sum = Math.Min(sum, x);
                            }
                        }
                        data2[cols * i + j] = sum;
                    }
                }

                for (int i = rows - 1; i >= 0; i--)
                {
                    for (int j = cols - 1; j >= 0; j--)
                    {
                        double sum = 10e7;
                        for (int k = 0; k < 3; k++)
                        {
                            int index0 = cols * Mod(i + k - 2, rows);
                            int index1 = 5 * k;

                            for (int l = 0; l < 5; l++)
                            {
                                if (index1 + l > 12)
                                    break;

                                double x = data2[index0 + Mod(j + l - 2, cols)] + kdata1[index1 + l];
                                sum = sum > x ? x : sum;//Math.Min(sum, x);
                            }
                        }
                        data2[cols * i + j] = sum;
                    }
                }
            });

            n = rows * cols;
            for (int i = 0; i < n; i++) data2[i] *= data1[i];

            return sp2;
        }
        /*
        public static IImage<double> DistanceTransform(this IImage<double> sp1)
        {
            double[] kdata1 = new double[5 * 5];
            double[] kdata2 = new double[5 * 5];

            kdata1[0] = kdata2[24] = 2.8284;
            kdata1[1] = kdata2[23] = 2.2;
            kdata1[2] = kdata2[22] = 2.0;
            kdata1[3] = kdata2[21] = 2.2;
            kdata1[4] = kdata2[20] = 2.8284;
            kdata1[5] = kdata2[19] = 2.2;
            kdata1[6] = kdata2[18] = 1.4;
            kdata1[7] = kdata2[17] = 1.0;
            kdata1[8] = kdata2[16] = 1.4;
            kdata1[9] = kdata2[15] = 2.2;
            kdata1[10] = kdata2[14] = 2.0;
            kdata1[11] = kdata2[13] = 1.0;
            kdata1[12] = kdata2[12] = 0.0;

            //
            int rows = sp1.Height;
            int cols = sp1.Width;
            int n = (int)(Math.Sqrt(rows * rows + cols * cols) / 2.0);
            var data1 = sp1;

            IImage<Double> sp2 = sp1.Outline(0, 10 * 10 ^ 7 /* 10e7 *//*);
            var data2 = sp2;

            Parallel.For(0, n, m =>
            {

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        double sum = 10e7;
                        for (int k = 2; k < 5; k++)
                        {
                            int index0 = cols * Mod(i + k - 2, rows);
                            int index1 = 5 * k;
                            for (int l = 0; l < 5; l++)
                            {
                                if (index1 + l < 12)
                                    continue;

                                double x = data2[index0 + Mod(j + l - 2, cols)] + kdata2[index1 + l];
                                sum = Math.Min(sum, x);
                            }
                        }
                        data2[cols * i + j] = sum;
                    }
                }

                for (int i = rows - 1; i >= 0; i--)
                {
                    for (int j = cols - 1; j >= 0; j--)
                    {
                        double sum = 10e7;
                        for (int k = 0; k < 3; k++)
                        {
                            int index0 = cols * Mod(i + k - 2, rows);
                            int index1 = 5 * k;

                            for (int l = 0; l < 5; l++)
                            {
                                if (index1 + l > 12)
                                    break;

                                double x = data2[index0 + Mod(j + l - 2, cols)] + kdata1[index1 + l];
                                sum = sum > x ? x : sum;//Math.Min(sum, x);
                            }
                        }
                        data2[cols * i + j] = sum;
                    }
                }
            });

            n = rows * cols;
            for (int i = 0; i < n; i++) data2[i] *= data1[i];

            return sp2;
        }
        */

        #region Convolve

        public static IImage<Double> ConvolveCols(this IImage<Double> image, params double[] kernel)
        {
            double[][] kernel2D = new double[kernel.Length][];

            for (int i = 0; i < kernel.Length; i++)
            {
                kernel2D[i] = new double[] { kernel[i] };
            }

            return Convolve(image, kernel2D);
        }

        public static IImage<Double> ConvolveRows(this IImage<Double> image, params double[] kernel)
        {
            double[][] kernel2D = new double[][]
                {
                    kernel
                };

            return Convolve(image, kernel2D);
        }

        /// <summary>
        /// Convolves image using a specific kernel.
        /// Edge Handling Method is crop.
        /// </summary>
        /// <param name="image">Image to convolve</param>
        /// <param name="kernel">convolution kernel</param>
        /// <returns>Convolution result</returns>
        public static IImage<Double> Convolve(this IImage<Double> image, double[][] kernel)
        {
            return Convolve(image, kernel, EdgeHandling.Crop);
        }

        /// <summary>
        /// Convolves the image using a specified kernel edge handling technique.
        /// </summary>
        /// <param name="image">Image to convolve</param>
        /// <param name="kernel">Convolution kernel</param>
        /// <param name="edgeHandling">Convolution Edge Handling Technique</param>
        /// <returns>Convolved Image</returns>
        public static IImage<Double> Convolve(this IImage<Double> image, double[][] kernel, EdgeHandling edgeHandling)
        {
            // Kernel
            var kernel_height = kernel.Length;
            var kernel_width = kernel[0].Length;

            // Mid Point
            var k_mid_h = kernel_height % 2 == 0 ? (kernel_height >> 1) - 1 : (kernel_height >> 1);
            var k_mid_w = kernel_width % 2 == 0 ? (kernel_width >> 1) - 1 : (kernel_width >> 1);

            // Dimensions for the new Image
            int width = image.Width;
            int height = image.Height;

            Func<Int32, Int32> edgeX;
            Func<Int32, Int32> edgeY;

            switch (edgeHandling)
            {
                // Wrap Pixels
                case EdgeHandling.Wrap:
                    edgeX = x => x >= width ? x - width : (x < 0 ? width + x : x);
                    edgeY = y => y >= height ? y - width : (y < 0 ? height + y : y);
                    break;

                // Start with offset and insert into cropped image
                case EdgeHandling.Crop:
                    width -= kernel_width;
                    height -= kernel_height;
                    edgeX = x => x + k_mid_w;
                    edgeY = y => y + k_mid_h;
                    break;

                // Extend
                case EdgeHandling.Extend:
                default:
                    edgeX = x => x < 0 ? 0 : (x >= width ? width - 1 : x);
                    edgeY = y => y < 0 ? 0 : (y >= height ? height - 1 : y);
                    break;
            }

            IImage<Double> conv = new Image(width, height);

            // Convolution is a general purpose filter effect for images
            // Is a matrix applied to an image and a mathematical operation
            // comprised of integers
            // It works by determining the value of a central pixel by adding the
            // weighted values of all its neighbors together
            // The output is a new modified filtered image
            // for each image row in output image,
            Parallel.For(0, height - 1, y =>
            {
                // for each pixel in image row:
                for (int x = 0; x < width; x++)
                {
                    // set accumulator to zero
                    double accumulator = 0.0;

                    // for each kernel row in kernel:
                    for (int k_x = 0; k_x < kernel_width; k_x++)
                    {
                        // for each element in kernel row:
                        for (int k_y = 0; k_y < kernel_height; k_y++)
                        {
                            // int index1 = image.Width * Mod(y + kcol - w, image.Height) + Mod(x + krow - h, image.Width);
                            int index1 = image.Width * edgeY(y + k_y - k_mid_h) + edgeX(x + k_x - k_mid_w);
                            int kernelX = kernel_width - k_x - 1;
                            int kernelY = kernel_height - k_y - 1;
                            accumulator += image[index1] * kernel[kernelY][kernelX];
                        }
                    }

                    // set output image pixel to accumulator
                    conv[y * width + x] = accumulator;
                }
            });

            return conv;
        }

        #endregion

        public static double ChiSquareDistance(this IImage<double> img1, IImage<double> img2)
        {
            // Get Histograms
            var h1 = img1.Histogram();
            var h2 = img2.Histogram();

            // Total Number of Histogram Bins
            var n = h1.Length;

            double sum = 0;
            for (int i = 0; i < n; i++)
            {
                double top = h1[i] - h2[i];
                double bot = h1[i];
                if (bot > 0.0)
                {
                    sum += top * top / bot;
                }
            }

            return sum;
        }

        /// <summary>
        /// BhattacharryaDistance
        /// </summary>
        /// <param name="img1"></param>
        /// <param name="img2"></param>
        /// <returns></returns>
        public static double HellingerDistance(this IImage<double> img1, IImage<double> img2)
        {
            // Get Histograms
            var h1 = img1.Histogram();
            var h2 = img2.Histogram();

            // Total Number of Histogram Bins
            var n = h1.Length;

            // H^ = (1 / N) * SUM_j( H^(j) )
            double _h1 = h1.Sum() / (double)n;
            double _h2 = h2.Sum() / (double)n;

            // SUM_i { sqrt[ h1(i) * h2(i) ] }
            double p = 0.0;
            for (int i = 0; i < n; i++)
            {
                p += Math.Sqrt(h1[i] * h2[i]);
            }

            // 1 / sqrt(H^_1 * H^_2 * N * N )
            var oneOver = (1.0 / Math.Sqrt(_h1 * _h2 * n * n));

            // d(H1, H2)
            return Math.Sqrt(1 - oneOver * p);
        }


        #region Columns / Rows

        public static ImageColumn<T> Column<T>(this IImage<T> image, int col)
            where T : struct, IEquatable<T>
        {
            if (col < 0 || col >= image.Width)
                throw new ArgumentOutOfRangeException("col");

            return new ImageColumn<T>(image, col);
        }

        public static ImageColumn<T>[] Columns<T>(this IImage<T> image)
            where T : struct, IEquatable<T>
        {
            var columns = new ImageColumn<T>[image.Width];

            for (int col = 0; col < image.Width; col++)
            {
                columns[col] = image.Column(col);
            }

            return columns;
        }

        public static ImageRow<T> Row<T>(this ImageColumn<T> col, int row)
            where T : struct, IEquatable<T>
        {
            return col.Image.Row(row);
        }

        public static ImageRow<T>[] Rows<T>(this ImageColumn<T> col)
            where T : struct, IEquatable<T>
        {
            return col.Image.Rows();
        }

        public static ImageRow<T> Row<T>(this IImage<T> img, int row)
            where T : struct, IEquatable<T>
        {
            if (row < 0 || row >= img.Height)
                throw new ArgumentOutOfRangeException();

            return new ImageRow<T>(img, row);
        }

        public static ImageRow<T>[] Rows<T>(this IImage<T> img)
            where T : struct, IEquatable<T>
        {
            var rows = new ImageRow<T>[img.Height];
            for (int row = 0; row < img.Height; row++)
            {
                rows[row] = new ImageRow<T>(img, row);
            }
            return rows;
        }

        public static ImageColumn<T> Column<T>(this ImageRow<T> row, int col)
            where T : struct, IEquatable<T>
        {
            return row.Image.Column(col);
        }

        public static ImageColumn<T>[] Columns<T>(this ImageRow<T> row)
            where T : struct, IEquatable<T>
        {
            return row.Image.Columns();
        }

        #endregion
    }
}
