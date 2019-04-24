using Hardcodet.Wpf.TaskbarNotification;
using SpotifyAPI.Models;
using SpotifyAPI.Pages;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SpotifyAPI
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ToastWindow toastWindow;
        DispatcherTimer toastTimer;
        int ctr = 0;
        TaskbarIcon taskIcon;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;

            App.HotKeyHost = new HotKeyHost((HwndSource)HwndSource.FromVisual(App.Current.MainWindow));
        }

        public MainWindow()
        {
            InitializeComponent();
            App.MainWindow = this;

            toastWindow = new ToastWindow();
            toastWindow.Hide();
            CreateToastTimer();
            CreateTaskIcon();

            this.Topmost = true;

            NavigationSingleton.CreateNavigationService(MainFrame);
            NavigationSingleton.GetNavigationService().NavigateToPage(new LoginPage());
        }

        public void CreateToastTimer()
        {
            toastTimer = new DispatcherTimer();
            toastTimer.Interval = new TimeSpan(0, 0, 1);
            toastTimer.Tick += CheckToastTimer;
        }

        public void CheckToastTimer(object sender, EventArgs e)
        {
            if (ctr >= 2)
            {
                HideToastWindow();

                toastTimer.Stop();
                ctr = 0;
            }
            else
            {
                ctr++;
            }
        }

        public void CreateTaskIcon()
        {
            taskIcon = new TaskbarIcon();
            taskIcon.TrayLeftMouseDown += OpenWindow;

            BitmapImage source = new BitmapImage();
            source.BeginInit();
            source.UriSource = new Uri("pack://application:,,,/Image/waldo.ico");
            source.EndInit();

            taskIcon.IconSource = source;
        }

        public void OpenWindow(object sender, EventArgs e)
        {
            this.Show();
            this.Topmost = true;
            this.Topmost = false;
            this.Focus();
        }

        public void HideWindow()
        {
            this.Hide();
        }

        public async void ShowToastWindow(FullTrack track = null)
        {
            if (Visibility == Visibility.Hidden)
            {
                var result = await toastWindow.SetCurrentTrack(track);

                if (result)
                {
                    toastWindow.Show();

                    var anim = new DoubleAnimation(1, (Duration)TimeSpan.FromSeconds(1));
                    anim.Completed += (s, _) => toastTimer.Start();
                    toastWindow.BeginAnimation(UIElement.OpacityProperty, anim);
                }
            }
        }

        public void HideToastWindow()
        {
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(1));
            anim.Completed += (s, _) => toastWindow.Hide();
            toastWindow.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
