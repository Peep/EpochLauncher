using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher
{
    public class Server : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _hostname;
        private int _currentPlayers;
        private int _maxPlayers;
        private int _ping;
        private bool _isOfficial;

        public string Hostname
        {
            get { return _hostname; }
            internal set
            {
                _hostname = value;
                OnPropertyChanged("Hostname");
            }
        }

        public int CurrentPlayers
        {
            get { return _currentPlayers; }
            internal set
            {
                _currentPlayers = value;
                OnPropertyChanged("CurrentPlayers");
            }
        }

        public int MaxPlayers
        {
            get { return _maxPlayers; }
            internal set
            {
                _maxPlayers = value;
                OnPropertyChanged("MaxPlayers");
            }
        }

        public int Ping
        {
            get { return _ping; }
            internal set
            {
                _ping = value;
                OnPropertyChanged("Ping");
            }
        }

        public bool IsOfficial
        {
            get { return _isOfficial; }
            internal set
            {
                _isOfficial = value;
                OnPropertyChanged("IsOfficial");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
