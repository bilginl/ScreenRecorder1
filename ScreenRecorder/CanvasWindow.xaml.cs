using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScreenRecorder
{

    public partial class CanvasWindow : Window , INotifyPropertyChanged
    {
        private CanvasWindow m_InstanceRef = null;
        public CanvasWindow InstanceRef
        {
            get
            {
                return m_InstanceRef;
            }
            set
            {
                m_InstanceRef = value;
            }

        }
        public CanvasWindow()
        {
            InitializeComponent();
            this.DataContext = this;

        }
        private Point _point;


        private readonly double _screenWidth = SystemParameters.PrimaryScreenWidth;
        private readonly double _screenHeight = SystemParameters.PrimaryScreenHeight;

        // True if a drag is in progress.
        private bool _dragInProgress = false;

        // The drag's last point.
        private Point _lastPoint;

        // The part of the rectangle under the mouse.
        private HitType _mouseHitType = HitType.None;

        public event PropertyChangedEventHandler PropertyChanged;

        public Point RectPoint
        {
            get { return this._point; }
            private set
            {
                this._point = value;
                this.OnPropertyChanged("RectPoint");
            }
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public double AreaWidth { get; private set; }
        public double AreaHeight { get; private set; }

        public Point GetLastPoint()
        {
            return _lastPoint;
        }

        // The part of the rectangle the mouse is over.
        private enum HitType
        {
            None, Body, UL, UR, LR, LL, L, R, T, B
        };

        // Return a HitType value to indicate what is at the point.
        private HitType SetHitType(Point point)
        {
            double left = Canvas.GetLeft(RecodBorderSizeObject);
            double top = Canvas.GetTop(RecodBorderSizeObject);
            double right = left + RecodBorderSizeObject.Width;
            double bottom = top + RecodBorderSizeObject.Height;
            if (point.X < left) return HitType.None;
            if (point.X > right) return HitType.None;
            if (point.Y < top) return HitType.None;
            if (point.Y > bottom) return HitType.None;

            const double GAP = 10;
            if (point.X - left < GAP)
            {
                // Left edge.
                if (point.Y - top < GAP) return HitType.UL;
                if (bottom - point.Y < GAP) return HitType.LL;
                return HitType.L;
            }
            if (right - point.X < GAP)
            {
                // Right edge.
                if (point.Y - top < GAP) return HitType.UR;
                if (bottom - point.Y < GAP) return HitType.LR;
                return HitType.R;
            }
            if (point.Y - top < GAP) return HitType.T;
            if (bottom - point.Y < GAP) return HitType.B;
            return HitType.Body;
        }

        // Set a mouse cursor appropriate for the current hit type.
        private void SetMouseCursor()
        {
            // See what cursor we should display.
            Cursor desired_cursor = Cursors.Arrow;
            switch (_mouseHitType)
            {
                case HitType.None:
                    desired_cursor = Cursors.Arrow;
                    break;
                case HitType.Body:
                    desired_cursor = Cursors.ScrollAll;
                    break;
                case HitType.UL:
                case HitType.LR:
                    desired_cursor = Cursors.SizeNWSE;
                    break;
                case HitType.LL:
                case HitType.UR:
                    desired_cursor = Cursors.SizeNESW;
                    break;
                case HitType.T:
                case HitType.B:
                    desired_cursor = Cursors.SizeNS;
                    break;
                case HitType.L:
                case HitType.R:
                    desired_cursor = Cursors.SizeWE;
                    break;
            }

            // Display the desired cursor.
            if (Cursor != desired_cursor) Cursor = desired_cursor;
        }

        // Start dragging.
        private void canvas1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _mouseHitType = SetHitType(Mouse.GetPosition(CanvasMain));
            SetMouseCursor();
            if (_mouseHitType == HitType.None) return;

            _lastPoint = Mouse.GetPosition(CanvasMain);
            _dragInProgress = true;
        }

        // If a drag is in progress, continue the drag.
        // Otherwise display the correct cursor.
        private void canvas1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragInProgress)
            {
                _mouseHitType = SetHitType(Mouse.GetPosition(CanvasMain));
                SetMouseCursor();
            }
            else
            {
                // See how much the mouse has moved.
                Point point = Mouse.GetPosition(CanvasMain);
                double offsetX = point.X - _lastPoint.X;
                double offsetY = point.Y - _lastPoint.Y;

                // Get the rectangle's current position.
                double newX = Canvas.GetLeft(RecodBorderSizeObject);
                double newY = Canvas.GetTop(RecodBorderSizeObject);
                double newWidth = RecodBorderSizeObject.Width;
                double newHeight = RecodBorderSizeObject.Height;

                if (newX <= 0)
                {
                    newX = 0;
                }

                if (newY <= 0)
                {
                    newY = 0;
                }

                if (newWidth > _screenWidth)
                {
                    newWidth = _screenWidth;
                }

                if (newHeight > _screenHeight)
                {
                    newHeight = _screenHeight;
                }

                if (newX + newWidth >= _screenWidth)
                {
                    newX -= newX + newWidth - _screenWidth;
                }

                if (newY + newHeight >= _screenHeight)
                {
                    newY -= newY + newHeight - _screenHeight;
                }

                // Update the rectangle.
                switch (_mouseHitType)
                {
                    case HitType.Body:
                        newX += offsetX;
                        newY += offsetY;
                        break;
                    case HitType.UL:
                        newX += offsetX;
                        newY += offsetY;
                        newWidth -= offsetX;
                        newHeight -= offsetY;
                        break;
                    case HitType.UR:
                        newY += offsetY;
                        newWidth += offsetX;
                        newHeight -= offsetY;
                        break;
                    case HitType.LR:
                        newWidth += offsetX;
                        newHeight += offsetY;
                        break;
                    case HitType.LL:
                        newX += offsetX;
                        newWidth -= offsetX;
                        newHeight += offsetY;
                        break;
                    case HitType.L:
                        newX += offsetX;
                        newWidth -= offsetX;
                        break;
                    case HitType.R:
                        newWidth += offsetX;
                        break;
                    case HitType.B:
                        newHeight += offsetY;
                        break;
                    case HitType.T:
                        newY += offsetY;
                        newHeight -= offsetY;
                        break;
                }

                // Don't use negative width or height.
                if ((newWidth > 250) && (newHeight > 250))
                {
                    // Update the rectangle.
                    Canvas.SetLeft(RecodBorderSizeObject, newX);
                    Canvas.SetTop(RecodBorderSizeObject, newY);
                    RecodBorderSizeObject.Width = newWidth;
                    RecodBorderSizeObject.Height = newHeight;

                    this.RectPoint = new Point(newX, newY);
                    this.AreaWidth = newWidth;
                    this.AreaHeight = newHeight;
                    // Save the mouse's new location.

                    _lastPoint = point;
                }
            }
        }
        public Point GetPoint()
        {
            return _lastPoint;
        }
        public double GetWidth()
        {
            return this.AreaWidth;
        }
        public double GetHeight()
        {
            return this.AreaHeight;
        }
        // Stop dragging.
        private void canvas1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _dragInProgress = false;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            // Application.Current.Shutdown();
            this.Close();
            
        }

        // ShortCuts from Keyboard
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.F5)
            {

                MainWindow mainScreen = new MainWindow();
                mainScreen.ScreenShot(this._point, this.RecodBorderSizeObject.Width, this.RecodBorderSizeObject.Height, true);
                mainScreen.Show();               
                this.Close();
               
            }
            else if(e.Key == Key.F4)
            {
                MainWindow mainScreen = new MainWindow();
                mainScreen.Show();
                this.Close();
            }

        }
    }
}
