using SpotifyAPI.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpotifyAPI.Pages
{
    /// <summary>
    /// Interakční logika pro PlaylistPage.xaml
    /// </summary>
    public partial class PlaylistPage : Page
    {
        public PlaylistPage()
        {
            InitializeComponent();
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            var list = await SpotifyClient.GetApi().GetUserPlaylistsAsync(SpotifyClient.GetSpotifyClient().PrivateProfile.Id);

            LView.ItemsSource = list.Items;
        }

        private async void LView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LView.SelectedItem != null)
            {
                SimplePlaylist playlist = LView.SelectedItem as SimplePlaylist;

                FullPlaylist full = await SpotifyClient.GetApi().GetPlaylistAsync(playlist.Id);

                LView2.ItemsSource = full.Tracks.Items;
            }
        }

        private async void LView2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LView2.SelectedItem != null)
            {
                var track = await SpotifyClient.GetApi().GetPlayingTrackAsync();

                if (track.Item != null)
                    Artist.Text = track.Item.Name;
            }
        }

        private async void Play_Click(object sender, RoutedEventArgs e)
        {
            if (LView.SelectedItem != null)
            {
                var devices = await SpotifyClient.GetApi().GetDevicesAsync();
                Device computer = null;

                if (devices.Devices != null)
                {
                    foreach (var device in devices.Devices)
                    {
                        if (computer == null)
                        {
                            if (device.Type == "Computer")
                            {
                                computer = device;
                            }
                        }
                    }
                }

                if (computer != null)
                {
                    SimplePlaylist playlist = LView.SelectedItem as SimplePlaylist;

                    FullPlaylist full = await SpotifyClient.GetApi().GetPlaylistAsync(playlist.Id);

                    List<string> list = new List<string>();

                    foreach (var track in full.Tracks.Items)
                    {
                        list.Add(track.Track.Uri);
                    }

                    var result = await SpotifyClient.GetApi().ResumePlaybackAsync(computer.Id, "", list, "", 0);
                }
            }
        }
    }
}
