using SpotifyAPI.Models;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
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
    /// Interakční logika pro LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();

        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            await SpotifyClient.GetSpotifyClient().Auth();
            NavigationSingleton.GetNavigationService().SetCurrentPage(this);
            NavigationSingleton.GetNavigationService().NavigateToPage(new PlayerPage());
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void MinimalizeBtn_Click(object sender, RoutedEventArgs e)
        {
            App.MainWindow.HideWindow();
        }
    }
}
