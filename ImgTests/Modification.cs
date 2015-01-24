using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageLibrary;
using ImageLibrary.Extensions;
using System.Numerics;
using System.Linq;

namespace ImgTests
{
    [TestClass]
    public class Modification : TestBase
    {
        private static void TestModify<T>(IImage<T> img)
            where T : struct, IEquatable<T>
        {
            var list = img.ToList();

            for (int i = 0; i < img.Length; i++)
            {
                if (!img[i].Equals(default(T)))
                {
                    img[i] = default(T);
                    Assert.IsTrue(img[i].Equals(default(T)));
                    break;
                }
            }

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    if (!img[y, x].Equals(default(T)))
                    {
                        img[y, x] = default(T);
                        Assert.IsTrue(img[y, x].Equals(default(T)));
                        break;
                    }
                }
            }
        }

        #region Throw on Clear()

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestClearDouble()
        {
            ImageFactory.Generate(50, 50).Clear();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestClearRgb()
        {
            ImageFactory.GenerateRgb(50, 50).Clear();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestClearComplex()
        {
            ImageFactory.GenerateComplex(50, 50).Clear();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestClearHsv()
        {
            ImageFactory.GenerateHsv(50, 50).Clear();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestClearHsl()
        {
            ImageFactory.GenerateHsl(50, 50).Clear();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestClearCmyk()
        {
            ImageFactory.GenerateCmyk(50, 50).Clear();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestClearBgra()
        {
            ImageFactory.GenerateBgra(50, 50).Clear();
        }

        #endregion

        #region Throw on Remove()

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestRemoveDouble()
        {
            ImageFactory.Generate(50, 50).Remove(default(double));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestRemoveRgb()
        {
            ImageFactory.GenerateRgb(50, 50).Remove(default(RGB));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestRemoveComplex()
        {
            ImageFactory.GenerateComplex(50, 50).Remove(default(Complex));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestRemoveHsv()
        {
            ImageFactory.GenerateHsv(50, 50).Remove(default(HSV));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestRemoveHsl()
        {
            ImageFactory.GenerateHsl(50, 50).Remove(default(HSL));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestRemoveCmyk()
        {
            ImageFactory.GenerateCmyk(50, 50).Remove(default(CMYK));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestRemoveBgra()
        {
            ImageFactory.GenerateBgra(50, 50).Remove(default(BGRA));
        }

        #endregion

        #region Throw on Add()

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestAddDouble()
        {
            ImageFactory.Generate(50, 50).Add(default(double));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestAddRgb()
        {
            ImageFactory.GenerateRgb(50, 50).Add(default(RGB));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestAddComplex()
        {
            ImageFactory.GenerateComplex(50, 50).Add(default(Complex));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestAddHsv()
        {
            ImageFactory.GenerateHsv(50, 50).Add(default(HSV));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestAddHsl()
        {
            ImageFactory.GenerateHsl(50, 50).Add(default(HSL));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestAddCmyk()
        {
            ImageFactory.GenerateCmyk(50, 50).Add(default(CMYK));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestAddBgra()
        {
            ImageFactory.GenerateBgra(50, 50).Add(default(BGRA));
        }

        #endregion

        #region IsReadOnly is True

        [TestMethod]
        public void TestIsReadOnlyDouble()
        {
            Assert.IsTrue(ImageFactory.Generate(50, 50).IsReadOnly);
        }

        [TestMethod]
        public void TestIsReadOnlyRgb()
        {
            Assert.IsTrue(ImageFactory.GenerateRgb(50, 50).IsReadOnly);
        }

        [TestMethod]
        public void TestIsReadOnlyComplex()
        {
            Assert.IsTrue(ImageFactory.GenerateComplex(50, 50).IsReadOnly);
        }

        [TestMethod]
        public void TestIsReadOnlyHsv()
        {
            Assert.IsTrue(ImageFactory.GenerateHsv(50, 50).IsReadOnly);
        }

        [TestMethod]
        public void TestIsReadOnlyHsl()
        {
            Assert.IsTrue(ImageFactory.GenerateHsl(50, 50).IsReadOnly);
        }

        [TestMethod]
        public void TestIsReadOnlyCmyk()
        {
            Assert.IsTrue(ImageFactory.GenerateCmyk(50, 50).IsReadOnly);
        }

        [TestMethod]
        public void TestIsReadOnlyBgra()
        {
            Assert.IsTrue(ImageFactory.GenerateBgra(50, 50).IsReadOnly);
        }

        #endregion

        #region IsSynchronized is False

        [TestMethod]
        public void TestIsSynchronizedDouble()
        {
            Assert.IsFalse(ImageFactory.Generate(50, 50).IsSynchronized);
        }

        [TestMethod]
        public void TestIsSynchronizedRgb()
        {
            Assert.IsFalse(ImageFactory.GenerateRgb(50, 50).IsSynchronized);
        }

        [TestMethod]
        public void TestIsSynchronizedComplex()
        {
            Assert.IsFalse(ImageFactory.GenerateComplex(50, 50).IsSynchronized);
        }

        [TestMethod]
        public void TestIsSynchronizedHsv()
        {
            Assert.IsFalse(ImageFactory.GenerateHsv(50, 50).IsSynchronized);
        }

        [TestMethod]
        public void TestIsSynchronizedHsl()
        {
            Assert.IsFalse(ImageFactory.GenerateHsl(50, 50).IsSynchronized);
        }

        [TestMethod]
        public void TestIsSynchronizedCmyk()
        {
            Assert.IsFalse(ImageFactory.GenerateCmyk(50, 50).IsSynchronized);
        }

        [TestMethod]
        public void TestIsSynchronizedBgra()
        {
            Assert.IsFalse(ImageFactory.GenerateBgra(50, 50).IsSynchronized);
        }

        #endregion

        #region TestModify

        [TestMethod]
        public void TestModifyDouble()
        {
            TestModify(ImageFactory.Generate(BunnyPath));
        }

        [TestMethod]
        public void TestModifyRgb()
        {
            TestModify(ImageFactory.GenerateRgb(BunnyPath));
        }

        [TestMethod]
        public void TestModifyComplex()
        {
            TestModify(ImageFactory.Generate(BunnyPath).ToComplexImage());
        }

        [TestMethod]
        public void TestModifyHsv()
        {
            TestModify(ImageFactory.GenerateHsv(BunnyPath));
        }

        [TestMethod]
        public void TestModifyHsl()
        {
            TestModify(ImageFactory.GenerateHsl(BunnyPath));
        }

        [TestMethod]
        public void TestModifyCmyk()
        {
            TestModify(ImageFactory.GenerateCmyk(BunnyPath));
        }

        [TestMethod]
        public void TestModifyBgra()
        {
            TestModify(ImageFactory.GenerateBgra(BunnyPath));
        }

        #endregion
    }
}
