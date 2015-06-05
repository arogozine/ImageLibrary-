using ImageLibrary.Extensions;
using System;
using System.Runtime.CompilerServices;

namespace ImageLibrary.EdgeDetection
{
    internal enum Direction
    {
        Top,
        Bottom,
        Left,
        Right
    }

    /// <summary>
    /// Adapted to C# from, 
    /// FAST-EDGE - A fast edge detection implementation in C.
    /// Copyright (c) 2009 Benjamin C. Haynor
    /// https://code.google.com/p/fast-edge/
    /// </summary>
    public static class CannyEdgeDetector
    {
        public static IImage<double> Run(IImage<double> img, CannyOptions options)
        {
            options = options ?? new CannyOptions();

            var img256 = img.Copy();
            
            if (options.NormalizeImage)
            {
                img256 = img256.Normalize256();
            }

            // or morph_close
            var img_out = options.NoiseReduce ? CannyEdgeDetector.GaussianNoiseReduce(img256) : img256;

            return CannyEdgeDetector.CannyEdgeDetect(img_out, options.HighThresholdPercentage, options.LowThresholdPercentage);
        }

        public static IImage<double> Run(IImage<double> img)
        {
            return CannyEdgeDetector.Run(img, null);
        }

        /*
        // ratio of lower:upper threshold 
        private static int ratio = 3;

        // maximum value for the lower Threshold 
        private static int max_lowThreshold = 100;
        private static double[][] Gx =
            new[] {
                new double[] { -1, 0, +1 },
                new double[] { -2, 0, +2 },
                new double[] { -1, 0, +1 }
            };

        private static double[][] Gy =
            new[] {
                new double[] { -1, -2, -1 },
                new double[] { +0, +0, +0 },
                new double[] { +1, +2, +1 }
            };
        */
        /*
 
        GAUSSIAN_NOISE_ REDUCE
 
        apply 5x5 Gaussian convolution filter, shrinks the image by 4 pixels in each direction, using Gaussian filter found here:
 
        http://en.wikipedia.org/wiki/Canny_edge_detector
 
*/
        private static IImage<double> CannyEdgeDetect(IImage<double> img_in, double highThresholdPercentage, double lowThresholdPercentage)
        {
            double[] g;
            int[] direction;
            CalcGradientSobel(img_in, out g, out direction);

            IImage<double> img_scratch = ImageFactory.Generate(img_in.Width, img_in.Height);

            CannyEdgeDetector.NonMaxSuppression(img_scratch, g, direction);

            double high, low;
            EstimateThreshold(img_scratch, highThresholdPercentage, lowThresholdPercentage, out high, out low);

            return hysteresis(high, low, img_scratch);
        }

        private static IImage<double> GaussianNoiseReduce(IImage<double> img_in)
        {
            IImage<double> img_out = new Image(img_in.Width, img_in.Height);

            int w = img_in.Width;
            int h = img_in.Height;

            int max_x = w - 2;
            int max_y = w * (h - 2);

            for (int y = w * 2; y < max_y; y += w)
            {
                for (int x = 2; x < max_x; x++)
                {
                    const double Div159 = 1/ 159.0;
                    double value =
                    2 * img_in[x + y - 2 - w - w] +
                    4 * img_in[x + y - 1 - w - w] +
                    5 * img_in[x + y - w - w] +
                    4 * img_in[x + y + 1 - w - w] +
                    2 * img_in[x + y + 2 - w - w] +
                    4 * img_in[x + y - 2 - w] +
                    9 * img_in[x + y - 1 - w] +
                    12 * img_in[x + y - w] +
                    9 * img_in[x + y + 1 - w] +
                    4 * img_in[x + y + 2 - w] +
                    5 * img_in[x + y - 2] +
                    12 * img_in[x + y - 1] +
                    15 * img_in[x + y] +
                    12 * img_in[x + y + 1] +
                    5 * img_in[x + y + 2] +
                    4 * img_in[x + y - 2 + w] +
                    9 * img_in[x + y - 1 + w] +
                    12 * img_in[x + y + w] +
                    9 * img_in[x + y + 1 + w] +
                    4 * img_in[x + y + 2 + w] +
                    2 * img_in[x + y - 2 + w + w] +
                    4 * img_in[x + y - 1 + w + w] +
                    5 * img_in[x + y + w + w] +
                    4 * img_in[x + y + 1 + w + w] +
                    2 * img_in[x + y + 2 + w + w];

                    img_out[x + y] = value * Div159;
                }
            }
            return img_out;
        }

        /// <summary>
        /// Calculates the result of the Sobel operator - http://en.wikipedia.org/wiki/Sobel_operator - and estimates edge direction angle
        /// </summary>
        /// <param name="img_in">Input Image</param>
        /// <param name="g"></param>
        /// <param name="direction">Direction array</param>
        private static void CalcGradientSobel(IImage<double> img_in, out double[] g, out int[] direction)
        {
            g = new double[img_in.Data.Length];
            direction = new int[img_in.Data.Length];

            double g_x, g_y;

            double g_div;

            int w = img_in.Width;
            int h = img_in.Height;

            int max_x = w - 3;

            int max_y = w * (h - 3);

            for (int y = w * 3; y < max_y; y += w)
            {
                for (int x = 3; x < max_x; x++)
                {
                    g_x = (2 * img_in[x + y + 1]
                            + img_in[x + y - w + 1]
                            + img_in[x + y + w + 1]
                            - 2 * img_in[x + y - 1]
                            - img_in[x + y - w - 1]
                            - img_in[x + y + w - 1]);

                    g_y = 2 * img_in[x + y - w]
                            + img_in[x + y - w + 1]
                            + img_in[x + y - w - 1]
                            - 2 * img_in[x + y + w]
                            - img_in[x + y + w + 1]
                            - img_in[x + y + w - 1];


                    g[x + y] = Math.Sqrt(g_x * g_x + g_y * g_y);

                    if (g_x == 0)
                    {
                        direction[x + y] = 2;
                    }
                    else
                    {
                        g_div = g_y / g_x;

                        /* the following commented-out code is slightly faster than the code that follows, but is a slightly worse approximation for determining the edge direction angle
                        if (g_div < 0) {
                                if (g_div < -1) {
                                        dir[n] = 0;
                                } else {
                                        dir[n] = 1;
                                }
                        } else {
                                if (g_div > 1) {
                                        dir[n] = 0;
                                } else {
                                        dir[n] = 3;
                                }
                        }
 
                        */

                        if (g_div < 0)
                        {
                            if (g_div < -2.41421356237)
                            {
                                direction[x + y] = 0;
                            }
                            else
                            {
                                if (g_div < -0.414213562373)
                                {
                                    direction[x + y] = 1;
                                }
                                else
                                {
                                    direction[x + y] = 2;
                                }
                            }
                        }
                        else
                        {
                            if (g_div > 2.41421356237)
                            {
                                direction[x + y] = 0;
                            }
                            else
                            {
                                if (g_div > 0.414213562373)
                                {
                                    direction[x + y] = 3;
                                }
                                else
                                {
                                    direction[x + y] = 2;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// calculates the result of the Scharr version of the Sobel operator - http://en.wikipedia.org/wiki/Sobel_operator - and estimates edge direction angle
        /// may have better rotational symmetry
        /// </summary>
        /// <param name="img_in"></param>
        /// <param name="g_x"></param>
        /// <param name="g_y"></param>
        /// <param name="g"></param>
        /// <param name="direction"></param>
        private static void CalcGradientCharr(IImage<double> img_in, double[] g_x, double[] g_y, double[] g, int[] direction)
        {
            // TODO
            int w = img_in.Width;
            int h = img_in.Height;

            int max_x = w - 1;
            int max_y = w * (h - 1);

            int n = 0;

            // For all Y except last row
            for (int y = w; y < max_y; y += w)
            {
                // For all X except law column
                for (int x = 1; x < max_x; x++)
                {
                    g_x[n] = 10 * img_in[x + y + 1]
                            + 3 * img_in[x + y - w + 1]
                            + 3 * img_in[x + y + w + 1]
                            - 10 * img_in[x + y - 1]
                            - 3 * img_in[x + y - w - 1]
                            - 3 * img_in[x + y + w - 1];

                    g_y[n] = 10 * img_in[x + y - w]
                            + 3 * img_in[x + y - w + 1]
                            + 3 * img_in[x + y - w - 1]
                            - 10 * img_in[x + y + w]
                            - 3 * img_in[x + y + w + 1]
                            - 3 * img_in[x + y + w - 1];

                    g[n] = Math.Abs(g_x[n]) + Math.Abs(g_y[n]);

                    if (g_x[n] == 0)
                    {
                        direction[n] = 2;
                    }
                    else
                    {
                        double g_div = g_y[n] / g_x[n];

                        if (g_div < 0)
                        {
                            if (g_div < -2.41421356237)
                            {
                                direction[n] = 0;
                            }
                            else
                            {
                                if (g_div < -0.414213562373)
                                {
                                    direction[n] = 1;
                                }
                                else
                                {
                                    direction[n] = 2;
                                }
                            }
                        }
                        else
                        {
                            if (g_div > 2.41421356237)
                            {
                                direction[n] = 0;
                            }
                            else
                            {
                                if (g_div > 0.414213562373)
                                {
                                    direction[n] = 3;
                                }
                                else
                                {
                                    direction[n] = 2;
                                }
                            }
                        }
                    }
                    n++;
                }
            }
        }

        /// <summary>
        /// Using the estimates of the Gx and Gy image gradients and the edge direction angle
        /// determines whether the magnitude of the gradient assumes a local  maximum in the gradient direction
        /// if the rounded edge direction angle is 0 degrees, checks the north and south directions
        /// if the rounded edge direction angle is 45 degrees, checks the northwest and southeast directions
        /// if the rounded edge direction angle is 90 degrees, checks the east and west directions
        /// if the rounded edge direction angle is 135 degrees, checks the northeast and southwest directions
        /// </summary>
        /// <param name="img"></param>
        /// <param name="g"></param>
        /// <param name="direction"></param>
        private static void NonMaxSuppression(IImage<double> img, double[] g, int[] direction)
        {
            int w = img.Width;
            int h = img.Height;

            int max_x = w - 3;
            int max_y = w * (h - 3);
            // int max_x = w;
            // int max_y = w * h;

            // for (int y = 0; y < max_y; y += w)
            for (int y = w * 3; y < max_y; y += w)
            {
                // for (int x = 0; x < max_x; x++)
                for (int x = 3; x < max_x; x++)
                {
                    switch (direction[x + y])
                    {

                        case 0:

                            if (g[x + y] > g[x + y - w] && g[x + y] > g[x + y + w])
                            {
                                if (g[x + y] > 255.0)
                                {
                                    img[x + y] = 0xFF;
                                }
                                else
                                {
                                    img[x + y] = g[x + y];
                                }
                            }
                            else
                            {
                                img[x + y] = 0.0;
                            }

                            break;

                        case 1:

                            if (g[x + y] > g[x + y - w - 1] && g[x + y] > g[x + y + w + 1])
                            {
                                if (g[x + y] > 255.0)
                                {
                                    img[x + y] = 0xFF;
                                }
                                else
                                {
                                    img[x + y] = g[x + y];
                                }
                            }
                            else
                            {
                                img[x + y] = 0.0;
                            }

                            break;

                        case 2:

                            if (g[x + y] > g[x + y - 1] && g[x + y] > g[x + y + 1])
                            {
                                if (g[x + y] > 255.0)
                                {
                                    img[x + y] = 0xFF;
                                }
                                else
                                {
                                    img[x + y] = g[x + y];
                                }
                            }
                            else
                            {
                                img[x + y] = 0.0;
                            }

                            break;

                        case 3:

                            if (g[x + y] > g[x + y - w + 1] && g[x + y] > g[x + y + w - 1])
                            {
                                if (g[x + y] > 255.0)
                                {
                                    img[x + y] = 0xFF;
                                }
                                else
                                {
                                    img[x + y] = g[x + y];
                                }

                            }
                            else
                            {
                                img[x + y] = 0.0;
                            }

                            break;

                        default:
                            throw new ArgumentException("ERROR - direction outside range 0 to 3", nameof(direction));
                    }

                }

            }
        }

        /// <summary>
        /// estimates hysteresis threshold, assuming that the top X% (as defined by the HIGH_THRESHOLD_PERCENTAGE) of edge pixels with the greatest intesity are true edges
        /// and that the low threshold is equal to the quantity of the high threshold plus the total number of 0s at the low end of the histogram divided by 2
        /// </summary>
        /// <param name="img"></param>
        /// <param name="high"></param>
        /// <param name="low"></param>
        private static void EstimateThreshold(IImage<double> img, double highThresholdPercentage, double lowThresholdPercentage, out double high, out double low)
        {
            int i, max, high_cutoff;
            double pixels;
            int[] histogram = new int[256];

            max = img.Width * img.Height;

            for (i = 0; i < max; i++)
            {
                histogram[(int)img[i]]++;
            }

            pixels = (max - histogram[0]) * highThresholdPercentage;
            high_cutoff = 0;
            i = 255;

            while (high_cutoff < pixels)
            {
                high_cutoff += histogram[i];
                i--;
            }

            high = i;
            i = 1;

            while (histogram[i] == 0)
            {
                i++;
            }

            low = (high + i) * lowThresholdPercentage;
        }

        private static IImage<double> hysteresis(double high, double low, IImage<double> img_in)
        {
            IImage<double> img_out = new Image(img_in.Width, img_in.Height);

            for (int y = 0; y < img_out.Height; y++)
            {
                for (int x = 0; x < img_out.Width; x++)
                {
                    if (img_in[y * img_out.Width + x] >= high)
                    {
                        CannyEdgeDetector.Trace(x, y, low, img_in, img_out);
                    }
                }
            }

            return img_out;
        }

        private static bool Trace(int x, int y, double low, IImage<double> img_in, IImage<double> img_out)
        {
            if (img_out[y * img_out.Width + x] == 0.0)
            {
                img_out[y * img_out.Width + x] = 0xFF;

                for (int y_off = -1; y_off <= 1; y_off++)
                {
                    for (int x_off = -1; x_off <= 1; x_off++)
                    {
                        if (!(y == 0 && x_off == 0) && Range(img_in, x + x_off, y + y_off) && img_in[(y + y_off) * img_out.Width + x + x_off] >= low)
                        {
                            if (Trace(x + x_off, y + y_off, low, img_in, img_out))
                            {
                                return true;
                            }
                        }
                    }
                }

                return true;
            }

            return false;
        }

        private static bool Range<T>(IImage<T> img, int x, int y)
            where T : struct, IEquatable<T>
        {
            return (x < 0 || x >= img.Width || y < 0 || y >= img.Height) ? false : true;
        }

        private static void dilate_1d_h(IImage<double> img, IImage<double> img_out)
        {
            // TODO
            int y_max = img.Height * (img.Width - 2);

            for (int y = 2 * img.Width; y < y_max; y += img.Width)
            {
                for (int x = 2; x < img.Width - 2; x++)
                {
                    int offset = x + y;
                    img_out[offset] = Max(Max(Max(Max(img[offset - 2], img[offset - 1]), img[offset]), img[offset + 1]), img[offset + 2]);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Max(double a, double b)
        {
            return a >= b ? a : b;
        }

        private static void dilate_1d_v(IImage<double> img, IImage<double> img_out)
        {
            int y_max = img.Height * (img.Width - 2);

            for (int y = 2 * img.Width; y < y_max; y += img.Width)
            {
                for (int x = 2; x < img.Width - 2; x++)
                {
                    int offset = x + y;
                    img_out[offset] = Max(Max(Max(Max(img[offset - 2 * img.Width], img[offset - img.Width]), img[offset]), img[offset + img.Width]), img[offset + 2 * img.Width]);
                }
            }
        }

        private static void erode_1d_h(IImage<double> img, IImage<double> img_out)
        {
            int y_max = img.Height * (img.Width - 2);

            for (int y = 2 * img.Width; y < y_max; y += img.Width)
            {
                for (int x = 2; x < img.Width - 2; x++)
                {
                    int offset = x + y;
                    img_out[offset] = Min(Min(Min(Min(img[offset - 2], img[offset - 1]), img[offset]), img[offset + 1]), img[offset + 2]);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Min(double a, double b)
        {
            return a <= b ? a : b;
        }

        private static void Erode1dV(IImage<double> img, IImage<double> img_out)
        {
            int y_max = img.Height * (img.Width - 2);
            for (int y = 2 * img.Width; y < y_max; y += img.Width)
            {
                for (int  x = 2; x < img.Width - 2; x++)
                {
                    int offset = x + y;
                    img_out[offset] = Min(Min(Min(Min(img[offset - 2 * img.Width], img[offset - img.Width]), img[offset]), img[offset + img.Width]), img[offset + 2 * img.Width]);
                }
            }
        }

        private static void Erode(IImage<double> img_in, IImage<double> img_scratch, IImage<double> img_out)
        {
            erode_1d_h(img_in, img_scratch);
            Erode1dV(img_scratch, img_out);
        }

        private static void Dilate(IImage<double> img_in, IImage<double> img_scratch, IImage<double> img_out)
        {
            dilate_1d_h(img_in, img_scratch);
            dilate_1d_v(img_scratch, img_out);
        }

        private static void MorphOpen(IImage<double> img_in, IImage<double> img_scratch, IImage<double> img_scratch2, IImage<double> img_out)
        {
            Erode(img_in, img_scratch, img_scratch2);
            Dilate(img_scratch2, img_scratch, img_out);
        }
        /*
        private static void morph_close(IImage<double> img_in, IImage<double> img_scratch, IImage<double> img_scratch2, IImage<double> img_out)
        {
            Dilate(img_in, img_scratch, img_scratch2);
            Erode(img_scratch2, img_scratch, img_out);
        }
        */
    }
}
