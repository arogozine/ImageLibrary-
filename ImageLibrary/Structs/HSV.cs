using System;

namespace ImageLibrary
{
    /// <summary>
    /// HSV / HSB
    /// </summary>
    [Serializable]
    public struct HSV : IEquatable<HSV>, IFormattable
    {
        /// <summary>
        /// Hue
        /// [0.0 - 360.0]
        /// </summary>
        public double H { get; set; }

        /// <summary>
        /// Saturation
        /// [0.0 - 1.0]
        /// </summary>
        public double S { get; set; }

        /// <summary>
        /// Value
        /// [0.0 - 1.0]
        /// </summary>
        public double V { get; set; }

        public static bool operator ==(HSV left, HSV right)
        {
            return
                left.H == right.H &&
                left.S == right.S &&
                left.V == right.V;
        }

        public static bool operator !=(HSV left, HSV right)
        {
            return
                left.H != right.H ||
                left.S != right.S ||
                left.V != right.V;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(HSV) != obj.GetType())
                return false;

            return this == (HSV)obj;
        }

        public override int GetHashCode()
        {
            return H.GetHashCode() ^ S.GetHashCode() ^ V.GetHashCode();
        }

        public bool Equals(HSV other)
        {
            return this == other;
        }

        public override string ToString()
        {
            return this.ToString(null, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "HSV [H:{0}, S:{1}, V:{2}]", this.H, this.S, this.V);
        }
    }
}
