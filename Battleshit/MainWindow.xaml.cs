﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;
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

        public MainWindow()
        {
            InitializeComponent();
            bgPlayer.Open(new Uri("pack://siteoforigin:,,,/Assets/sound-bg.mp3"));
            bgPlayer.MediaFailed += OnMediaFailed;
            bgPlayer.MediaEnded += OnMediaEnded;
            bgPlayer.Play();
        }

        private void OnMediaFailed(object sender, ExceptionEventArgs e)
        {
            var exception = e.ErrorException;
            throw exception; 
        }
        private void OnMediaEnded(object sender, EventArgs e)
        {
            var player = (MediaPlayer)sender;
            player.Play();
        }

        private async void Play_btn_Click(object sender, RoutedEventArgs e)
        {
            await StartGameEffect();
            this.Content = new SinglePlayer();
        }

        private async Task StartGameEffect()
        {
            flushPlayer.Open(new Uri("pack://siteoforigin:,,,/Assets/toilet-flushing.mp3"));
            flushPlayer.Play();
            await Task.Delay(3000);
        }

        private void Exit_btn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            bgPlayer.Volume = (double)volume.Value;
            flushPlayer.Volume = (double)volume.Value;
        }
    }
}
