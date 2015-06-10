using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ImageLibrary
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct RGB : IEquatable<RGB>, IFormattable
    {
        [FieldOffset(0)]
        private double r;

        [FieldOffset(8)]
        private double g;

        [FieldOffset(16)]
        private double b;

        /// <summary>
        /// Red
        /// </summary>
        public double R
        {
            get { return this.r; }
            set { this.r = value; }
        }

        /// <summary>
        /// Green
        /// </summary>
        public double G
        {
            get { return this.g; }
            set { this.g = value; }
        }

        /// <summary>
        /// Blue
        /// </summary>
        public double B
        {
            get { return this.b; }
            set { this.b = value; }
        }

        public RGB(double red, double green, double blue)
        {
            this.r = red;
            this.g = green;
            this.b = blue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGB operator +(RGB a, RGB b)
        {
            return new RGB(a.R + b.R, a.G + b.G, a.B + b.B);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGB operator -(RGB a, RGB b)
        {
            return new RGB(a.R - b.R, a.G - b.G, a.B - b.B);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGB operator *(RGB a, RGB b)
        {
            return new RGB(a.R * b.R, a.G * b.G, a.B * b.B);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGB operator *(RGB a, double b)
        {
            return new RGB(a.R * b, a.G * b, a.B * b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGB operator *(double b, RGB a)
        {
            return new RGB(a.R * b, a.G * b, a.B * b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGB operator /(RGB a, RGB b)
        {
            return new RGB(a.R / b.R, a.G / b.G, a.B / b.B);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGB operator /(RGB a, double b)
        {
            return new RGB(a.R / b, a.G / b, a.B / b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(RGB item1, RGB item2)
        {
            return (
                item1.R == item2.R
                && item1.G == item2.G
                && item1.B == item2.B
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(RGB item1, RGB item2)
        {
            return (
                item1.R != item2.R
                || item1.G != item2.G
                || item1.B != item2.B
                );
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            return (this == (RGB)obj);
        }

        public override int GetHashCode()
        {
            return R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode();
        }

        public bool Equals(RGB other)
        {
            return this == other;
        }

        public override string ToString()
        {
            return this.ToString(null, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "RGB [R:{0}, G:{1}, B:{2}]", this.r, this.g, this.b);
        }
    }
}