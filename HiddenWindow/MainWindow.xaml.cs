using Subliner.Common;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace HiddenWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }

        #region DLLImports

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

        #endregion

        private Brush _glassBrush;
        private Brush _titleBarBrush;
        private Brush _transparentBrush;
        private HwndSource _hwndSource;

        public MainWindow()
        {
            InitializeComponent();
            _glassBrush = new SolidColorBrush(Color.FromArgb(60, 240, 240, 240));
            _titleBarBrush = new SolidColorBrush(Color.FromArgb(160, 230, 236, 240));
            _transparentBrush = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128));

            void OnViewLoaded(object s, RoutedEventArgs e)
            {
                Loaded -= OnViewLoaded;
                _hwndSource = (HwndSource)PresentationSource.FromVisual(this);
                this.EnableBlur();
            }

            Loaded += OnViewLoaded;

            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;
        }

        /// <summary>
        /// Sends the Window Message for resize
        /// </summary>
        /// <param name="direction">Resize Direction</param>
        private void ResizeWindow(ResizeDirection direction)
        {
            if (_hwndSource == null)
                return;

            SendMessage(_hwndSource.Handle, 0x112, (IntPtr)(61440 + direction), IntPtr.Zero);
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            Background = _glassBrush;
            TitleBar.Background = _titleBarBrush;
            CloseButton.Foreground = Brushes.Black;
            //CommandGrid.Opacity = 1;
            TitleTB.Foreground = Brushes.Black;
            this.EnableBlur();
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            Background = _transparentBrush;
            TitleBar.Background = Brushes.Transparent;
            CloseButton.Foreground = Brushes.Transparent;
            //CommandGrid.Opacity = 0;
            TitleTB.Foreground = Brushes.Transparent;
            this.DisableBlur();
        }

        private void OnDragTitleBar(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void OnCloseApplication(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnResizeWindow(object sender, MouseButtonEventArgs e)
        {
            ResizeWindow(ResizeDirection.BottomRight);
        }
    }
}
