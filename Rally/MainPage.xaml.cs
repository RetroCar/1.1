using Bing.Maps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Rally
{
    public sealed partial class MainPage : Page
    {
        #region Private Properties 

        private DispatcherTimer gameTimer;
        private Car MyCar;

        private ObservableCollection<Track> Tracks = new ObservableCollection<Track>()
        {
            new Track("Daytona International Speedway", new Location(29.18782, -81.07275), 215),
            new Track("Indianapolis Motor Speedway", new Location(39.79314, -86.2389), 0),
            new Track("Silverstone Circuit", new Location(52.07875, -1.0171), 80)
        };

        #endregion

        #region Consturctor 

        public MainPage()
        {
            this.InitializeComponent();

            Canvas baseCanvas = new Canvas();
            MyCar = new Car();
            baseCanvas.Children.Add(MyCar);
            MyMap.Children.Add(baseCanvas);

            FillTrackFlyout();

            MyMap.Center = Tracks[0].StartLocation;
            MyCar.Heading = Tracks[0].StartHeading;
            TrackName.Text = Tracks[0].Name;

            MyMap.KeyDownOverride += (s, e) =>
            {
                e.Handled = true;
            };

            MyMap.PointerPressedOverride += (s, e) =>
            {
                e.Handled = true;
            };

            gameTimer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 0, 0, 10)
            };
            gameTimer.Tick += gameTimer_Tick;
            gameTimer.Start();
        }

        #endregion

        #region Event Handlers 

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (gameTimer != null && !gameTimer.IsEnabled)
            {
                gameTimer.Start();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (gameTimer != null && gameTimer.IsEnabled)
            {
                gameTimer.Stop();
            }
        }

        #endregion

        #region Private Methods 

        private void gameTimer_Tick(object sender, object e)
        {
            var offset = MyCar.GetPositionOffset();

            Point centerPixel;

            if (MyMap.TryLocationToPixel(MyMap.Center, out centerPixel))
            {
                Canvas.SetLeft(MyCar, centerPixel.X);
                Canvas.SetTop(MyCar, centerPixel.Y);

                centerPixel.X += offset.X;
                centerPixel.Y += offset.Y;

                Location newCenter;

                if (MyMap.TryPixelToLocation(centerPixel, out newCenter))
                {
                    MyMap.Center = newCenter;
                }

                //Calculate driving speed 
                double pixelDistance = Math.Sqrt(Math.Pow(offset.X, 2) + Math.Pow(offset.Y, 2));
                double earthRadiusKm = 6378.135;
                double groundResolution = (Math.Cos(MyMap.Center.Latitude * Math.PI / 180)) * 2 * Math.PI * earthRadiusKm / Math.Round(256 * Math.Pow(2, MyMap.ZoomLevel));
                double distanceKm = pixelDistance * groundResolution;
                double time = gameTimer.Interval.TotalHours;
                double speed = distanceKm / time;

                Speedometer.Text = string.Format("{0:N0} KM/H", speed);
            }
        }

        private void FillTrackFlyout()
        {
            foreach (var t in Tracks)
            {
                var item = new MenuFlyoutItem()
                {
                    Text = t.Name,
                    Tag = t
                };

                item.Tapped += TrackChanged;

                TrackFlyout.Items.Add(item);
            }
        }

        void TrackChanged(object sender, TappedRoutedEventArgs e)
        {
            var track = (sender as MenuFlyoutItem).Tag as Track;
            MyMap.Center = track.StartLocation;
            MyCar.Heading = track.StartHeading;
            TrackName.Text = track.Name;
        }

        #endregion
    }
}
