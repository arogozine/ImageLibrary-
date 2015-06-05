using ImageLibrary;
using ImageLibraryGui.ImageViewer;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace ImageLibraryGui
{
    public static class ExtensionClass
    {
        private static Application App { get; set; }

        private static Thread AppThread { get; set; }

        private static void EnsureAppRunning()
        {
            // Application is currently running
            if (App != null && Application.Current != null)
                return;

            // Application already exists
            if (Application.Current != null)
            {
                App = Application.Current;
            }

            var appStartedEvent = new ManualResetEvent(false);

            // Create GUI STA Thread
            AppThread = new Thread(new ThreadStart(() =>
            {
                App = new Application() { ShutdownMode = ShutdownMode.OnExplicitShutdown };

                // Notify that the thread started
                App.Startup += (a, b) => appStartedEvent.Set();

                // Run GUI
                App.Run();
            }));

            AppThread.SetApartmentState(ApartmentState.STA);
            AppThread.Start();

            // Wait for GUI to initialize
            // before Invoking Dispatcher
            appStartedEvent.WaitOne();
        }

        public static void Shutdown()
        {
            if (AppThread != null && App != null)
            {
                var appQuitEvent = new ManualResetEvent(false);

                App.Dispatcher.Invoke(() => {
                    try
                    {
                        App.Shutdown();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                    finally
                    {
                        App = null;
                        AppThread = null;
                        appQuitEvent.Set();
                    }
                });

                appQuitEvent.WaitOne();
            }
        }

        private static void Invoke(Action action)
        {
            EnsureAppRunning();

            App.Dispatcher.Invoke(action);
        }

        /// <summary>
        /// Shows image in a Window
        /// </summary>
        /// <param name="bm"></param>
        public static void Show<T>(this IImage<T> image)
            where T : struct, IEquatable<T>
        {
            Invoke(() => {
                var imgData = new ImageData(image.ToBitmap());
                Viewer v = new Viewer(imgData);
                v.Show();
            });
        }

        public static void ShowHistogram(this IImage<double> image)
        {
            Invoke(() =>
            {
                var imgData = new HistogramData(image);
                Histogram v = new Histogram(imgData);
                v.Show();
            });
        }
    }
}
