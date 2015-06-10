using ImageLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImageLibrary
{
    internal class CmykImage : IImage<CMYK>
    {
        private int width;
        private int height;
        private CMYK[] data;

        public CmykImage(int width, int height, CMYK[] data)
        {
            this.width = width;
            this.height = height;
            this.data = data;
        }

        public void ToIndexedBgra(Action<int, BGRA> iRgba)
        {
            Parallel.For(0, this.Length, i => {
                iRgba(i, TypeConversion.ToBgra(this[i]));
            });
        }

        public IImage<CMYK> Pad(int width, int height)
            => ImageBase.Pad(this, CmykImage.Generate, width, height);

        public IImage<CMYK> Transpose()
            => ImageBase.Transpose(this, CmykImage.Generate);

        private static CmykImage Generate(int width, int height, CMYK[] data)
            => new CmykImage(width, height, data);

        public IImage<CMYK> Upsample()
            => ImageBase.Upsample(this, Generate);

        public IImage<CMYK> UpsampleCols()
            => ImageBase.UpsampleCols(this, Generate);

        public IImage<CMYK> UpsampleRows()
            => ImageBase.UpsampleRows(this, Generate);

        public IImage<CMYK> Downsample()
            => ImageBase.Downsample(this, Generate);

        public IImage<CMYK> DownsampleCols()
            => ImageBase.DownsampleCols(this, Generate);

        public IImage<CMYK> DownsampleRows()
            => ImageBase.DownsampleRows(this, Generate);
        
        public IImage<CMYK> FlipX()
            => ImageBase.FlipX(this, Generate);

        public IImage<CMYK> FlipY()
            => ImageBase.FlipY(this, Generate);

        public IImage<CMYK> FlipXY()
            => ImageBase.FlipXY(this, Generate);

        public IImage<CMYK> Crop(System.Drawing.Rectangle rect)
            => ImageBase.Crop(this, Generate, rect);

        public IImage<CMYK> Crop(int x1, int y1, int width, int height)
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

        public CMYK[] Data
        {
            get { return this.data; }
        }

        public IImage<CMYK> Copy()
        {
            CMYK[] data2 = new CMYK[this.Data.Length];
            Array.Copy(this.Data, data2, this.Data.Length);
            return new CmykImage(this.width, this.height, data2);
        }

        public CMYK this[int i, int j]
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

        public CMYK this[int index]
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

        public byte[] ToImage()
            => ImageBase.ToImage(this);

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
            return ((IEnumerable<CMYK>)this).GetEnumerator();
        }

        public void Add(CMYK item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(CMYK item)
            => ImageBase.Contains(this, item);

        public void CopyTo(CMYK[] array, int arrayIndex)
            => ImageBase.CopyTo(this, array, arrayIndex);

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(CMYK item)
        {
            throw new NotSupportedException();
        }

        IEnumerator<CMYK> IEnumerable<CMYK>.GetEnumerator()
        {
            return ImageBase.GetEnumerator(this);
        }

        public void Dispose()
        {
            return;
        }
    }
}
