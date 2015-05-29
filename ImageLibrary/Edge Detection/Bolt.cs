using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ImageLibrary.Extensions;

namespace ImageLibrary.EdgeDetection
{
    public enum CrossingMethod
    {
        One,
        Two,
        Three
    }

    /// <summary>
    /// Adapted from provided C language code from UNM Digital Image Processing class
    /// </summary>
    public static class Bolt
    {
        #region Zero Crossings

        public static Sketch ZeroCrossings(this IImage<Double> sp1)
        {
            return ZeroCrossings(sp1, CrossingMethod.One);
        }

        public static Sketch ZeroCrossings(this IImage<Double> sp1, CrossingMethod method)
        {
            // Convolve with 3 x 3 kernel
            var img1 = sp1.Convolve(
                new double[][]{
                new[] { -0.125, -0.125, -0.125 },
                new[] { -0.125, +1.000, -0.125 },
                new[] { -0.125, -0.125, -0.125 }
                });

            var img2 = sp1.Convolve(
                new double[][] {
                new[] { 0.25, 0.00, -0.25 },
                new[] { 0.50, 0.00, -0.50 },
                new[] { 0.25, 0.00, -0.25 }
                });

            var img3 = sp1.Convolve(new double[][] {
               new[]{ +0.25, +0.50, +0.25 },
               new[]{ +0.00, +0.00, +0.00 },
               new[]{ -0.25, -0.50, -0.25 }
            });

            return ZeroCrossings(img1, img2, img3, method);
        }

        private static Sketch ZeroCrossings(IImage<double> sp1, IImage<double> sp2, IImage<double> sp3, CrossingMethod method)
        {

            if (sp1.Height != sp2.Height || sp2.Height != sp3.Height)
            {
                throw new ArgumentException("Images have different heights");
            }

            if (sp1.Width != sp2.Width || sp2.Width != sp3.Width)
            {
                throw new ArgumentException("Images have different widths");
            }

            ulong code;

            double contrast;
            double lapl00, lapl01, lapl10, lapl11;
            double dx00, dx01, dx10, dx11;
            double dy00, dy01, dy10, dy11;
            double x0, y0, x1, y1, dxc, dyc, xc, yc;

            x0 = y0 = x1 = y1 = 0f;


            int rows = sp1.Height;
            int cols = sp1.Width;

            Sketch result = new Sketch(cols, rows);

            var lapl = sp1;
            var dx = sp2;
            var dy = sp3;
            for (int i = 0; i < rows - 1; i++)
            {
                for (int j = 0; j < cols - 1; j++)
                {

                    // 00 -- 01
                    // |     |
                    // 10 -- 11
                    lapl00 = lapl[i * cols + j];
                    lapl01 = lapl[i * cols + j + 1];
                    lapl10 = lapl[(i + 1) * cols + j];
                    lapl11 = lapl[(i + 1) * cols + j + 1];

                    code = 0;

                    if (lapl00 * lapl01 * lapl10 * lapl11 != 0)
                    {
                        if (lapl00 > 0) code |= 1;
                        if (lapl01 > 0) code |= 2;
                        if (lapl10 > 0) code |= 4;
                        if (lapl11 > 0) code |= 8;
                    }

                    // + to the right
                    switch (code)
                    {
                        case 0:
                            // - -
                            // - -
                            continue;
                        case 1:
                            // + -
                            // - -
                            x0 = j + lapl00 / (lapl00 - lapl01);
                            y0 = i;
                            x1 = j;
                            y1 = i + lapl00 / (lapl00 - lapl10);
                            break;
                        case 2:
                            // - +
                            // - -
                            x0 = j + 1;
                            y0 = i + lapl01 / (lapl01 - lapl11);
                            x1 = j + lapl00 / (lapl00 - lapl01);
                            y1 = i;
                            break;
                        case 3:
                            // + +
                            // - -
                            x0 = j + 1;
                            y0 = i + lapl01 / (lapl01 - lapl11);
                            x1 = j;
                            y1 = i + lapl00 / (lapl00 - lapl10);
                            break;
                        case 4:
                            // - -
                            // + -
                            x0 = j;
                            y0 = i + lapl00 / (lapl00 - lapl10);
                            x1 = j + lapl10 / (lapl10 - lapl11);
                            y1 = i + 1;
                            break;
                        case 5:
                            // + -
                            // + -
                            x0 = j + lapl00 / (lapl00 - lapl01);
                            y0 = i;
                            x1 = j + lapl10 / (lapl10 - lapl11);
                            y1 = i + 1;
                            break;
                        case 6:
                            /* - + */
                            /* + - */
                            break;
                        case 7:
                            /* + + */
                            /* + - */
                            x0 = j + 1;
                            y0 = i + lapl01 / (lapl01 - lapl11);
                            x1 = j + lapl10 / (lapl10 - lapl11);
                            y1 = i + 1;
                            break;
                        case 8:
                            /* - - */
                            /* - + */
                            x0 = j + lapl10 / (lapl10 - lapl11);
                            y0 = i + 1;
                            x1 = j + 1;
                            y1 = i + lapl01 / (lapl01 - lapl11);
                            break;
                        case 9:
                            /* + - */
                            /* - + */
                            continue;
                        case 10:
                            /* - + */
                            /* - + */
                            x0 = j + lapl10 / (lapl10 - lapl11);
                            y0 = i + 1;
                            x1 = j + lapl00 / (lapl00 - lapl01);
                            y1 = i;
                            break;
                        case 11:
                            /* + + */
                            /* - + */
                            x0 = j + lapl10 / (lapl10 - lapl11);
                            y0 = i + 1;
                            x1 = j;
                            y1 = i + lapl00 / (lapl00 - lapl10);
                            break;
                        case 12:
                            /* - - */
                            /* + + */
                            x0 = j;
                            y0 = i + lapl00 / (lapl00 - lapl10);
                            x1 = j + 1;
                            y1 = i + lapl01 / (lapl01 - lapl11);
                            break;
                        case 13:
                            /* + - */
                            /* + + */
                            x0 = j + lapl00 / (lapl00 - lapl01);
                            y0 = i;
                            x1 = j + 1;
                            y1 = i + lapl01 / (lapl01 - lapl11);
                            break;
                        case 14:
                            /* - + */
                            /* + + */
                            x0 = j;
                            y0 = i + lapl00 / (lapl00 - lapl10);
                            x1 = j + lapl00 / (lapl00 - lapl01);
                            y1 = i;
                            break;
                        case 15:
                            /* + + */
                            /* + + */
                            continue;
                        default:
                            continue;
                    }

                    xc = (x0 + x1) / 2 - j;
                    yc = (y0 + y1) / 2 - i;

                    dx00 = dx[i * cols + j];
                    dx01 = dx[i * cols + j + 1];
                    dx10 = dx[(i + 1) * cols + j];
                    dx11 = dx[(i + 1) * cols + j + 1];

                    dy00 = dy[i * cols + j];
                    dy01 = dy[i * cols + j + 1];
                    dy10 = dy[(i + 1) * cols + j];
                    dy11 = dy[(i + 1) * cols + j + 1];

                    dxc = Interpolate(dx00, dx01, dx10, dx11, xc, yc);
                    dyc = Interpolate(dy00, dy01, dy10, dy11, xc, yc);
                    contrast = Math.Sqrt(dxc * dxc + dyc * dyc);

                    switch (method)
                    {
                        case CrossingMethod.One:
                            AddZc0(x0, y0, x1, y1, contrast, result);
                            break;

                        case CrossingMethod.Two:
                            AddZc1((x0 + x1) / 2, (y0 + y1) / 2, Math.Atan2(-dxc, dyc), contrast, result);
                            break;

                        case CrossingMethod.Three:
                            AddZc1(x0, y0, Math.Atan2(-dxc, dyc), contrast, result);
                            break;
                    }
                }
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Interpolate(double f00, double f01, double f10, double f11, double x, double y)
        {
            return (f01 - f00) * x + (f10 - f00) * y + (f11 + f00 - f10 - f01) * x * y + f00;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddZc0(double x0, double y0, double x1, double y1, double contrast, Sketch sketch)
        {
            double dx = x1 - x0;
            double dy = y1 - y0;

            Line sp2 = new Line(x0, y0, x1, y1, contrast)
            {
                Theta = Math.Atan2(dy, dx),
                Length = Math.Sqrt(dx * dx + dy * dy),
                Coverage = 1.0,
                Age = 0.0,
                Replaced = false,
                Merged = false
            };

            sketch.List.Add(sp2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddZc1(double x, double y, double theta, double contrast, Sketch sketch)
        {

            double dx = Math.Cos(theta) / 2.0;
            double dy = Math.Sin(theta) / 2.0;

            Line sp2 = new Line(x - dx, y - dy, x + dx, y + dy, contrast)
            {
                Theta = theta,
                Length = 1.0,
                Coverage = 1.0,
                Age = 0.0,
                Replaced = false,
                Merged = false
            };

            sketch.List.Add(sp2);
        }

        #endregion

        #region Filter

        public static Sketch Filter(this Sketch sketch, Func<Line, bool> filter)
        {
            IEnumerable<Line> lines = sketch.List
                .Where(filter);

            return new Sketch(sketch.Width, sketch.Height, lines);
        }

        public static Sketch FilterOnContrast(this Sketch input, double lower)
        {
            return FilterOnContrast(input, lower, 256.0);
        }

        public static Sketch FilterOnContrast(this Sketch input, double lower, double upper)
        {
            return Filter(input, line =>
            {
                double contrast = line.Contrast;
                return contrast > lower && contrast < upper;
            });
        }

        public static Sketch FilterOnLength(this Sketch input, double lower)
        {
            return FilterOnLength(input, lower, 1000.0);
        }

        public static Sketch FilterOnLength(this Sketch input, double lower, double upper)
        {
            return Filter(input, line =>
            {
                double length = line.Length;
                return length > lower && length < upper;
            });
        }

        public static Sketch FilterOnAngle(this Sketch input, double lower, double upper)
        {
            return Filter(input, line =>
            {
                double theta = line.Theta;
                return AngleDifference(theta, lower) <= Math.PI && AngleDifference(theta, upper) >= Math.PI;
            });
        }

        #endregion

        #region Display Link Graph

        public static byte[] DisplayLinkGraph(this Sketch s)
        {
            return DisplayLinkGraph(s, ImageFormat.Png);
        }

        public static byte[] DisplayLinkGraph(this Sketch s, ImageFormat format)
        {
            int width = (int)(s.Width * s.Scale);
            int height = (int)(s.Height * s.Scale);

            using (Bitmap _img = new Bitmap(width, height, PixelFormat.Format32bppArgb))
            {
                s.DrawLines(_img, 0 /* RGB */ , 0, 0, 255 /* BLUE */);
                draw_links(s, _img);
                using (MemoryStream ms = new MemoryStream())
                {
                    _img.Save(ms, format);
                    return ms.ToArray();
                }
            }
        }

        private static void draw_links(Sketch s, Bitmap image)
        {
            IList<Line> lines = s.List;

            using (var graphics = Graphics.FromImage(image))           
            using (Pen whitePen = new Pen(Color.White, 1f))
            {
                foreach (Line ln1 in lines)
                {
                    for (int end1 = 0; end1 <= 1; end1++)
                    {
                        bool _end1 = end1 == 0;
                        bool _end2 = !_end1;

                        // Links can be on END and on START
                        var lns = ln1.LinkOnEnd;

                        if (lns == null)
                            continue;

                        foreach (var ln2 in lns)
                        {
                            var x1 = _end1 ? ln1.XStart : ln1.XEnd;
                            var y1 = _end1 ? ln1.YStart : ln1.YEnd;
                            var x2 = _end2 ? ln2.XStart : ln2.XEnd;
                            var y2 = _end2 ? ln2.YStart : ln2.YEnd;

                            drawLink(graphics, whitePen, s.Scale, x1, y1, x2, y2);
                        }
                    }
                }
            }
            
        }

        private static void drawLink(Graphics g, Pen p, double scale, double x1, double y1, double x2, double y2)//, double sx, double sy)
        {
            double r = 0.5f;

            double d = Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));

            double cos_theta = d / (r * 2.0);
            double sin_theta = Math.Sin(Math.Acos(cos_theta));

            // Avoid Non Values
            if (double.IsNaN(sin_theta))
                return;

            double cosPhi = (x2 - x1) / d;
            double sinPhi = (y2 - y1) / d;

            double xc = x1 + r * (cos_theta * cosPhi - sin_theta * sinPhi);
            double yc = y1 + r * (sin_theta * cosPhi + cos_theta * sinPhi);

            double rad1 = Math.Atan2(y1 - yc, x1 - xc);
            double rad2 = Math.Atan2(y2 - yc, x2 - xc);

            //
            // Calculate Scale Factors Once
            //

            xc *= scale;
            yc *= scale;
            r *= scale;

            //
            // Determine bigger radius
            //

            double iValue;
            double cValue;

            if (AngleDifference(rad1, rad2) > AngleDifference(rad2, rad1))
            {
                iValue = rad2;
                cValue = rad1;
            }
            else
            {
                iValue = rad1;
                cValue = rad2;
            }

            //
            // Draw Circles and Semi Circles
            //

            List<PointF> points = new List<PointF>();

            for (double t = iValue; AngleDifference(t, cValue) > 0.05; t = t + 0.05)
            {
                var a = xc + Math.Cos(t) * r;
                var b = yc + Math.Sin(t) * r;

                PointF point = new PointF((float)a, (float)b);
                points.Add(point);
            }

            if (points.Count == 0)
                return;

            g.DrawLines(p, points.ToArray());
        }

        #endregion

        #region PO Link

        public static Sketch PoLink(this Sketch sketch, LSchedule schedule)
        {
            return PoLink(sketch,
                schedule.GetLinesMode,
                schedule.LinkRadius,
                schedule.ContrastRatioMin,
                schedule.ContrastRatioMax,
                schedule.DeltaThetaMax,
                schedule.EndProjectRelMax,
                schedule.LateralDistAbsMaxSq);
        }

        public static Sketch PoLink(
    this Sketch sketch,
    bool getLinesMode,
    double linkRadius,
    double contrastRatioMin,
    double contrastRatioMax,
    double deltaThetaMax,
    double endProjectRelMax,
    double lateralDistAbsMaxSq
    )
        {
            var rows = sketch.Height;
            var cols = sketch.Width;

            var ls = sketch.List;

            if (getLinesMode)
            {
                po_initialize_grid_for_all_lns(sketch, (int)(rows / (linkRadius * 2.0)), (int)(cols / (linkRadius * 2.0)));
            }
            else
            {
                po_initialize_grid_for_ln_ends(sketch, (int)(rows / (linkRadius * 2.0)), (int)(cols / (linkRadius * 2.0)));
            }

            foreach (var ln in ls)
            {
                for (int end = 0; end <= 1; end++)
                {
                    double x, y;

                    if (end == 0)
                    {
                        x = ln.XStart;
                        y = ln.YStart;
                    }
                    else
                    {
                        x = ln.XEnd;
                        y = ln.YEnd;
                    }

                    Line[] _lns;

                    if (getLinesMode)
                    {
                        _lns = GetAllLinesInACircle(sketch, x, y, linkRadius)
                            .ToArray();
                    }
                    else
                    {
                        _lns = get_all_lns_end_in_circle(sketch, x, y, end == 1, linkRadius)
                            .ToArray();
                    }

                    // Process as Parallel Enumeration
                    var filterMag = FilterOnMagnitude(ln, contrastRatioMin, contrastRatioMax);
                    var filterDir = FilterOnDirection(ln, deltaThetaMax);
                    var filterEnd = FilterEndProject(ln, end, endProjectRelMax);
                    var filterLat = FilterOnLateralDistance(ln, lateralDistAbsMaxSq);

                    // Sets values that should be filtered out to null
                    Parallel.For(0, _lns.Length, _x =>
                    {
                        Line l = _lns[_x];

                        if (l == ln)
                        {
                            _lns[_x] = null;
                            return;
                        }
                        else if (filterMag(l))
                        {
                            _lns[_x] = null;
                            return;
                        }
                        else if (filterDir(l))
                        {
                            _lns[_x] = null;
                            return;
                        }
                        else if (filterEnd(l))
                        {
                            _lns[_x] = null;
                            return;
                        }

                        else if (filterLat(l))
                        {
                            _lns[_x] = null;
                            return;
                        }
                    });

                    ln.LinkOnEnd = _lns
                        .Where(l => l != null) // Remove Filtered Elements
                        .ToList();
                }
            }

            return sketch;
        }

        private static void po_initialize_grid_for_all_lns(Sketch sketch, int bixelRows, int bixelCols)
        {
            int rows = sketch.Height;
            int cols = sketch.Width;

            int bixels = bixelRows * bixelCols;
            double bixel_dx = (double)cols / (double)bixelCols;
            double bixel_dy = (double)rows / (double)bixelRows;

            var grid = new IList<Line>[bixels];

            for (int index = 0; index < bixels; index++)
                grid[index] = new List<Line>();

            foreach (var ln in sketch.List)
            {
                int index;

                var x0 = ln.XStart;
                var y0 = ln.YStart;
                var x1 = ln.XEnd;
                var y1 = ln.YEnd;

                var dy = y1 - y0;
                var dx = x1 - x0;

                var x = x0;
                var y = y0;

                if (Math.Abs(dx) > Math.Abs(dy))
                {

                    if (x <= x1)
                    {
                        while (x <= x1)
                        {
                            index = (int)(Math.Floor(y / bixel_dy) * bixelCols + Math.Floor(x / bixel_dx));

                            if (index >= 0 && index < bixels)
                                grid[index].Add(ln);

                            x += bixel_dx;
                            y = y0 + (x - x0) * (dy / dx);
                        }
                    }
                    else
                    {
                        while (x >= x1)
                        {
                            index = (int)(Math.Floor(y / bixel_dy) * bixelCols + Math.Floor(x / bixel_dx));

                            if (index >= 0 && index < bixels)
                                grid[index].Add(ln);

                            x -= bixel_dx;
                            y = y0 + (x - x0) * (dy / dx);
                        }
                    }

                }
                else // |dy| > |dx|
                {
                    if (y <= y1)
                    {
                        while (y <= y1)
                        {
                            index = (int)(Math.Floor(y / bixel_dy) * bixelCols + Math.Floor(x / bixel_dx));

                            if (index >= 0 && index < bixels)
                                grid[index].Add(ln);

                            y += bixel_dy;
                            x = x0 + (y - y0) * (dx / dy);
                        }
                    }
                    else
                    {
                        while (y >= y1)
                        {
                            index = (int)(Math.Floor(y / bixel_dy) * bixelCols + Math.Floor(x / bixel_dx));

                            if (index >= 0 && index < bixels)
                                grid[index].Add(ln);

                            y -= bixel_dy;
                            x = x0 + (y - y0) * (dx / dy);
                        }
                    }
                }

                index = (int)(Math.Floor(y1 / bixel_dy) * bixelCols + Math.Floor(x1 / bixel_dx));

                if (index >= 0 && index < bixels)
                    grid[index].Add(ln);
            }

            sketch.BixelWidth = bixelCols;
            sketch.BixelHeight = bixelRows;

            sketch.BixelDx = bixel_dx;
            sketch.BixelDy = bixel_dy;
            sketch.Grid = grid;
        }

        private static void po_initialize_grid_for_ln_ends(Sketch sketch, int bixelRows, int bixelCols)
        {
            int rows = sketch.Height;
            int cols = sketch.Width;

            int bixels = bixelRows * bixelCols;
            double bixel_dx = (double)cols / (double)bixelCols;
            double bixel_dy = (double)rows / (double)bixelRows;

            var grid = new IList<Line>[bixels];

            for (int index = 0; index < bixels; index++)
                grid[index] = new List<Line>();

            foreach (var ln in sketch.List)
            {
                var x0 = ln.XStart;
                var y0 = ln.YStart;
                var x1 = ln.XEnd;
                var y1 = ln.YEnd;


                int index;

                // Starts
                index = (int)(Math.Floor(y0 / bixel_dy) * bixelCols + Math.Floor(x0 / bixel_dx));

                if (index >= 0 && index < bixels)
                    grid[index].Add(ln);

                // Ends
                index = (int)(Math.Floor(y1 / bixel_dy) * bixelCols + Math.Floor(x1 / bixel_dx));

                if (index >= 0 && index < bixels)
                    grid[index].Add(ln);

            }

            sketch.BixelWidth = bixelCols;
            sketch.BixelHeight = bixelRows;

            sketch.BixelDx = bixel_dx;
            sketch.BixelDy = bixel_dy;
            sketch.Grid = grid;
        }

        public static List<Line> GetAllLinesInACircle(Sketch skwtch, double x, double y, double radius)
        {
            double rr = radius * radius;
            double bixel_dx = skwtch.BixelDx;
            double bixel_dy = skwtch.BixelDy;

            int bixel_rows = skwtch.BixelHeight;
            int bixel_cols = skwtch.BixelWidth;

            int bixel_i0 = (int)Math.Floor((y - radius) / bixel_dy);
            int bixel_j0 = (int)Math.Floor((x - radius) / bixel_dx);

            int bixel_i1 = (int)Math.Ceiling((y + radius) / bixel_dy);
            int bixel_j1 = (int)Math.Ceiling((x + radius) / bixel_dx);

            bixel_i0 = Math.Max(0, bixel_i0);
            bixel_j0 = Math.Max(0, bixel_j0);
            bixel_i1 = Math.Min(bixel_i1, bixel_rows);
            bixel_j1 = Math.Min(bixel_j1, bixel_cols);

            List<Line> linesInACircle = new List<Line>();

            var grid = skwtch.Grid;

            for (int bixel_i = bixel_i0; bixel_i < bixel_i1; bixel_i++)
            {
                for (int bixel_j = bixel_j0; bixel_j < bixel_j1; bixel_j++)
                {
                    int index = bixel_i * bixel_cols + bixel_j;

                    IList<Line> lns_in_bixel = grid[index];

                    foreach (var ln in lns_in_bixel)
                    {
                        var x0 = ln.XStart;
                        var y0 = ln.YStart;
                        var x1 = ln.XEnd;
                        var y1 = ln.YEnd;

                        bool condition = (x0 - x) * (x0 - x) + (y0 - y) * (y0 - y) < rr
                            || (x1 - x) * (x1 - x) + (y1 - y) * (y1 - y) < rr
                            || lineIntersectsCircle(x0, y0, x1, y1, x, y, radius);

                        if (condition && !linesInACircle.Contains(ln))
                        {
                            linesInACircle.Add(ln);
                        }
                    }
                }
            }

            return linesInACircle;
        }

        public static bool lineIntersectsCircle(double x0, double y0, double x1, double y1, double x, double y, double r)
        {
            double dx = x1 - x0;
            double dy = y1 - y0;

            double u, v;

            double m, b, d, f, g, h;

            if (Math.Abs(dx) >= Math.Abs(dy) || dy == 0)
            {
                m = dy / dx;
                b = -m * (x0 - x) + (y0 - y);
                f = -2 * m * b;
                g = 2 * (m * m + 1);
                h = f * f + 2 * g * r * r;

                if (h < 0)
                    return false;

                d = Math.Sqrt(h);
                u = (f + d) / g + x;
                v = (f - d) / g + x;
                return x1 >= u && u >= x0 && x1 >= v && v >= x0;
            }
            else
            {
                m = dx / dy;
                b = -m * (y0 - y) + (x0 - x);
                f = -2 * m * b;
                g = 2 * (m * m + 1);
                h = f * f + 2 * g * r * r;

                if (h < 0)
                    return false;

                d = Math.Sqrt(h);
                u = (f + d) / g + x;
                v = (f - d) / g + x;
                return x1 >= u && u >= x0 && x1 >= v && v >= x0;
            }
        }

        private static List<Line> get_all_lns_end_in_circle(Sketch sketch, double x, double y, bool end, double radius)
        {
            double rr = radius * radius;

            double bixel_dx = sketch.BixelDx;
            double bixel_dy = sketch.BixelDy;

            int bixel_rows = sketch.BixelHeight;
            int bixel_cols = sketch.BixelWidth;

            int bixel_i0 = (int)Math.Floor((y - radius) / bixel_dy);
            int bixel_j0 = (int)Math.Floor((x - radius) / bixel_dx);

            int bixel_i1 = (int)Math.Ceiling((y + radius) / bixel_dy);
            int bixel_j1 = (int)Math.Ceiling((x + radius) / bixel_dx);

            var grid = sketch.Grid;

            List<Line> linesInCircle = new List<Line>();

            bixel_i0 = Math.Max(0, bixel_i0);
            bixel_j0 = Math.Max(0, bixel_j0);
            bixel_i1 = Math.Min(bixel_i1, bixel_rows);
            bixel_j1 = Math.Min(bixel_j1, bixel_cols);

            for (int bixel_i = bixel_i0; bixel_i < bixel_i1; bixel_i++)
            {
                for (int bixel_j = bixel_j0; bixel_j < bixel_j1; bixel_j++)
                {

                    var index = bixel_i * bixel_cols + bixel_j;
                    IList<Line> lns_in_bixel = grid[index];

                    foreach (var ln in lns_in_bixel)
                    {
                        var x0 = end ? ln.XEnd : ln.XStart;
                        var y0 = end ? ln.YEnd : ln.XStart;

                        if (((x0 - x) * (x0 - x) + (y0 - y) * (y0 - y) < rr) && !linesInCircle.Contains(ln))
                        {
                            linesInCircle.Add(ln);
                        }

                    }
                }
            }
            return linesInCircle;
        }


        private static Func<Line, bool> FilterOnMagnitude(Line line, double contrastRatioMin, double contrastRatioMax)
        {
            double contrast = line.Contrast;

            return x =>
            {
                double contrast_ratio = x.Contrast / contrast;
                return contrast_ratio < contrastRatioMin || contrast_ratio > contrastRatioMax;
            };
        }

        private static Func<Line, bool> FilterOnDirection(Line line, double deltaThetaMax)
        {
            const double TWOPI = Math.PI * 2.0;
            double theta0 = line.Theta;

            return x =>
            {
                double theta1 = x.Theta;

                double deltaTheta = Math.Abs(AngleDifference(theta0, theta1));

                deltaTheta = Math.Min(deltaTheta, TWOPI - deltaTheta);

                return deltaTheta > deltaThetaMax;
            };
        }

        private static Func<Line, bool> FilterEndProject(Line line, int end, double end_proj_rel_max)
        {
            bool _end = end == 1;

            return x =>
            {
                // No need to divide by length!
                double d1 = proj_pt_on_line_rel_dist(line, _end, _end ? x.XStart : x.XEnd, _end ? x.YStart : x.YEnd);
                double d2 = proj_pt_on_line_rel_dist(line, _end, _end ? x.XEnd : x.XStart, _end ? x.YEnd : x.XStart);

                return d1 > end_proj_rel_max || d1 <= d2;
            };
        }

        public static double proj_pt_on_line_rel_dist(Line line, bool end, double x, double y)
        {
            double x0 = end ? line.XEnd : line.XStart;
            double y0 = end ? line.YEnd : line.YStart;
            double x1 = end ? line.XStart : line.XEnd;
            double y1 = end ? line.YStart : line.YEnd;
            double length = line.Length;

            double dx = x1 - x0;
            double dy = y1 - y0;

            return ((x - x0) * dx + (y - y0) * dy) / (length * length);
        }

        private static Func<Line, bool> FilterOnLateralDistance(Line line, double lateralDistAbsMaxSq)
        {
            double x0 = line.XStart;
            double y0 = line.YStart;

            double x1 = line.XEnd;
            double y1 = line.YEnd;

            return x =>
            {
                double xm = (x.XStart + x.XEnd) / 2.0;
                double ym = (x.YStart + x.YEnd) / 2.0;

                return pt_line_dist_sq(x0, y0, x1, y1, xm, ym) > lateralDistAbsMaxSq;
            };
        }

        public static double pt_line_dist_sq(double x0, double y0, double x1, double y1, double x, double y)
        {
            double dx = x1 - x0;
            double dy = y1 - y0;
            double length_squared = dx * dx + dy * dy;

            double dot = ((x - x0) * dx + (y - y0) * dy) / length_squared;

            double u = x0 + dx * dot;
            double v = y0 + dy * dot;

            return (u - x) * (u - x) + (v - y) * (v - y);
        }

        #endregion

        /// <summary>
        /// svd.c: Perform a singular value decomposition A = USV' of square matrix.
        /// This routine has been adapted with permission from a Pascal implementation
        /// (c) 1988 J. C. Nash, "Compact numerical methods for computers", Hilger 1990.
        /// The A matrix must be pre-allocated with 2n rows and n columns. On calling
        /// the matrix to be decomposed is contained in the first n rows of A. On return
        /// the n first rows of A contain the product US and the lower n rows contain V
        /// (not V'). The S2 vector returns the square of the singular values.
        /// (c) Copyright 1996 by Carl Edward Rasmussen. */
        /// </summary>
        /// <param name="A">Matrix</param>
        /// <param name="S2">Square of Singular Values</param>
        /// <param name="n">First n rows</param>
        public static void Svd(double[][] A, double[] S2, int n)
        {
            int i, j, k, EstColRank = n, RotCount = n, SweepCount = 0,
            slimit = (n < 120) ? 30 : n / 4;
            double eps = 1e-15, e2 = 10.0 * n * eps * eps, tol = 0.1 * eps, vt, p, x0,
                 y0, q, r, c0, s0, d1, d2;

            for (i = 0; i < n; i++) { for (j = 0; j < n; j++) A[n + i][j] = 0.0; A[n + i][i] = 1.0; }
            while (RotCount != 0 && SweepCount++ <= slimit)
            {
                RotCount = EstColRank * (EstColRank - 1) / 2;
                for (j = 0; j < EstColRank - 1; j++)
                    for (k = j + 1; k < EstColRank; k++)
                    {
                        p = q = r = 0.0;
                        for (i = 0; i < n; i++)
                        {
                            x0 = A[i][j]; y0 = A[i][k];
                            p += x0 * y0; q += x0 * x0; r += y0 * y0;
                        }
                        S2[j] = q; S2[k] = r;
                        if (q >= r)
                        {
                            if (q <= e2 * S2[0] || Math.Abs(p) <= tol * q)
                                RotCount--;
                            else
                            {
                                p /= q; r = 1.0 - r / q; vt = Math.Sqrt(4.0 * p * p + r * r);
                                c0 = Math.Sqrt(0.5 * (1.0 + r / vt)); s0 = p / (vt * c0);
                                for (i = 0; i < 2 * n; i++)
                                {
                                    d1 = A[i][j]; d2 = A[i][k];
                                    A[i][j] = d1 * c0 + d2 * s0; A[i][k] = -d1 * s0 + d2 * c0;
                                }
                            }
                        }
                        else
                        {
                            p /= r; q = q / r - 1.0; vt = Math.Sqrt(4.0 * p * p + q * q);
                            s0 = Math.Sqrt(0.5 * (1.0 - q / vt));
                            if (p < 0.0) s0 = -s0;
                            c0 = p / (vt * s0);
                            for (i = 0; i < 2 * n; i++)
                            {
                                d1 = A[i][j]; d2 = A[i][k];
                                A[i][j] = d1 * c0 + d2 * s0; A[i][k] = -d1 * s0 + d2 * c0;
                            }
                        }
                    }
                while (EstColRank > 2 && S2[EstColRank - 1] <= S2[0] * tol + tol * tol) EstColRank--;
            }

            if (SweepCount > slimit)
            {
                Console.WriteLine("Warning: Reached maximum number of sweeps {0} in SVD routine...\n", slimit);
            }

            /*    printf("Warning: Reached maximum number of sweeps (%d) in SVD routine...\n"
                   , ); */
        }

        public static Sketch PoMerge(params Sketch[] sketches)
        {
            return PoMerge(sketches);
        }

        public static Sketch PoMerge(IEnumerable<Sketch> sketches)
        {
            // Height / Width Check
            var first = sketches.First();

            if (sketches.Any(x => x.Height != first.Height || x.Width != first.Width))
            {
                throw new ArgumentException("Arguments must be sketches with equal dimensions");
            }

            IEnumerable<Line> list = sketches
                .AsParallel()
                .SelectMany(x => x.List)
                .Where(line => !line.Replaced && !line.Merged)
                .Distinct()
                .Select(line => { line.Merged = true; return line; });

            Sketch sketch = new Sketch(first.Width, first.Height, list)
            {
                Scale = first.Scale,
                BixelDx = first.BixelDx,
                BixelDy = first.BixelDy,
                BixelHeight = first.BixelHeight,
                BixelWidth = first.BixelWidth
            };

            if (first.Grid != null)
            {
                sketch.Grid = new List<Line>[first.Grid.Length];

                for (int i = 0; i < sketch.Grid.Length; i++)
                {
                    sketch.Grid[i] = new List<Line>(first.Grid[i]);
                }
            }

            return sketch;
        }

        public static Sketch Union(params Sketch[] sketches)
        {
            if (sketches.Length == 0)
                return null;

            return Union(sketches.AsEnumerable());
        }

        public static Sketch Union(IEnumerable<Sketch> sketches)
        {
            if (sketches == null || !sketches.Any())
                return null;

            Sketch first = sketches.First();

            if (sketches.Any(x => x.Width != first.Width || x.Height != first.Height))
            {
                throw new ArgumentException("Arguments must be sketches with equal dimensions.");
            }

            var range = sketches
                .AsParallel()
                .SelectMany(x => x.List)
                .Distinct();

            return new Sketch(first.Height, first.Width, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double AngleDifference(double rad1, double rad2)
        {
            const double TWOPI = Math.PI * 2;

            return Remainder(rad1 - rad2, TWOPI);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Remainder(double x, double y)
        {
            double ratio = x / y;
            return y * (ratio - Math.Floor(ratio));
        }

        public static Sketch PoReplace(this Sketch sketch, RSchedule schedule)
        {
            return sketch.PoReplace(
                schedule.ReplaceRadius,
                schedule.Straightness,
                schedule.CoverageFilterOn,
                schedule.MinCumulativeCoverage,
                schedule.MinCoverage,
                schedule.DeltaAbstLevMax,
                schedule.ReplaceStraightestPath);
        }

        /// <summary>
        /// Enumerates all paths in the link-graph which contain a line and lie
        /// within a circle of replace radius centered on the line's midpoint.
        /// Lines belonging to paths which are straight enough are replaced by new lines
        /// representing the entire straight path (see Bolt et al.)
        /// 
        /// Replaced lines in the sketch are marked replaced and the age of all other lines
        /// is incramented by one.
        /// </summary>
        /// <param name="sp1"></param>
        /// <param name="replaceRadius"></param>
        /// <param name="straightness"></param>
        /// <param name="coverageFilterOn"></param>
        /// <param name="minCumulativeCoverage"></param>
        /// <param name="minCoverage"></param>
        /// <param name="deltaAbstLevMax"></param>
        /// <param name="replaceStraightestPath"></param>
        /// <returns>New Sketch containing a mixture of new and unreplaced lines</returns>
        public static Sketch PoReplace(this Sketch sp1,
            double replaceRadius,
            double straightness,
            bool coverageFilterOn,
            double minCumulativeCoverage,
            double minCoverage,
            int deltaAbstLevMax,
            bool replaceStraightestPath)
        {
            double best_mse = double.MaxValue;
            double best_u00 = 0, best_v00 = 0, best_u0 = 0, best_v0 = 0;
            double best_u1 = 0, best_v1 = 0, best_u11 = 0, best_v11 = 0;

            Line best_ln = null, best_ln0 = null, best_ln1 = null;

            double best_avg_contrast = 0, best_cumulative_coverage = 0;

            Sketch result = new Sketch(sp1.Width, sp1.Height);

            double rr = replaceRadius * replaceRadius;

            double[][] A = new double[4][];
            double[] s = new double[2];

            int[] path_histogram = new int[10];

            for (int i = 0; i < 4; i++)
                A[i] = new double[2];

            // Enumerate length three paths
            foreach (var ln in sp1.List.Where(x => !x.Replaced))
            {
                best_mse = double.MaxValue;

                double x0 = ln.XStart;
                double y0 = ln.YStart;

                double x1 = ln.XEnd;
                double y1 = ln.YEnd;

                double x = (x0 + x1) / 2.0;
                double y = (y0 + y1) / 2.0;

                foreach (var ln0 in ln.LinkOnStart.Where(ln0 => !ln0.Replaced))
                {
                    double x00 = ln0.XStart;
                    double y00 = ln0.YStart;

                    if (((x - x00) * (x - x00) + (y - y00) * (y - y00)) > rr)
                    {
                        continue;
                    }

                    double x01 = ln0.XEnd;
                    double y01 = ln0.YEnd;

                    var ls1 = ln.LinkOnEnd;

                    foreach (var ln1 in ls1.Where(ln1 => !ln1.Replaced))
                    {
                        double x11 = ln1.XEnd;
                        double y11 = ln1.YEnd;

                        if (((x - x11) * (x - x11) + (y - y11) * (y - y11)) > rr)
                        {
                            continue;
                        }

                        double x10 = ln1.XStart;
                        double y10 = ln1.YStart;

                        double xm = (x00 + x01 + x0 + x1 + x10 + x11) / 6.0;
                        double ym = (y00 + y01 + y0 + y1 + y10 + y11) / 6.0;

                        A[0][0] = (x00 - xm) * (x00 - xm) + (x01 - xm) * (x01 - xm) +
                          (x0 - xm) * (x0 - xm) + (x1 - xm) * (x1 - xm) + (x10 - xm) * (x10 - xm) + (x11 - xm) * (x11 - xm);

                        A[1][1] = (y00 - ym) * (y00 - ym) + (y01 - ym) * (y01 - ym) +
                          (y0 - ym) * (y0 - ym) + (y1 - ym) * (y1 - ym) + (y10 - ym) * (y10 - ym) + (y11 - ym) * (y11 - ym);

                        A[0][1] = A[1][0] = (x00 - xm) * (y00 - ym) + (x01 - xm) * (y01 - ym) +
                          (x0 - xm) * (y0 - ym) + (x1 - xm) * (y1 - ym) + (x10 - xm) * (y10 - ym) + (x11 - xm) * (y11 - ym);

                        Bolt.Svd(A, s, 2);

                        double l = A[2][0] * A[2][0] + A[2][1] * A[2][1];

                        double dx = -A[2][0] / l;
                        double dy = A[2][1] / l;

                        double dot00 = dx * (x00 - xm) + dy * (y00 - ym);

                        double u00 = xm + dot00 * dx;
                        double v00 = ym + dot00 * dy;

                        double dot01 = dx * (x01 - xm) + dy * (y01 - ym);

                        double u01 = xm + dot01 * dx;
                        double v01 = ym + dot01 * dy;

                        double dot0 = dx * (x0 - xm) + dy * (y0 - ym);

                        double u0 = xm + dot0 * dx;
                        double v0 = ym + dot0 * dy;

                        double dot1 = dx * (x1 - xm) + dy * (y1 - ym);

                        double u1 = xm + dot1 * dx;
                        double v1 = ym + dot1 * dy;

                        double dot10 = dx * (x10 - xm) + dy * (y10 - ym);

                        double u10 = xm + dot10 * dx;
                        double v10 = ym + dot10 * dy;

                        double dot11 = dx * (x11 - xm) + dy * (y11 - ym);

                        double u11 = xm + dot11 * dx;
                        double v11 = ym + dot11 * dy;

                        double l0 = ln0.Length;
                        l = ln.Length;
                        double l1 = ln1.Length;

                        double scale = ((x00 - x11) * (x00 - x11) + (y00 - y11) * (y00 - y11)) * 4.0;

                        double mse = (pt_line_dist_sq(u00, v00, u11, v11, x00, y00) * l0 +
                               pt_line_dist_sq(u00, v00, u11, v11, x01, y01) * l0 +
                               pt_line_dist_sq(u00, v00, u11, v11, x0, y0) * l +
                               pt_line_dist_sq(u00, v00, u11, v11, x1, y1) * l +
                               pt_line_dist_sq(u00, v00, u11, v11, x10, y10) * l1 +
                               pt_line_dist_sq(u00, v00, u11, v11, x11, y11) * l1) / scale;

                        // paths++;

                        if (mse < best_mse && mse < straightness)
                        {
                            double m0 = Math.Sqrt((u01 - u00) * (u01 - u00) + (v01 - v00) * (v01 - v00));
                            double m = Math.Sqrt((u1 - u0) * (u1 - u0) + (v1 - v0) * (v1 - v0));
                            double m1 = Math.Sqrt((u11 - u10) * (u11 - u10) + (v11 - v10) * (v11 - v10));

                            double n0 = Math.Sqrt((u00 - u0) * (u00 - u0) + (v00 - v0) * (v00 - v0));
                            double n1 = Math.Sqrt((u11 - u1) * (u11 - u1) + (v11 - v1) * (v11 - v1));

                            double o0, o1;

                            if (n0 < m0)
                                o0 = m0 - n0;
                            else
                                o0 = 0.0;

                            if (n1 < m1)
                                o1 = m1 - n1;
                            else
                                o1 = 0.0;

                            double k0 = ln0.Coverage;
                            double k = ln.Coverage;
                            double k1 = ln1.Coverage;

                            double ll = Math.Sqrt((u11 - u00) * (u11 - u00) + (v11 - v00) * (v11 - v00));

                            double cumulative_coverage = (m0 * k0 + m * k + m1 * k1 - o0 - o1) / ll;
                            double coverage = (m0 + m + m1 - o0 - o1) / ll;

                            if (!coverageFilterOn || (coverageFilterOn &&
                                cumulative_coverage > minCumulativeCoverage && coverage > minCoverage))
                            {
                                best_mse = mse;

                                double c0 = ln0.Contrast;
                                double c = ln.Contrast;
                                double c1 = ln1.Contrast;

                                best_avg_contrast = (c0 * l0 + c * l + c1 * l1) / (l0 + l + l1);
                                best_cumulative_coverage = cumulative_coverage;

                                best_u00 = u00;
                                best_v00 = v00;
                                best_u11 = u11;
                                best_v11 = v11;

                                best_ln0 = ln0;
                                best_ln = ln;
                                best_ln1 = ln1;

                                if (!replaceStraightestPath)
                                    goto break_break;
                            }
                        }
                    }
                }

                break_break:
                if (best_mse < double.MaxValue)
                {
                    AddNewLine(best_u00, best_v00, best_u11, best_v11, best_avg_contrast, best_cumulative_coverage, result);

                    best_ln0.Replaced = true;
                    best_ln.Replaced = true;
                    best_ln1.Replaced = true;
                }
            }

            for (int i = 0; i <= 9; i++)
                path_histogram[i] = 0;

            // Enumerate length two left-paths
            foreach (var ln in sp1.List.Where(ln => !ln.Replaced))
            {
                best_mse = double.MaxValue;

                double x0 = ln.XStart;
                double y0 = ln.YStart;

                double x1 = ln.XEnd;
                double y1 = ln.YEnd;

                double x = (x0 + x1) / 2.0;
                double y = (y0 + y1) / 2.0;

                foreach (var ln0 in ln.LinkOnStart.Where(ln0 => !ln0.Replaced))
                {
                    double x00 = ln0.XStart;
                    double y00 = ln0.YStart;

                    if (((x - x00) * (x - x00) + (y - y00) * (y - y00)) > rr)
                    {
                        continue;
                    }

                    double x01 = ln0.XEnd;
                    double y01 = ln0.YEnd;

                    double xm = (x00 + x01 + x0 + x1) / 4.0;
                    double ym = (y00 + y01 + y0 + y1) / 4.0;

                    A[0][0] = (x00 - xm) * (x00 - xm) + (x01 - xm) * (x01 - xm) + (x0 - xm) * (x0 - xm) + (x1 - xm) * (x1 - xm);
                    A[1][1] = (y00 - ym) * (y00 - ym) + (y01 - ym) * (y01 - ym) + (y0 - ym) * (y0 - ym) + (y1 - ym) * (y1 - ym);
                    A[0][1] = A[1][0] = (x00 - xm) * (y00 - ym) + (x01 - xm) * (y01 - ym) + (x0 - xm) * (y0 - ym) + (x1 - xm) * (y1 - ym);

                    Svd(A, s, 2);

                    double l = A[2][0] * A[2][0] + A[2][1] * A[2][1];

                    double dx = -A[2][0] / l;
                    double dy = A[2][1] / l;

                    double dot00 = dx * (x00 - xm) + dy * (y00 - ym);

                    double u00 = xm + dot00 * dx;
                    double v00 = ym + dot00 * dy;

                    double dot01 = dx * (x01 - xm) + dy * (y01 - ym);

                    double u01 = xm + dot01 * dx;
                    double v01 = ym + dot01 * dy;

                    double dot0 = dx * (x0 - xm) + dy * (y0 - ym);

                    double u0 = xm + dot0 * dx;
                    double v0 = ym + dot0 * dy;

                    double dot1 = dx * (x1 - xm) + dy * (y1 - ym);

                    double u1 = xm + dot1 * dx;
                    double v1 = ym + dot1 * dy;

                    double l0 = ln0.Length;
                    l = ln.Length;

                    double scale = ((x00 - x1) * (x00 - x1) + (y00 - y1) * (y00 - y1)) * 2.0;

                    double mse = (pt_line_dist_sq(u00, v00, u1, v1, x00, y00) * l0 +
                       pt_line_dist_sq(u00, v00, u1, v1, x01, y01) * l0 +
                       pt_line_dist_sq(u00, v00, u1, v1, x0, y0) * l +
                       pt_line_dist_sq(u00, v00, u1, v1, x1, y1) * l) / scale;

                    if (mse < best_mse && mse < straightness)
                    {
                        double m0 = Math.Sqrt((u01 - u00) * (u01 - u00) + (v01 - v00) * (v01 - v00));
                        double m = Math.Sqrt((u1 - u0) * (u1 - u0) + (v1 - v0) * (v1 - v0));

                        double n0 = Math.Sqrt((u00 - u0) * (u00 - u0) + (v00 - v0) * (v00 - v0));

                        double o0;
                        if (n0 < m0)
                            o0 = m0 - n0;
                        else
                            o0 = 0.0;

                        double k0 = ln0.Coverage;
                        double k = ln.Coverage;

                        double ll = Math.Sqrt((u1 - u00) * (u1 - u00) + (v1 - v00) * (v1 - v00));

                        double cumulative_coverage = (m0 * k0 + m * k - o0) / ll;
                        double coverage = (m0 + m - o0) / ll;

                        if (!coverageFilterOn || (coverageFilterOn &&
                             cumulative_coverage > minCumulativeCoverage && coverage > minCoverage))
                        {
                            best_mse = mse;

                            double c0 = ln0.Contrast;
                            double c = ln.Contrast;

                            best_avg_contrast = (c0 * l0 + c * l) / (l0 + l);
                            best_cumulative_coverage = cumulative_coverage;

                            best_u00 = u00;
                            best_v00 = v00;
                            best_u1 = u1;
                            best_v1 = v1;

                            best_ln0 = ln0;
                            best_ln = ln;

                            if (!replaceStraightestPath)
                                break;
                        }
                    }
                }

                if (best_mse < double.MaxValue)
                {
                    AddNewLine(best_u00, best_v00, best_u1, best_v1, best_avg_contrast, best_cumulative_coverage, result);

                    best_ln0.Replaced = true;
                    best_ln.Replaced = true;
                }
            }

            // Enumerate length two right-paths
            foreach (var ln in sp1.List.Where(ln => !ln.Replaced))
            {
                best_mse = double.MaxValue;

                double x0 = ln.XStart;
                double y0 = ln.YStart;

                double x1 = ln.XEnd;
                double y1 = ln.YEnd;

                double x = (x0 + x1) / 2.0;
                double y = (y0 + y1) / 2.0;

                foreach (var ln1 in ln.LinkOnEnd.Where(ln1 => !ln1.Replaced))
                {
                    double x11 = ln1.XEnd;
                    double y11 = ln1.YEnd;

                    if (((x - x11) * (x - x11) + (y - y11) * (y - y11)) > rr)
                    {
                        continue;
                    }

                    double x10 = ln1.XStart;
                    double y10 = ln1.YStart;

                    double xm = (x0 + x1 + x10 + x11) / 4.0;
                    double ym = (y0 + y1 + y10 + y11) / 4.0;

                    A[0][0] = (x0 - xm) * (x0 - xm) + (x1 - xm) * (x1 - xm) + (x10 - xm) * (x10 - xm) + (x11 - xm) * (x11 - xm);
                    A[1][1] = (y0 - ym) * (y0 - ym) + (y1 - ym) * (y1 - ym) + (y10 - ym) * (y10 - ym) + (y11 - ym) * (y11 - ym);
                    A[0][1] = A[1][0] = (x0 - xm) * (y0 - ym) + (x1 - xm) * (y1 - ym) + (x10 - xm) * (y10 - ym) + (x11 - xm) * (y11 - ym);

                    Svd(A, s, 2);

                    double l = A[2][0] * A[2][0] + A[2][1] * A[2][1];

                    double dx = -A[2][0] / l;
                    double dy = A[2][1] / l;

                    double dot0 = dx * (x0 - xm) + dy * (y0 - ym);

                    double u0 = xm + dot0 * dx;
                    double v0 = ym + dot0 * dy;

                    double dot1 = dx * (x1 - xm) + dy * (y1 - ym);

                    double u1 = xm + dot1 * dx;
                    double v1 = ym + dot1 * dy;

                    double dot10 = dx * (x10 - xm) + dy * (y10 - ym);

                    double u10 = xm + dot10 * dx;
                    double v10 = ym + dot10 * dy;

                    double dot11 = dx * (x11 - xm) + dy * (y11 - ym);

                    double u11 = xm + dot11 * dx;
                    double v11 = ym + dot11 * dy;

                    l = ln.Length;
                    double l1 = ln1.Length;

                    double scale = ((x0 - x11) * (x0 - x11) + (y0 - y11) * (y0 - y11)) * 2.0;

                    double mse = (pt_line_dist_sq(u0, v0, u11, v11, x0, y0) * l +
                       pt_line_dist_sq(u0, v0, u11, v11, x1, y1) * l +
                       pt_line_dist_sq(u0, v0, u11, v11, x10, y10) * l1 +
                       pt_line_dist_sq(u0, v0, u11, v11, x11, y11) * l1) / scale;

                    if (mse < best_mse && mse < straightness)
                    {

                        double m = Math.Sqrt((u1 - u0) * (u1 - u0) + (v1 - v0) * (v1 - v0));
                        double m1 = Math.Sqrt((u11 - u10) * (u11 - u10) + (v11 - v10) * (v11 - v10));

                        double n1 = Math.Sqrt((u11 - u1) * (u11 - u1) + (v11 - v1) * (v11 - v1));

                        double o1;
                        if (n1 < m1)
                            o1 = m1 - n1;
                        else
                            o1 = 0.0;

                        double k = ln.Coverage;
                        double k1 = ln1.Coverage;

                        double ll = Math.Sqrt((u11 - u0) * (u11 - u0) + (v11 - v0) * (v11 - v0));

                        double cumulative_coverage = (m * k + m1 * k1 - o1) / ll;
                        double coverage = (m + m1 - o1) / ll;

                        if (!coverageFilterOn || (coverageFilterOn &&
                             cumulative_coverage > minCumulativeCoverage && coverage > minCoverage))
                        {
                            best_mse = mse;

                            double c = ln.Contrast;
                            double c1 = ln1.Contrast;

                            best_avg_contrast = (c * l + c1 * l1) / (l + l1);
                            best_cumulative_coverage = cumulative_coverage;

                            best_u0 = u0;
                            best_v0 = v0;
                            best_u11 = u11;
                            best_v11 = v11;

                            best_ln = ln;
                            best_ln1 = ln1;

                            if (!replaceStraightestPath)
                                break;
                        }
                    }
                }

                if (best_mse < double.MaxValue)
                {
                    AddNewLine(best_u0, best_v0, best_u11, best_v11, best_avg_contrast, best_cumulative_coverage, result);

                    best_ln.Replaced = true;
                    best_ln1.Replaced = true;
                }
            }

            // Copy non-replaced lines
            foreach (var ln in sp1.List.Where(ln => !ln.Replaced))
            {
                if (ln.Age > deltaAbstLevMax)
                    continue;

                AddOldLine(ln, result);
            }

            return result;
        }

        public static void AddOldLine(Line sp0, Sketch sp1)
        {
            sp0.Age += 1;
            sp0.LinkOnStart = new List<Line>();
            sp0.LinkOnEnd = new List<Line>();
            sp1.List.Add(sp0);
        }

        public static void AddNewLine(double x0, double y0, double x1, double y1, double contrast, double coverage, Sketch sp1)
        {
            double dx = x1 - x0;
            double dy = y1 - y0;

            var sp2 = new Line(x0, y0, x1, y1, contrast)
            {
                Theta = Math.Atan2(dy, dx),
                Length = Math.Sqrt(dx * dx + dy * dy),
                Coverage = coverage,
                Age = 0,
                Replaced = false,
                Merged = false
            };

            sp1.List.Add(sp2);
        }


        private static Sketch ReplaceStep(Sketch sketch, int n)
        {
            return sketch
                .PoLink(LinkSchedule[n])
                .PoReplace(ReplaceSchedule[n]);
        }

        public static IEnumerable<Sketch> LinkReplaceCycle(Sketch zcs, int m)
        {
            var acc = new List<Sketch>() { zcs };

            for (int n = 0; n < m; n++)
            {
                var sketch = ReplaceStep(acc.First(), n);
                acc.Add(sketch);
            }

            return acc;
        }

        public static Sketch Boldt(String filename)
        {
            return Boldt(filename, 17);
        }

        public static Sketch Boldt(string fileName, int n)
        {
            var image = ImageFactory.Generate(fileName)
                .ZeroCrossings(CrossingMethod.One);

            return PoMerge(LinkReplaceCycle(image, n));
        }

        public static readonly IReadOnlyList<RSchedule> ReplaceSchedule = new RSchedule[]
        {
            new RSchedule (  1.2, 0.005, true, 0.5, 0.7, 4, true),
            new RSchedule (  1.7, 0.005, true, 0.5, 0.7, 4, true),
            new RSchedule (  2.5, 0.005, true, 0.5, 0.7, 4, true),
            new RSchedule (  4.0, 0.005, true, 0.5, 0.7, 4, true),
            new RSchedule (  6.0, 0.005, true, 0.5, 0.7, 4, true),
            new RSchedule ( 10.0, 0.005, true, 0.6, 0.8, 4, true),
            new RSchedule ( 16.0, 0.005, true, 0.6, 0.8, 4, true),
            new RSchedule ( 24.0, 0.005, true, 0.6, 0.8, 4, true),
            new RSchedule ( 36.0, 0.005, true, 0.6, 0.8, 4, true),
            new RSchedule ( 54.0, 0.005, true, 0.6, 0.8, 4, true),
            new RSchedule ( 81.0, 0.005, true, 0.6, 0.8, 4, true),
            new RSchedule ( 120.0, 0.005, true, 0.7, 0.9, 4, true),
            new RSchedule ( 180.0, 0.005, true, 0.7, 0.9, 4, true),
            new RSchedule ( 270.0, 0.005, true, 0.7, 0.9, 4, true),
            new RSchedule ( 270.0, 0.005, true, 0.7, 0.9, 4, true),
            new RSchedule ( 270.0, 0.005, true, 0.7, 0.9, 4, true),
            new RSchedule ( 270.0, 0.005, true, 0.7, 0.9, 4, true),
            new RSchedule ( 270.0, 0.005, true, 0.7, 0.9, 4, true)
        };

        public static readonly IReadOnlyList<LSchedule> LinkSchedule = new LSchedule[]
        {
            new LSchedule (false,  1.0, 0.5, 2.0, 0.785398, 0.6,  2.0),
            new LSchedule (false,  1.2, 0.5, 2.0, 0.785398, 0.5,  2.0),
            new LSchedule (false,  1.4, 0.5, 2.0, 0.785398, 0.4,  2.0),
            new LSchedule (false,  1.6, 0.5, 2.0, 0.698132, 0.35, 2.0),
            new LSchedule (false,  1.8, 0.5, 2.0, 0.610865, 0.30, 2.0),
            new LSchedule (false,  2.0, 0.5, 2.0, 0.523599, 0.25, 2.0),
            new LSchedule (false,  3.0, 0.5, 2.0, 0.436332, 0.20, 2.0),
            new LSchedule (false,  4.0, 0.5, 2.0, 0.349066, 0.15, 2.0),
            new LSchedule (false,  5.0, 0.5, 2.0, 0.261799, 0.1,  2.0),
            new LSchedule (false,  6.0, 0.5, 2.0, 0.174533, 0.05, 2.0),
            new LSchedule (false,  7.0, 0.5, 2.0, 0.174533, 0.05, 2.0),
            new LSchedule (false,  8.0, 0.5, 2.0, 0.174533, 0.05, 2.0),
            new LSchedule (false,  9.0, 0.5, 2.0, 0.174533, 0.05, 2.0),
            new LSchedule (false, 10.0, 0.5, 2.0, 0.174533, 0.05, 2.0),
            new LSchedule (false, 11.0, 0.5, 2.0, 0.174533, 0.05, 2.0),
            new LSchedule (false, 12.0, 0.5, 2.0, 0.174533, 0.05, 2.0),
            new LSchedule (false, 13.0, 0.5, 2.0, 0.174533, 0.05, 2.0),
            new LSchedule (false, 14.0, 0.5, 2.0, 0.174533, 0.05, 2.0)
        };
    }
}