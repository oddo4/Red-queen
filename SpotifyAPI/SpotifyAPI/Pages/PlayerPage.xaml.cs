using SpotifyAPI.Models;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace SpotifyAPI.Pages
{
    /// <summary>
    /// Interakční logika pro PlayerPage.xaml
    /// </summary>
    public partial class PlayerPage : Page
    {
        SpotifyWebAPI api = null;
        FullTrack currentTrack = null;
        DispatcherTimer timer;
        public PlayerPage()
        {
            InitializeComponent();

            DataInit();
        }

        public void DataInit()
        {
            api = SpotifyClient.GetApi();

            UserNick.Content = SpotifyClient.GetSpotifyClient().PrivateProfile.DisplayName;

            bool premium = SpotifyClient.GetSpotifyClient().Premium;
            var type = SpotifyClient.GetSpotifyClient().PrivateProfile.Product;

            if (type == "premium")
            {
                premium = true;
            }
            else
            {
                type = "free";
            }

            PlayerPanel.IsEnabled = premium;
            SpotifyClient.GetSpotifyClient().Premium = premium;
            UserType.Content = type.First().ToString().ToUpper() + type.Substring(1);

            CheckDeviceActive();

            CheckTrackTimer();

            CreateHotKeys();

            PlaybackImgSwitch();
            SetPlayerControls();

            App.MainWindow.Topmost = false;
        }

        public async void CheckDeviceActive()
        {
            var devices = await api.GetDevicesAsync();

            if (devices.Devices != null)
            {
                foreach (var device in devices.Devices)
                {
                    if (SpotifyClient.GetSpotifyClient().CurrentDevice == null && device.Type == "Computer")
                    {
                        SpotifyClient.GetSpotifyClient().CurrentDevice = device;
                        SetCurrentTrack();
                    }
                }
            }
        }

        public void CheckTrackTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            timer.Tick += GetCurrentTrack;
            timer.Start();
        }

        public void GetCurrentTrack(object sender, EventArgs e)
        {
            SetCurrentTrack();
        }

        public void CreateHotKeys()
        {
            App.HotKeyHost.AddHotKey(new CustomHotKey("Play/Pause", Key.M, ModifierKeys.Control | ModifierKeys.Shift, true, this) { Option = 1 });
            App.HotKeyHost.AddHotKey(new CustomHotKey("Previous", Key.Left, ModifierKeys.Control | ModifierKeys.Shift, true, this) { Option = 2 });
            App.HotKeyHost.AddHotKey(new CustomHotKey("Next", Key.Right, ModifierKeys.Control | ModifierKeys.Shift, true, this) { Option = 3 });
        }

        public void PlaybackBtn_Click(object sender, RoutedEventArgs e)
        {
            bool premium = SpotifyClient.GetSpotifyClient().Premium;

            if (premium)
            {
                var device = SpotifyClient.GetSpotifyClient().CurrentDevice;

                if (device != null)
                {
                    Playback(device);

                    SetCurrentTrack();
                }
            }
        }

        public async void Playback(Device device)
        {
            var state = (await api.GetPlaybackAsync()).IsPlaying;

            PlaybackImgSwitch(!state);

            ErrorResponse result = null;

            switch (state)
            {
                case true:
                    result = await api.PausePlaybackAsync(device.Id);
                    break;
                case false:
                    result = await api.ResumePlaybackAsync(device.Id, "", null, "", 0);
                    break;
            }

            if (result.HasError())
            {
                Debug.WriteLine(result.Error.Status + " - " + result.Error.Message);
            }
        }

        public async void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            bool premium = SpotifyClient.GetSpotifyClient().Premium;

            if (premium)
            {
                var device = SpotifyClient.GetSpotifyClient().CurrentDevice;

                if (device != null)
                {
                    var result = await api.SkipPlaybackToNextAsync(device.Id);

                    if (result.HasError())
                    {
                        Debug.WriteLine(result.Error.Status + " - " + result.Error.Message);
                    }
                }
            }
        }

        public async void PrevBtn_Click(object sender, RoutedEventArgs e)
        {
            bool premium = SpotifyClient.GetSpotifyClient().Premium;

            if (premium)
            {
                var device = SpotifyClient.GetSpotifyClient().CurrentDevice;

                if (device != null)
                {
                    var result =  await api.SkipPlaybackToPreviousAsync(device.Id);

                    if (result.HasError())
                    {
                        Debug.WriteLine(result.Error.Status + " - " + result.Error.Message);
                    }
                }
            }
        }

        public async void SetCurrentTrack(FullTrack track = null)
        {
            if (track == null)
                track = (await api.GetPlaybackAsync()).Item;

            if (currentTrack == null || (track != null && currentTrack != null && currentTrack.Id != track.Id))
            {
                App.MainWindow.ShowToastWindow(track);

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
                {
                    AlbumCover.Source = cover;
                }

                SongName.Text = track.Name;

                StringBuilder artists = new StringBuilder();

                foreach (var artist in track.Artists)
                {
                    artists.AppendFormat("{0}, ", artist.Name);
                }

                ArtistName.Text = artists.ToString().Remove(artists.Length - 2);

                currentTrack = track;
            }
        }

        public async void PlaybackImgSwitch(bool? state = null)
        {
            if (state == null)
                state = (await api.GetPlaybackAsync()).IsPlaying;

            BitmapImage control = new BitmapImage();
            control.BeginInit();

            switch (state)
            {
                case true:
                    control.UriSource = new Uri("pack://application:,,,/Image/pause.png");
                    break;
                case false:
                default:
                    control.UriSource = new Uri("pack://application:,,,/Image/play.png");
                    break;
            }

            control.EndInit();

            PlaybackImg.Source = control;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void MinimalizeBtn_Click(object sender, RoutedEventArgs e)
        {
            App.MainWindow.HideWindow();
        }

        private async void RepeatBtn_Click(object sender, RoutedEventArgs e)
        {
            bool premium = SpotifyClient.GetSpotifyClient().Premium;

            if (premium)
            {
                var playback = await api.GetPlaybackAsync();
                RepeatState repeat = RepeatState.Off;

                switch (playback.RepeatState)
                {
                    case RepeatState.Off:
                        repeat = RepeatState.Context;
                        break;
                    case RepeatState.Context:
                        repeat = RepeatState.Track;
                        break;
                    case RepeatState.Track:
                        repeat = RepeatState.Off;
                        break;
                }

                BitmapImage control = new BitmapImage();
                control.BeginInit();

                control.UriSource = new Uri("pack://application:,,,/Image/repeat" + (int)repeat + ".png");

                control.EndInit();

                var result = await api.SetRepeatModeAsync(repeat);

                RepeatImg.Source = control;

                if (result.HasError())
                {
                    Debug.WriteLine(result.Error.Status + " - " + result.Error.Message);
                }
            }
        }

        private async void ShuffleBtn_Click(object sender, RoutedEventArgs e)
        {
            bool premium = SpotifyClient.GetSpotifyClient().Premium;

            if (premium)
            {
                var playback = await api.GetPlaybackAsync();

                BitmapImage control = new BitmapImage();
                control.BeginInit();

                control.UriSource = new Uri("pack://application:,,,/Image/shuffle" + Convert.ToInt32(!playback.ShuffleState) + ".png");

                control.EndInit();

                var result = await api.SetShuffleAsync(!playback.ShuffleState);

                ShuffleImg.Source = control;

                if (result.HasError())
                {
                    Debug.WriteLine(result.Error.Status + " - " + result.Error.Message);
                }
            }
        }

        public async void SetPlayerControls()
        {
            bool premium = SpotifyClient.GetSpotifyClient().Premium;

            if (premium)
            {
                var playback = await api.GetPlaybackAsync();

                BitmapImage shuffle = new BitmapImage();
                shuffle.BeginInit();
                shuffle.UriSource = new Uri("pack://application:,,,/Image/shuffle" + Convert.ToInt32(playback.ShuffleState) + ".png");
                shuffle.EndInit();

                ShuffleImg.Source = shuffle;

                BitmapImage repeat = new BitmapImage();
                repeat.BeginInit();
                repeat.UriSource = new Uri("pack://application:,,,/Image/repeat" + (int)playback.RepeatState + ".png");
                repeat.EndInit();

                RepeatImg.Source = repeat;
            }
        }
    }
}
