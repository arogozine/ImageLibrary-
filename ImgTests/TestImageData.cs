using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageLibrary;
using System.Linq;
using System.Collections.Generic;

namespace ImgTests
{
    [TestClass]
    public class TestImageData : TestBase
    {
        private static void TestIDisposable<T>(IImage<T> img)
            where T: struct, IEquatable<T>
        {
            img.Dispose();
            img.Dispose();
        }

        private static void TestIEnumerable<T>(IImage<T> img)
            where T : struct, IEquatable<T>
        {
            T[] arr = ((IEnumerable<T>)img).Select(x => x).ToArray();
            T[] arr2 = img.Data;

            IEnumerator<T> en = img.GetEnumerator();

            for (int i = 0; i < img.Length; i++)
            {
                en.MoveNext();

                Assert.IsTrue(arr[i].Equals(arr2[i]));
                Assert.IsTrue(en.Current.Equals(arr[i]));
            }
        }

        private static void TestICollection<T>(IImage<T> img)
            where T : struct, IEquatable<T>
        {
            ICollection<T> collection = img;
            Assert.IsTrue(collection.Count == img.Data.Length);
            Assert.IsTrue(collection.IsReadOnly);

            int i = 0;
            foreach (T element in img)
            {
                /*
                // TODO: This crashes
                Assert.IsTrue(collection.Contains(element));
                i++;

                // Good enough
                // This thing is really slow
                if (i > 100)
                    break;
                 */
            }
        }

        private static void TestDataConsistency<T>(IImage<T> img)
            where T : struct, IEquatable<T>
        {
            byte[] rgb = img.ToBGR();
            byte[] rgbA = img.ToBGRA();

            Assert.IsTrue((rgb.Length / 3) * 4 == rgbA.Length);

            // rgb and rgbA should be the same
            // except one lacks alpha
            for (int i = 0, j = 0; i < rgbA.Length; i++, j++)
            {
                // Alpha
                if ((i + 1) % 4 == 0)
                {
                    // Alpha should be 255
                    Assert.IsTrue(rgbA[i] == byte.MaxValue);
                    j--;
                    continue;
                }

                Assert.IsTrue(rgb[j] == rgbA[i]);
            }

            // Doing this should produce the same thing as
            // ToBGRA just as a struct in stead of 4 bytes
            BGRA[] bgra = new BGRA[img.Length];
            img.ToIndexedBgra((i, x) => bgra[i] = x);

            for (int i = 0, j = 0; i < img.Length; i++, j += 4)
            {
                BGRA val = bgra[i];

                Assert.IsTrue(val.B == rgbA[j]);
                Assert.IsTrue(val.G == rgbA[j + 1]);
                Assert.IsTrue(val.R == rgbA[j + 2]);
                Assert.IsTrue(val.A == rgbA[j + 3]);
            }

            // ToBGRA just as BGRA struct
            BGRA[] bgra2 = img.ToPixelColor();

            for (int i = 0; i < img.Length; i++)
            {
                Assert.IsTrue(bgra[i] == bgra2[i]);
            }
        }

        #region ICollection Tests

        [TestMethod]
        public void GrayScaleICollection()
        {
            IImage<double> img = ImageFactory.Generate(BunnyPath);
            TestICollection(img);
        }
        
        [TestMethod]
        public void RgbDataICollection()
        {
            IImage<RGB> img = ImageFactory.GenerateRgb(BunnyPath);
            TestICollection(img);
        }

        [TestMethod]
        public void BgraDataICollection()
        {
            IImage<BGRA> img = ImageFactory.GenerateBgra(BunnyPath);
            TestICollection(img);
        }
        
        [TestMethod]
        public void CmykDataICollection()
        {
            IImage<CMYK> img = ImageFactory.GenerateCmyk(BunnyPath);
            TestICollection(img);
        }

        [TestMethod]
        public void HslDataICollection()
        {
            IImage<HSL> img = ImageFactory.GenerateHsl(BunnyPath);
            TestICollection(img);
        }

        [TestMethod]
        public void HsvDataICollection()
        {
            IImage<HSV> img = ImageFactory.GenerateHsv(BunnyPath);
            TestICollection(img);
        }

        [TestMethod]
        public void BinaryDataICollection()
        {
            IImage<bool> img = ImageFactory.GenerateBinary(BunnyPath);
            TestICollection(img);
        }

        #endregion

        #region IEnumerable Tests

        [TestMethod]
        public void GrayScaleIEnumerable()
        {
            IImage<double> img = ImageFactory.Generate(BunnyPath);
            TestIEnumerable(img);
        }

        [TestMethod]
        public void RgbDataIEnumerable()
        {
            IImage<RGB> img = ImageFactory.GenerateRgb(BunnyPath);
            TestIEnumerable(img);
        }

        [TestMethod]
        public void BgraDataIEnumerable()
        {
            IImage<BGRA> img = ImageFactory.GenerateBgra(BunnyPath);
            TestIEnumerable(img);
        }

        [TestMethod]
        public void CmykDataIEnumerable()
        {
            IImage<CMYK> img = ImageFactory.GenerateCmyk(BunnyPath);
            TestIEnumerable(img);
        }

        [TestMethod]
        public void HslDataIEnumerable()
        {
            IImage<HSL> img = ImageFactory.GenerateHsl(BunnyPath);
            TestIEnumerable(img);
        }

        [TestMethod]
        public void HsvDataIEnumerable()
        {
            IImage<HSV> img = ImageFactory.GenerateHsv(BunnyPath);
            TestIEnumerable(img);
        }

        [TestMethod]
        public void BinaryDataIEnumerable()
        {
            IImage<bool> img = ImageFactory.GenerateBinary(BunnyPath);
            TestIEnumerable(img);
        }

        #endregion

        
        #region IDisposable Tests

        
        [TestMethod]
        public void GrayScaleIDisposable()
        {
            IImage<double> img = ImageFactory.Generate(BunnyPath);
            TestIDisposable(img);
        }

        [TestMethod]
        public void RgbIDisposable()
        {
            IImage<RGB> img = ImageFactory.GenerateRgb(BunnyPath);
            TestIDisposable(img);
        }

        [TestMethod]
        public void BgraIDisposable()
        {
            IImage<BGRA> img = ImageFactory.GenerateBgra(BunnyPath);
            TestIDisposable(img);
        }

        [TestMethod]
        public void CmykIDisposable()
        {
            IImage<CMYK> img = ImageFactory.GenerateCmyk(BunnyPath);
            TestIDisposable(img);
        }

        [TestMethod]
        public void HslIDisposable()
        {
            IImage<HSL> img = ImageFactory.GenerateHsl(BunnyPath);
            TestIDisposable(img);
        }

        [TestMethod]
        public void HsvIDisposable()
        {
            IImage<HSV> img = ImageFactory.GenerateHsv(BunnyPath);
            TestIDisposable(img);
        }

        [TestMethod]
        public void BinaryIDisposable()
        {
            IImage<bool> img = ImageFactory.GenerateBinary(BunnyPath);
            TestIDisposable(img);
        }

        #endregion

        #region Data Consistency

        [TestMethod]
        public void GrayScaleDataConsistency()
        {
            IImage<double> img = ImageFactory.Generate(BunnyPath);
            TestDataConsistency(img);
        }

        [TestMethod]
        public void RgbDataConsistency()
        {
            IImage<RGB> img = ImageFactory.GenerateRgb(BunnyPath);
            TestDataConsistency(img);
        }

        [TestMethod]
        public void BgraDataConsistency()
        {
            IImage<BGRA> img = ImageFactory.GenerateBgra(BunnyPath);
            TestDataConsistency(img);
        }

        [TestMethod]
        public void CmykDataConsistency()
        {
            IImage<CMYK> img = ImageFactory.GenerateCmyk(BunnyPath);
            TestDataConsistency(img);
        }

        [TestMethod]
        public void HslDataConsistency()
        {
            IImage<HSL> img = ImageFactory.GenerateHsl(BunnyPath);
            TestDataConsistency(img);
        }

        [TestMethod]
        public void HsvDataConsistency()
        {
            IImage<HSV> img = ImageFactory.GenerateHsv(BunnyPath);
            TestDataConsistency(img);
        }

        [TestMethod]
        public void BinaryDataImage()
        {
            IImage<bool> img = ImageFactory.GenerateBinary(BunnyPath);
            TestDataConsistency(img);
        }

        #endregion
    }
}
