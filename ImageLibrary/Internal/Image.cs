using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageLibrary
{
    internal class Image : IImage<double>
    {
        private int width;
        private int height;
        private readonly object _lockObject = new object();

        public Image(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.Data = new double[width*height];
        }
        
        public Image(int width, int height, double[] data)
        {
            this.width = width;
            this.height = height;
            this.Data = data;
        }

        private void MinScale(out double min, out double scale)
        {
            double max = double.MinValue;
            min = double.MaxValue;

            foreach (double a in this.Data)
            {
                max = a > max ? a : max;
                min = a < min ? a : min;
            }

            scale = max - min;
        }

        public void ToIndexedBgra(Action<int, BGRA> iRgba)
        {
            double min, scale;
            MinScale(out min, out scale);
            scale /= 255.0;

            for (int i = 0; i < this.Data.Length; i++)
            {
                byte b = (byte)((this.Data[i] - min) / scale);
                iRgba(i, new BGRA() { B = b, R = b, G = b, A = byte.MaxValue });
            }
        }

        public IImage<double> Pad(int width, int height)
        {
            return ImageBase.Pad(this, Image.Generate, width, height);
        }

        public unsafe IImage<double> Transpose()
        {
            var denseMatrix = Matrix<double>.Build
                .Dense(this.Cols, this.Rows, this.Data);

            double[] newData = denseMatrix
                .Transpose()
                .ToColumnWiseArray();

            return Generate(this.height, this.width, newData);
        }

        private static Image Generate(int width, int height, double[] data)
        {
            return new Image(width, height, data);
        }

        public IImage<double> Upsample()
        {
            return ImageBase.Upsample(this, Generate);
        }

        public IImage<double> UpsampleCols()
        {
            return ImageBase.UpsampleCols(this, Generate);
        }

        public IImage<double> UpsampleRows()
        {
            return ImageBase.UpsampleRows(this, Generate);
        }

        public IImage<double> Downsample()
        {
            return ImageBase.Downsample(this, Generate);
        }

        public IImage<double> DownsampleCols()
        {
            return ImageBase.DownsampleCols(this, Generate);
        }

        public IImage<double> DownsampleRows()
        {
            return ImageBase.DownsampleRows(this, Generate); ;
        }

        public IImage<double> FlipX()
        {
            return ImageBase.FlipX(this, Generate);
        }

        public IImage<double> FlipY()
        {
            return ImageBase.FlipY(this, Generate);
        }

        public IImage<double> FlipXY()
        {
            return ImageBase.FlipXY(this, Generate);
        }

        public IImage<double> Crop(System.Drawing.Rectangle rect)
        {
            return ImageBase.Crop(this, Generate, rect);
        }

        public IImage<double> Crop(int x1, int y1, int width, int height)
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
            get { return this.Data.Length; }
        }

        public double[] Data
        {
            get;
            protected set;
        }

        public IImage<double> Copy()
        {
            double[] data2 = new double[this.Data.Length];
            Array.Copy(this.Data, data2, this.Data.Length);
            return new Image(this.width, this.height, data2);
        }

        public double this[int i, int j]
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

        public double this[int index]
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
            this.Data.CopyTo(array, index);
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
            get { return this._lockObject; }
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return ((IEnumerable<double>)this).GetEnumerator();
        }

        public void Add(double item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(double item)
        {
            return ImageBase.Contains(this, item);
        }

        public void CopyTo(double[] array, int arrayIndex)
        {
            this.Data.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(double item)
        {
            throw new NotSupportedException();
        }

        IEnumerator<double> IEnumerable<double>.GetEnumerator()
        {
            return ImageBase.GetEnumerator(this);
        }

        public void Dispose()
        {
            return;
        }
    }
}