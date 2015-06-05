using ImageLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageLibrary
{
    internal class HsvImage : IImage<HSV>
    {
        private int width;
        private int height;
        private HSV[] data;

        public HsvImage(int width, int height, HSV[] data)
        {
            this.width = width;
            this.height = height;
            this.data = data;
        }

        public HsvImage(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.data = new HSV[width*height];
        }

        public void ToIndexedBgra(Action<int, BGRA> iRgba)
        {
            for (int index = 0; index < this.Data.Length; index++)
            {
                HSV hsi = this.Data[index];
                iRgba(index, TypeConversion.ToBgra(hsi));
            }
        }

        public IImage<HSV> Pad(int width, int height)
        {
            return ImageBase.Pad(this, HsvImage.Generate, width, height);
        }

        public IImage<HSV> Transpose()
        {
            return ImageBase.Transpose(this, HsvImage.Generate);
        }

        private static HsvImage Generate(int width, int height, HSV[] data)
        {
            return new HsvImage(width, height, data);
        }

        public IImage<HSV> Upsample()
        {
            return ImageBase.Upsample(this, Generate);
        }

        public IImage<HSV> UpsampleCols()
        {
            return ImageBase.UpsampleCols(this, Generate);
        }

        public IImage<HSV> UpsampleRows()
        {
            return ImageBase.UpsampleRows(this, Generate);
        }

        public IImage<HSV> Downsample()
        {
            return ImageBase.Downsample(this, Generate);
        }

        public IImage<HSV> DownsampleCols()
        {
            return ImageBase.DownsampleCols(this, Generate);
        }

        public IImage<HSV> DownsampleRows()
        {
            return ImageBase.DownsampleRows(this, Generate); ;
        }

        public IImage<HSV> FlipX()
        {
            return ImageBase.FlipX(this, Generate);
        }

        public IImage<HSV> FlipY()
        {
            return ImageBase.FlipY(this, Generate);
        }

        public IImage<HSV> FlipXY()
        {
            return ImageBase.FlipXY(this, Generate);
        }

        public IImage<HSV> Crop(System.Drawing.Rectangle rect)
        {
            return ImageBase.Crop(this, Generate, rect);
        }

        public IImage<HSV> Crop(int x1, int y1, int width, int height)
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

        public HSV[] Data
        {
            get { return this.data; }
        }

        public IImage<HSV> Copy()
        {
            HSV[] data2 = new HSV[this.Data.Length];
            Array.Copy(this.Data, data2, this.Data.Length);
            return new HsvImage(this.width, this.height, data2);
        }

        public HSV this[int i, int j]
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

        public HSV this[int index]
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
            return ((IEnumerable<HSV>)this).GetEnumerator(); 
        }

        public void Add(HSV item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(HSV item)
        {
            return this.data.Contains(item);
        }

        public void CopyTo(HSV[] array, int arrayIndex)
        {
            ImageBase.CopyTo(this, array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(HSV item)
        {
            throw new NotSupportedException();
        }

        IEnumerator<HSV> IEnumerable<HSV>.GetEnumerator()
        {
            return ImageBase.GetEnumerator(this);
        }

        public void Dispose()
        {
            return;
        }
    }
}
