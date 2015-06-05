using System.Windows;

namespace ImageLibraryGui.ImageViewer
{
    /// <summary>
    /// Interaction logic for Hisogram.xaml
    /// </summary>
    public partial class Histogram : Window
    {
        public Histogram(HistogramData dataContext)
        {
            InitializeComponent();
            this.DataContext = dataContext;
        }
    }
}
