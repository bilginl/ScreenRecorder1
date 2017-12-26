using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;

namespace ScreenRecorder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        public static AutoResetEvent autoReset = new AutoResetEvent(false);
        public MainWindow()
        {
            InitializeComponent();
        }

        public void ScreenShot(System.Windows.Point inXYPoint, double inWidth, double inHeight,bool inIsROISelected )
        {

            if (inIsROISelected)
            {

                System.Drawing.Size imgSize = new System.Drawing.Size();
                imgSize.Width = (int)inWidth;
                imgSize.Height = (int)inHeight;
                /*
                                Bitmap bmpImage = new System.Drawing.Bitmap((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight,
                                                                            System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                                using (System.Drawing.Graphics graph = System.Drawing.Graphics.FromImage(bmpImage))
                                {
                                    // take an empty shot
                                    Console.WriteLine("Bitmap Image Size: " + bmpImage.Size);
                                    graph.CopyFromScreen((int)inXYPoint.X, (int)inXYPoint.Y, (int)(inXYPoint.X + inWidth),
                                             (int)(inXYPoint.Y + inHeight),imgSize);
                                }

                                IntPtr handle = IntPtr.Zero;

                                try
                                {

                                    handle = bmpImage.GetHbitmap();

                                    //Convert to WPF image.
                                    ImgBox.Source = Imaging.CreateBitmapSourceFromHBitmap(handle,
                                                            IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                                    bmpImage.Save("myImage.bmp");
                                }
                                finally
                                {
                                    DeleteObject(handle);
                                }
                */
                using (Bitmap bmpImage = new System.Drawing.Bitmap((int)inWidth, (int)inHeight))
                { 
                    using (System.Drawing.Graphics graph = System.Drawing.Graphics.FromImage(bmpImage))
                    {
                    // take an empty shot
                        Console.WriteLine("Bitmap Image Size: " + bmpImage.Size);
                        graph.CopyFromScreen((int)inXYPoint.X, (int)inXYPoint.Y, (int)(inXYPoint.X + inWidth),
                             (int)(inXYPoint.Y + inHeight), bmpImage.Size);
                    }
                    bmpImage.Save("myImage.bmp",System.Drawing.Imaging.ImageFormat.Bmp);
                }

            }
        }



        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {      
            CanvasWindow canvasInstance = new CanvasWindow();
            canvasInstance.Show();
            this.Close();
        }

        private void widowRecorder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
