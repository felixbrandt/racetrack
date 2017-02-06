using System;
using Windows.Data.Json;

namespace RaceTrack
{
    /// <author>David Fraas</author>
    /// <summary>
    /// A RacePoints stores information about a single moment of time in the Race.
    /// A new RacePoint gets created everytime the position of the device changes
    /// </summary>
    class RacePoint
    {
        // JSON key names
        private const string timestampKey = "timestamp";
        private const string longitudeKey = "longitude";
        private const string latitudeKey = "latitude";
        private const string speedKey = "speed";
        private const string gForceKey = "gForce";
        private const string tiltKey = "tilt";

        // Attributes
        public int Timestamp;
        public double Longitude;
        public double Latitude;
        public double Speed;
        public double GForce;
        public double Tilt;

        /// <summary>
        /// Create a new RacePoint
        /// </summary>
        /// <param name="longitude">Longitude</param>
        /// <param name="latitude">Latitude</param>
        /// <param name="speed">Speed in m/s</param>
        /// <param name="gForce">G-Force</param>
        /// <param name="tilt">Tilt in degrees</param>
        public RacePoint(double longitude, double latitude, double speed, double gForce, double tilt)
        {
            Timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds); // Seconds since 01/01/1970
            Longitude = longitude;
            Latitude = latitude;
            Speed = speed;
            GForce = gForce;
            Tilt = tilt;
        }

        /// <summary>
        /// Writes data from RacePoint JSON to the object
        /// </summary>
        /// <param name="jsonObject">RacePoint JsonObject</param>
        public RacePoint(JsonObject jsonObject)
        {
            JsonObject racePointObject = jsonObject;

            if (racePointObject != null)
            {
                // Fill the RacePoint with data from the JsonObject
                Timestamp = (Int32)racePointObject.GetNamedNumber(timestampKey);
                Longitude = racePointObject.GetNamedNumber(longitudeKey, 0);
                Latitude = racePointObject.GetNamedNumber(latitudeKey, 0);
                Speed = racePointObject.GetNamedNumber(speedKey, 0);
                GForce = racePointObject.GetNamedNumber(gForceKey, 0);
                Tilt = racePointObject.GetNamedNumber(tiltKey, 0);
            }
        }

        /// <summary>
        /// Converts the Object to a JSON Object
        /// </summary>
        /// <returns>JsonObject of the RacePoint</returns>
        public JsonObject ToJsonObject()
        {
            JsonObject racePointObject = new JsonObject();
            // Set key-value pairs
            racePointObject.SetNamedValue(timestampKey, JsonValue.CreateNumberValue(Timestamp));
            racePointObject.SetNamedValue(longitudeKey, JsonValue.CreateNumberValue(Longitude));
            racePointObject.SetNamedValue(latitudeKey, JsonValue.CreateNumberValue(Latitude));
            racePointObject.SetNamedValue(speedKey, JsonValue.CreateNumberValue(Speed));
            racePointObject.SetNamedValue(gForceKey, JsonValue.CreateNumberValue(GForce));
            racePointObject.SetNamedValue(tiltKey, JsonValue.CreateNumberValue(Tilt));

            return racePointObject;
        }
    }
}