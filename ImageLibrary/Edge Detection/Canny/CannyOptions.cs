namespace ImageLibrary.EdgeDetection
{
    public class CannyOptions
    {
        /// <summary>
        /// Percentage of pixels that meet the high threshold - for example 0.15 will ensure that at least 15% of edge pixels are considered to meet the high threshold
        /// </summary>
        public double HighThresholdPercentage { get; set; } = 0.8;

        /// <summary>
        /// Percentage of the high threshold value that the low threshold shall be set at
        /// </summary>
        public double LowThresholdPercentage { get; set; } = 0.1;

        /// <summary>
        /// Image is normalized by default to be within [0 - 255]
        /// </summary>
        public bool NormalizeImage { get; set; } = true;

        /// <summary>
        /// A gaussian filter is applied by default to reduce noise
        /// </summary>
        public bool NoiseReduce { get; set; } = true;
    }
}
