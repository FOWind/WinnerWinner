using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace ScreenShot
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private double x = 0;
        private double y = 0;
        private double startX = 0;
        private double startY = 0;
        private double endX = 0;
        private double driftX = 0;
        private double driftY = 0;
        private double endY = 0;
        private bool isDraging = false;
        private void InitMask()
        {
            x = SystemParameters.PrimaryScreenWidth;
            y = SystemParameters.PrimaryScreenHeight;
            wholeMask.Rect = new Rect(0, 0, x, y);
        }
        public MainWindow()
        {
            
            InitializeComponent();
            InitMask();
            mainCanvas.Width = SystemParameters.PrimaryScreenWidth;
            mainCanvas.Height = SystemParameters.PrimaryScreenHeight;
        }
        private void Path_MouseDown(object sender, MouseButtonEventArgs e)
        {
            checkBox.Opacity = 0;
            isDraging = true;
            Point pp = Mouse.GetPosition(e.Source as FrameworkElement);
            startX = pp.X;
            startY = pp.Y;
        }

        private void Path_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDraging = false;
            Point pp = Mouse.GetPosition(e.Source as FrameworkElement);
            endX = pp.X;
            endY = pp.Y;
            checkBox.SetValue(Canvas.TopProperty, endY);
            checkBox.SetValue(Canvas.LeftProperty, endX);
            checkBox.Opacity = 1;
        }

        private void Path_MouseMove(object sender, MouseEventArgs e)
        {

            double tempStartX = startX;
            double tempStartY = startY;
            
            if (isDraging)
            {
                Point pp = Mouse.GetPosition(e.Source as FrameworkElement);
                endX = pp.X;
                endY = pp.Y;
                driftX = endX - startX;
                driftY = endY - startY;
                if(driftX < 0)
                {
                    tempStartX = endX;
                    driftX = -driftX;
                }
                if(driftY < 0)
                {
                    tempStartY = endY;
                    driftY = -driftY;
                }
                holeMask.Rect = new Rect(tempStartX, tempStartY, driftX, driftY);
            }
        }

        private void WindowStyle_KeyDown(object sender, KeyEventArgs e)
        {
            
            if(e.Key == Key.Escape)
            {
                DialogResult = false;
            }
        }

        private void Cancle_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            App.Cr.StartX = startX;
            App.Cr.StartY = startY;
            App.Cr.Width = driftX;
            App.Cr.Height = driftY;
        }
    }
}
