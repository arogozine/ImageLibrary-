using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageLibraryGui.ImageViewer
{
    public class ImageData : INotifyPropertyChanged
    {
        public ImageData(Bitmap bitmap)
        {
            SetBitmap(bitmap);
        }

        public ImageData(byte[] pngBinary)
        {
            SetPng(pngBinary);
        }

        private void SetPng(byte[] pngBinary)
        {
            using (var ms = new MemoryStream(pngBinary))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                this.ImageSource = bitmapImage;
            }
        }

        private void SetBitmap(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                this.ImageSource = bitmapImage;
            }
        }


        private BitmapImage imageSrc;

        public BitmapImage ImageSource
        {
            get { return imageSrc; }
            set
            {
                if (imageSrc != value)
                {
                    imageSrc = value;
                    OnPropertyChanged("ImageSource");
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
