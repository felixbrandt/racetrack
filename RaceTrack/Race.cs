using System;
using System.Collections.ObjectModel;
using Windows.Data.Json;
using Windows.Storage;

namespace RaceTrack
{
    class Race
    {
        // JSON key names
        private const string startTimeKey = "startTime";
        private const string endTimeKey = "endTime";
        private const string roundsKey = "rounds";
        private const string nameKey = "name";

        // Attributes
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public string Name { get; set; }
        public ObservableCollection<Round> Rounds { get; set; }
        private Round currentRound;

        /// <author>Manuel Schreiber</author>
        /// <author>Felix Brandt</author>
        /// <summary>
        /// Creates and starts a new race
        /// </summary>
        public Race()
        {
            StartTime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds); // Seconds since 01/01/1970
            Rounds = new ObservableCollection<Round>();
            currentRound = new Round();
            Rounds.Add(currentRound);
        }

        /// <summary>
        /// Creates a Race from a JSON string
        /// </summary>
        /// <param name="jsonString">JSON string representing a Race</param>
        public Race(string jsonString)
        {
            // Create jsonObject from string
            JsonObject jsonObject = JsonObject.Parse(jsonString);

            // Set Attributes
            StartTime = (int)jsonObject.GetNamedNumber(startTimeKey, 0);
            EndTime = (int)jsonObject.GetNamedNumber(endTimeKey, 0);
            Name = jsonObject.GetNamedString(nameKey, "No Name Race");

            // Add laps
            foreach (IJsonValue jsonValue in jsonObject.GetNamedArray(roundsKey, new JsonArray()))
            {
                if (jsonValue.ValueType == JsonValueType.Object)
                {
                    var test = jsonValue.GetObject();

                    Rounds.Add(new Round(jsonValue.GetObject()));
                }
            }
        }

        /// <summary>
        /// Writes data from Round JSON to the object
        /// </summary>
        /// <param name="jsonObject"></param>
        public Race(JsonObject jsonObject)
        {
            JsonObject raceObject = jsonObject;
            Rounds = new ObservableCollection<Round>();

            if (raceObject != null)
            {
                // Set Attributes
                StartTime = (Int32)raceObject.GetNamedNumber(startTimeKey);
                EndTime = (Int32)raceObject.GetNamedNumber(endTimeKey);
                Name = raceObject.GetNamedString(nameKey);

                // Add laps
                foreach (IJsonValue jsonValue in raceObject.GetNamedArray(roundsKey, new JsonArray()))
                {
                    if (jsonValue.ValueType == JsonValueType.Object)
                    {
                        Rounds.Add(new Round(jsonValue.GetObject()));
                    }
                }
            }
        }

        /// <summary>
        /// Getter for maximum speed
        /// </summary>
        /// <returns>Maximum speed of this race</returns>
        internal double GetMaximumSpeed()
        {
            double max = 0;
            foreach (Round round in Rounds)
            {
                foreach (RacePoint point in round.RacePoints)
                {
                    if (point.Speed > max)
                    {
                        max = point.Speed;
                    }
                }
            }

            return max;
        }

        /// <summary>
        /// Getter for maximum tilt
        /// </summary>
        /// <returns>Maximum tilt of this race</returns>
        internal double GetMaxTilt()
        {
            double max = 0;
            foreach (Round round in Rounds)
            {
                foreach (RacePoint point in round.RacePoints)
                {
                    if (point.Tilt > max)
                    {
                        max = point.Tilt;
                    }
                }
            }

            return max;
        }

        /// <summary>
        /// Getter for fastest round time
        /// </summary>
        /// <returns>Fastest round time of this race in seconds</returns>
        internal int GetFastestRoundTime()
        {
            var min = Rounds[0].EndTime - Rounds[0].StartTime;

            for (int i = 1; i<Rounds.Count; i++)
            {
                var duration = Rounds[i].EndTime - Rounds[i].StartTime;
                if (duration < min)
                {
                    min = duration;
                }
            }

            return min;
        }

        /// <summary>
        /// Getter for maximum force
        /// </summary>
        /// <returns>Maximum G-Force in this race</returns>
        internal double GetMaximumForce()
        {
            double max = 0;
            foreach (Round round in Rounds)
            {
                foreach (RacePoint point in round.RacePoints)
                {
                    if (point.GForce > max)
                    {
                        max = point.GForce;
                    }
                }
            }

            return max;
        }

        /// <summary>
        /// Converts the Object to a JSON Object
        /// </summary>
        /// <returns>JsonObject of the Race</returns>
        public JsonObject ToJsonObject()
        {
            JsonObject raceObject = new JsonObject();
            raceObject.SetNamedValue(startTimeKey, JsonValue.CreateNumberValue(StartTime));
            raceObject.SetNamedValue(endTimeKey, JsonValue.CreateNumberValue(EndTime));
            raceObject.SetNamedValue(nameKey, JsonValue.CreateStringValue(Name));

            var roundsJsonArray = new JsonArray();
            foreach (Round round in Rounds)
            {
                roundsJsonArray.Add(round.ToJsonObject());
            }

            raceObject.SetNamedValue(roundsKey, roundsJsonArray);
            return raceObject;
        }

        /// <summary>
        /// Ends the race and saves it to disk
        /// </summary>
        public async void EndRace()
        {
            // End the currently active round
            currentRound.EndRound();

            // Set the endtime
            EndTime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds); // Seconds since 01/01/1970
            
            // Open races folder on disk
            var folder = ApplicationData.Current.LocalFolder;
            var racesFolder = await folder.CreateFolderAsync("races", CreationCollisionOption.OpenIfExists);

            // Write startTime.json file, the chance that the file already exists is ~0
            var race = await racesFolder.CreateFileAsync(StartTime + ".json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(race, this.ToJsonObject().ToString());
        }

        /// <summary>
        /// Calculate the total race distance
        /// </summary>
        /// <returns>Total distance in meters</returns>
        public int CalculateTotalDistance()
        {
            int distance = 0;
            foreach (Round round in Rounds)
            {
                // Don't check last point because it has no successor
                for (var i = 0; i < round.RacePoints.Count - 1; i++)
                {
                    var point = round.RacePoints[i];
                    var nextPoint = round.RacePoints[i+1];
                    distance += Haversine.HaversineInM(point.Latitude, point.Longitude, nextPoint.Latitude, nextPoint.Longitude);
                }
            }
            return distance;
        }

        /// <summary>
        /// Calculates the average speed for this race
        /// </summary>
        /// <returns>Average speed in m/s</returns>
        public double CalculateAverageSpeed()
        {
            int racepointCount = 0;
            double totalSpeed = 0;

            foreach (Round round in Rounds)
            {
                // Sum up speed of all RPs
                foreach (RacePoint rp in round.RacePoints)
                {
                    racepointCount++;
                    totalSpeed += rp.Speed;
                }
            }

            // Average = count/amount
            if (racepointCount > 0)
            {
                return totalSpeed / racepointCount;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Adds a new round to the race
        /// </summary>
        /// <returns>The newly created lap</returns>
        public Round NewRound()
        {
            // Current round is over
            currentRound.EndRound();

            // Newxt round starts
            currentRound = new Round();
            Rounds.Add(currentRound);

            return currentRound;
        }

        /// <summary>
        /// Updates the race with data, delegating to the current round
        /// </summary>
        /// <param name="xPos">Longitude</param>
        /// <param name="yPos">Latitude</param>
        /// <param name="speed">Speed in m/s</param>
        /// <param name="gForce">G-Force</param>
        /// <param name="tilt">Tilt in degrees</param>
        /// <returns>The created RacePoint</returns>
        public RacePoint Update(double longitude, double latitude, double speed, double gForce, double tilt)
        {
            return currentRound.Update(longitude, latitude, speed, gForce, tilt);
        }

        /// <summary>
        /// Get the current lap time
        /// </summary>
        /// <returns>Lap time of active lap</returns>
        public Int32 GetRoundTime()
        {
            var now = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds; // Seconds since 01/01/1970
            return now - currentRound.StartTime;
        }

        /// <summary>
        /// Get the current race time
        /// </summary>
        /// <returns>Current time of the race</returns>
        public Int32 GetTotalTime()
        {
            var now = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds; // Seconds since 01/01/1970
            return now - StartTime;
        }
    }
}