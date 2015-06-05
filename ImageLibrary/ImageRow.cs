using System;
using System.Collections;
using System.Collections.Generic;

namespace ImageLibrary
{
    /// <summary>
    /// Represents a row of an image.
    /// This class is just an abstration of IImage`T
    /// </summary>
    /// <typeparam name="T">Pixel Representation</typeparam>
    public class ImageRow<T> : IEnumerable<T>, ICollection<T>
        where T : struct, IEquatable<T>
    {
        private readonly IImage<T> image;
        private readonly int row;
        private readonly int min;
        private readonly int max;
        private readonly int width;

        /// <summary>
        /// Row Width
        /// </summary>
        public int Width
        {
            get
            {
                return this.width;
            }
        }

        /// <summary>
        /// Row Number
        /// </summary>
        public int Row
        {
            get
            {
                return this.row;
            }
        }

        /// <summary>
        /// Underlying Image
        /// </summary>
        public IImage<T> Image
        {
            get
            {
                return this.image;
            }
        }

        internal ImageRow(IImage<T> image, int row)
        {
            this.image = image;
            this.row = row;
            this.width = image.Width;
            this.min = this.width * row;
            this.max = this.min + this.width - 1;
        }

        public T this[int column]
        {
            get
            {
                if (column >= this.width)
                {
                    throw new ArgumentOutOfRangeException(nameof(column));
                }

                return this.image[min + column];
            }

            set
            {
                if (column >= this.width)
                {
                    throw new ArgumentOutOfRangeException(nameof(column));
                }

                this.image[min + column] = value;
            }
        }

        #region IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < this.width; i++)
            {
                yield return this.image[min + i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
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
            for (int i = 0; i < this.width; i++)
            {
                if (item.Equals(this.image[min + i]))
                {
                    return true;
                }
            }

            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(this.image.Data, this.min, array, arrayIndex, this.width);
        }

        public int Count
        {
            get { return this.width; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(T item)
        {
            // Not supported by IImage
            throw new NotSupportedException();
        }

        #endregion
    }
}
