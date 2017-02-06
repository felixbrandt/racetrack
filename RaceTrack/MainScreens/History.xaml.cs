using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace RaceTrack.MainScreens
{
    /// <author>Felix Brandt</author>
    /// <summary>
    /// Displays a history of all the races in the past
    /// </summary>
    public sealed partial class History : Page
    {
        ApplicationDataContainer roamingSettings;

        /// <summary>
        /// All Races as Race Objects
        /// </summary>
        private ObservableCollection<Race> allMyRaces { get; set; }
        
        /// <summary>
        /// All Races as RaceDisplay for binding to view
        /// </summary>
        internal  ObservableCollection<RaceDisplay> allMyRacesDisplay { get; set; }

        public History()
        {
            this.InitializeComponent();

            roamingSettings = ApplicationData.Current.RoamingSettings;

            // Loads the Races from storage
            updateAllRaces();
        }

        /// <summary>
        /// Updates allMyRaces and allMyRacesDisplay from storage
        /// </summary>
        private async void updateAllRaces()
        {
            allMyRaces = new ObservableCollection<Race>();
            allMyRacesDisplay = new ObservableCollection<RaceDisplay>();

            // Open races folder
            var folder = ApplicationData.Current.LocalFolder;
            var racesFolder = await folder.CreateFolderAsync("races", CreationCollisionOption.OpenIfExists);

            // Read all files stored in the folder
            var allFiles = await racesFolder.GetFilesAsync();

            // Create new race for each file, add it to allMyRaces
            foreach (StorageFile file in allFiles)
            {
                var content = await FileIO.ReadTextAsync(file);
                allMyRaces.Add(new Race(JsonObject.Parse(content)));
            }              

            // Create RaceDisplay for each race to display in the view
            foreach(Race race in allMyRaces)
            {
                DateTimeOffset startTime = DateTimeOffset.FromUnixTimeSeconds(race.StartTime).ToLocalTime();
                string startTimeString = String.Format("{0:00}.{1:00}.{2:00} {3:00}:{4:00}", startTime.Day, startTime.Month, startTime.Year, startTime.Hour, startTime.Minute);

                DateTimeOffset endTime = DateTimeOffset.FromUnixTimeSeconds(race.EndTime).ToLocalTime();
                string endTimeString = String.Format("{0:00}:{1:00}", endTime.Hour, endTime.Minute);

                string averageSpeedString;
                string totalDistanceString;

                if ((roamingSettings.Values["unit"]).ToString() == "metric")
                {
                    // Average speed in kmh
                    averageSpeedString = String.Format("{0:0} km/h", race.CalculateAverageSpeed() * 3.6);

                    // Total distance in km
                    totalDistanceString = String.Format("{0:0.00} km", (double)race.CalculateTotalDistance() / 1000);
                } else
                {
                    // Average speed in kmh
                    averageSpeedString = String.Format("{0:0} mph", race.CalculateAverageSpeed() * 2.23694);

                    // Total distance in km
                    totalDistanceString = String.Format("{0:0.00} mi", (double)race.CalculateTotalDistance() / 1609.344);
                }
                
                // RaceDisplay class used for data binding    
                allMyRacesDisplay.Add(new RaceDisplay()
                {
                    Name = race.Name,
                    StartTime = startTimeString,
                    EndTime = endTimeString,
                    AverageSpeed = averageSpeedString,
                    TotalDistance = totalDistanceString,
                    RoundCount = String.Format("{0} Runde(n)", race.Rounds.Count),
                });
            }

            // Reverse the list so newer races are displayed at the top
            DataContext = allMyRacesDisplay.Reverse<RaceDisplay>();
        }
    }
}