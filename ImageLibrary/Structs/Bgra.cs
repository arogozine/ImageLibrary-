using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageLibrary
{
    /// <summary>
    /// BGRA struct for easy unsafe bitmap manipulation
    /// See,
    /// http://stackoverflow.com/questions/1176910/finding-specific-pixel-colors-of-a-bitmapimage
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct BGRA : IEquatable<BGRA>, IFormattable
    {
        [FieldOffset(0)]
        private uint bgra;

        [FieldOffset(0)]
        private byte blue;

        [FieldOffset(1)]
        private byte green;

        [FieldOffset(2)]
        private byte red;

        [FieldOffset(3)]
        private byte alpha;

        public static readonly BGRA Black = (BGRA)4278190080U;

        /// <summary>
        /// 32 bit BGRA
        /// </summary>
        public uint Value
        {
            get { return this.bgra; }
            set { this.bgra = value; }
        }

        /// <summary>
        /// Blue Color
        /// </summary>
        public byte B
        {
            get { return this.blue; }
            set { this.blue = value; }
        }

        /// <summary>
        /// Green Color
        /// </summary>
        public byte G
        {
            get { return this.green; }
            set { this.green = value; }
        }

        /// <summary>
        /// Red Color
        /// </summary>
        public byte R
        {
            get { return this.red; }
            set { this.red = value; }
        }

        /// <summary>
        /// Alpha
        /// </summary>
        public byte A
        {
            get { return this.alpha; }
            set { this.alpha = value; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(BGRA item1, BGRA item2)
        {
            return item1.Value == item2.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(BGRA item1, BGRA item2)
        {
            return item1.Value != item2.Value;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || typeof(BGRA) != obj.GetType())
                return false;

            return this == (BGRA)obj;
        }

        public override int GetHashCode()
        {
            return this.bgra.GetHashCode();
        }

        public override string ToString()
        {
            return this.ToString(null, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "[B:{0}, G:{1}, R:{2}, A:{3}]", this.blue, this.green, this.red, this.alpha);
        }

        public static BGRA Parse(string input, NumberStyles style, IFormatProvider provider)
        {
            string[] parts;

            if (input == null)
            {
                throw new ArgumentNullException("input", "String input was not specified");
            }

            if ((parts = input.Split(',')).Length != 4)
            {
                throw new FormatException("Invalid Input");
            }

            var output = new BGRA()
            {
                R = byte.Parse(parts[0].Substring(3)),
                G = byte.Parse(parts[1].Substring(3)),
                B = byte.Parse(parts[2].Substring(3)),
                A = byte.Parse(parts[3].Substring(3, parts[3].Length - 1)),
            };
            
            return output;
        }

        public static BGRA Parse(String input, IFormatProvider provider)
        {
            return Parse(input, NumberStyles.None, provider);
        }

        public static BGRA Parse(String input)
        {
            return Parse(input, null);
        }

        public bool Equals(BGRA other)
        {
            return this == other;
        }

        public static implicit operator BGRA(Int32 val)
        {
            unchecked
            {
                return new BGRA() { Value = (UInt32)val };
            }
        }

        public static implicit operator Int32(BGRA bgra)
        {
            unchecked
            {
                return (int)bgra.Value;
            }
        }

        public static unsafe implicit operator BGRA(UInt32 val)
        {
            return new BGRA() { Value = val };
        }

        public static implicit operator UInt32(BGRA val)
        {
            return val.Value;
        }

        public static unsafe BGRA AsBgra(byte* val)
        {
            return *((BGRA*)val);
        }

        public static unsafe BGRA AsBgra(Int32* val)
        {
            return *((BGRA*)val);
        }

        public static unsafe BGRA AsBgra(UInt32* val)
        {
            return *val;
        }

        public static unsafe BGRA AsBgra(void* val)
        {
            return *((BGRA*)val);
        }

        public static unsafe void AsBgra(UInt64* val, out BGRA left, out BGRA right)
        {
            UInt32* ptr = (UInt32*)val;
            left = *ptr;
            right = *(++ptr);
        }

        public static unsafe void AsBgra(Int64* val, out BGRA left, out BGRA right)
        {
            UInt32* ptr = (UInt32*)val;
            left = *ptr;
            right = *(++ptr);
        }

        public static unsafe void AsBgra(UInt64 val, out BGRA left, out BGRA right)
        {
            unchecked
            {
                left = (UInt32)(val >> 32);
                right = (UInt32)((val << 32) >> 32);
            }
        }
    }
}
