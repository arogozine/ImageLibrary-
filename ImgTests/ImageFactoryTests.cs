using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageLibrary;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Numerics;

namespace ImgTests
{
    [TestClass]
    public class ImageFactoryTests
    {
        [TestMethod]
        public void TestGenerateBinary()
        {
            GenerateTest<bool>("GenerateBinary");
        }

        [TestMethod]
        public void TestGenerate()
        {
            GenerateTest<double>("Generate");
        }

        [TestMethod]
        public void TestGenerateComplex()
        {
            GenerateTest<Complex>("GenerateComplex");
        }

        [TestMethod]
        public void TestGenerateRgb()
        {
            GenerateTest<RGB>("GenerateRgb");
        }

        [TestMethod]
        public void TestGenerateCmyk()
        {
            GenerateTest<CMYK>("GenerateCmyk");
        }

        [TestMethod]
        public void TestGenerateHsl()
        {
            GenerateTest<HSL>("GenerateHsl");
        }

        [TestMethod]
        public void TestGenerateHsv()
        {
            GenerateTest<HSV>("GenerateHsv");
        }

        [TestMethod]
        public void TestGenerateBgra()
        {
            GenerateTest<BGRA>("GenerateBgra");
        }

        #region Reflection Based Generic Test

        public void GenerateTest<T>(string methodName)
            where T : struct, IEquatable<T>
        {
            // Get all static functions of ImageFactory
            MethodInfo[] methods = typeof(ImageFactory).GetMethods(BindingFlags.Public | BindingFlags.Static);

            int count = 0;

            // There should be multiple overloads for each method
            foreach (MethodInfo method in methods)
            {
                // Each function and its overloads are for a certain type of an image
                if (method.Name == methodName)
                {
                    count++;
                    ParameterInfo[] parameters = method.GetParameters();

                    // Number of Parameters determine how to test it
                    switch (parameters.Length)
                    {
                        case 1:
                            if (method.GetParameters()[0].ParameterType.Equals(typeof(string)))
                            {
                                // Input is a file name
                                TestGenerateOneParam<T>(method);
                            }
                            break;
                        case 2:
                            // Input is Width and Height
                            TestGenerateTwoParam<T>(method);
                            break;
                        case 3:
                            // Input is Width, Height and
                            if (method.GetParameters()[2].ParameterType.Equals(typeof(T[])))
                            {
                                // array of T[]
                                TestGenerateThreeParamArray<T>(method);
                            }
                            else
                            {
                                // IEnumerable<T>
                                TestGenerateThreeParamEnumeration<T>(method);
                            }
                            
                            break;
                        default:
                            // Should not happen
                            throw new NotSupportedException();
                    }
                }
            }

            // There should be at least two of each method
            Assert.IsTrue(count > 1);
        }

        public static void TestGenerateOneParam<T>(MethodInfo method)
            where T : struct, IEquatable<T>
        {
            // method to function for ease
            Func<string, IImage<T>> func = (Func<string, IImage<T>>)
                     Delegate.CreateDelegate(typeof(Func<string, IImage<T>>), method);

            foreach(var input in new [] { null, string.Empty})
            {
                try
                {
                    IImage<T> img = func(input);
                    Assert.Fail();
                }
                catch (ArgumentException)
                {
                    // Expected for bad input
                }
            }
        }

        public void TestGenerateTwoParam<T>(MethodInfo method)
            where T : struct, IEquatable<T>
        {
            // method to function for ease
            Func<int, int, IImage<T>> func = (Func<int, int, IImage<T>>)
                Delegate.CreateDelegate(typeof(Func<int, int, IImage<T>>), method);

            foreach (int y in new [] { -1, 0 })
            foreach (int x in new [] { -1, 0 })
            {
                try
                {
                    IImage<T> img = func(x, y);
                    Assert.Fail();
                }
                catch (ArgumentException)
                {
                    // Expected for bad input
                }
            }
        }

        public void TestGenerateThreeParamArray<T>(MethodInfo method)
            where T : struct, IEquatable<T>
        {
            Func<int, int, T[], IImage<T>> func = (Func<int, int, T[], IImage<T>>)
                Delegate.CreateDelegate(typeof(Func<int, int, T[], IImage<T>>), method);

            TestGenerateThreeParam(func);
        }

        public void TestGenerateThreeParamEnumeration<T>(MethodInfo method)
            where T : struct, IEquatable<T>
        {
            Func<int, int, IEnumerable<T>, IImage<T>> func = (Func<int, int, IEnumerable<T>, IImage<T>>)
                Delegate.CreateDelegate(typeof(Func<int, int, IEnumerable<T>, IImage<T>>), method);

            TestGenerateThreeParam(func);
        }

        private void TestGenerateThreeParam<T>(Func<int, int, T[], IImage<T>> func)
            where T: struct, IEquatable<T>
        {
            foreach (int y in new[] { -1, 0, 100 })
            { 
                foreach (int x in new[] { -1, 0, 100 })
                { 
                    foreach (T[] data in new[] { null, new T[0], new T[10000] })
                    {
                        // This is the valid combination
                        // It should not fail
                        if (y == 100 && x == 100 && data != null && data.Length == 10000)
                        {
                            IImage<T> img = func(x, y, data);
                            Assert.IsNotNull(img);
                            continue;
                        }

                        try
                        {
                            func(x, y, data);
                            Assert.Fail();
                        }
                        catch (ArgumentException)
                        {
                            // Expected
                        }
                    }
                }
            }
        }

        #endregion
    }
}
