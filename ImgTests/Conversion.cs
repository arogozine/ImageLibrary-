using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageLibrary;
using ImageLibrary.Extensions;

namespace ImgTests
{
    /// <summary>
    /// Summary description for Conversion
    /// </summary>
    [TestClass]
    public class Conversion : TestBase
    {

        private static void PermuteRgb(Action<RGB> action)
        {
            for (double r = byte.MinValue; r < byte.MaxValue; r++)
            {
                for (double g = byte.MinValue; r < byte.MaxValue; r++)
                {
                    for (double b = byte.MinValue; r < byte.MaxValue; r++)
                    {
                        var rgb = new RGB() { R = r, G = g, B = b };
                        action(rgb);
                    }
                }
            }        
        }

        private static bool WithinEpsilon(double epsilon, RGB a, RGB b)
        {
            return
                Math.Abs(a.R - b.R) < epsilon &&
                Math.Abs(a.G - b.G) < epsilon &&
                Math.Abs(a.B - b.B) < epsilon;
        }

        [TestMethod]
        public void RgbToHsl()
        {
            Conversion.PermuteRgb(rgb => 
                {
                    HSL hsl = rgb.ToHsl();
                    RGB rgb2 = hsl.ToRgb();
                    Assert.IsTrue(WithinEpsilon(0.05, rgb, rgb2));
                });
        }

        [TestMethod]
        public void TestHSLConversion()
        {
            IImage<RGB> rgb = ImageFactory.GenerateRgb(BunnyPath);
            IImage<RGB> rgb2hsl2rgb = rgb.ToHsl().ToRgb();

            // Check for minimal loss due to conversion
            ImagesEqualWithinEpsilon(rgb, rgb2hsl2rgb, (a, b) => WithinEpsilon(0.05, a, b));
        }

        [TestMethod]
        public void TestBgraStruct()
        {
            BGRA black = BGRA.Black;

            int blackInt32 = black;
            UInt32 blackUInt32 = black;

            Assert.IsTrue(((BGRA)blackInt32) == black);
            Assert.IsTrue(((BGRA)blackUInt32) == black);
        }

        [TestMethod]
        public void ToBgra()
        {
            // GrayScale -> BGRA
            var bgraFromDouble = TypeConversion.ToBgra(default(double));

            Assert.IsTrue(bgraFromDouble.B == byte.MinValue);
            Assert.IsTrue(bgraFromDouble.G == byte.MinValue);
            Assert.IsTrue(bgraFromDouble.R == byte.MinValue);
            Assert.IsTrue(bgraFromDouble.A == byte.MaxValue);

            // RGB -> BGRA
            var bgraFromRgb = TypeConversion.ToBgra(default(RGB));

            Assert.IsTrue(bgraFromRgb.B == byte.MinValue);
            Assert.IsTrue(bgraFromRgb.G == byte.MinValue);
            Assert.IsTrue(bgraFromRgb.R == byte.MinValue);
            Assert.IsTrue(bgraFromRgb.A == byte.MaxValue);

            // CMYK -> BGRA
            var bgraFromCmyk = TypeConversion.ToBgra(default(CMYK));

            Assert.IsTrue(bgraFromCmyk.B == byte.MaxValue);
            Assert.IsTrue(bgraFromCmyk.G == byte.MaxValue);
            Assert.IsTrue(bgraFromCmyk.R == byte.MaxValue);
            Assert.IsTrue(bgraFromCmyk.A == byte.MaxValue);

            // HSL -> BGRA
            var bgraFromHsl = TypeConversion.ToBgra(default(HSL));

            Assert.IsTrue(bgraFromHsl.B == byte.MinValue);
            Assert.IsTrue(bgraFromHsl.G == byte.MinValue);
            Assert.IsTrue(bgraFromHsl.R == byte.MinValue);
            Assert.IsTrue(bgraFromHsl.A == byte.MaxValue);

            // BOOL -> BGRA
            var bgraFromBool = TypeConversion.ToBgra(false);

            Assert.IsTrue(bgraFromBool.B == byte.MinValue);
            Assert.IsTrue(bgraFromBool.G == byte.MinValue);
            Assert.IsTrue(bgraFromBool.R == byte.MinValue);
            Assert.IsTrue(bgraFromBool.A == byte.MaxValue);

            // HSV -> BGRA
            var bgraFromHsv = TypeConversion.ToBgra(default(HSV));

            Assert.IsTrue(bgraFromHsv.B == byte.MinValue);
            Assert.IsTrue(bgraFromHsv.G == byte.MinValue);
            Assert.IsTrue(bgraFromHsv.R == byte.MinValue);
            Assert.IsTrue(bgraFromHsv.A == byte.MaxValue);
        }

        [TestMethod]
        public void HsvHslConversion()
        {
            Conversion.PermuteRgb(HsvHslConversionPrivate);
        }

        private void HsvHslConversionPrivate(RGB rgb)
        {
            HSL hslFromRgb = rgb.ToHsl();
            HSV hsvFromRgb = rgb.ToHsv();

            HSL hslFromHsv = hsvFromRgb.ToHsl();
            HSV hsvFromHsl = hslFromRgb.ToHsv();

            CloseEnough(hslFromHsv, hslFromRgb);
            CloseEnough(hsvFromHsl, hsvFromRgb);
        }

        private static void CloseEnough(HSL a, HSL b)
        {
            const double eps = 1.0 / 1000000;
            const double eps360 = eps * 360.0;

            Assert.IsTrue(Math.Abs(a.H - b.H) < eps360);
            Assert.IsTrue(Math.Abs(a.S - b.S) < eps);
            Assert.IsTrue(Math.Abs(a.L - b.L) < eps);
        }

        private static void CloseEnough(HSV a, HSV b)
        {
            const double eps = 1.0 / 1000000;
            const double eps360 = eps * 360.0;

            Assert.IsTrue(Math.Abs(a.H - b.H) < eps360);
            Assert.IsTrue(Math.Abs(a.S - b.S) < eps);
            Assert.IsTrue(Math.Abs(a.V - b.V) < eps);
        }
    }
}
