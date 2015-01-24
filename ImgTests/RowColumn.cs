using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageLibrary;
using ImageLibrary.Extensions;

namespace ImgTests
{
    [TestClass]
    public class RowColumn : TestBase
    {
        [TestMethod]
        public void Rows()
        {
            IImage<Double> image = ImageFactory.Generate(BunnyPath);

            ImageRow<Double>[] rows = image.Rows();

            Assert.IsNotNull(rows);
            Assert.IsTrue(rows.Length == image.Height);

            for (int i = 0; i < image.Height; i++)
            {
                ImageRow<Double> row = rows[i];
                Assert.IsNotNull(row);
                Assert.IsTrue(row.Count == row.Width);
                Assert.IsTrue(row.Count == image.Width);

                for (int x = 0; x < image.Width; x++)
                {
                    Assert.IsTrue(image[i, x].Equals(row[x]));
                }
                
            }
        }

        [TestMethod]
        public void Cols()
        {
            IImage<Double> image = ImageFactory.Generate(BunnyPath);

            ImageColumn<Double>[] cols = image.Columns();
            Assert.IsNotNull(cols);
            Assert.IsTrue(cols.Length == image.Height);

            for (int j = 0; j < image.Width; j++)
            {
                ImageColumn<double> col = cols[j];
                Assert.IsNotNull(col);
                Assert.IsTrue(col.Count == col.Height);
                Assert.IsTrue(col.Count == image.Height);
            }
        }
    }
}
