using LaserTag.DesktopClient.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace LaserTag.DesktopClient
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private LaserTagStatsService _statsService;
        private LaserTagCommandsService _commandService;

        public event PropertyChangedEventHandler PropertyChanged;

        private int _ammo;
        public int Ammo
        {
            get { return _ammo; }
            set
            {
                _ammo = value;
                NotifyPropertyChanged();
            }
        }

        private int _clips;
        public int Clips
        {
            get { return _clips; }
            set
            {
                _clips = value;
                NotifyPropertyChanged();
            }
        }

        private int _health;
        public int Health
        {
            get { return _health; }
            set
            {
                _health = value;
                NotifyPropertyChanged();
            }
        }

        public MainPage()
        {
            InitializeComponent();

            DataContext = this;

            Loaded += OnLoaded;
        }


        private async Task SetupBtLeServices()
        {
            Ammo = 0;
            Clips = 0;
            Health = 0;

            _statsService = new LaserTagStatsService();
            await _statsService.InitializeAsync();

            _statsService.GunChanged += (ammo, clips) =>
            {
                Ammo = ammo;
                Clips = clips;
            };
            _statsService.PlayerChanged += (health, shotBy) => Health = health;

            _commandService = new LaserTagCommandsService();
            await _commandService.InitializeAsync();
        }


        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await SetupBtLeServices();
        }


        private async void OnReloadClicked(object sender, RoutedEventArgs e)
        {
            await _commandService.Reload();
        }

        private async void OnRespawnClicked(object sender, RoutedEventArgs e)
        {
            await _commandService.Respawn();
        }

        private async void OnRdsPowerClicked(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;

            await _commandService.SetRedDotSightPower(button.IsChecked.Value);
        }

        private async void NotifyPropertyChanged([CallerMemberName]string name = null)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => PropertyChanged(this, new PropertyChangedEventArgs(name)));
        }
    }
}
