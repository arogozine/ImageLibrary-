﻿using ImageLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageLibrary
{
    internal class HslImage : IImage<HSL>
    {
        private int width;
        private int height;
        private HSL[] data;

        public HslImage(int width, int height, HSL[] data)
        {
            this.width = width;
            this.height = height;
            this.data = data;
        }

        public void ToIndexedBgra(Action<int, BGRA> action)
        {
            for (int index = 0; index < this.Data.Length; index++)
            {
                HSL hsi = this.Data[index];
                action(index, TypeConversion.ToBgra(hsi));
            }
        }

        public IImage<HSL> Pad(int width, int height)
            => ImageBase.Pad(this, Generate, width, height);

        public IImage<HSL> Transpose()
            => ImageBase.Transpose(this, Generate);

        private static HslImage Generate(int width, int height, HSL[] data)
            => new HslImage(width, height, data);

        public IImage<HSL> Upsample()
            => ImageBase.Upsample(this, Generate);

        public IImage<HSL> UpsampleCols()
            => ImageBase.UpsampleCols(this, Generate);

        public IImage<HSL> UpsampleRows()
            => ImageBase.UpsampleRows(this, Generate);

        public IImage<HSL> Downsample()
            => ImageBase.Downsample(this, Generate);

        public IImage<HSL> DownsampleCols()
            => ImageBase.DownsampleCols(this, Generate);

        public IImage<HSL> DownsampleRows()
            => ImageBase.DownsampleRows(this, Generate);

        public IImage<HSL> FlipX()
            => ImageBase.FlipX(this, Generate);

        public IImage<HSL> FlipY()
            => ImageBase.FlipY(this, Generate);

        public IImage<HSL> FlipXY()
            => ImageBase.FlipXY(this, Generate);

        public IImage<HSL> Crop(System.Drawing.Rectangle rect)
            => ImageBase.Crop(this, Generate, rect);

        public IImage<HSL> Crop(int x1, int y1, int width, int height)
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

        public HSL[] Data
        {
            get { return this.data; }
        }

        public IImage<HSL> Copy()
        {
            HSL[] data2 = new HSL[this.Data.Length];
            Array.Copy(this.Data, data2, this.Data.Length);
            return Generate(this.width, this.height, data2);
        }

        public HSL this[int i, int j]
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

        public HSL this[int index]
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

        public void CopyTo(Array array, int arrayIndex)
            => ImageBase.CopyTo(this, array, arrayIndex);

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
            return ((IEnumerable<HSL>)this).GetEnumerator();
        }

        public void Add(HSL item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(HSL item)
        {
            return this.data.Contains(item);
        }

        public void CopyTo(HSL[] array, int arrayIndex)
        {
            ImageBase.CopyTo(this, array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(HSL item)
        {
            throw new NotSupportedException();
        }

        IEnumerator<HSL> IEnumerable<HSL>.GetEnumerator()
        {
            return ImageBase.GetEnumerator(this);
        }

        public void Dispose()
        {
            return;
        }
    }
}
