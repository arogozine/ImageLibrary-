using System;
using System.Globalization;
using System.IO;

namespace ImageLibrary
{
    /// <summary>
    /// Adapted from C code provided by UNM Digitial Image Processing Class
    /// </summary>
    public static class PostScript
    {
        public static void SketchToPostScript(Sketch sketch, String fileName)
        {
            var ls = sketch.List;
            var rows = sketch.Width;
            var cols = sketch.Height;
            var ratio = Math.Max((double)rows / cols, (double)cols / rows);

            using (StreamWriter writer = new StreamWriter(fileName))
            {

                GenerateHeader(writer, (float)rows / cols, Path.GetFileName(fileName));
                writer.Write("0.00115741 setlinewidth\n");

                float scale = (float)Math.Min(rows * ratio, cols * ratio);

                foreach (Line ln in ls)
                {
                    float x0 = (float)ln.XStart / scale;
                    float y0 = (float)ln.YStart / scale;
                    float x1 = (float)ln.XEnd / scale;
                    float y1 = (float)ln.YEnd / scale;

                    string dispLine = string.Format(CultureInfo.CurrentCulture, "{0} {1} {2} {3} displine\n", y0, x0, y1, x1);
                    writer.Write(dispLine);
                }

                writer.Write("showpage\n");
            }
        }

        private static void GenerateHeader(StreamWriter sb, float ratio, string title)
        {
            sb.Write("%%!PS-Adobe-2.0 EPSF-1.2\n");
            sb.Write("%%%%Title: ");
            sb.Write(title);
            sb.Write('\n');
            sb.Write("%%%%Creator: Image Library 1.0\n");

            if (ratio < 1.0)
                sb.Write("%%%%BoundingBox: 0 %g 432 432\n", 432 * (1.0 - ratio));
            else
                sb.Write("%%%%BoundingBox: 0 0 %g 432\n", 432 / ratio);

            sb.Write("%%%%Pages: 1\n");
            sb.Write("%%%%EndComments\n");
            sb.Write("save\n");
            sb.Write("/inch {72 mul} def\n");
            sb.Write("/displine { /y2 exch def /x2 exch def /y1 exch def /x1 exch def\n");
            sb.Write("x1 y1 moveto x2 y2 lineto stroke } def\n");
            sb.Write("%%%%EndProlog\n");
            sb.Write("%%%%%%Page: 1 1\n");
            sb.Write("0 0 translate\n");
            sb.Write("432 432 scale\n");
            sb.Write("0.00115741 setlinewidth\n");
            sb.Write("0.5 dup translate -90 rotate -0.5 dup translate\n");

            if (ratio < 1.0)
                sb.Write("0 0 moveto 0 1 lineto {0} 1 lineto {1} 0 lineto closepath\n", ratio, ratio);
            else
                sb.Write("0 0 moveto 0 {0} lineto 1 {1} lineto 1 0 lineto closepath\n", 1.0 / ratio, 1.0 / ratio);

            sb.Write("stroke\n");
        }
    }
}
