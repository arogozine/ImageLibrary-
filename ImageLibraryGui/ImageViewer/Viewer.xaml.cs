using System.Windows;

namespace ImageLibraryGui.ImageViewer
{
    /// <summary>
    /// Interaction logic for Viewer.xaml
    /// </summary>
    public partial class Viewer : Window
    {
        public Viewer(ImageData imgData)
        {
            InitializeComponent();
            this.DataContext = imgData;
        }
    }
}
