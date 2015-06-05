using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using ImageLibrary.Extensions;
using System.Diagnostics;

namespace ImageLibrary
{
    /// <summary>
    /// Factory for making 
    /// </summary>
    public static class ImageFactory
    {
        #region Binary Image

        public static IImage<bool> GenerateBinary(string path)
        {
            return ReadToArray(path,
                (x) => x.B == 255 && x.G == 255 && x.R == 255,
                (x) => x.B == 255,
                (width, height, data) => new BinaryImage(width, height, data));
        }

        public static IImage<bool> GenerateBinary(System.Drawing.Image image)
        {
            return ReadToArray(image,
                (x) => x.B == 255 && x.G == 255 && x.R == 255,
                (x) => x.B == 255,
                (width, height, data) => new BinaryImage(width, height, data));            
        }

        public static IImage<bool> GenerateBinary(int width, int height)
        {
            ErrorChecker.CheckWidthHeight(width, height);

            return new BinaryImage(width, height, new bool[width*height]);
        }

        public static IImage<bool> GenerateBinary(int width, int height, bool[] data)
        {
            ErrorChecker.CheckWidthHeightData(width, height, data);

            return new BinaryImage(width, height, data);
        }

        public static IImage<bool> GenerateBinary(int width, int height, IEnumerable<bool> data)
        {
            bool[] dataArr = ErrorChecker.CheckWidthHeightData(width, height, data);

            return new BinaryImage(width, height, dataArr);
        }

        #endregion

        #region GrayScale

        public static IImage<double> Generate(string path)
        {
            return ReadToArray(path,
                TypeConversion.ToGrayscale,
                bgra => (double)bgra.B,
                (width, height, data) => new Image(width, height, data));
        }

        public static IImage<double> Generate(System.Drawing.Image img)
        {
            return ReadToArray(img,
                TypeConversion.ToGrayscale,
                bgra => (double)bgra.B,
                (width, height, data) => new Image(width, height, data));
        }

        public static IImage<double> Generate(int width, int height)
        {
            ErrorChecker.CheckWidthHeight(width, height);

            return new Image(width, height, new double[width*height]);
        }

        public static IImage<double> Generate(int width, int height, double[] data)
        {
            ErrorChecker.CheckWidthHeightData(width, height, data);

            return new Image(width, height, data);
        }

        public static IImage<double> Generate(int width, int height, IEnumerable<double> data)
        {
            double[] dataArr = ErrorChecker.CheckWidthHeightData(width, height, data);

            return new Image(width, height, dataArr);
        }

        #endregion

        #region Complex

        /// <summary>
        /// Generates a blank complex image
        /// </summary>
        /// <param name="width">Image Width</param>
        /// <param name="height">Image Height</param>
        /// <returns>"Blank" Complex Image</returns>
        public static IImage<Complex> GenerateComplex(int width, int height)
        {
            ErrorChecker.CheckWidthHeight(width, height);

            return new ComplexImage(width, height);
        }

        public static IImage<Complex> GenerateComplex(int width, int height, Complex[] data)
        {
            ErrorChecker.CheckWidthHeightData(width, height, data);

            return new ComplexImage(width, height, data);
        }

        public static IImage<Complex> GenerateComplex(int width, int height, IEnumerable<Complex> data)
        {
            Complex[] dataArray = ErrorChecker.CheckWidthHeightData(width, height, data);

            return new ComplexImage(width, height, dataArray);
        }

        #endregion

        #region RGB

        public static IImage<RGB> GenerateRgb(string path)
        {
            return ReadToArray(path,
                TypeConversion.ToRgb,
                bgra => new RGB() { B = bgra.B, R = bgra.R, G = bgra.G },
                (width, height, data) => new RGBImage(width, height, data));
        }

        public static IImage<RGB> GenerateRgb(System.Drawing.Image img)
        {
            return ReadToArray(img,
                TypeConversion.ToRgb,
                bgra => new RGB() { B = bgra.B, R = bgra.R, G = bgra.G },
                (width, height, data) => new RGBImage(width, height, data));
        }

        public static IImage<RGB> GenerateRgb(int width, int height)
        {
            ErrorChecker.CheckWidthHeight(width, height);

            return new RGBImage(width, height);
        }

        public static IImage<RGB> GenerateRgb(int width, int height, RGB[] data)
        {
            ErrorChecker.CheckWidthHeightData(width, height, data);

            return new RGBImage(width, height, data);
        }

        public static IImage<RGB> GenerateRgb(int width, int height, IEnumerable<RGB> data)
        {
            // Input check and convert IEnumerable<RGB> to RGB[]
            RGB[] dataArr = ErrorChecker.CheckWidthHeightData(width, height, data);

            return new RGBImage(width, height, dataArr);
        }

        #endregion

        #region CMYK

        public static IImage<CMYK> GenerateCmyk(string path)
        {
            return ReadToArray(path,
                TypeConversion.ToCmyk, TypeConversion.ToCmyk,
                (width, height, data) => new CmykImage(width, height, data));
        }

        public static IImage<CMYK> GenerateCmyk(System.Drawing.Image img)
        {
            return ReadToArray(img,
                TypeConversion.ToCmyk, TypeConversion.ToCmyk,
                (width, height, data) => new CmykImage(width, height, data));
        }

        public static IImage<CMYK> GenerateCmyk(int width, int height)
        {
            ErrorChecker.CheckWidthHeight(width, height);

            return new CmykImage(width, height, new CMYK[width * height]);
        }

        public static IImage<CMYK> GenerateCmyk(int width, int height, CMYK[] data)
        {
            ErrorChecker.CheckWidthHeightData(width, height, data);

            return new CmykImage(width, height, data);
        }

        public static IImage<CMYK> GenerateCmyk(int width, int height, IEnumerable<CMYK> data)
        {
            CMYK[] dataArr = ErrorChecker.CheckWidthHeightData(width, height, data);

            return new CmykImage(width, height, dataArr);
        }

        #endregion

        #region HSL

        public static IImage<HSL> GenerateHsl(string path)
        {
            return ReadToArray(path,
                TypeConversion.ToHsl,
                TypeConversion.ToHsl,
                (width, height, data) => new HslImage(width, height, data));
        }

        public static IImage<HSL> GenerateHsl(System.Drawing.Image img)
        {
            return ReadToArray(img,
                TypeConversion.ToHsl,
                TypeConversion.ToHsl,
                (width, height, data) => new HslImage(width, height, data));
        }

        public static IImage<HSL> GenerateHsl(int width, int height)
        {
            ErrorChecker.CheckWidthHeight(width, height);

            return new HslImage(width, height, new HSL[width * height]);
        }

        public static IImage<HSL> GenerateHsi(int width, int height, HSL[] data)
        {
            ErrorChecker.CheckWidthHeightData(width, height, data);

            return new HslImage(width, height, data);
        }

        public static IImage<HSL> GenerateHsi(int width, int height, IEnumerable<HSL> data)
        {
            HSL[] dataArr = ErrorChecker.CheckWidthHeightData(width, height, data);

            return new HslImage(width, height, dataArr);
        }

        #endregion

        #region HSV

        public static IImage<HSV> GenerateHsv(string path)
        {
            return ReadToArray(path,
                TypeConversion.ToHsv,
                TypeConversion.ToHsv,
                (width, height, data) => new HsvImage(width, height, data));
        }

        public static IImage<HSV> GenerateHsv(System.Drawing.Image image)
        {
            return ReadToArray(image,
                TypeConversion.ToHsv,
                TypeConversion.ToHsv,
                (width, height, data) => new HsvImage(width, height, data));        
        }

        public static IImage<HSV> GenerateHsv(int width, int height)
        {
            ErrorChecker.CheckWidthHeight(width, height);

            return new HsvImage(width, height, new HSV[width * height]);
        }

        public static IImage<HSV> GenerateHsv(int width, int height, HSV[] data)
        {
            ErrorChecker.CheckWidthHeightData(width, height, data);

            return new HsvImage(width, height, data);
        }

        public static IImage<HSV> GenerateHsv(int width, int height, IEnumerable<HSV> data)
        {
            HSV[] dataArr = ErrorChecker.CheckWidthHeightData(width, height, data);

            return new HsvImage(width, height, dataArr);
        }

        #endregion

        #region BGRA

        public static IImage<BGRA> GenerateBgra(string fileName)
        {
            return ReadToArray(fileName, 
                x => x, 
                x => x, 
                (w, h, data) => new BgraImage(w, h, data));
        }

        public static IImage<BGRA> GenerateBgra(System.Drawing.Image image)
        {
            return ReadToArray(image,
                x => x,
                x => x,
                (w, h, data) => new BgraImage(w, h, data));
        }

        public static IImage<BGRA> GenerateBgra(int width, int height)
        {
            ErrorChecker.CheckWidthHeight(width, height);

            return new BgraImage(width, height, new BGRA[width * height]);
        }

        public static IImage<BGRA> GenerateBgra(int width, int height, BGRA[] data)
        {
            ErrorChecker.CheckWidthHeightData(width, height, data);

            return new BgraImage(width, height, data);
        }

        public static IImage<BGRA> GenerateBgra(int width, int height, IEnumerable<BGRA> data)
        {
            BGRA[] dataArr = ErrorChecker.CheckWidthHeightData(width, height, data);

            return new BgraImage(width, height, dataArr);
        }

        #endregion

        private static unsafe IImage<T> ReadToArray<T>(System.Drawing.Image img, Func<BGRA, T> convColor, Func<BGRA, T> convBlack, Func<int, int, T[], IImage<T>> generator)
            where T : struct, IEquatable<T>
        {
            Bitmap bitmap = img as Bitmap ?? new Bitmap(img);

            Func<BGRA, T> conv = convColor;

            using (var bitmapInRgbFormat = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format32bppArgb))
            {
                BitmapData d = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmapInRgbFormat.PixelFormat);

                var width = bitmapInRgbFormat.Width;
                var height = bitmapInRgbFormat.Height;
                var data = new T[width * height];

                int i = 0;

                BGRA* ptr = (BGRA*)d.Scan0.ToPointer();
                BGRA* lastptr = ptr + (data.Length - 1);

                // We want to use the grayscale transform if possible to ensure most accurate conversion
                // So ensure that the file is color and not greyscale
                if (bitmap.PixelFormat != PixelFormat.Format16bppGrayScale)
                {
                    bool useGrayscale = true;

                    for (BGRA* ptr2 = ptr; ptr2 < lastptr; ptr2++)
                    {
                        if (ptr2->R != ptr2->G || ptr2->G != ptr2->B)
                        {
                            useGrayscale = false;
                            break;
                        }
                    }

                    if (useGrayscale)
                    {
                        // Use Grayscale Algorithm
                        conv = convBlack;
                    }
                }
                else
                {
                    // Use Grayscale Algorithm
                    conv = convBlack;
                }

                while (ptr <= lastptr)
                {
                    data[i++] = conv(*(ptr));
                    ptr++;
                }

                bitmap.UnlockBits(d);

                return generator(width, height, data);
            }
        }

        private static unsafe IImage<T> ReadToArray<T>(string filename, Func<BGRA, T> convColor, Func<BGRA, T> convBlack, Func<int, int, T[], IImage<T>> generator)
            where T : struct, IEquatable<T>
        {
            ErrorChecker.CheckImagePath(filename);

            using (var bitmap = new Bitmap(filename))
            {
                Debug.Print("{0}", bitmap.PixelFormat);
                return ReadToArray(bitmap, convColor, convBlack, generator);
            }
        }
    }
}