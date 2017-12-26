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
        private string ScreenPath;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
        public static AutoResetEvent autoReset = new AutoResetEvent(false);
        public MainWindow()
        {
            InitializeComponent();
        }
        public void SaveMyFile(Bitmap inBmpImg)
        {


            ScreenPath = "";

            saveFileDialog1.DefaultExt = "bmp";
            saveFileDialog1.Filter = "bmp files (*.bmp)|*.bmp|jpg files (*.jpg)|*.jpg|tiff files (*.tiff)|*.tiff|png files (*.png)|*.png";
            saveFileDialog1.Title = "Save screenshot to...";
            saveFileDialog1.ShowDialog();
            ScreenPath = saveFileDialog1.FileName;

  


            if (ScreenPath != "" )
            {

                inBmpImg.Save(ScreenPath);

            }

            else
            {

                System.Windows.MessageBox.Show("File save cancelled", "TeboScreen", MessageBoxButton.OK);
            }

        }
        public void ScreenShot(System.Windows.Point inXYPoint, double inWidth, double inHeight,bool inIsROISelected )
        {

            if (inIsROISelected)
            {
                System.Drawing.Point SourcePoint = new System.Drawing.Point(Convert.ToInt32(inXYPoint.X),Convert.ToInt32(inXYPoint.Y));
                System.Drawing.Point DestinationPoint = new System.Drawing.Point(0,0);

                System.Drawing.Size imgSize = new System.Drawing.Size();
                imgSize.Width = (int)inWidth;
                imgSize.Height = (int)inHeight;
                
                Bitmap bmpImage = new System.Drawing.Bitmap((int)inWidth, (int)inHeight,
                                                            System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                using (System.Drawing.Graphics graph = System.Drawing.Graphics.FromImage(bmpImage))
                {
                    // take an empty shot
                    Console.WriteLine("Bitmap Image Size: " + bmpImage.Size);
                    graph.CopyFromScreen(SourcePoint,DestinationPoint,bmpImage.Size);
                }

                IntPtr handle = IntPtr.Zero;

                try
                {

                    handle = bmpImage.GetHbitmap();

                    //Convert to WPF image.
                    ImgBox.Source = Imaging.CreateBitmapSourceFromHBitmap(handle,
                                            IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                    SaveMyFile(bmpImage);
                }
                finally
                {
                    DeleteObject(handle);
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
                System.Windows.Application.Current.Shutdown();
            }
            else if (e.Key == Key.S)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }
    }
}
