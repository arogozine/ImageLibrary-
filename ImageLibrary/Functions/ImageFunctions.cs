using ImageLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageLibrary
{
    public static unsafe class ImageFunctions
    {
        private static IImage<T> Run<T>(Func<T, T, T> func, IImage<T>[] images)
            where T : struct, IEquatable<T>
        {
            if (images == null)
            {
                throw new ArgumentNullException(nameof(images), "No images were supplied");
            }

            // Size Check
            IImage<T> first = images[0];

            int width = first.Width;
            int height = first.Height;

            for (int i = 1; i < images.Length; i++)
            {
                var img = images[i];

                if (img.Height != height || img.Width != width)
                    throw new ArgumentException("Image Dimensions Don't Match", nameof(images));
            }

            //
            IImage<T> newImage = first.Copy();

            for(int i = 1; i < images.Length; i++)
            {
                var image = images[i];
                
                for (int j = 0; j < image.Length; j++)
                {
                    newImage[j] = func(newImage[j], image[j]);
                }
            }

            return newImage;
        }

        public static IImage<Complex> Add(params IImage<Complex>[] images)
        {
            return ImageFunctions.Run((a, b) => a + b, images);
        }

        public static IImage<Complex> Subtract(params IImage<Complex>[] images)
        {
            return ImageFunctions.Run((a, b) => a - b, images);
        }

        public static IImage<Complex> Divide(params IImage<Complex>[] images)
        {
            return ImageFunctions.Run((a, b) => a / b, images);
        }

        public static IImage<Complex> Multiply(params IImage<Complex>[] images)
        {
            return ImageFunctions.Run((a, b) => a * b, images);
        }

        public static IImage<double> Add(params IImage<double>[] images)
        {
            return ImageFunctions.Run((a, b) => a + b, images);
        }

        public static IImage<double> Subtract(params IImage<double>[] images)
        {
            return ImageFunctions.Run((a, b) => a - b, images);
        }

        public static IImage<double> Divide(params IImage<double>[] images)
        {
            return ImageFunctions.Run((a, b) => a / b, images);
        }

        public static IImage<double> Multiply(params IImage<double>[] images)
        {
            return ImageFunctions.Run((a, b) => a * b, images);
        }

        public static IImage<RGB> Add(params IImage<RGB>[] images)
        {
            return ImageFunctions.Run((a, b) => a + b, images);
        }

        /// <summary>
        /// Subtracts color values of first iamge from rest
        /// </summary>
        /// <param name="images"></param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <returns></returns>
        public static IImage<RGB> Subtract(params IImage<RGB>[] images)
        {
            return ImageFunctions.Run((a, b) => a - b, images);
        }

        public static IImage<RGB> Divide(params IImage<RGB>[] images)
        {
            return ImageFunctions.Run((a, b) => a / b, images);
        }

        public static IImage<RGB> Multiply(params IImage<RGB>[] images)
        {
            return ImageFunctions.Run((a, b) => a * b, images);
        }

        private static T[] _leftToRight<T>(params IImage<T>[] images)
            where T : struct, IEquatable<T>
        {
            // Ensure all images are the same height
            IEnumerable<int> heights = images
                .Select(x => x.Height)
                .Distinct();

            if (heights.Count() > 1)
            {
                throw new ArgumentException("Images must be the same height", nameof(images));
            }

            // Sum all the widths
            int height = heights.First();
            int width = images
                .Select(x => x.Width)
                .Sum();

            T[] newT = new T[width * height];

            for (int i = 0, y = 0; y < height; y++)
            {
                for (int z = 0; z < images.Length; z++)
                {
                    IImage<T> img = images[z];

                    int j = img.Width * y;
                    int jEnd = img.Width * (y + 1);

                    for (; j < jEnd; j++, i++)
                    {
                        newT[i] = img[j];
                    }
                }
            }

            return newT;
        }

        private static T[] _topToBottom<T>(params IImage<T>[] images)
            where T : struct, IEquatable<T>
        {
            if (images.Select(x => x.Width).Distinct().Count() > 1)
            {
                throw new ArgumentException("Images must be the same width", nameof(images));
            }

            return images
                .SelectMany(x => x.Data)
                .ToArray();
        }

        public static IImage<double> TopToBottom(params IImage<double>[] images)
        {
            return new Image(
                images.First().Width, 
                images.Select(x => x.Height).Sum(), 
                _topToBottom(images));
        }

        public static IImage<RGB> TopToBottom(params IImage<RGB>[] images)
        {
            return new RGBImage(
                images.First().Width,
                images.Select(x => x.Height).Sum(),
                _topToBottom(images));
        }

        public static IImage<Complex> TopToBottom(params IImage<Complex>[] images)
        {
            return new ComplexImage(
                images.First().Width,
                images.Select(x => x.Height).Sum(),
                _topToBottom(images));
        }

        public static IImage<double> LeftToRight(params IImage<double>[] images)
        {
            return new Image(
                images.Select(x => x.Width).Sum(),
                images.First().Height,
                _leftToRight(images));
        }

        public static IImage<RGB> LeftToRight(params IImage<RGB>[] images)
        {
            return new RGBImage(
                images.Select(x => x.Width).Sum(),
                images.First().Height,
                _leftToRight(images));
        }

        public static IImage<Complex> LeftToRight(params IImage<Complex>[] images)
        {
            return new ComplexImage(
                images.Select(x => x.Width).Sum(),
                images.First().Height,
                _leftToRight(images));
        }
        /*
        public static IImage<T> Insert<T>(this IImage<T> parent, IImage<T> child, int x, int y)
        {
            // Make a copy of parent image
            parent = parent.Copy();

            // Calculate where child image belongs
            int childStartX = x;
            int childStartY = y;
            int childEndX = x + child.Width;
            int childEndY = y + child.Height;

            // Insert Child Image into parent
            for (int w = childStartX, wc = 0; w < childStartY; w++, wc++)
            {
                for (int h = childStartY, hc = 0; h < childEndY; h++, hc++)
                {
                    parent[w, h] = child[wc, hc];
                }
            }

            return parent;
        }
        */
    }
}
