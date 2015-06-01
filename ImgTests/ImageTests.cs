using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using ImageLibrary;
using ImageLibrary.Extensions;
using System.Numerics;
using System.Linq;
using ImageLibrary.EdgeDetection;

// http://www.codeproject.com/Articles/751744/Image-Segmentation-using-Unsupervised-Watershed-Al
// http://www.tannerhelland.com/3643/grayscale-image-algorithm-vb6/
// http://clickdamage.com/sourcecode/index.php
// http://www.codeproject.com/Articles/236394/Bi-Cubic-and-Bi-Linear-Interpolation-with-GLSL
// Test that Copy() doesn't write to the same image as the original (inner array was not copied :()
namespace ImgTests
{
    [TestClass]
    public class ImageTests : TestBase
    {
        [TestMethod]
        public void TestGenerate()
        {
            // GrayScale Image
            var img = ImageFactory.Generate(BunnyPath);
            Assert.IsFalse(ImageEmpty(img));
        }

        [TestMethod]
        public void TestGenerateRgb()
        {
            // RGB Image
            var img = ImageFactory.GenerateRgb(BunnyPath);
            Assert.IsFalse(ImageEmpty(img));
        }

        [TestMethod]
        public void TestGenerateBgra()
        {
            // BGRA Image
            var img = ImageFactory.GenerateBgra(BunnyPath);
            Assert.IsFalse(ImageEmpty(img));
        }

        [TestMethod]
        public void TestGenerateHsv()
        {
            // HSV Image
            var img = ImageFactory.GenerateHsv(BunnyPath);
            Assert.IsFalse(ImageEmpty(img));
        }

        [TestMethod]
        public void TestGenerateHsl()
        {
            // HSL Image
            var img = ImageFactory.GenerateHsl(BunnyPath);
            Assert.IsFalse(ImageEmpty(img));
        }

        [TestMethod]
        public void TestGenerateCmyk()
        {   
            // CMYK Image
            var img = ImageFactory.GenerateCmyk(BunnyPath);
            Assert.IsFalse(ImageEmpty(img));
        }

        [TestMethod]
        public void TestSaveImage()
        {
            ImageFactory.Generate(BunnyPath)
                .WriteImage(Path.Combine(WorkingDirectory, "Bun BW"));
        }

        [TestMethod]
        public void TestSaveRGBImage()
        {
            ImageFactory.GenerateRgb(BunnyPath)
                .WriteImage(WorkingDirectory, "Bun RGB");
        }

        [TestMethod]
        public void TestImageCropRectangle()
        {
            IImage<double> image = ImageFactory.Generate(BunnyPath);

            int dimensions = Math.Min(image.Width, image.Height);

            image.Crop(new System.Drawing.Rectangle() { Height = dimensions, Width = dimensions / 2 })
                .WriteImage(Path.Combine(WorkingDirectory, "Bun Crop Rectangle"));
        }

        [TestMethod]
        public void TestImageCrop()
        {
            IImage<double> image = ImageFactory.Generate(BunnyPath);

            int dimensions = Math.Min(image.Width, image.Height / 2);

            var cropped = image.Crop(0, 0, dimensions, dimensions);

            Assert.IsTrue(cropped.Height == dimensions);
            Assert.IsTrue(cropped.Width == dimensions);

            for (int y = 0; y < dimensions; y++)
            {
                for (int x = 0; x < dimensions; x++)
                {
                    Assert.IsTrue(cropped[y, x].Equals(image[y, x]));
                }
            }

            cropped.WriteImage(Path.Combine(WorkingDirectory, "Bun Crop"));
        }

        [TestMethod]
        public void TestRGBImageCropRectangle()
        {
            IImage<RGB> image = ImageFactory.GenerateRgb(BunnyPath);

            int dimensions = Math.Min(image.Width, image.Height / 2);

            image.Crop(new System.Drawing.Rectangle() { Height = dimensions, Width = dimensions })
                .WriteImage(Path.Combine(WorkingDirectory, "RGB Bun Crop Rectangle"));
        }

        [TestMethod]
        public void TestRGBImageCrop()
        {
            IImage<RGB> image = ImageFactory.GenerateRgb(BunnyPath);

            int dimensions = Math.Min(image.Width, image.Height);

            image.Crop(0, 0, dimensions, dimensions / 2)
                .WriteImage(Path.Combine(WorkingDirectory, "RGB Bun Crop"));
        }

        [TestMethod]
        public void TestTopToBottom()
        {
            var bun = ImageFactory.Generate(BunnyPath);

            ImageFunctions.TopToBottom(bun, bun, bun)
                .WriteImage(Path.Combine(WorkingDirectory, "TopToBottom"));
        }

        [TestMethod]
        public void TestLeftToRight()
        {
            var bun = ImageFactory.Generate(BunnyPath);

            ImageFunctions.LeftToRight(bun, bun, bun)
                .WriteImage(Path.Combine(WorkingDirectory, "LeftToRight"));
        }

        [TestMethod]
        public void TestTopToBottomRGB()
        {
            var bun = ImageFactory.GenerateRgb(BunnyPath);

            ImageFunctions.TopToBottom(bun, bun, bun)
                .WriteImage(Path.Combine(WorkingDirectory, "TopToBottomRGB"));
        }

        [TestMethod]
        public void TestLeftToRightRGB()
        {
            var bun = ImageFactory.GenerateRgb(BunnyPath);

            ImageFunctions.LeftToRight(bun, bun, bun)
                .WriteImage(Path.Combine(WorkingDirectory, "LeftToRightRGB"));
        }

        [TestMethod]
        public void TestTranspose()
        {
            ImageFactory.Generate(BunnyPath)
            .Transpose()
            .WriteImage(Path.Combine(WorkingDirectory, "Transpose"));
        }

        #region Filters

        [TestMethod]
        public void TestMedianFilter()
        {
            ImageFactory.Generate(ImageTests.BunnyPath)
                .MedianFilter(5, 5)
                .WriteImage(Path.Combine(WorkingDirectory, "Median Filter 5 5"));
        }

        [TestMethod]
        public void TestGaussianFilter()
        {
            Filters.MakeFilter(512, 512, Filters.Gaussian(256.0))
                .WriteImage(WorkingDirectory, "Gaussian Filter");
        }

        [TestMethod]
        public void TestLaplacianOfGaussianFilter()
        {
            Filters.MakeFilter(512, 512, Filters.LaplacianOfGaussian(256.0))
                .WriteImage(WorkingDirectory, "LaplacianOfGaussian Filter");
        }

        [TestMethod]
        public void ApplyFiler()
        {
            var filter = Filters.MakeFilter(512, 512, Filters.Gaussian(256.0));
            
            IImage<Complex> bun = ImageFactory
                .Generate(ImageTests.BunnyPath)
                .Crop(0, 0, 512, 512)
                .Fft();

            ImageFunctions
                .Multiply(bun, filter.ToComplexImage())
                .InverseFft()
                .WriteImage(WorkingDirectory, "Apply Filter");
        }

        #endregion

        #region Convolution

        [TestMethod]
        public void TestConvolve()
        {
            var bun = ImageFactory.Generate(ImageTests.BunnyPath);

            bun.Copy()
                .ConvolveRows(1, -1)
                .WriteImage(Path.Combine(WorkingDirectory, "C Rows"));

            bun.Copy()
                .ConvolveCols(1, -1)
                .WriteImage(Path.Combine(WorkingDirectory, "C Cols"));

            bun.Copy()
                .Convolve(
                new double[][]
                {
                    new[] {-1.0, +1},
                    new[] {+1.0, -1}
                })
                .WriteImage(Path.Combine(WorkingDirectory, "C Rows and Cols"));

            bun.Copy()
                .Convolve(
                new double[][]
                {
                    new double[] {1, 1, 1},
                    new double[] {1, -8, 1},
                    new double[] {1, 1, 1,}
                })
                .WriteImage(Path.Combine(WorkingDirectory, "C Rows and Cols 2"));
        }

        [TestMethod]
        public void TestConvolveOther()
        {
            var bunny = ImageFactory.Generate(ImageTests.BunnyPath);

            bunny.GaussianBlur()
                .WriteImage(Path.Combine(WorkingDirectory, "GaussianBlur"));

            bunny.GaussianBlur2()
                .WriteImage(Path.Combine(WorkingDirectory, "GaussianBlur2"));

            bunny.GaussianBlur3()
                .WriteImage(Path.Combine(WorkingDirectory, "GaussianBlur3"));

            bunny.Unsharpen()
                .WriteImage(Path.Combine(WorkingDirectory, "Unsharpen"));

            bunny.Sharpness()
                .WriteImage(Path.Combine(WorkingDirectory, "Sharpness"));

            bunny.Sharpen()
                .WriteImage(Path.Combine(WorkingDirectory, "Sharpness"));

            bunny.EdgeDetect()
                .WriteImage(Path.Combine(WorkingDirectory, "EdgeDetect"));

            bunny.EdgeDetect2()
                .WriteImage(Path.Combine(WorkingDirectory, "EdgeDetect2"));

            bunny.EdgeDetect3()
                .WriteImage(Path.Combine(WorkingDirectory, "EdgeDetect3"));

            bunny.EdgeDetect4()
                .WriteImage(Path.Combine(WorkingDirectory, "EdgeDetect45"));

            bunny.EdgeDetect5()
                .WriteImage(Path.Combine(WorkingDirectory, "EdgeDetect5"));

            bunny.EdgeDetect6()
                .WriteImage(Path.Combine(WorkingDirectory, "EdgeDetect6"));

            bunny.SobelHorizontal()
                .WriteImage(Path.Combine(WorkingDirectory, "SobelHorizontal"));

            bunny.SobelVertical()
                .WriteImage(Path.Combine(WorkingDirectory, "SobelVertical"));

            bunny.PrevitVertical()
                .WriteImage(Path.Combine(WorkingDirectory, "PrevitVerical"));

            bunny.PrevitHorizontal()
                .WriteImage(Path.Combine(WorkingDirectory, "PrevitHorizontal"));

            bunny.BoxBlur()
                .WriteImage(Path.Combine(WorkingDirectory, "BoxBlur"));

            bunny.TriangleBlur()
                .WriteImage(Path.Combine(WorkingDirectory, "TriangleBlur"));
        }

        #endregion

        #region Sketch

        [TestMethod]
        public void TestSketch()
        {
            var sketch = new Sketch(64, 64,
                new Line(16, 16, 48, 16),
                new Line(48, 16, 48, 48),
                new Line(48, 48, 16, 48),
                new Line(16, 48, 16, 16));

            // Scale up 800%
            sketch.Scale = 8.0;

            sketch.WriteImage(WorkingDirectory, "Bolt");
        }

        [TestMethod]
        public void TestUnion()
        {
            var sketchA = new Sketch(64, 64,
                    new Line(16, 16, 48, 16),
                    new Line(48, 16, 48, 48));

            var sketchB = new Sketch(64, 64,
                new Line(48, 48, 16, 48),
                new Line(16, 48, 16, 16));

            var sketchUnion = Bolt.Union(sketchA, sketchB);
            sketchUnion.Scale = 8.0;

            sketchUnion.WriteImage(WorkingDirectory, "Union");
        }

        [TestMethod]
        public void TestZeroCrossingsChimney()
        {
            var chimney = ImageFactory.Generate(ImageTests.ChimneyPath)
                .ZeroCrossings(CrossingMethod.One);

            chimney.Scale = 20.0;

            chimney.WriteImage(WorkingDirectory, "ChimneyZeroCross");
        }

        [TestMethod]
        public void TestPoLink()
        {
            var chimney = Bolt.Boldt(ImageTests.ChimneyPath, 2);

            chimney.Scale = 20.0;

            byte[] poLink = chimney.PoLink(false, 1.0, 0.5, 2.0, 0.785398, 0.6, 2.0)
                .DisplayLinkGraph();

            string path = Path.Combine(WorkingDirectory, "PoLinkChimney") + ".png";

            File.WriteAllBytes(path, poLink);
        }

        [TestMethod]
        public void TestZeroCrossings()
        {
            var bunny = ImageFactory
                .Generate(ImageTests.BunnyPath)
                .ZeroCrossings(0);

            bunny.Scale = 8.0;
            bunny.WriteImage(WorkingDirectory, "ZeroCrossings");
        }

        [TestMethod]
        public void TestZeroCrossings1()
        {
            var bunny = ImageFactory
                .Generate(ImageTests.BunnyPath)
                .ZeroCrossings(CrossingMethod.One);

            bunny.Scale = 8.0;
            bunny.WriteImage(WorkingDirectory, "ZeroCrossings1");
        }

        [TestMethod]
        public void TestZeroCrossings2()
        {
            var bunny = ImageFactory
                .Generate(ImageTests.BunnyPath)
                .ZeroCrossings(CrossingMethod.Two);

            bunny.Scale = 8.0;
            bunny.WriteImage(WorkingDirectory, "ZeroCrossings2");
        }

        [TestMethod]
        public void TestPoLink2()
        {
            var bunny = ImageFactory
                .Generate(ImageTests.BunnyPath)
                .ZeroCrossings(CrossingMethod.Three);

            bunny.Scale = 16.0;

            var link = bunny
                   .PoLink(false, 1.0, 0.5, 2.0, 0.785398, 0.6, 2.0)
                   .DisplayLinkGraph();

            string path = Path.Combine(WorkingDirectory, "DisplayLinkGraph2") + ".png";

            File.WriteAllBytes(path, link);
        }

        #endregion

        [TestMethod]
        public void TestHistogram()
        {
            IImage<double> frog = ImageFactory
                .Generate(ImageTests.FrogPath);

            int[] histogram = frog.Histogram();

            Assert.IsNotNull(histogram);
            Assert.IsTrue(histogram.Length > 10);
            // Assert.IsTrue(histogram.Distinct().Count() == histogram.Length);
        }

        [TestMethod]
        public void TestCdf()
        {
            double[] cdf = ImageFactory.Generate(ImageTests.FrogPath).CumulativeDistribution();

            Assert.IsNotNull(cdf);
            Assert.IsTrue(cdf.Length != 0);
        }

        [TestMethod]
        public void TestEqualizeFrog()
        {
            ImageFactory
                .Generate(ImageTests.FrogPath)
                .Equalize()
                .WriteImage(Path.Combine(WorkingDirectory, "frog equalized"));
        }

        [TestMethod]
        public void TestProject()
        {
            var mona = ImageFactory.Generate(ImageTests.MonaPath);
            var mona2 = mona.UpsampleRows();
            Assert.IsFalse(ImageEmpty(mona2));
            Assert.IsTrue(mona.Height * 2 == mona2.Height);
            mona2.WriteImage(WorkingDirectory, "projected");
        }

        [TestMethod]
        public void TestHarmonicSignal()
        {
            ImageFactory.GenerateComplex(128, 128)
                .MapLocation(Filters.HarmonicSignal((3.0 / 128.0), (2.0 / 128.0)))
                .WriteImage(WorkingDirectory, "Complex Harmonic");
        }

        [TestMethod]
        public void TestMatrixProduct()
        {
            var bun = ImageFactory.Generate(ImageTests.BunnyPath);
            Assert.IsFalse(ImageEmpty(bun));

            bun.Multiply(bun)
                .WriteImage(WorkingDirectory, "Matrix Product");
        }

        [TestMethod]
        public void TestMatrixProductRGB()
        {
            var bun = ImageFactory.GenerateRgb(ImageTests.BunnyPath);
            Assert.IsFalse(ImageEmpty(bun));

            bun.Multiply(bun)
                .WriteImage(Path.Combine(WorkingDirectory, "Matrix Product RGB"));
        }

        [TestMethod]
        public void TestCentersOFMass()
        {
            ImageFactory.Generate(ImageTests.BunnyPath)
                .MapValue(x => x > 90 ? 1 : 0)
                .CentersOfMass();
        }

        [TestMethod]
        public void TestFFT()
        {
            // [512 * 512] image
            IImage<double> bun = ImageFactory
                .Generate(ImageTests.BunnyPath)
                .Crop(0, 0, 512, 512);

            Assert.IsTrue(bun.Height == 512);
            Assert.IsTrue(bun.Width == 512);

            // Fast Forrier Transform
            IImage<Complex> fft = bun.Fft();

            IImage<double> realPart = fft.RealPart();
            IImage<double> imagPart = fft.ImaginaryPart();

            Assert.IsFalse(ImageEmpty(realPart));
            Assert.IsFalse(ImageEmpty(imagPart));
            Assert.IsTrue(realPart.Width == imagPart.Width);
            Assert.IsTrue(realPart.Height == imagPart.Height);

            // IMAGINARY(COMPLEX) + REAL(COMPLEX) == COMPLEX
            IImage<Complex> fftEq = ImageFactory
                .GenerateComplex(512, 512, realPart.Zip(imagPart, (real, img) => new Complex(real, img)));

            Assert.IsTrue(ImagesEqual(fft, fftEq));


            realPart
                .WriteImage(Path.Combine(WorkingDirectory, "FFT RealPart"));

            imagPart
                .WriteImage(Path.Combine(WorkingDirectory, "FFT ImaginaryPart"));

            fft
                .Phase()
                .WriteImage(Path.Combine(WorkingDirectory, "FFT Phase"));

            fft
                .MagnitudePart()
                .WriteImage(Path.Combine(WorkingDirectory, "FFT Magnitude"));

            fft
                .InverseFft()
                .WriteImage(Path.Combine(WorkingDirectory, "FFT Inverse FFT"));
        }

        [TestMethod]
        public void TestHotImage()
        {
            ImageFactory.Generate(ImageTests.BunnyPath)
            .MakeHotImage()
            .WriteImage(Path.Combine(WorkingDirectory, "Hot Image"));
        }

        [TestMethod]
        public void BhattacharryaDistance()
        {
            var bunny = ImageFactory.Generate(ImageTests.BunnyPath);
            var value = bunny.HellingerDistance(bunny);

            // No difference between the images
            Assert.IsTrue(value == 0.0);
        }
    }
}
