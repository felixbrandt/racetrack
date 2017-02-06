using System;
using System.Linq;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace RaceTrack.MainScreens
{
    /// <author>Manuel Schreiber</author>
    /// <summary>
    /// Landing screen / Homepage
    /// </summary>
    public sealed partial class Home : Page
    {
        ApplicationDataContainer roamingSettings;

        public Home()
        {
            this.InitializeComponent();

            roamingSettings = ApplicationData.Current.RoamingSettings;

            // Display information about the last race from storage
            getLastRace();
        }

        /// <summary>
        /// Set DataContexts to last races information from the stored file
        /// </summary>
        private async void getLastRace()
        {
            // Open races folder
            var folder = ApplicationData.Current.LocalFolder;
            var racesFolder = await folder.CreateFolderAsync("races", CreationCollisionOption.OpenIfExists);

            // Get a list of all stored races
            var allRaces = await racesFolder.GetFilesAsync();

            // Are there any races?
            if (allRaces.Count > 0)
            {
                // Open most recent race
                var content = await FileIO.ReadTextAsync(allRaces.Last());
                
                // Create Race Object
                Race lastRace = new Race(JsonObject.Parse(content));

                // Set race name in view
                LastRaceName.DataContext = lastRace.Name;

                // Set race start time in view
                var date = DateTimeOffset.FromUnixTimeSeconds(lastRace.StartTime).ToLocalTime();
                LastRaceStartTime.DataContext = String.Format("{0:00}.{1:00}.{2:00} {3:00}:{4:00}", date.Day, date.Month, date.Year, date.Hour, date.Minute);

                // Set race average speed in view
                // + Set race total distance in view
                // + Set race maximum speed in view
                if ((roamingSettings.Values["unit"]).ToString() == "metric")
                {
                    // metric
                    // *3.6 => m/s to km/h
                    AverageSpeedText.DataContext = String.Format("{0:0 km/h}", lastRace.CalculateAverageSpeed() * 3.6);

                    // /1000 => m to km
                    TotalDistanceText.DataContext = String.Format("{0:0.00 km}", (double)lastRace.CalculateTotalDistance() / 1000);

                    MaxSpeed.DataContext = String.Format("{0:0.00} km/h", lastRace.GetMaximumSpeed() * 3.6);
                }
                else
                {
                    // imperial
                    // *2.23694 => m/s to miles/h
                    AverageSpeedText.DataContext = String.Format("{0:0 mph}", lastRace.CalculateAverageSpeed() * 2.23694);

                    // /1609.344 => m to mile
                    TotalDistanceText.DataContext = String.Format("{0:0.00 mi}", (double)lastRace.CalculateTotalDistance() / 1609.344);

                    MaxSpeed.DataContext = String.Format("{0:0.00} mph", lastRace.GetMaximumSpeed() * 2.23694);
                }

                // Set race maximum force in view
                var maximumForce = lastRace.GetMaximumForce();
                MaxForce.DataContext = String.Format("{0:0.00}", maximumForce);

                // Set race maximum tilt in view
                MaxTilt.DataContext = String.Format("{0:0.00}°", lastRace.GetMaxTilt());

                // Set race fastest round in view
                int fastestRoundTime = lastRace.GetFastestRoundTime();
                var hours = fastestRoundTime / 60 / 60;
                var minutes = (fastestRoundTime / 60) % 60;
                var seconds = fastestRoundTime % 60;
                FastestRound.DataContext = String.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);

                // Set last race day in view
                LastRaceDay.DataContext = date.Day;
            }
            else
            {
                // No race found (First time using the app?)
                LastRaceName.DataContext = "Kein letztes Rennen.";
                LastRaceStartTime.DataContext = "00:00";
                AverageSpeedText.DataContext = 0;
                TotalDistanceText.DataContext = 0;

                MaxForce.DataContext = 0;
                MaxSpeed.DataContext = 0;
                MaxTilt.DataContext = 0;
                FastestRound.DataContext = "00:00:00";

                LastRaceDay.DataContext = "-";
            }

        }

        /// <summary>
        /// Start new race button tapped. Switches to the second pivot item (new Race)
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Tapped Routed Event Args</param>
        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var Pivot = (Pivot)((PivotItem)((Frame)Parent).Parent).Parent;
            Pivot.SelectedIndex++;
        }
    }
}