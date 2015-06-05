namespace ImageLibrary.EdgeDetection
{
    public class RSchedule
    {
        internal RSchedule(
            double replaceRadius,
            double straightness,
            bool coverageFilterOn,
            double minCumulativeCoverage,
            double minCoverage,
            int deltaAbstLevMax,
            bool replaceStraightestPath)
        {
            this.ReplaceRadius = replaceRadius;
            this.Straightness = straightness;
            this.CoverageFilterOn = coverageFilterOn;
            this.MinCumulativeCoverage = minCumulativeCoverage;
            this.MinCoverage = minCoverage;
            this.DeltaAbstLevMax = deltaAbstLevMax;
            this.ReplaceStraightestPath = replaceStraightestPath;
        }

        public double ReplaceRadius { get; set; }
        public double Straightness { get; set; }
        public bool CoverageFilterOn { get; set; }

        /// <summary>
        /// Minimum Cumulative Coverage
        /// </summary>
        public double MinCumulativeCoverage { get; set; }

        /// <summary>
        /// Minimum Coverage
        /// </summary>
        public double MinCoverage { get; set; }

        /// <summary>
        /// Delta Abstraction Level Maximum
        /// </summary>
        public int DeltaAbstLevMax { get; set; }

        /// <summary>
        /// Replace Straightest Path
        /// </summary>
        public bool ReplaceStraightestPath { get; set; }
    }
}
