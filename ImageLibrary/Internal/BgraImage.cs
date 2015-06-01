using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageLibrary
{
    internal class BgraImage : IImage<BGRA>
    {
        private int width;
        private int height;
        private BGRA[] data;

        internal BgraImage(int width, int height, BGRA[] data)
        {
            this.width = width;
            this.height = height;
            this.data = data;
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

        public BGRA[] Data
        {
            get { return this.data; }
        }

        public IImage<BGRA> Copy()
        {
            BGRA[] data2 = new BGRA[this.Data.Length];
            Array.Copy(this.Data, data2, this.Data.Length);
            return new BgraImage(this.Width, this.Length, data2);
        }

        public IImage<BGRA> Crop(System.Drawing.Rectangle rect)
        {
            return ImageBase.Crop(this, BgraImage.Generate, rect);
        }

        public IImage<BGRA> Crop(int x1, int y1, int width, int height)
        {
            return ImageBase.Crop(this, BgraImage.Generate, x1, y1, width, height);
        }

        public IImage<BGRA> Pad(int width, int height)
        {
            return ImageBase.Pad(this, BgraImage.Generate, width, height);
        }

        public IImage<BGRA> Upsample()
        {
            return ImageBase.Upsample(this, BgraImage.Generate);
        }

        public IImage<BGRA> UpsampleCols()
        {
            return ImageBase.UpsampleCols(this, BgraImage.Generate);
        }

        public IImage<BGRA> UpsampleRows()
        {
            return ImageBase.UpsampleRows(this, BgraImage.Generate);
        }

        public IImage<BGRA> Downsample()
        {
            return ImageBase.Downsample(this, BgraImage.Generate);
        }

        public IImage<BGRA> DownsampleCols()
        {
            return ImageBase.DownsampleCols(this, BgraImage.Generate);
        }

        public IImage<BGRA> DownsampleRows()
        {
            return ImageBase.DownsampleRows(this, BgraImage.Generate);;
        }

        public IImage<BGRA> FlipX()
        {
            return ImageBase.FlipX(this, BgraImage.Generate);
        }

        public IImage<BGRA> FlipY()
        {
            return ImageBase.FlipY(this, BgraImage.Generate);
        }

        public IImage<BGRA> FlipXY()
        {
            return ImageBase.FlipXY(this, BgraImage.Generate);
        }

        public IImage<BGRA> Transpose()
        {
            return ImageBase.Transpose(this, BgraImage.Generate);
        }

        public BGRA this[int i, int j]
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

        public BGRA this[int index]
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

        public void ToIndexedBgra(Action<int, BGRA> iRgba)
        {
            Parallel.For(0, this.data.Length, i => iRgba(i, this.data[i]));
        }

        public byte[] ToBGR()
        {
            return ImageBase.ToRGB(this);
        }

        unsafe public byte[] ToBGRA()
        {
            byte[] bgra = new byte[this.Length * 4];

            fixed(BGRA* _dataPtr = &this.data[0])
            fixed (byte* _bgraPtr = &bgra[0])
            {
                BGRA* bgraPtr = (BGRA*)_bgraPtr;
                BGRA* dataPtr = _dataPtr;

                for (int i = 0; i <= this.data.Length; i++)
                {
                    *(bgraPtr++) = *(dataPtr++);
                }
            }

            return bgra;
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
            return ((IEnumerable<BGRA>)this).GetEnumerator();
        }

        public void Add(BGRA item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(BGRA item)
        {
            for (int i = 0; i < this.data.Length; i++)
            {
                if (this.data[i] == item)
                {
                    return true;
                }
            }

            return false;
        }

        public void CopyTo(BGRA[] array, int arrayIndex)
        {
            ImageBase.CopyTo(this, array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(BGRA item)
        {
            throw new NotSupportedException();
        }

        IEnumerator<BGRA> IEnumerable<BGRA>.GetEnumerator()
        {
            return ImageBase.GetEnumerator(this);
        }

        private static BgraImage Generate(int width, int height, BGRA[] data)
        {
            return new BgraImage(width, height, data);
        }

        public void Dispose()
        {
            return;
        }
    }
}
