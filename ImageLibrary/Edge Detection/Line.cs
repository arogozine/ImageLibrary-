using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageLibrary
{
    /// <summary>
    /// Adapted from C code from UNM Digital Image Processing Class
    /// </summary>
    public class Line
    {
        private double? theta = null;
        private double? length = null;

        public double Coverage
        {
            get;
            internal set;
        }

        public double Age
        {
            get;
            internal set;
        }

        public bool Replaced
        {
            get;
            internal set;
        }

        public bool Merged
        {
            get;
            internal set;
        }

        public double Contrast
        {
            get;
            internal set;
        }

        public double Theta
        {
            get {
                // Lazy Eval
                if (this.theta == null)
                    this.theta = Math.Atan2(this.XEnd - this.XStart, this.YEnd - this.YStart);

                return this.theta.Value; 
            }
            set { this.theta = value; }
        }

        public double Length
        {
            get {
                
                // Lazy Eval
                if (this.length == null)
                {
                    double dx = this.XEnd - this.XStart;
                    double dy = this.YEnd - this.YStart;
                    this.length = Math.Sqrt(dx * dx + dy * dy);
                }

                return this.length.Value; 
            }
            set { this.length = value; }
        }

        public double XStart
        {
            get;
            internal set;
        }

        public double XEnd
        {
            get;
            internal set;
        }

        public double YStart
        {
            get;
            internal set;
        }

        public double YEnd
        {
            get;
            internal set;
        }

        public List<Line> LinkOnStart
        {
            get;
            set;
        }

        public List<Line> LinkOnEnd
        {
            get;
            set;
        }

        public Line(double x0, double y0, double x1, double y1)
        {
            this.XStart = x0;
            this.XEnd = x1;

            this.YStart = y0;
            this.YEnd = y1;

            this.Contrast = 0.0;
            this.Coverage = 1.0;

            LinkOnStart = new List<Line>();
            LinkOnEnd = new List<Line>();
        }

        public Line(double x0, double y0, double x1, double y1, double contrast)
        {
            this.XStart = x0;
            this.XEnd = x1;

            this.YStart = y0;
            this.YEnd = y1;

            this.Contrast = contrast;
            this.Coverage = 1.0;

            LinkOnStart = new List<Line>();
            LinkOnEnd = new List<Line>();
        }
    }
}