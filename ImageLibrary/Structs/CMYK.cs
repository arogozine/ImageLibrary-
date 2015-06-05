using System;

namespace ImageLibrary
{
    [Serializable]
    public struct CMYK : IEquatable<CMYK>, IFormattable
    {
        /// <summary>
        /// Cyan
        /// [0.0 - 1.0]
        /// </summary>
        public double C { get; set; }

        /// <summary>
        /// Magenta 
        /// [0.0 - 1.0]
        /// </summary>
        public double M { get; set; }

        /// <summary>
        /// Yellow
        /// [0.0 - 1.0]
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Black Key Color
        /// [0.0 - 1.0]
        /// </summary>
        public double K { get; set; }

        public static bool operator == (CMYK left, CMYK right)
        {
            return 
                left.C == right.C && 
                left.M == right.M && 
                left.Y == right.Y && 
                left.K == right.K;
        }

        public static bool operator !=(CMYK left, CMYK right)
        {
            return
                left.C != right.C ||
                left.M != right.M ||
                left.Y != right.Y ||
                left.K != right.K;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(CMYK) != obj.GetType()) 
                return false;

            return this == (CMYK)obj;
        }

        public override int GetHashCode()
        {
            return C.GetHashCode() ^ M.GetHashCode() ^ Y.GetHashCode() ^ K.GetHashCode();
        }

        public override string ToString()
        {
            return this.ToString(null, null);
        }

        public bool Equals(CMYK other)
        {
            return this == other;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format(formatProvider, "CMYK [C:{0}, M:{1}, Y:{2}, K:{3}]", this.C, this.M, this.Y, this.K);
        }
    }
}
