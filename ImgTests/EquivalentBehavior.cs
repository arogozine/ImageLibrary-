using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageLibrary;
using ImageLibrary.Extensions;
using System.IO;
using System.Drawing.Imaging;
using System.Globalization;
using System.Diagnostics;
using System.Numerics;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;

namespace ImgTests
{
    /// <summary>
    /// Summary description for EquivalentBehavior
    /// </summary>
    [TestClass]
    public class EquivalentBehavior : TestBase
    {
        #region Test Transforms

        public void Transforms<T>(IImage<T> image)
            where T : struct, IEquatable<T>
        {
            // Double Transpose should return the original image
            var trans = image.Transpose().Transpose();
            Assert.IsTrue(ImagesEqual(image, trans));

            // Double flip should return the same image
            var flipX = image.FlipX().FlipX();
            Assert.IsTrue(ImagesEqual(image, flipX));

            var flipY = image.FlipY().FlipY();
            Assert.IsTrue(ImagesEqual(image, flipY));

            var flipXY = image.FlipXY().FlipXY();
            Assert.IsTrue(ImagesEqual(image, flipXY));
        }

        [TestMethod]
        public void TestGrayscaleTransforms()
        {
            this.Transforms(ImageFactory.Generate(BunnyPath));
        }

        [TestMethod]
        public void TestHslTransforms()
        {
            this.Transforms(ImageFactory.GenerateHsl(BunnyPath));
        }

        [TestMethod]
        public void TestHsvTransforms()
        {
            this.Transforms(ImageFactory.GenerateHsv(BunnyPath));
        }

        [TestMethod]
        public void TestRgbTransforms()
        {
            this.Transforms(ImageFactory.GenerateRgb(BunnyPath));
        }

        #endregion

        #region Test Copy

        public static void TestCopyGeneric<T>(IImage<T> img)
            where T : struct, IEquatable<T>
        {
            T[] imgCopyData = img.Copy().Data;
            Array arrayGenericData = new T[img.Length];
            T[] arrayTdata = new T[img.Length];
            img.CopyTo(arrayGenericData, 0);
            img.CopyTo(arrayTdata, 0);

            // Must be Equal
            for (int i = 0; i < img.Length; i++)
            {
                object aVal = arrayGenericData.GetValue(i);
                T bVal = arrayTdata[i];
                T cVal = imgCopyData[i];

                Assert.IsTrue(aVal.Equals(bVal));
                Assert.IsTrue(aVal.Equals(cVal));
            }

            // Must not reference the same array
            int j = 0;
            for(; img[j].Equals(default(T)); j++);

            // Ensure that the img copy does not reference the same data as the original image
            img[j] = default(T);
            Assert.IsFalse(imgCopyData[j].Equals(default(T)));
        }

        [TestMethod]
        public void TestCopyGreyscale()
        {
            var img = ImageFactory.Generate(BunnyPath);
            TestCopyGeneric(img);
        }

        [TestMethod]
        public void TestCopyRgb()
        {
            var img = ImageFactory.GenerateRgb(BunnyPath);
            TestCopyGeneric(img);
        }

        [TestMethod]
        public void TestCopyHsl()
        {
            var img = ImageFactory.GenerateHsl(BunnyPath);
            TestCopyGeneric(img);
        }

        [TestMethod]
        public void TestCopyHsv()
        {
            var img = ImageFactory.GenerateHsv(BunnyPath);
            TestCopyGeneric(img);
        }

        [TestMethod]
        public void TestCopyBinary()
        {
            var img = ImageFactory.GenerateBinary(250, 250);
            img.MapIndex(i => i % 2 == 0);

            TestCopyGeneric(img);
        }

        #endregion

        #region Save Test Lossless

        private void SaveTestGeneric<T>(string fileName, Func<string, IImage<T>> generator)
            where T : struct, IEquatable<T>
        {
            IImage<T> img = generator(fileName);

            var supportedTypes = new[] {
                ImageFormat.Bmp,
                ImageFormat.Png, 
                ImageFormat.Tiff
            };

            foreach(var format in supportedTypes)
            {
                Debug.Print("format {0}", format);

                var tmpFileName = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetTempFileName()))
                    + "." + format.ToString().ToLower(CultureInfo.CurrentCulture);

                img.WriteImage(tmpFileName, format);
                IImage<T> img2 = generator(tmpFileName);
                Assert.IsTrue(ImagesEqual(img, img2));
                File.Delete(tmpFileName);
            }
        }

        [TestMethod]
        public void SaveImage()
        {
            // This needs to be a grayscale image
            SaveTestGeneric(FrogPath, ImageFactory.Generate);
        }

        [TestMethod]
        public void SaveRgbImage()
        {
            SaveTestGeneric(BunnyPath,ImageFactory.GenerateRgb);
        }

        [TestMethod]
        public void SaveBinary()
        {
            SaveTestGeneric(FrogPath, ImageFactory.GenerateBinary);
        }

        #endregion

        [TestMethod]
        public void AddSubtract()
        {
            IImage<double> bunny = ImageFactory.Generate(BunnyPath);
            // bunny + bunny
            IImage<double> added = ImageFunctions.Add(bunny, bunny);
            // bunny + bunny - bunny
            IImage<double> original = ImageFunctions.Subtract(added, bunny);

            // bunny + bunny - bunny == bunny
            Assert.IsTrue(ImagesEqual(bunny, original));
        }

        [TestMethod]
        public void AddSubtractRgb()
        {
            IImage<RGB> bunny = ImageFactory.GenerateRgb(BunnyPath);
            IImage<RGB> added = ImageFunctions.Add(bunny, bunny);
            IImage<RGB> original = ImageFunctions.Subtract(added, bunny);

            // bunny + bunny - bunny == bunny
            Assert.IsTrue(ImagesEqual(bunny, original));
        }

        [TestMethod]
        public void DoubleToFromComplex()
        {
            IImage<double> bunny = ImageFactory.Generate(BunnyPath);
            IImage<Complex> complxBunny = bunny.ToComplexImage();
            IImage<double> bunny2 = complxBunny.RealPart();

            // Real(Complex(bunny)) == bunny
            Assert.IsTrue(ImagesEqual(bunny, bunny2));
        }

        #region Equivalence

        public void Equivalence<T>(IImage<T> img)
            where T : struct, IEquatable<T>
        {
            Assert.IsTrue(img.Length == img.Height * img.Width);
            Assert.IsTrue(img.Cols == img.Width);
            Assert.IsTrue(img.Rows == img.Height);
            Assert.IsTrue(img.Length == ((ICollection)img).Count);
            Assert.IsTrue(img.Length == ((ICollection<T>)img).Count);
            Assert.IsTrue(img.Length == img.Count());
            Assert.IsTrue(img.Length == ((IReadOnlyList<T>)img).Count);

            T[] data = img.Data;
            Assert.IsTrue(img.Length == data.Length);

            for (int i = 0; i < img.Length; i++)
            {
                int x = i % img.Width;
                int y = i / img.Height;
                T one = img[i];
                T two = img[y, x];
                Assert.IsTrue(one.Equals(two));
            }
        }

        [TestMethod]
        public void TestEquivalenceDouble()
        {
            Equivalence(ImageFactory.Generate(BunnyPath));
        }

        [TestMethod]
        public void TestEquivalenceRgb()
        {
            Equivalence(ImageFactory.GenerateRgb(BunnyPath));
        }

        [TestMethod]
        public void TestEquivalenceComplex()
        {
            Equivalence(ImageFactory.Generate(BunnyPath).ToComplexImage());
        }

        [TestMethod]
        public void TestEquivalenceHsv()
        {
            Equivalence(ImageFactory.GenerateHsv(BunnyPath));
        }

        [TestMethod]
        public void TestEquivalenceHsl()
        {
            Equivalence(ImageFactory.GenerateHsl(BunnyPath));
        }

        [TestMethod]
        public void TestEquivalenceCmyk()
        {
            Equivalence(ImageFactory.GenerateCmyk(BunnyPath));
        }

        [TestMethod]
        public void TestEquivalenceBgra()
        {
            Equivalence(ImageFactory.GenerateBgra(BunnyPath));
        }

        #endregion

        [TestMethod]
        public void FftTests()
        {
            IImage<double> bunny = ImageFactory.Generate(BunnyPath)
                                .Crop(0, 0, 512, 512);

            IImage<double> bunny2 = bunny.Fft().InverseFft().Normalize256();
            
            // IMG = INVFFT( FFT(IMG) )
            Assert.IsTrue(ImagesEqualWithinEpsilon(bunny, bunny2, (x, y) => Math.Abs(x - y) < 0.05));
        }

        [TestMethod]
        public void FftFilterPassAll()
        {
            IImage<double> bunny = ImageFactory.Generate(BunnyPath)
                    .Crop(0, 0, 512, 512);

            IImage<double> bunny2 = bunny.ApplyFilter((x, y) => 1.0).Normalize256();

            // Filter is pass through (multiplication w/ 1 for all frequencies)
            // IMG = INVFFT( FFT(IMG) )
            Assert.IsTrue(ImagesEqualWithinEpsilon(bunny, bunny2, (x, y) => Math.Abs(x - y) < 0.05));
        }

        [TestMethod]
        public void ShiftNoShiftSameFft()
        {
            // Expectation
            // INVFFT(SHIFTEDFFT(IMG) * SHIFTEDFILTER) == INVFFT(FFT(IMG) * FILTER)

            IImage<double> bunny = ImageFactory.Generate(BunnyPath)
                .Crop(0, 0, 512, 512);

            // Shift and NoShift FFT
            var noShiftFft = bunny.Fft(false);
            var shiftFft = bunny.Fft(true);

            // Shift and NoShift Filter
            var noShiftfilter = Filters.MakeFilter(512, 512, Filters.IdealLowPass(50));
            var shiftFilter = Filters.MakeFilterWithOffset(512, 512, Filters.IdealLowPass(50));

            // Multiply Filter by FFT
            IImage<Complex> shift = ImageFactory.GenerateComplex(512, 512);
            IImage<Complex> noShift = ImageFactory.GenerateComplex(512, 512);

            Parallel.For(0, bunny.Length, i =>
                {
                    shift[i] = shiftFilter[i] * shiftFft[i];
                    noShift[i] = noShiftfilter[i] * noShiftFft[i];
                });

            // Get Inverse FFT
            var shiftResult = shift.InverseFft();
            var noShiftResult = noShift.InverseFft();

            Assert.IsTrue(ImagesEqualWithinEpsilon(noShiftResult, shiftResult, (x, y) => Math.Abs(x - y) < 0.05));
        }

        /*
        [TestMethod]
        public void RotateTests()
        {
            IImage<double> bunny = ImageFactory.Generate(BunnyPath);
            var bunny360 = bunny.Rotate(360);

            // Rotate(360, Original) == Original
            Assert.IsTrue(ImagesEqual(bunny, bunny360));
        }
        */
    }
}
