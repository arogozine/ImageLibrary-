using ImageLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
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
        {
            return ImageBase.Pad(this, CmykImage.Generate, width, height);
        }

        public IImage<CMYK> Transpose()
        {
            return ImageBase.Transpose(this, CmykImage.Generate);
        }

        private static CmykImage Generate(int width, int height, CMYK[] data)
        {
            return new CmykImage(width, height, data);
        }

        public IImage<CMYK> Upsample()
        {
            return ImageBase.Upsample(this, Generate);
        }

        public IImage<CMYK> UpsampleCols()
        {
            return ImageBase.UpsampleCols(this, Generate);
        }

        public IImage<CMYK> UpsampleRows()
        {
            return ImageBase.UpsampleRows(this, Generate);
        }

        public IImage<CMYK> Downsample()
        {
            return ImageBase.Downsample(this, Generate);
        }

        public IImage<CMYK> DownsampleCols()
        {
            return ImageBase.DownsampleCols(this, Generate);
        }

        public IImage<CMYK> DownsampleRows()
        {
            return ImageBase.DownsampleRows(this, Generate); ;
        }

        public IImage<CMYK> FlipX()
        {
            return ImageBase.FlipX(this, Generate);
        }

        public IImage<CMYK> FlipY()
        {
            return ImageBase.FlipY(this, Generate);
        }

        public IImage<CMYK> FlipXY()
        {
            return ImageBase.FlipXY(this, Generate);
        }

        public IImage<CMYK> Crop(System.Drawing.Rectangle rect)
        {
            return ImageBase.Crop(this, Generate, rect);
        }

        public IImage<CMYK> Crop(int x1, int y1, int width, int height)
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

        public byte[] ToImage()
        {
            return ImageBase.ToImage(this);
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
        {
            return ImageBase.Contains(this, item);
        }

        public void CopyTo(CMYK[] array, int arrayIndex)
        {
            ImageBase.CopyTo(this, array, arrayIndex);
        }

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
