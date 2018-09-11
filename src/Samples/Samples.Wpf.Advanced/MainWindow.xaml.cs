using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using Vlc.DotNet.Wpf;

namespace Samples.Wpf.Advanced
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DirectoryInfo vlcLibDirectory;
        private VlcControl control;

        public MainWindow()
        {
            InitializeComponent();
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            // Default libraries are stored here, but they are old, don't use them.
            // We need a better way to install them, see https://github.com/ZeBobo5/Vlc.DotNet/issues/288
            if (IntPtr.Size == 4)
                vlcLibDirectory = new DirectoryInfo(Path.Combine(currentDirectory, @"..\..\..\..\..\lib\x86\"));
            else
                vlcLibDirectory = new DirectoryInfo(Path.Combine(currentDirectory, @"..\..\..\..\..\lib\x64\"));

            this.control?.Dispose();
            this.control = new VlcControl();
            this.control.Stretch = System.Windows.Media.Stretch.Fill;
            this.ControlContainer.Content = this.control;
            this.control.SourceProvider.CreatePlayer(this.vlcLibDirectory);

            // This can also be called before EndInit
            this.control.SourceProvider.MediaPlayer.Log += (_, args) =>
            {
                string message = $"libVlc : {args.Level} {args.Message} @ {args.Module}";
                System.Diagnostics.Debug.WriteLine(message);
            };
            // this.control.SourceProvider.MediaPlayer.PositionChanged += MediaPlayer_PositionChanged;
            this.control.SourceProvider.MediaPlayer.TimeChanged += MediaPlayer_TimeChanged;
            this.control.SourceProvider.MediaPlayer.Paused += MediaPlayer_Paused;
            this.control.SourceProvider.MediaPlayer.Playing += MediaPlayer_Playing;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.control?.Dispose();
            base.OnClosing(e);
        }

        private void OnPlayButtonClick(object sender, RoutedEventArgs e)
        {       
            if(this.control !=null)
            {
                if (this.control.SourceProvider.MediaPlayer.IsPlaying())
                    this.control.SourceProvider.MediaPlayer.Pause();
                else if (this.control.SourceProvider.MediaPlayer.IsPausable())
                    this.control.SourceProvider.MediaPlayer.Play();

            }
        }

        private void OnStopButtonClick(object sender, RoutedEventArgs e)
        {
            //this.control?.Dispose();
            //this.control = null;
            this.control.SourceProvider.MediaPlayer.Stop();
        }

        private void bt_rateDown_Click(object sender, RoutedEventArgs e)
        {
            if(this.control != null)
            {
                this.control.SourceProvider.MediaPlayer.Rate -=(float) 0.1;

            }
        }

        private void bt_rateUp_Click(object sender, RoutedEventArgs e)
        {
            if (this.control != null)
            {
                this.control.SourceProvider.MediaPlayer.Rate += (float)0.1;

            }
        }

        private void Mute_Click(object sender, RoutedEventArgs e)
        {
            if (this.control != null)
            {
                if (this.control.SourceProvider.MediaPlayer.Audio.IsMute == true)
                    this.control.SourceProvider.MediaPlayer.Audio.IsMute = false;
                else
                    this.control.SourceProvider.MediaPlayer.Audio.IsMute = true;

            }

        }

        private void GetCurrentTime_Click(object sender, RoutedEventArgs e)
        {
            if (this.control == null)
            {
                return;
            }

            GetCurrentTime.Content = this.control.SourceProvider.MediaPlayer.Time + " ms";
        }

        private void SetCurrentTime_Click(object sender, RoutedEventArgs e)
        {
            if (this.control == null)
            {
                return;
            }

            this.control.SourceProvider.MediaPlayer.Time = 5000;
            SetCurrentTime.Content = this.control.SourceProvider.MediaPlayer.Time + " ms";
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            control.SourceProvider.MediaPlayer.Play(new Uri("F:\\bh1.mp4"));
        }

        private void MediaPlayer_TimeChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerTimeChangedEventArgs e)
        {
            if (this.control != null)
            {
                this.Dispatcher.BeginInvoke((Action)(() =>
                {
                    string _currenTime = this.control.SourceProvider.MediaPlayer.Time.ToString("##");
                    string _lenght = this.control.SourceProvider.MediaPlayer.Length.ToString("##");
                    string _volume = this.control.SourceProvider.MediaPlayer.Audio.Volume.ToString("##");
                    string _currentTrack = this.control.SourceProvider.MediaPlayer.Audio.Tracks.Current.ID.ToString();
                    string _trackCount = this.control.SourceProvider.MediaPlayer.Audio.Tracks.Count.ToString();
                    string _rate = this.control.SourceProvider.MediaPlayer.Rate.ToString();

                    textBlock.Text = _currenTime + " / " + _lenght + " - Volume=" + _volume + " - Track: " + _currentTrack + "/" + _trackCount + "- Rate:" + _rate;
                }));
            }
        }

        private void MediaPlayer_Playing(object sender, Vlc.DotNet.Core.VlcMediaPlayerPlayingEventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                bt_Play.Content = "Pause";
            }));       
        }

        private void MediaPlayer_Paused(object sender, Vlc.DotNet.Core.VlcMediaPlayerPausedEventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                bt_Play.Content = "Play";
            }));
        }

        private void MediaPlayer_PositionChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerPositionChangedEventArgs e)
        {          

        }

        private void sliderBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(this.control != null)
                this.control.SourceProvider.MediaPlayer.Audio.Volume =(int)sliderBar.Value;
        }

       
    }
}
