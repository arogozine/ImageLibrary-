using System;
using System.Collections.Generic;

namespace ImageLibrary
{
    /// <summary>
    /// Represents an column of an image
    /// </summary>
    /// <typeparam name="T">Image Pixel Type</typeparam>
    public class ImageColumn<T> : IEnumerable<T>, ICollection<T>
        where T : struct, IEquatable<T>
    {
        private readonly IImage<T> image;
        private readonly int height;
        private readonly int width;
        private readonly int column;
        private readonly int max;
        private readonly int min;

        /// <summary>
        /// Column Number
        /// </summary>
        public int Column
        {
            get
            {
                return this.column;
            }
        }

        /// <summary>
        /// Height of the Column
        /// </summary>
        public int Height
        {
            get
            {
                return this.height;
            }
        }

        /// <summary>
        /// Underlying image
        /// </summary>
        public IImage<T> Image
        {
            get
            {
                return this.image;
            }
        }

        internal ImageColumn(IImage<T> image, int column)
        {
            this.image = image;
            this.column = column;
            this.height = image.Height;
            this.width = image.Width;
            this.min = column;
            this.max = (this.height - 2) * this.width + this.min;
        }

        #region IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = min; i <= max; i += this.width )
            {
                yield return this.image[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }

        #endregion

        #region ICollection<T>

        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            for (int i = min; i <= max; i += this.width)
            {
                if(this.image[i].Equals(item))
                {
                    return true;
                }
            }

            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = min; i <= max; i += this.width, arrayIndex++)
            {
                array[arrayIndex] = this.image[i];
            }
        }

        public int Count
        {
            get { return this.height; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
