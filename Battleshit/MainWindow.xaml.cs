using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Battleshit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer bgPlayer = new MediaPlayer();
        private MediaPlayer flushPlayer = new MediaPlayer();
        public static double GameVolume { get; set; }
        public static double keepVolume { get; set; } = 0;

        public static bool GameMute { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            bgPlayer.Open(new Uri("pack://siteoforigin:,,,/Assets/sound-bg.mp3"));
            bgPlayer.MediaFailed += OnMediaFailed;
            bgPlayer.MediaEnded += OnMediaEnded;
            bgPlayer.Play();
            GameVolume = 0.5;
            GameMute = false;
        }

        private void OnMediaFailed(object sender, ExceptionEventArgs e)
        {
            var exception = e.ErrorException;
            throw exception; 
        }
        private void OnMediaEnded(object sender, EventArgs e)
        {
            var player = (sender as MediaPlayer);
            player.Position = TimeSpan.Zero;
            player.Play();
        }

        private async void Play_btn_Click(object sender, RoutedEventArgs e)
        {
            var btn = (sender as Button);
            btn.IsEnabled = false;
            await StartGameEffect();
            this.Content = new SinglePlayer();
        }

        private async Task StartGameEffect()
        {
            bgPlayer.Volume -= (double)bgPlayer.Volume/2;
            flushPlayer.Open(new Uri("pack://siteoforigin:,,,/Assets/toilet-flushing.mp3"));
            flushPlayer.Play();
            WaveStoryboard.Storyboard.SetSpeedRatio(this, 1.5);
            await Task.Delay(3000);
            bgPlayer.Stop();
        }

        private void Exit_btn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            bgPlayer.Volume = (double)volume.Value;
            flushPlayer.Volume = (double)volume.Value;
            GameVolume = (double)volume.Value;
        }

        private void HandleMuteCheck(object sender, RoutedEventArgs e)
        {
            keepVolume = bgPlayer.Volume;
            GameMute = true;
            volume.Value = 0;
            bgPlayer.Volume = 0;
        }

        private void HandleMuteUnchecked(object sender, RoutedEventArgs e)
        {
            GameMute = false;
            volume.Value = keepVolume;
            bgPlayer.Volume = keepVolume;
        }
    }
}
