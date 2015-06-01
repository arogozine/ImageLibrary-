using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.IO;
using MathNet.Numerics.LinearAlgebra;

namespace ImageLibrary
{
    internal class ComplexImage : IImage<Complex>
    {
        private int width;
        private int height;
        private Complex[] data;

        public ComplexImage(int width, int height, Complex[] data)
        {
            this.width = width;
            this.height = height;
            this.data = data;
        }

        public ComplexImage(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.data = new Complex[width*height];
        }

        private double Scale()
        {
            double maximum = double.MinValue;
            double minimum = double.MaxValue;

            for (int i = 0; i < this.Data.Length; i++)
            {
                double x = this.Data[i].Real;
                double y = this.Data[i].Imaginary;
                if (x > maximum) maximum = x;
                if (x < minimum) minimum = x;
                if (y > maximum) maximum = y;
                if (y < minimum) minimum = y;
            }

            return 2.0 / (maximum - minimum);
        }

        public void ToIndexedBgra(Action<int, BGRA> rgbaAction)
        {
            var data = this.Data;
            double scale = this.Scale();

            Parallel.For(0, data.Length,
                (i) =>
                {
                    Complex c = data[i];

                    if (c == default(Complex))
                    {
                        rgbaAction(i, BGRA.Black);
                        return;
                    }

                    double x = scale * c.Real;
                    double y = scale * c.Imaginary;

                    double radius = Math.Sqrt(x * x + y * y);
                    double a = 0.40824829046386301636 * x;
                    double b = 0.70710678118654752440 * y;
                    double d = 1.0 / (1.0 + radius * radius);
                    double R = 0.5 + 0.81649658092772603273 * x * d;
                    double G = 0.5 - d * (a - b);
                    double B = 0.5 - d * (a + b);
                    d = 0.5 - radius * d;
                    if (radius < 1) d = -d;

                    d *= 255.0;
                    R *= 255.0;
                    G *= 255.0;
                    B *= 255.0;

                    byte r_byte = (byte)(R + d);
                    byte g_byte = (byte)(G + d);
                    byte b_byte = (byte)(B + d);

                    rgbaAction(i, new BGRA() { R = r_byte, G = g_byte, B = b_byte, A = byte.MaxValue });
                }
            );
        }

        public IImage<Complex> Pad(int width, int height)
        {
            return ImageBase.Pad(this, ComplexImage.Generate, width, height);
        }

        public IImage<Complex> Transpose()
        {
            var denseMatrix = Matrix<Complex>.Build
                .Dense(this.Cols, this.Rows, this.Data);

            Complex[] newData = denseMatrix
                .Transpose()
                .ToColumnWiseArray();

            return Generate(this.height, this.width, newData);
        }

        private static ComplexImage Generate(int width, int height, Complex[] data)
        {
            return new ComplexImage(width, height, data);
        }

        public IImage<Complex> Upsample()
        {
            return ImageBase.Upsample(this, Generate);
        }

        public IImage<Complex> UpsampleCols()
        {
            return ImageBase.UpsampleCols(this, Generate);
        }

        public IImage<Complex> UpsampleRows()
        {
            return ImageBase.UpsampleRows(this, Generate);
        }

        public IImage<Complex> Downsample()
        {
            return ImageBase.Downsample(this, Generate);
        }

        public IImage<Complex> DownsampleCols()
        {
            return ImageBase.DownsampleCols(this, Generate);
        }

        public IImage<Complex> DownsampleRows()
        {
            return ImageBase.DownsampleRows(this, Generate); ;
        }

        public IImage<Complex> FlipX()
        {
            return ImageBase.FlipX(this, Generate);
        }

        public IImage<Complex> FlipY()
        {
            return ImageBase.FlipY(this, Generate);
        }

        public IImage<Complex> FlipXY()
        {
            return ImageBase.FlipXY(this, Generate);
        }

        public IImage<Complex> Crop(System.Drawing.Rectangle rect)
        {
            return ImageBase.Crop(this, Generate, rect);
        }

        public IImage<Complex> Crop(int x1, int y1, int width, int height)
        {
            return ImageBase.Crop(this, Generate, x1, y1, width, height);
        }

        public int Cols
        {
            get { return this.width; }
        }

        public int Rows
        {
            get { return this.height; }
        }

        public int Height
        {
            get { return this.height; }
        }

        public int Width
        {
            get { return this.width; }
        }

        public int Length
        {
            get { return this.data.Length; }
        }

        public Complex[] Data
        {
            get { return this.data; }
        }

        public IImage<Complex> Copy()
        {
            Complex[] data2 = new Complex[this.Data.Length];
            Array.Copy(this.Data, data2, this.Data.Length);
            return new ComplexImage(this.width, this.height, data2);
        }

        public Complex this[int i, int j]
        {
            get
            {
                return this[i * this.Width + j];
            }
            set
            {
                this[i * this.Width + j] = value;
            }
        }

        public Complex this[int index]
        {
            get
            {
                return this.Data[index];
            }
            set
            {
                this.Data[index] = value;
            }
        }

        public byte[] ToBGR()
        {
            return ImageBase.ToRGB(this);
        }

        public byte[] ToBGRA()
        {
            return ImageBase.ToRGBA(this);
        }

        public BGRA[] ToPixelColor()
        {
            return ImageBase.ToPixelColor(this);
        }

        public void CopyTo(Array array, int index)
        {
            ImageBase.CopyTo(this, array, index);
        }

        public int Count
        {
            get { return this.Data.Length; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return ((IEnumerable<Complex>)this).GetEnumerator();
        }

        public void Add(Complex item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(Complex item)
        {
            return ImageBase.Contains(this, item);
        }

        public void CopyTo(Complex[] array, int arrayIndex)
        {
            ImageBase.CopyTo(this, array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(Complex item)
        {
            throw new NotSupportedException();
        }

        IEnumerator<Complex> IEnumerable<Complex>.GetEnumerator()
        {
            return ImageBase.GetEnumerator(this);
        }

        public void Dispose()
        {
            return;
        }
    }
}