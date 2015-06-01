using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageLibrary.Extensions
{
    public static class TypeConversion
    {
        #region HSVToX

        public static RGB ToRgb(this HSV hsv)
        {
            return ToRgb(ToBgra(hsv));
        }

        /// <summary>
        /// http://www.rapidtables.com/convert/color/hsv-to-rgb.htm
        /// </summary>
        /// <param name="hsv"></param>
        /// <returns></returns>
        public static BGRA ToBgra(this HSV hsv)
        {
            double H = hsv.H;
            double S = hsv.S;
            double V = hsv.V;

            double C = V * S;
            double X = C * (1 - Math.Abs((H / 60.0) % 2 - 1));
            double m = V - C;

            double R = 0.0, G = 0.0, B = 0.0;

            if (0.0 <= H && H < 60.0)
            {
                R = C;
                G = X;
            }
            else if (60.0 <= H && H < 120.0)
            {
                R = X;
                G = C;
            }
            else if (120.0 <= H && H < 180.0)
            {
                G = C;
                B = X;
            }
            else if (180.0 <= H && H < 240.0)
            {
                G = X;
                B = C;
            }
            else if (240.0 <= H && H < 300.0)
            {
                R = X;
                B = C;
            }
            else // if (300.0 <= h && h < 360.0)
            {
                R = C;
                B = X;
            }

            R += m;
            G += m;
            B += m;
            R *= 255.0;
            G *= 255.0;
            B *= 255.0;

            return new BGRA()
            {
                R = (byte)R,
                G = (byte)G,
                B = (byte)B,
                A = byte.MaxValue
            };
        }

        public static HSL ToHsl(this HSV hsv)
        {
            if (hsv == default(HSV))
            {
                return default(HSL);
            }

            double L = 0.5 * hsv.V * (2.0 - hsv.S);
            double S = hsv.V * hsv.S / (1 - Math.Abs(L * 2.0 - 1.0));
            return new HSL() { H = hsv.H, L = L, S = S };
        }

        #endregion

        #region HSLToX

        /// <summary>
        /// http://www.rapidtables.com/convert/color/hsl-to-rgb.htm
        /// </summary>
        /// <param name="hsl"></param>
        /// <returns></returns>
        public static RGB ToRgb(this HSL hsl)
        {
            return ToRgb(hsl.H, hsl.S, hsl.L);
        }

        /// <summary>
        /// http://www.rapidtables.com/convert/color/hsl-to-rgb.htm
        /// </summary>
        /// <param name="hsl"></param>
        /// <returns></returns>
        public static BGRA ToBgra(this HSL hsl)
        {
            const double oneHalf = 1.0 / 2.0;
            const double divSixty = 1.0 / 60.0;

            double l = hsl.L;
            double s = hsl.S;
            double h = hsl.H;

            double H = h * divSixty;

            double C = (1 - Math.Abs(2 * l - 1)) * s;
            double X = C * (1 - Math.Abs((H % 2.0) - 1));
            double m = l - C * oneHalf;


            double R, G, B;

            if (0.0 <= h && h < 60.0)
            {
                R = C;
                G = X;
                B = 0;
            }
            else if (60.0 <= h && h < 120.0)
            {
                R = X;
                G = C;
                B = 0;
            }
            else if (120.0 <= h && h < 180.0)
            {
                R = 0;
                G = C;
                B = X;
            }
            else if (180.0 <= h && h < 240.0)
            {
                R = 0;
                G = X;
                B = C;
            }
            else if (240.0 <= h && h < 300.0)
            {
                R = X;
                G = 0;
                B = C;
            }
            else if (300.0 <= h && h < 360.0)
            {
                R = C;
                G = 0;
                B = X;
            }
            else
            {
                R = G = B = 0;
            }

            byte r = (byte)(255.0 * (R + m));
            byte g = (byte)(255.0 * (G + m));
            byte b = (byte)(255.0 * (B + m));

            return new BGRA()
            {
                R = r,
                B = b,
                G = g,
                A = byte.MaxValue
            };
        }

        public static HSV ToHsv(this HSL hsl)
        {
            if (hsl == default(HSL))
            {
                return default(HSV);
            }

            double ll = hsl.L;
            double ss = (ll <= 0.5) ? ll : 1.0 - ll;
            double V = ll +ss;
            double S = (2 * ss) / (ll + ss);

            return new HSV() { H = hsl.H, S = S, V = V  };
        }

        #endregion

        #region RGBtoX

        public static double ToGrayscale(this RGB rgb)
        {
            return rgb.R * 0.299 + rgb.G * 0.587 + rgb.B * 0.114;
        }

        /// <summary>
        /// Assumption: (R G B) in [0.0 - 255.0 ]
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
        public static BGRA ToBgra(this RGB rgb)
        {
            return new BGRA()
            {
                A = byte.MaxValue,
                R = (byte)rgb.R,
                B = (byte)rgb.B,
                G = (byte)rgb.G
            };
        }

        /// <summary>
        /// http://www.rapidtables.com/convert/color/rgb-to-hsl.htm
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
        public static HSL ToHsl(this RGB rgb)
        {
            return ToHsl(rgb.R, rgb.G, rgb.B);
        }

        public static HSV ToHsv(this RGB rgb)
        {
            return ToHsv(rgb.R, rgb.G, rgb.B);
        }

        #endregion

        #region CMYKToX

        public static RGB ToRgb(this CMYK cmyk)
        {
            return CmykToX(cmyk, (R, G, B) =>
                new RGB() { R = R, G = G, B = B, });
        }

        public static BGRA ToBgra(this CMYK cmyk)
        {
            return CmykToX(cmyk, (R, G, B) =>
                new BGRA()
                {
                    R = (byte)R,
                    G = (byte)G,
                    B = (byte)B,
                    A = byte.MaxValue
                });
        }

        private static T CmykToX<T>(this CMYK cmyk, Func<double, double, double, T> func)
        {
            double oneMinK255 = 255.0 * (1 - cmyk.K);

            double R = (1.0 - cmyk.C) * oneMinK255;
            double G = (1.0 - cmyk.M) * oneMinK255;
            double B = (1.0 - cmyk.Y) * oneMinK255;

            return func(R, G, B);
        }

        #endregion

        #region BgraToX

        /// <summary>
        /// http://www.rapidtables.com/convert/color/rgb-to-hsl.htm
        /// </summary>
        /// <param name="pixel"></param>
        /// <returns></returns>
        public static HSL ToHsl(this BGRA pixel)
        {
            return ToHsl(pixel.R, pixel.G, pixel.B);
        }

        public static RGB ToRgb(this BGRA pixel)
        {
            return new RGB(pixel.R, pixel.G, pixel.B);
        }

        public static double ToGrayscale(this BGRA pixel)
        {
            return pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114;
        }

        /// <summary>
        /// http://www.rapidtables.com/convert/color/rgb-to-hsv.htm
        /// </summary>
        /// <param name="pixel"></param>
        /// <returns></returns>
        public static HSV ToHsv(this BGRA pixel)
        {
            return ToHsv((double)pixel.R, (double)pixel.G, (double)pixel.B);
        }

        /// <summary>
        /// http://www.rapidtables.com/convert/color/rgb-to-cmyk.htm
        /// </summary>
        /// <param name="pixel"></param>
        /// <returns></returns>
        public static CMYK ToCmyk(this BGRA pixel)
        {
            // special case for all black (K = 1)
            // to avoid a NaN
            if (pixel == BGRA.Black)
            {
                return new CMYK() { K = 1.0 };
            }

            double R = (double)pixel.R / 255.0;
            double G = (double)pixel.G / 255.0;
            double B = (double)pixel.B / 255.0;

            // K = 1-max(R', G', B')
            double K = 1 - (R > G ? (R > B ? R : B) : (G > B ? G : B));

            // 1 - K
            double oneMinK = 1 - K;
            double oneMinDiv1 = 1.0 / oneMinK;

            // C = (1-R'-K) / (1-K)
            double C = (oneMinK - R) * oneMinDiv1;

            // M = (1-G'-K) / (1-K)
            double M = (oneMinK - G) * oneMinDiv1;

            // Y = (1-B'-K) / (1-K)
            double Y = (oneMinK - B) * oneMinDiv1;

            return new CMYK() { C = C, M = M, Y = Y, K = K };
        }

        public static Color ToColor(this BGRA pixel)
        {
            return Color.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B);
        }

        #endregion

        #region BWtoX

        public static HSL ToHsl(double grayscale)
        {
            return TypeConversion.ToHsl(grayscale, grayscale, grayscale);
        }

        public static RGB ToRgb(double grayscale)
        {
            return new RGB() { R = grayscale, G = grayscale, B = grayscale };
        }

        public static Complex ToComplex(double grayscale)
        {
            return new Complex(grayscale, 0.0);
        }

        public static BGRA ToBgra(double grayscale)
        {
            if (grayscale < 0.0)
            {
                throw new ArgumentException("Value is negative", "grayscale");
            }

            if (grayscale > 255.0)
            {
                throw new ArgumentException("Value cannot fix into a byte", "grayscale");
            }

            byte grayByte = (byte)grayscale;

            return new BGRA()
            {
                A = byte.MaxValue,
                B = grayByte,
                G = grayByte,
                R = grayByte
            };
        }

        #endregion

        #region ByteToX

        public static HSL ToHsl(bool boolean)
        {
            byte value = boolean ? byte.MaxValue : byte.MinValue;

            return ToHsl(value, value, value);
        }

        public static RGB ToRgb(bool boolean)
        {
            byte value = boolean ? byte.MaxValue : byte.MinValue;

            return new RGB() { R = value, G = value, B = value };
        }

        public static Complex ToComplex(bool boolean)
        {
            byte value = boolean ? byte.MaxValue : byte.MinValue;

            return new Complex(value, 0.0);
        }

        public static BGRA ToBgra(bool boolean)
        {
            byte value = boolean ? byte.MaxValue : byte.MinValue;

            return new BGRA()
            {
                A = byte.MaxValue,
                B = value,
                G = value,
                R = value
            };
        }

        #endregion

        #region ColorToX

        /// <summary>
        /// Converts Color to RGB Struct
        /// </summary>
        /// <param name="c">System Drawing Color</param>
        /// <returns>Double Precision RGB Struct</returns>
        public static RGB ToRgb(this System.Drawing.Color c)
        {
            return new RGB(c.R, c.G, c.B);
        }

        public static BGRA ToBgra(this System.Drawing.Color c)
        {
            return new BGRA()
            {
                A = c.A,
                G = c.G,
                B = c.B,
                R = c.R
            };
        }

        public static HSV ToHsv(this System.Drawing.Color c)
        {
            return new HSV()
            {
                H = c.GetHue(),
                S = c.GetSaturation(),
                V = c.GetBrightness(),
            };
        }

        #endregion

        #region ABCtoX

        /// <summary>
        /// http://www.rapidtables.com/convert/color/hsl-to-rgb.htm
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static RGB ToRgb(double h, double s, double l)
        {
            double C = (1 - Math.Abs(2 * l - 1)) * s;
            double X = C * (1 - Math.Abs((h / 60.0) % 2 - 1));
            double m = l - C / 2.0;


            double R, G, B;

            if (0.0 <= h && h < 60.0)
            {
                R = C;
                G = X;
                B = 0;
            }
            else if (60.0 <= h && h < 120.0)
            {
                R = X;
                G = C;
                B = 0;
            }
            else if (120.0 <= h && h < 180.0)
            {
                R = 0;
                G = C;
                B = X;
            }
            else if (180.0 <= h && h < 240.0)
            {
                R = 0;
                G = X;
                B = C;
            }
            else if (240.0 <= h && h < 300.0)
            {
                R = X;
                G = 0;
                B = C;
            }
            else // if (300.0 <= h && h < 360.0)
            {
                R = C;
                G = 0;
                B = X;
            }

            return new RGB(255.0 * (R + m), 255.0 * (G + m), 255.0 * (B + m));
        }


        public static HSV ToHsv(double r, double g, double b)
        {
            double R = r / (double)byte.MaxValue;
            double G = g / (double)byte.MaxValue;
            double B = b / (double)byte.MaxValue;

            // Cmax
            var Cmax = (R > B ? R : B);
            Cmax = Cmax > G ? Cmax : G;

            // Cmin
            var Cmin = (R < B ? R : B);
            Cmin = Cmin < G ? Cmin : G;

            // Δ
            double c = Cmax - Cmin;

            double H = 0.0;
            double S = 0.0;
            double V = Cmax;

            if (c != 0.0)
            {
                if (Cmax == R)
                {
                    H = ((G - B) / c) % 6.0;
                }
                else if (Cmax == G)
                {
                    H = 2.0 + (B - R) / c;
                }
                else // Cmax == B
                {
                    H = 4.0 + (R - G);
                }

                H *= 60.0;
                S = c / Cmax;
            }

            if (H < 0.0) H += 360;
            if (S > 1.0) S = 1.0;

            return new HSV() { H = H, S = S, V = V };
        }

        /// <summary>
        /// http://www.rapidtables.com/convert/color/rgb-to-hsl.htm
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static HSL ToHsl(double r, double g, double b)
        {
            const double oneHalf = 1.0 / 2.0;

            // R', G', B'
            double R = r / 255.0;
            double G = g / 255.0;
            double B = b / 255.0;

            // Cmax
            var Cmax = (R > B ? R : B);
            Cmax = Cmax > G ? Cmax : G;

            // Cmin
            var Cmin = (R < B ? R : B);
            Cmin = Cmin < G ? Cmin : G;

            // Δ
            double c = Cmax - Cmin;

            double S = 0.0;
            double H = 0.0;
            double L = (Cmax + Cmin) * oneHalf;

            // non achromatic
            if (c != 0.0)
            {
                // Cmax = R'
                if (Cmax == R)
                {
                    H = ((G - B) / c) % 6.0;
                }
                else if (Cmax == G)
                {
                    H = 2.0 + (B - R) / c;
                }
                else if (Cmax == B)
                {
                    H = 4.0 + (R - G) / c;
                }

                H *= 60.0;
                S = c / (1.0 - Math.Abs(2 * L - 1));
            }

            if (H < 0.0) H += 360;
            if (S > 1.0) S = 1.0;

            return new HSL() { H = H, S = S, L = L };
        }

        #endregion
    }

    /// <summary>
    /// Static Class for Converting Between Image Formats
    /// </summary>
    public static class ImageConversion
    {
        private static void CheckEqualDimensions<T>(params IImage<T>[] images)
            where T : struct, IEquatable<T>
        {
            if (images == null)
            {
                throw new ArgumentNullException(nameof(images), "Images passed in are null");
            }

            // 0 or 1 image
            // Nothing to compare against
            if (images.Length < 2)
            {
                return;
            }

            // Get Dimensions of the first image
            int width = images[0].Width;
            int height = images[0].Height;

            // Iterate through other images
            for (int i = 1; i < images.Length; i++)
            {
                IImage<T> img = images[i];

                // Check that the dimensions are equal
                if (img.Height != height || img.Width != width)
                {
                    throw new ArgumentException("Images must have same proportions");
                }
            }
        }

        #region Extensions

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Output color representation</typeparam>
        /// <typeparam name="Y">Input color representation</typeparam>
        /// <param name="data"></param>
        /// <param name="convertFunc"></param>
        /// <returns></returns>
        private static T[] FastConvert<T, Y>(IImage<Y> data, Func<Y, T> convertFunc)
            where T : struct, IEquatable<T>
            where Y : struct, IEquatable<Y>
        {
            T[] newData = new T[data.Length];

            Parallel.For(0, data.Length, i =>
            {
                newData[i] = convertFunc(data[i]);
            });

            return newData;
        }

        /// <summary>
        /// Generates an Image of the Magnitude scaled up by a constant.
        /// 
        /// Intended for easy viewing.
        /// 
        /// Formula: (double)(byte)(Scale * LOG(1 + Magnitude) / MaxValue)
        /// </summary>
        /// <param name="image">Complex Image</param>
        /// <param name="scale">How much to emphasize the magnitude</param>
        /// <returns></returns>
        public static IImage<double> Magnitude(this IImage<Complex> image, double scale)
        {
            Func<Complex, double> fun =
                x => Math.Log(1 + x.Magnitude);

            // Get Magnitude
            var mag = image.Data
                .Select(fun)
                .ToArray();

            double max = mag.Max();

            // Apply Scale
            Parallel.For(0, mag.Length,
                i => mag[i] = (double)(byte)(scale * mag[i] / max));

            return new Image(image.Width, image.Height, mag);
        }

        public static IImage<double> MagnitudePart(this IImage<Complex> image)
        {
            return new Image(
                image.Width,
                image.Height,
                image.Select(x => x.Magnitude).ToArray()
                );
        }

        public static IImage<double> Phase(this IImage<Complex> image)
        {
            Func<Complex, double> fun =
                x => Math.Log(1 + Math.Abs(x.Phase));

            var data = FastConvert(image, fun);
            var max = data.Max();

            return new Image(
                image.Width,
                image.Height,
                data.Select(x => (double)(byte)(255.0 * x / max)).ToArray()
                );
        }

        public static IImage<double> PhasePart(this IImage<Complex> image)
        {
            return new Image(image.Width, image.Height, image.Select(x => x.Phase).ToArray());
        }

        public static IImage<double> ToGrayscale(this IImage<RGB> img)
        {
            return new Image(
                    img.Width,
                    img.Height,
                    FastConvert(img, TypeConversion.ToGrayscale));
        }

        public static IImage<RGB> ToColor(this IImage<double> img)
        {
            return new RGBImage(
                 img.Width,
                 img.Height,
                 FastConvert(img, TypeConversion.ToRgb));
        }

        public static IImage<RGB> ToColor(this IImage<Complex> img)
        {
            return new RGBImage(
                img.Width,
                img.Height,
                ComplexToColor(img));
        }

        public static IImage<RGB> ToRgb(this IImage<HSL> img)
        {
            return new RGBImage(
                img.Width,
                img.Height,
                FastConvert(img, TypeConversion.ToRgb));
        }

        public static IImage<HSL> ToHsl(this IImage<RGB> img)
        {
            var normalizedImage = img.Copy().Normalize256();

            return new HslImage(
                img.Width,
                img.Height,
                FastConvert(normalizedImage, TypeConversion.ToHsl));
        }

        public static IImage<HSL> ToHsl(this IImage<double> img)
        {
            var normalizedImage = img.Normalize256();

            return new HslImage(
                img.Width,
                img.Height,
                FastConvert(normalizedImage, TypeConversion.ToHsl));
        }

        public static IImage<Complex> ToComplexImage(this IImage<double> img)
        {
            Func<double, Complex> conv = x => x;

            return new ComplexImage(img.Width, img.Height, FastConvert(img, conv));
        }

        private static RGB[] ComplexToColor(this IImage<Complex> data)
        {
            var colorImage = new RGB[data.Length];

            double maximum = double.MinValue;
            double minimum = double.MaxValue;

            for (int i = 0; i < colorImage.Length; i++)
            {
                double x = data[i].Real;
                double y = data[i].Imaginary;
                if (x > maximum) maximum = x;
                if (x < minimum) minimum = x;
                if (y > maximum) maximum = y;
                if (y < minimum) minimum = y;
            }

            double scale = 2.0 / (maximum - minimum);

            Parallel.For(0, colorImage.Length,
                (i) =>
                {
                    double x = scale * data[i].Real;
                    double y = scale * data[i].Imaginary;

                    double radius = Math.Sqrt(x * x + y * y);
                    double a = 0.40824829046386301636 * x;
                    double b = 0.70710678118654752440 * y;
                    double d = 1.0 / (1.0 + radius * radius);
                    double R = 0.5 + 0.81649658092772603273 * x * d;
                    double G = 0.5 - d * (a - b);
                    double B = 0.5 - d * (a + b);
                    d = 0.5 - radius * d;
                    if (radius < 1) d = -d;

                    colorImage[i].R = R + d;
                    colorImage[i].G = G + d;
                    colorImage[i].B = B + d;
                }
            );

            return colorImage;
        }

        public static IImage<RGB> FromHslToRgb(IImage<double> hue, IImage<double> saturation, IImage<double> intensity)
        {
            RGB[] result = new RGB[hue.Length];

            Parallel.For(0, hue.Length, i =>
            {
                result[i] = TypeConversion.ToRgb(hue[i], saturation[i], intensity[i]);
            });

            return new RGBImage(hue.Width, hue.Height, result);
        }

        public static IImage<BGRA> ToBgra(IImage<double> blue, IImage<double> green, IImage<double> red, IImage<double> alpha)
        {
            CheckEqualDimensions(blue, green, red, alpha);

            // Normalize to [0 - 255]
            blue = blue.Normalize256();
            green = green.Normalize256();
            red = red.Normalize256();
            alpha = alpha.Normalize256();

            // Construct BGRA values
            BGRA[] data = new BGRA[blue.Length];
            Parallel.For(0, blue.Length, i =>
            {
                data[i] = new BGRA()
                {
                    B = (byte)blue[i],
                    G = (byte)green[i],
                    R = (byte)red[i],
                    A = (byte)alpha[i]
                };
            });

            return new BgraImage(blue.Width, blue.Height, data);
        }

        public static IImage<BGRA> ToBgra(IImage<double> blue, IImage<double> green, IImage<double> red)
        {
            CheckEqualDimensions(blue, green, red);

            // Normalize to [0 - 255]
            blue = blue.Normalize256();
            green = green.Normalize256();
            red = red.Normalize256();

            // Consruct BGRA values
            BGRA[] data = new BGRA[blue.Length];
            Parallel.For(0, blue.Length, i =>
            {
                data[i] = new BGRA() { B = (byte)blue[i], G = (byte)green[i], R = (byte)red[i] };
            });

            return new BgraImage(blue.Width, blue.Height, data);
        }

        public static IImage<RGB> ToRgb(IImage<double> red, IImage<double> green, IImage<double> blue)
        {
            if (red.Width != green.Width || blue.Width != green.Width)
            {
                throw new ArgumentException("Images must have the same width");
            }

            if (red.Height != green.Height || blue.Height != green.Height)
            {
                throw new ArgumentException("Images must have the same height");
            }

            var result = new RGBImage(red.Width, red.Height);

            RGB[] resultData = result.Data;
            for (int i = 0; i < resultData.Length; i++)
            {
                resultData[i] = new RGB(red[i], green[i], blue[i]);
            }

            return result;
        }

        #endregion

        public static RGB ToRgb(this Complex c, double scale)
        {
            double x = scale * c.Real;
            double y = scale * c.Imaginary;

            double a = 0.40824829046386301636 * x;
            double b = 0.70710678118654752440 * y;
            double radius = Math.Sqrt(x * x + y * y);
            double d = 1.0 / (1.0 + radius * radius);
            double R = 0.5 + 0.81649658092772603273 * x * d;
            double G = 0.5 - d * (a - b);
            double B = 0.5 - d * (a + b);
            d = 0.5 - radius * d;
            if (radius < 1) d = -d;

            return new RGB() { R = R + d, G = G + d, B = B + d };
        }

        private static X ComplexToX<X>(Complex c, double scale, Func<double, double, double, X> func)
        {
            double x = scale * c.Real;
            double y = scale * c.Imaginary;

            double radius = Math.Sqrt(x * x + y * y);
            double a = 0.40824829046386301636 * x;
            double b = 0.70710678118654752440 * y;
            double d = 1.0 / (1.0 + radius * radius);
            double R = 0.5 + 0.81649658092772603273 * x * d;
            double G = 0.5 - d * (a - b);
            double B = 0.5 - d * (a + b);
            d = 0.5 - radius * d;
            if (radius < 1) d = -d;

            return func(R + d, G + d, B + d);
        }

        public static YCbCr ToYCbCr_JPEG(this BGRA color)
        {
            double R = color.R;
            double G = color.G;
            double B = color.B;

            double Y  = R *  0.29900 + G *  0.58700 + B *  0.11400;
            double Cb = R * -0.16874 + G * -0.33126 + B *  0.50000 + 128;
            double Cr = R *  0.50000 + G * -0.41869 + B * -0.08131 + 128;

            return new YCbCr() { Y = (byte)Y, Cb = (byte)Cb, Cr = (byte)Cr };
        }

        public static RGB ToRgb_JPEG(this YCbCr ycbcr)
        {
            return YCbCrToX_JPEG(ycbcr, (r, g, b) =>
                new RGB() { R = r, G = g, B = b });
        }

        public static BGRA ToBgra_JPEG(this YCbCr ycbcr)
        {
            return YCbCrToX_JPEG(ycbcr, (r, g, b) =>
                new BGRA() { R = (byte)r, G = (byte)g, B = (byte)b, A = byte.MaxValue });
        }

        private static X YCbCrToX_JPEG<X>(YCbCr ycbcr, Func<double, double, double, X> func)
        {
            double Y = ycbcr.Y;
            double Cr = ycbcr.Cr;
            double Cb = ycbcr.Cb;

            double R = Y                        + 1.40200 * (Cr - 128);
            double G = Y - 0.34414 * (Cb - 128) - 0.71414 * (Cr - 128);
            double B = Y + 1.77200 * (Cb - 128);

            return func(R, G, B);
        }

        /// <summary>
        /// For standard definition TV applications (SDTV)
        /// </summary>
        /// <typeparam name="X"></typeparam>
        /// <param name="ycbcr"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private static X YCbCrToX_BT601<X>(YCbCr ycbcr, Func<double, double, double, X> func)
        {
            double Y = ycbcr.Y;
            double Cr = ycbcr.Cr;
            double Cb = ycbcr.Cb;

            // http://www.equasys.de/colorconversion.html
            double R = 1.164 * (Y - 16) + 1.596 * (Cr - 128);
            double G = 1.164 * (Y - 16) - 0.392 * (Cb - 128) - 0.813 * (Cr - 128);
            double B = 1.164 * (Y - 16) + 2.017 * (Cb - 128);

            return func(R, G, B);
        }

        /// <summary>
        /// For high definition TV (HDTV)
        /// </summary>
        /// <typeparam name="X"></typeparam>
        /// <param name="ycbcr"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private static X YCbCrToX_BT709<X>(YCbCr ycbcr, Func<double, double, double, X> func)
        {
            double Y = ycbcr.Y;
            double Cr = ycbcr.Cr;
            double Cb = ycbcr.Cb;

            // http://www.equasys.de/colorconversion.html
            double R = 1.164 * (Y - 16) + 1.596 * (Cr - 128);
            double G = 1.164 * (Y - 16) - 0.392 * (Cb - 128) - 0.813 * (Cr - 128);
            double B = 1.164 * (Y - 16) + 2.017 * (Cb - 128);

            return func(R, G, B);
        }
    }
}
