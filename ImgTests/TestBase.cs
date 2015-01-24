using ImageLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

namespace ImgTests
{
    public abstract class TestBase
    {
        protected static readonly string WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Images/");
        protected static readonly string BunnyPath = Path.Combine(ImageTests.WorkingDirectory, "bun.jpg");
        protected static readonly string ChimneyPath = Path.Combine(ImageTests.WorkingDirectory, "chimney.png");
        protected static readonly string FrogPath = Path.Combine(ImageTests.WorkingDirectory, "1_frog.png");
        protected static readonly string MonaPath = Path.Combine(ImageTests.WorkingDirectory, "mona45.png");

        /// <summary>
        /// Is the image Empty
        /// </summary>
        /// <typeparam name="T">Image Pixel Type</typeparam>
        /// <param name="img">Image</param>
        /// <returns>Whether or not image is "blank"</returns>
        protected static bool ImageEmpty<T>(IImage<T> img)
            where T : struct, IEquatable<T>
        {
            return img.All(x => x.Equals(default(T)));
        }

        protected static bool ImagesEqualWithinEpsilon<T>(IImage<T> a, IImage<T> b, Func<T, T, bool> comparor)
            where T : struct, IEquatable<T>
        {
            Assert.IsNotNull(a);
            Assert.IsNotNull(b);

            // Width must match
            if (a.Width != b.Width)
            {
                return false;
            }

            // Height must match
            if (a.Height != b.Height)
            {
                return false;
            }

            // Compare each pixel for equality
            for (int i = 0; i < a.Length; i++)
            {
                if (!comparor(a[i], b[i]))
                {
                    return false;
                }
            }

            return true;            
        }

        protected static bool ImagesEqual<T>(IImage<T> a, IImage<T> b) 
            where T : struct, IEquatable<T>
        {
            // Width must match
            if (a.Width != b.Width)
            {
                return false;
            }

            // Height must match
            if (a.Height != b.Height)
            {
                return false;
            }

            // Compare each pixel for equality
            for (int i = 0; i < a.Length; i++)
            {
                if (!a[i].Equals(b[i]))
                {
                    return false;
                }

                // != Operator should do the same thing
                // grated with C# limitations
                // this needs to be done with dynamic :(
                dynamic ai = a[i];
                dynamic bi = b[i];

                // (AI eq BI) === not (AI not eq BI)
                Assert.IsTrue((ai == bi) == !(ai != bi));

                if (ai != bi)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
