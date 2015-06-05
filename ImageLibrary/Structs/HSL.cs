using System;

namespace ImageLibrary
{
    /// <summary>
    /// HSL / HSI
    /// </summary>
    [Serializable]
    public struct HSL : IEquatable<HSL>, IFormattable
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
        /// Lightness / Intensity
        /// [0.0 - 1.0]
        /// </summary>
        public double L { get; set; }

        public static bool operator == (HSL item1, HSL item2)
        {
            return item1.H == item2.H
                && item1.S == item2.S
                && item1.L == item2.L;
        }

        public static bool operator != (HSL item1, HSL item2)
        {
            return item1.H != item2.H
                || item1.S != item2.S
                || item1.L != item2.L;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(HSL) != obj.GetType())
                return false;

            return this == (HSL)obj;
        }

        public override int GetHashCode()
        {
            return H.GetHashCode() ^ S.GetHashCode() ^ L.GetHashCode();
        }

        public bool Equals(HSL other)
        {
            return this == other;
        }

        public override string ToString()
        {
            return this.ToString(null, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format(formatProvider, "HSL [H:{0}, S:{1}, L:{2}]", this.H, this.S, this.L);
        }
    }
}
