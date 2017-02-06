using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Data.Json;

namespace RaceTrack
{
    /// <author>Manuel Schreiber</author>
    /// <summary>
    /// A Round represents a single lap in a race. It consists of multiple RacePoints and belongs to a race.
    /// </summary>
    class Round
    {
        // JSON key names
        private const string startTimeKey = "startTime";
        private const string endTimeKey = "endTime";
        private const string racePointsKey = "racePoints";

        // Attributes
        public int StartTime;
        public int EndTime;
        public ObservableCollection<RacePoint> RacePoints;


        /// <summary>
        /// Creates a new round
        /// </summary>
        public Round()
        {
            StartTime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds); // Seconds since 01/01/1970
            RacePoints = new ObservableCollection<RacePoint>();
        }

        /// <summary>
        /// Writes data from Round JSON to the object
        /// </summary>
        /// <param name="jsonObject">Round JsonObject</param>
        public Round(JsonObject jsonObject)
        {
            RacePoints = new ObservableCollection<RacePoint>();

            JsonObject roundObject = jsonObject;

            if (roundObject != null)
            {
                StartTime = (Int32)roundObject.GetNamedNumber(startTimeKey);
                EndTime = (Int32)roundObject.GetNamedNumber(endTimeKey);

                // Adds RacePoints to the round
                foreach (IJsonValue jsonValue in roundObject.GetNamedArray(racePointsKey, new JsonArray()))
                {
                    if (jsonValue.ValueType == JsonValueType.Object)
                    {
                        RacePoints.Add(new RacePoint(jsonValue.GetObject()));
                    }
                }
            }
        }

        /// <summary>
        /// Converts the Object to a JSON Object
        /// </summary>
        /// <returns>JsonObject of the Round</returns>
        public JsonObject ToJsonObject()
        { 
            JsonObject racePointObject = new JsonObject();
            racePointObject.SetNamedValue(startTimeKey, JsonValue.CreateNumberValue(StartTime));
            racePointObject.SetNamedValue(endTimeKey, JsonValue.CreateNumberValue(EndTime));

            var racePointJsonArray = new JsonArray();
            foreach(RacePoint racePoint in RacePoints)
            {
                racePointJsonArray.Add(racePoint.ToJsonObject());
            }

            racePointObject.SetNamedValue(racePointsKey, racePointJsonArray);
            return racePointObject;
        }

        /// <summary>
        /// Ends the round, sets EndTime
        /// </summary>
        public void EndRound()
        {
            EndTime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds); // Seconds since 01/01/1970
        }

        /// <summary>
        /// Updates the lap with data, creating a new RacePoint
        /// </summary>
        /// <param name="xPos">Longitude</param>
        /// <param name="yPos">Latitude</param>
        /// <param name="speed">Speed in m/s</param>
        /// <param name="gForce">G-Force</param>
        /// <param name="tilt">Tilt in degrees</param>
        /// <returns>The created RacePoint</returns>
        internal RacePoint Update(double xPos, double yPos, double speed, double gForce, double tilt)
        {
            RacePoint rp = new RacePoint(xPos, yPos, speed, gForce, tilt);

            if (RacePoints.Count > 0)
            {
                RacePoint prevPoint = RacePoints.Last();
            }            

            RacePoints.Add(rp);
            return rp;
        }
    }
}