using ImageLibrary;
using ImageLibrary.Extensions;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace ImageLibraryGui.ImageViewer
{
    public class HistogramData : INotifyPropertyChanged
    {
        public HistogramData(IImage<double> image)
        {
            int[] histogram = image.Histogram();

            PointCollection points = new PointCollection();

            int max = histogram.Max();
            points.Add(new Point(0, max));
            for (int i = 0; i < histogram.Length; i++)
            {
                points.Add(new Point(i, max - histogram[i]));
            }

            points.Add(new Point(histogram.Length - 1, max));

            this.histogramPoints = points;
        }

        private PointCollection histogramPoints;

        public PointCollection HistogramPoints
        {
            get
            {
                return this.histogramPoints;
            }
            set
            {
                
                if (this.histogramPoints != value)
                {
                    this.histogramPoints = value;

                    OnPropertyChanged(nameof(HistogramPoints));
                }
            }
        }


        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
