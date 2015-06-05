using System;
using System.Numerics;

namespace ImageLibrary
{

    // The basic model for filtering in the frequency domain
    // where F(u,v): the Fourier transform of the image to be smoothed
    // H(u,v): a filter transfer function



    // http://retouchpro.com/tutorials/?m=show&id=185
    // http://www.imagemagick.org/Usage/filter/#filter
    // http://stackoverflow.com/questions/2303690/resizing-an-image-in-an-html5-canvas
    // http://www.amazon.com/dp/1846283795/?tag=stackoverfl08-20

    /// <summary>
    /// See,
    /// http://www.idlcoyote.com/ip_tips/freqfiltering.html
    /// </summary>
    public static class Filters
    {
        public static double Tent(double x)
        {
            if (x < 0.0) x = -x;

            return (x < 1.0) ? 1.0 - x : 0.0;
        }

        public static double Box(double x)
        {
            return (x >= -0.5 && x <= 0.5) ? 1.0 : 0.0;
        }

        public static double Bell(double x)
        {
            if (x < 0.0) x = -x;

            if (x < .5)
            {
                return .75f - x * x;
            }
            else if (x < 1.5)
            {
                x = (x - 1.5f);
                return .5 * x * x;
            }
            else
            {
                return 0.0;
            }
        }

        public static double BSpline(double t)  /* box (*) box (*) box (*) box */
        {
            double tt;

            if (t < 0.0f) t = -t;

            if (t < 1.0f)
            {
                tt = t * t;
                return ((.5f * tt * t) - tt + (2.0f / 3.0f));
            }
            else if (t < 2.0f)
            {
                t = 2.0f - t;
                return ((1.0f / 6.0f) * (t * t * t));
            }
            else
            {
                return 0.0;
            }
        }

        private const double QUADRATIC_SUPPORT = 1.5;

        // Dodgson, N., "Quadratic Interpolation for Image Resampling"
        public static Func<double, double> Quadratic(double r)
        {
            return x => QudraticGeneric(x, r);
        }

        public static double QudraticGeneric(double t, double R)
        {
            if (t < 0.0f)
                t = -t;
            if (t < QUADRATIC_SUPPORT)
            {
                double tt = t * t;
                if (t <= .5f)
                    return (-2.0f * R) * tt + .5f * (R + 1.0f);
                else
                    return (R * tt) + (-2.0f * R - .5f) * t + (3.0f / 4.0f) * (R + 1.0f);
            }
            else
                return 0.0f;
        }

        public static double Sinc(double x)
        {
            x = x * Math.PI;

            if (x < 0.01 && x > -0.01)
                return 1.0 + x * x * (-1.0 / 6.0 + x * x * 1.0 / 120.0);

            return Math.Sin(x) / x;
        }

        public static double NearestNeighbor(double x)
        {
            return Math.Round(x, MidpointRounding.AwayFromZero);
        }

        public static Func<double, double> Lanczos(double size)
        {
            return x => LanczosKernel(x, size);
        }

        #region Hamming

        public static Func<double, double> Hamming(double size)
        {
            return x => HammingKernel(x, size);
        }

        private static double HammingKernel(double x, double size)
        {
            // Taken from Chromium Version
            // http://src.chromium.org/svn/trunk/src/skia/ext/image_operations.cc
            // Adapted for double though

            // numeric_limits<double>::epsilon() DNE in C# ??
            const double eps = 2.22045e-016;

            if (x <= -size || x >= size)
            {
                return 0.0;
            }

            if (x > -eps && x < eps)
            {
                return 1.0;
            }

            // http://en.wikipedia.org/wiki/Window_function#Hamming_window

            const double alpha = 0.54;
            const double beta = 1.0 - alpha;

            double xpi = x * Math.PI;

            return (Math.Sin(xpi) / xpi) *
                (beta + alpha * Math.Cos(xpi / size));
        }

        #endregion

        private static double LanczosKernel(double x, double size)
        {
            // Taken from Chromium Version
            // http://src.chromium.org/svn/trunk/src/skia/ext/image_operations.cc
            // Adapted for double though

            // numeric_limits<double>::epsilon() DNE in C# ??
            const double eps = 2.22045e-016;

            if (x <= -size || x >= size)
            {
                return 0.0;
            }

            if (x > -eps && x < eps)
            {
                return 1.0;
            }

            double xpi = x * Math.PI;
            double xpi_div_size = xpi / size;

            return (Math.Sin(xpi) / xpi) * Math.Sin(xpi_div_size) / xpi_div_size;
        }

        public static IImage<double> MakeFilterWithOffset(int width, int height, Func<int, int, double> func)
        {
            var filterFunc = MakeFilterFuncWithOffset(width, height, func);
            return new Image(width, height).MapLocation(filterFunc);
        }

        public static Func<int, int, double> MakeFilterFuncWithOffset(int width, int height, Func<int, int, double> func)
        {
            int halfWidth = width >> 1;
            int halfHeight = height >> 1;
            
            return (i, j) =>
                {
                    int x = halfWidth - i;
                    int y = halfHeight - j;
                    x = x > 0 ? x : -x;
                    y = y > 0 ? y : -y;

                    return func(x, y);
                };
        }

        public static IImage<double> MakeFilter(int width, int height, Func<int, int, double> func)
        {
            Func<int, int, double> filterFunc = MakeFilterFunc(width, height, func);

            return new Image(width, height, new double[width*height])
                .MapLocation(filterFunc);
        }

        public static Func<int, int, double> MakeFilterFunc(int width, int height, Func<int, int, double> func)
        {
            int halfWidth = width >> 1;
            int halfHeight = height >> 1;

            return
                (i, j) => func(
                    i <= halfHeight ? i : i - height,
                    j <= halfWidth ? j : j - width);
        }

        public static Func<int, int, double> Gaussian(double variance)
        {
            const double twoPi = 2 * Math.PI;
            double twoPiVar = twoPi * variance;

            return (i, j) =>
            {
                double r = i * i + j * j;
                double x = r / twoPiVar;
                return Math.Exp(-x);
            };
        }

        public static Func<int, int, double> LaplacianOfGaussian(double stddev)
        {
            double tmp = -Math.PI / (stddev * stddev);
            return
                (i, j) =>
                {
                    double r = i * i + j * j;
                    double x = r / (2.0 * stddev);
                    return tmp * (1.0 - x) * Math.Exp(-x);
                };
        }

        public static Func<int, int, Complex> HarmonicSignal(double u, double v)
        {
            Complex c = new Complex(0, -Math.PI * 2);
            return (m, n) => Complex.Exp(c * ((u * m) + (v * n)));
        }

        #region Ideal Filters

        public static Func<int, int, double> IdealLowPass(double stddev)
        {
            return (x, y) => Math.Sqrt(x * x + y * y) < stddev ? 1.0 : 0.0;
        }

        public static Func<int, int, double> IdealHighPass(double stddev)
        {
            return (x, y) => Math.Sqrt(x * x + y * y) > stddev ? 1.0 : 0.0;
        }

        public static Func<int, int, double> IdealBandPass(double start, double end)
        {
            return (x, y) =>
                {
                    double r = Math.Sqrt(x * x + y * y);
                    return r > start && r < end ? 1.0 : 0.0;
                };
        }

        #endregion
    }
}
