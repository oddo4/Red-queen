using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyAPI.Models
{
    public class SpotifyClient
    {
        private string clientId = "4dab4bc197084c7db90f8201c5990abe";
        private string secretId = "e5f5c68a50ff48068eb1bfea84cc57a4";
        private bool authenticated = false;

        private static SpotifyClient spotifyClient = null;
        public static SpotifyClient GetSpotifyClient()
        {
            if (spotifyClient == null)
                spotifyClient = new SpotifyClient();
            return spotifyClient;
        }

        private static SpotifyWebAPI api = null;
        public static SpotifyWebAPI GetApi()
        {
            if (api == null)
            {
                GetSpotifyClient().Auth();
            }

            return api;
        }

        public PrivateProfile PrivateProfile = null;
        public bool Premium = false;
        public Device CurrentDevice = null;

        public Task<bool> Auth()
        {
            if (api == null)
            {
                AuthorizationCodeAuth auth =
        new AuthorizationCodeAuth(clientId, secretId, "http://localhost:4002", "http://localhost:4002",
            Scope.AppRemoteControl |
            Scope.PlaylistModifyPrivate |
            Scope.PlaylistModifyPublic |
            Scope.PlaylistReadCollaborative |
            Scope.PlaylistReadPrivate |
            Scope.Streaming |
            Scope.UgcImageUpload |
            Scope.UserFollowModify |
            Scope.UserFollowRead |
            Scope.UserLibraryModify |
            Scope.UserLibraryRead |
            Scope.UserModifyPlaybackState |
            Scope.UserReadBirthdate |
            Scope.UserReadCurrentlyPlaying |
            Scope.UserReadEmail |
            Scope.UserReadPlaybackState |
            Scope.UserReadPrivate |
            Scope.UserReadRecentlyPlayed |
            Scope.UserTopRead);

                auth.AuthReceived += async (sender, payload) =>
                {
                    auth.Stop();
                    Token token = await auth.ExchangeCode(payload.Code);
                    api = new SpotifyWebAPI() { TokenType = token.TokenType, AccessToken = token.AccessToken };

                    PrivateProfile = await api.GetPrivateProfileAsync();

                    authenticated = true;
                };

                auth.Start(); // Starts an internal HTTP Server
                auth.OpenBrowser();

                while (!authenticated) ;

                return Task.FromResult(authenticated);
            }

            return Task.FromResult(authenticated);
        }
    }
}
