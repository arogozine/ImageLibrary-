using ImageLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageLibrary
{
    /// <summary>
    /// Adapted from provided C language code from UNM Digital Image Processing class
    /// </summary>
    public sealed class Sketch
    {
        const int HOT = 1;

        private IList<Line>[] _grid;

        private List<Line> _list = new List<Line>();

        public double Scale { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        /// <summary>
        /// Number of lines in a Sketch
        /// </summary>
        public int Count
        {
            get
            {
                return List.Count;
            }
        }

        public int BixelHeight { get; set; }

        public int BixelWidth { get; set; }

        public double BixelDx { get; set; }

        public double BixelDy { get; set; }

        public IList<Line>[] Grid
        {
            get
            {
                return _grid;
            }
            set
            {
                this._grid = value;
            }
        }
        
        public List<Line> List
        {
            get
            {
                return this._list;
            }
        }

        public void DrawLines(System.Drawing.Image image, int color, double r, double g, double b)
        {
            double total = this._list
                .Select(x => x.Contrast)
                .Sum();

            double avg = total / this._list.Count;

            total = this._list
                .Select(x => x.Contrast - avg)
                .Select(delta => delta * delta)
                .Sum();

            double stddev = avg + Math.Sqrt(total / this._list.Count);
             //
            using (var graphics = Graphics.FromImage(image))
            using (SolidBrush solidBrush = new SolidBrush(Color.FromArgb(0, 0, 0)))
            {
                // Make Black Background
                graphics.FillRectangle(solidBrush, 0, 0, image.Width, image.Height);

                // Draw Lines
                for (int i = 0; i < this._list.Count; i++)
                {
                    Line first = this._list[i];

                    if (color == HOT)
                    {
                        double contrast = first.Contrast;

                        if (contrast == 0.0)
                        {
                            r = g = byte.MaxValue;
                            b = byte.MaxValue >> 1;
                        }
                        else
                        {
                            double max = (double)byte.MaxValue;

                            r = ImageExtensions.rhot(0f, stddev, contrast) * max;
                            g = ImageExtensions.ghot(0f, stddev, contrast) * max;
                            b = ImageExtensions.bhot(0f, stddev, contrast) * max;

                            r = r > max ? max : r;
                            g = g > max ? max : g;
                            b = b > max ? max : b;
                            //Console.WriteLine(r);
                        }
                    }

                    float x0 = (float)(first.XStart * this.Scale);
                    float y0 = (float)(first.YStart * this.Scale);
                    float x1 = (float)(first.XEnd * this.Scale);
                    float y1 = (float)(first.YEnd * this.Scale);

                    using (Pen blackPen = new Pen(Color.FromArgb((int)r, (int)g, (int)b), 1f))
                    {
                        graphics.DrawLine(blackPen, x0, y0, x1, y1);
                    }
                }
            }
        }

        public Sketch(int width, int height, IEnumerable<Line> line)
        {
            Init(width, height);
            this._list.AddRange(line);
        }

        public Sketch(int width, int height, params Line[] line)
        {
            Init(width, height);
            this._list.AddRange(line);
        }

        public Sketch(int width, int height)
        {
            Init(width, height);
        }

        private void Init(int width, int height)
        {
            this.Width = width;
            this.Height = height;

            this._grid = new Line[height * width][];
        }

        public Line this[int index]
        {
            get
            {
                return this._list[index];
            }
            set
            {
                this._list[index] = value;
            }
        }

        public unsafe void ToIndexedBgra(Action<int, BGRA> iRgba)
        {
            int width = (int)(this.Width * this.Scale);
            int height = (int)(this.Height * this.Scale);

            using (Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
            {
                DrawLines(bitmap, HOT, 0, 0, 0);

                using (var bitmapInRgbFormat = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format32bppArgb))
                {
                    BitmapData d = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmapInRgbFormat.PixelFormat);

                    int i = 0;

                    BGRA* ptr = (BGRA*)d.Scan0.ToPointer();
                    BGRA* lastptr = ptr + (width * height - 1);

                    while (ptr < lastptr)
                    {
                        iRgba(i++, *(ptr++));
                    }

                    bitmap.UnlockBits(d);
                }

            }
        }

        public byte[] ToBGR()
        {
            byte[] rgb = new byte[(int)(this.Height * this.Width * this.Scale * this.Scale * 3)];
            ToIndexedBgra((i, pixel) =>
            {
                int j = i * 3;
                rgb[j++] = pixel.B;
                rgb[j++] = pixel.G;
                rgb[j] = pixel.R;
            });

            return rgb;
        }

        public byte[] ToBGRA()
        {
            byte[] rgba = new byte[(int)(this.Height * this.Width * this.Scale * this.Scale * 4)];
            ToIndexedBgra((i, pixel) =>
            {
                int j = i * 4;
                rgba[j++] = pixel.B;
                rgba[j++] = pixel.G;
                rgba[j++] = pixel.R;
                rgba[j] = pixel.A;
            });

            return rgba;
        }

        public BGRA[] ToPixelColor()
        {
            BGRA[] rgba = new BGRA[(int)(this.Width * this.Height * this.Scale * this.Scale)];
            ToIndexedBgra((i, pixel) => { rgba[i] = pixel; });
            return rgba;
        }

        private BgraImage AsBgraImage()
        {
            int width = (int)(this.Width * this.Scale);
            int height = (int)(this.Height * this.Scale);

            return new BgraImage(width, height, ToPixelColor());
        }

        public void WriteImage(string fullPath)
        {
            AsBgraImage().WriteImage(fullPath);
        }

        public void WriteImage(string imagePath, string dirPath)
        {
            AsBgraImage().WriteImage(imagePath, dirPath);
        }

        public void WriteImage(string fullPath, ImageFormat format)
        {
            AsBgraImage().WriteImage(fullPath, format);
        }
    }

}
