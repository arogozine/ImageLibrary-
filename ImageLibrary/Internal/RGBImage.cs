using System;
using System.Collections.Generic;

namespace ImageLibrary
{

    internal class RGBImage : IImage<RGB>
    {
        private int width;
        private int height;
        private RGB[] data;

        public RGBImage(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.data = new RGB[width * height];
        }
        
        public RGBImage(int width, int height, RGB[] data)
        {
            this.width = width;
            this.height = height;
            this.data = data;
        }

        private void MinScale(out double min, out double scale)
        {
            double max = double.MinValue;
            min = double.MaxValue;

            foreach (RGB a in this.Data)
            {
                max = a.B > max ? a.B : max;
                max = a.G > max ? a.G : max;
                max = a.R > max ? a.R : max;

                min = a.B < min ? a.B : min;
                min = a.G < min ? a.G : min;
                min = a.R < min ? a.R : min;
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
                RGB rgb = this.Data[i];

                byte r = (byte)((rgb.R - min) / scale);
                byte g = (byte)((rgb.G - min) / scale);
                byte b = (byte)((rgb.B - min) / scale);

                iRgba(i, new BGRA { R = r, G = g, B = b, A = byte.MaxValue });
            }
        }

        public IImage<RGB> Pad(int width, int height)
            => ImageBase.Pad(this, Generate, width, height);

        public IImage<RGB> Transpose()
            => ImageBase.Transpose(this, Generate);

        private static RGBImage Generate(int width, int height, RGB[] data)
            => new RGBImage(width, height, data);

        public IImage<RGB> Upsample()
            => ImageBase.Upsample(this, Generate);

        public IImage<RGB> UpsampleCols()
            => ImageBase.UpsampleCols(this, Generate);

        public IImage<RGB> UpsampleRows()
            => ImageBase.UpsampleRows(this, Generate);

        public IImage<RGB> Downsample()
            => ImageBase.Downsample(this, Generate);

        public IImage<RGB> DownsampleCols()
            => ImageBase.DownsampleCols(this, Generate);

        public IImage<RGB> DownsampleRows()
            => ImageBase.DownsampleRows(this, Generate);

        public IImage<RGB> FlipX()
            => ImageBase.FlipX(this, Generate);

        public IImage<RGB> FlipY()
            => ImageBase.FlipY(this, Generate);

        public IImage<RGB> FlipXY()
            => ImageBase.FlipXY(this, Generate);

        public IImage<RGB> Crop(System.Drawing.Rectangle rect)
            => ImageBase.Crop(this, Generate, rect);

        public IImage<RGB> Crop(int x1, int y1, int width, int height)
            => ImageBase.Crop(this, Generate, x1, y1, width, height);

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

        public RGB[] Data
        {
            get { return this.data; }
        }

        public IImage<RGB> Copy()
        {
            RGB[] data2 = new RGB[this.Data.Length];
            Array.Copy(this.Data, data2, this.Data.Length);
            return new RGBImage(this.width, this.height, data2);
        }

        public RGB this[int i, int j]
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

        public RGB this[int index]
        {
            get
            {
                return this.data[index];
            }
            set
            {
                this.data[index] = value;
            }
        }

        public byte[] ToBGR()
            => ImageBase.ToRGB(this);

        public byte[] ToBGRA()
            => ImageBase.ToRGBA(this);

        public BGRA[] ToPixelColor()
            => ImageBase.ToPixelColor(this);

        public void CopyTo(Array array, int index)
            => ImageBase.CopyTo(this, array, index);

        public int Count
        {
            get { return this.data.Length; }
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
            return ((IEnumerable<RGB>)this).GetEnumerator();
        }

        public void Add(RGB item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(RGB item)
            => ImageBase.Contains(this, item);

        public void CopyTo(RGB[] array, int arrayIndex)
            => ImageBase.CopyTo(this, array, arrayIndex);

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(RGB item)
        {
            throw new NotSupportedException();
        }

        IEnumerator<RGB> IEnumerable<RGB>.GetEnumerator()
            => ImageBase.GetEnumerator(this);

        public void Dispose()
        {
            return;
        }
    }
}