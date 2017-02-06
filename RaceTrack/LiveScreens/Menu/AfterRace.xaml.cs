using System;
using System.Collections.ObjectModel;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;

namespace RaceTrack.LiveScreens.Menu
{
    /// <author>Manuel Schreiber</author>
    /// <summary>
    /// Displays info about the recently ended race
    /// </summary>
    public sealed partial class AfterRace : Page
    {
        public AfterRace()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Resets the page to begin another race
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Tapped Routed Event Args</param>
        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ((MainScreens.LiveRace)((Grid)((Frame)Parent).Parent).Parent).startNewRaceAgain();
        }

        /// <summary>
        /// Displays info on the screen
        /// </summary>
        /// <param name="race">Race about which to display information</param>
        internal void DisplayInfo(Race race)
        {
            RaceName.DataContext = race.Name;
            
            var startTime = DateTimeOffset.FromUnixTimeSeconds(race.StartTime).ToLocalTime();
            var startTimeString = String.Format("{0:00}:{1:00}", startTime.Hour, startTime.Minute);

            var endTime = DateTimeOffset.FromUnixTimeSeconds(race.EndTime).ToLocalTime();
            var endTimeString = String.Format("{0:00}:{1:00}", endTime.Hour, endTime.Minute);

            RaceInfo.Text = String.Format("({0}-{1}) / ", startTimeString, endTimeString);
            RaceInfo.Text += race.Rounds.Count;
            RaceInfo.Text += " Runde(n)";

            for (var i=0; i<race.Rounds.Count; i++)
            {
                var roundTimeSeconds = race.Rounds[i].EndTime - race.Rounds[i].StartTime;

                var hours = roundTimeSeconds / 60 / 60;
                var minutes = (roundTimeSeconds / 60) % 60;
                var seconds = roundTimeSeconds % 60;

                RaceInfo.Text += String.Format("\nRunde {3}: {0:00}:{1:00}:{2:00}", hours, minutes, seconds, i);

            }

            /* Draws race path to the map */
            var PosList = new ObservableCollection<BasicGeoposition>();

            foreach (Round round in race.Rounds)
            {
                foreach (RacePoint rp in round.RacePoints)
                {
                    PosList.Add(new BasicGeoposition()
                    {
                        Latitude = rp.Latitude,
                        Longitude = rp.Longitude,
                    });
                }
            }

            var linePath = new Geopath(PosList);

            var mapPolyline = new MapPolyline();
            mapPolyline.Path = linePath;

            mapPolyline.StrokeThickness = 5;
            mapPolyline.StrokeColor = Colors.Red;

            AfterRaceMap.MapElements.Add(mapPolyline);
            AfterRaceMap.ZoomLevel = 13D;

            AfterRaceMap.Center = new Geopoint(
                new BasicGeoposition()
                {
                    Latitude = race.Rounds[0].RacePoints[0].Latitude,
                    Longitude = race.Rounds[0].RacePoints[0].Longitude,
                }
            );
        }
    }
}
