using System;
using System.Numerics;
using System.Threading.Tasks;

namespace ImageLibrary.Extensions
{
    /// <summary>
    /// Adapted from,
    /// http://www.codeproject.com/Articles/44166/D-FFT-of-an-Image-in-C
    /// 2D FFT of an Image in C#
    /// Dr. Vinayak Ashok Bharadi,  20 Jul 2012
    /// CPOL http://www.codeproject.com/info/cpol10.aspx
    /// </summary>
    public static class FastFourierTransform
    {
        /// <summary>
        /// Perform a 2D FFT inplace given a complex array
        /// The direction dir, 1 for forward, -1 for reverse
        /// The size of the array (nx,ny)
        /// Return false if there are memory problems or
        /// the dimensions are not powers of 2
        /// </summary>
        /// <param name="c">Complex Array</param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static Complex[] FFT2D(Complex[] c, int nx, int ny, int dir)
        {
            int i, j;
            int m;//Power of 2 for current number of points
            Complex[] output;
            output = c; // Copying Array
            // Transform the Rows 
            double[] real = new double[nx];
            double[] imag = new double[nx];

            for (j = 0; j < ny; j++)
            {
                for (i = 0; i < nx; i++)
                {
                    real[i] = c[i * nx + j].Real;
                    imag[i] = c[i * nx + j].Imaginary;
                }

                // Calling 1D FFT Function for Rows
                m = (int)Math.Log((double)nx, 2);//Finding power of 2 for current number of points e.g. for nx=512 m=9

                FFT1D(dir, m, ref real, ref imag);

                for (i = 0; i < nx; i++)
                {
                    output[i * nx + j] = new Complex(real[i], imag[i]);
                }
            }
            // Transform the columns  
            real = new double[ny];
            imag = new double[ny];

            for (i = 0; i < nx; i++)
            {
                int index = i * nx;

                for (j = 0; j < ny; j++)
                {
                    real[j] = output[index + j].Real;
                    imag[j] = output[index + j].Imaginary;
                }
                // Calling 1D FFT Function for Columns
                m = (int)Math.Log((double)ny, 2);//Finding power of 2 for current number of points e.g. for nx=512 m=9
                FFT1D(dir, m, ref real, ref imag);
                for (j = 0; j < ny; j++)
                {
                    //
                    // TODO
                    //

                    output[index + j] = new Complex(real[j], imag[j]);
                }
            }

            return output;
        }

        /// <summary>
        /// This computes an in-place complex-to-complex FFT
        /// x and y are the real and imaginary arrays of 2^m points.
        /// dir = -1 gives reverse transform
        /// dir = 1 gives forward transform
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="m"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private static void FFT1D(int dir, int m, ref double[] x, ref double[] y)
        {
            long nn, i, i1, j, k, i2, l, l1, l2;
            double c1, c2, tx, ty, t1, t2, u1, u2, z;
            // Calculate the number of points
            nn = 1;
            for (i = 0; i < m; i++)
                nn *= 2;
            // Do the bit reversal
            i2 = nn >> 1;
            j = 0;
            for (i = 0; i < nn - 1; i++)
            {
                if (i < j)
                {
                    tx = x[i];
                    ty = y[i];
                    x[i] = x[j];
                    y[i] = y[j];
                    x[j] = tx;
                    y[j] = ty;
                }
                k = i2;
                while (k <= j)
                {
                    j -= k;
                    k >>= 1;
                }
                j += k;
            }

            // Compute the FFT 
            c1 = -1.0;
            c2 = 0.0;
            l2 = 1;
            for (l = 0; l < m; l++)
            {
                l1 = l2;
                l2 <<= 1;
                u1 = 1.0;
                u2 = 0.0;
                for (j = 0; j < l1; j++)
                {
                    for (i = j; i < nn; i += l2)
                    {
                        i1 = i + l1;
                        t1 = u1 * x[i1] - u2 * y[i1];
                        t2 = u1 * y[i1] + u2 * x[i1];
                        x[i1] = x[i] - t1;
                        y[i1] = y[i] - t2;
                        x[i] += t1;
                        y[i] += t2;
                    }
                    z = u1 * c1 - u2 * c2;
                    u2 = u1 * c2 + u2 * c1;
                    u1 = z;
                }
                c2 = Math.Sqrt((1.0 - c1) / 2.0);
                if (dir == 1)
                    c2 = -c2;
                c1 = Math.Sqrt((1.0 + c1) / 2.0);
            }

            // Scaling for forward transform
            if (dir == 1)
            {
                for (i = 0; i < nn; i++)
                {
                    x[i] /= (double)nn;
                    y[i] /= (double)nn;

                }
            }

            return;
        }

        private static Complex[] ForwardFft(IImage<double> img, bool shift)
        {
            int Height = img.Height;
            int Width = img.Width;

            // Initializing Fourier Transform Array
            Complex[] Fourier = new Complex[Width * Height];
            Complex[] Output = new Complex[Width * Height];

            for (int i = 0; i <= Width * Height - 1; i++)
            {
                Fourier[i] = new Complex(img[i], 0);
            }

            // Calling Forward Fourier Transform
            Output = FFT2D(Fourier, img.Width, img.Height, 1);

            if (shift)
            {
                // Shift
                Complex[] FFTShifted = new Complex[img.Width * img.Height];
                for (int i = 0; i <= (Width / 2) - 1; i++)
                {
                    for (int j = 0; j <= (Height / 2) - 1; j++)
                    {
                        FFTShifted[(i + (Width / 2)) * Width + (j + (Height / 2))] = Output[i * Width + j];
                        FFTShifted[i * Width + j] = Output[(i + (Width / 2)) * Width + (j + (Height / 2))];
                        FFTShifted[(i + (Width / 2)) * Width + j] = Output[(i * Width) + (j + (Height / 2))];
                        FFTShifted[i * Width + (j + (Width / 2))] = Output[(i + (Width / 2)) * Width + j];
                    }
                }

                return FFTShifted;
            }

            return Output;
        }

        /// <summary>
        /// Fast Fourier Transform
        /// </summary>
        /// <param name="img">Image to apply FFT to</param>
        /// <param name="shift">Whether to shift (align center) the FFT</param>
        /// <returns>Complex Image of the FFT</returns>
        public static IImage<Complex> Fft(this IImage<double> img, bool shift)
        {
            return new ComplexImage(img.Width, img.Height, ForwardFft(img, shift));
        }

        /// <summary>
        /// Fast Fourier Transform (Shifted)
        /// </summary>
        /// <param name="img">Image to apply FFT to</param>
        /// <param name="shift"></param>
        /// <returns>Complex Image of the FFT (Shifted)</returns>
        public static IImage<Complex> Fft(this IImage<double> img)
        {
            return new ComplexImage(img.Width, img.Height, ForwardFft(img, true));
        }

        public static IImage<double> InverseFft(this IImage<Complex> image)
        {
            Complex[] Output = FFT2D(image.Data, image.Width, image.Height, -1);

            double[] _magnitude = new double[image.Width * image.Height];

            for (int i = 0; i < image.Width * image.Height; i++)
            {
                _magnitude[i] = Output[i].Magnitude;
            }

            return new Image(image.Width, image.Height, _magnitude);
        }

        public static IImage<double> ApplyFilter(this IImage<double> img, IImage<double> filter)
        {
            return ApplyFilter(img, filter, false);
        }

        public static IImage<double> ApplyFilter(this IImage<double> img, IImage<double> filter, bool offset)
        {
            var complexImage = img.Fft(offset);

            Parallel.For(0, complexImage.Length, i =>
            {
                complexImage[i] *= filter[i];
            });

            return complexImage
                .InverseFft();
        }

        public static IImage<double> ApplyFilter(this IImage<double> img, Func<int, int, double> filterFunc)
        {
            var filter = Filters.MakeFilter(img.Width, img.Height, filterFunc);

            return ApplyFilter(img, filter, false);
        }

    }
}
