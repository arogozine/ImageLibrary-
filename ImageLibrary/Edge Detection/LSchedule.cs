using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageLibrary.EdgeDetection
{
    public class LSchedule
    {
        public LSchedule(bool getLinesMode,
            double linkRadius,
            double contrastRatioMin,
            double contrastRatioMax,
            double deltaThetaMax,
            double endProjectRelMax,
            double lateralDistAbsMaxSq)
        {
            this.GetLinesMode = getLinesMode;
            this.LinkRadius = linkRadius;
            this.ContrastRatioMin = contrastRatioMin;
            this.ContrastRatioMax = contrastRatioMax;
            this.DeltaThetaMax = deltaThetaMax;
            this.EndProjectRelMax = endProjectRelMax;
            this.LateralDistAbsMaxSq = lateralDistAbsMaxSq;
        }

        public bool GetLinesMode { get; set; }
        public double LinkRadius { get; set; }
        public double ContrastRatioMin { get; set; }
        public double ContrastRatioMax { get; set; }

        /// <summary>
        /// Delta Theta Maximum (Degrees)
        /// </summary>
        public double DeltaThetaMax { get; set; }

        /// <summary>
        /// End Project Relative Maximum
        /// </summary>
        public double EndProjectRelMax { get; set; }

        /// <summary>
        /// Lateral Distance Absolute Maximum Squared
        /// </summary>
        public double LateralDistAbsMaxSq { get; set; }
    }
}