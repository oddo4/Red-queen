using SpotifyAPI.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SpotifyAPI.Models
{
    [Serializable]
    public class CustomHotKey : HotKey
    {
        public int Option = 0;
        public PlayerPage Player;

        public CustomHotKey(string name, Key key, ModifierKeys modifiers, bool enabled, PlayerPage player)
            : base(key, modifiers, enabled)
        {
            Name = name;
            Player = player;
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    name = value;
                    OnPropertyChanged(name);
                }
            }
        }

        protected async override void OnHotKeyPress()
        {
            var api = SpotifyClient.GetApi();

            var track = (await api.GetPlaybackAsync()).Item;

            switch(Option)
            {
                case 1:
                    Player.PlaybackBtn_Click(null, null);
                    break;
                case 2:
                    Player.PrevBtn_Click(null, null);
                    App.MainWindow.ShowToastWindow(track);
                    break;
                case 3:
                    Player.NextBtn_Click(null, null);
                    App.MainWindow.ShowToastWindow(track);
                    break;
            }

            base.OnHotKeyPress();
        }


        protected CustomHotKey(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            Name = info.GetString("Name");
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Name", Name);
        }
    }
}
