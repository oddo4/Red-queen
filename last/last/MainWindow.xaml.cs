using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace last
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;
        double xspeed = 3;
        double yspeed = 3;
        List<Brush> colorList = new List<Brush>() { Brushes.Blue, Brushes.LawnGreen, Brushes.Orange, Brushes.Purple };

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            timer.Tick += Move;
            timer.Start();
        }

        private void Move(object sender, EventArgs e)
        {
            var width = this.Width - 15;
            var height = this.Height - 35;

            var rectwidth = rect.Width;
            var rectheight = rect.Height;

            var x = Canvas.GetLeft(rect) + xspeed;
            var y = Canvas.GetTop(rect) + yspeed;

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);

            if (x + rectwidth >= width)
            {
                xspeed = -xspeed;
                x = width - rectwidth;
                ChangeColor();
            }
            else if (x <= 0)
            {
                xspeed = -xspeed;
                x = 0;
                ChangeColor();
            }

            if (y + rectheight >= height)
            {
                yspeed = -yspeed;
                y = height - rectheight;
                ChangeColor();
            }
            else if (y <= 0)
            {
                yspeed = -yspeed;
                y = 0;
                ChangeColor();
            }
        }

        public void ChangeColor()
        {
            Random rand = new Random();

            var color = rect.Background;

            var newList = new List<Brush>(colorList);
            newList.Remove(color);

            var newColor = newList[rand.Next(0, 3)];

            rect.Background = newColor;
        }

        private void Rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            xspeed += (xspeed * 0.2);
            yspeed += (yspeed * 0.2);

            var clicks = int.Parse(score.Content.ToString());

            score.Content = clicks + 1;
        }
    }
}
