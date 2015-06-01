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
            IImage<double> image = ImageFactory.Generate(BunnyPath);

            ImageRow<double>[] rows = image.Rows();

            Assert.IsNotNull(rows);
            Assert.IsTrue(rows.Length == image.Height);

            for (int i = 0; i < image.Height; i++)
            {
                ImageRow<double> row = rows[i];
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
            IImage<double> image = ImageFactory.Generate(BunnyPath);

            ImageColumn<double>[] cols = image.Columns();
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
