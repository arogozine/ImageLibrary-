﻿using System;

namespace ImageLibrary
{
    /// <summary>
    /// YCbCr, Y′CbCr, or Y Pb/Cb Pr/Cr
    /// </summary>
    [Serializable]
    public struct YCbCr : IEquatable<YCbCr>, IFormattable
    {
        /// <summary>
        /// [16 - 235]
        /// </summary>
        public byte Y { get; set; }

        /// <summary>
        /// [16 - 240]
        /// </summary>
        public byte Cb { get; set; }

        /// <summary>
        /// [16 - 240]
        /// </summary>
        public byte Cr { get; set; }

        public static bool operator ==(YCbCr left, YCbCr right)
        {
            return
                left.Y == right.Y &&
                left.Cb == right.Cb &&
                left.Cr == right.Cr;
        }

        public static bool operator !=(YCbCr left, YCbCr right)
        {
            return
                left.Y != right.Y ||
                left.Cb != right.Cb ||
                left.Cr != right.Cr;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(YCbCr) != obj.GetType())
                return false;

            return this == (YCbCr)obj;
        }

        public override string ToString()
        {
            return this.ToString(null, null);
        }

        public override int GetHashCode()
        {
            return Y.GetHashCode() ^ Cb.GetHashCode() ^ Cr.GetHashCode();
        }

        public bool Equals(YCbCr other)
        {
            return this == other;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "YCbCr [Y:{0}, Cb:{1}, Cr:{2}]", this.Y, this.Cb, this.Cr);
        }
    }
}
