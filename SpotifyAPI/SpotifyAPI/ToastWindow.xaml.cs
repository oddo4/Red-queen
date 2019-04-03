using SpotifyAPI.Web.Models;
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
using System.Windows.Shapes;

namespace SpotifyAPI
{
    /// <summary>
    /// Interakční logika pro ToastWindow.xaml
    /// </summary>
    public partial class ToastWindow : Window
    {
        FullTrack currentTrack = null;
        public ToastWindow()
        {
            InitializeComponent();

            this.Topmost = true;
        }

        public Task<bool> SetCurrentTrack(FullTrack track)
        {
            if (track != null)
            {
                BitmapImage old_source = null;
                if (AlbumCover.Source != null)
                {
                    old_source = AlbumCover.Source as BitmapImage;
                }

                BitmapImage cover = new BitmapImage();
                cover.BeginInit();
                cover.UriSource = new Uri(track.Album.Images[0].Url);
                cover.EndInit();

                if (old_source == null || cover.UriSource != old_source.UriSource)
                    AlbumCover.Source = cover;

                SongName.Text = track.Name;

                StringBuilder artists = new StringBuilder();

                foreach (var artist in track.Artists)
                {
                    artists.AppendFormat("{0}, ", artist.Name);
                }

                ArtistName.Text = artists.ToString().Remove(artists.Length - 2);

                currentTrack = track;

                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }

        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();

            App.MainWindow.WindowState = WindowState.Normal;
        }
    }
}
