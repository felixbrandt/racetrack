using System;

namespace RaceTrack
{
    /// <author>Roman Makarov @ stackoverflow</author>
    /// <summary>
    /// Calculates the distance between two latitude-longitude points
    /// Thanks to Roman Makarov on stackoverflow (http://stackoverflow.com/a/7595937/5920982)
    /// </summary>
    class Haversine
    {
        /// <summary>
        /// Radius of the earth in km
        /// </summary>
        const double _eQuatorialEarthRadius = 6378.1370D;

        /// <summary>
        /// 1 degree
        /// </summary>
        const double _d2r = (Math.PI / 180D);

        /// <summary>
        /// Calculates the distance between two latitude-longitude points in meters
        /// </summary>
        /// <param name="lat1">Latitude of the first point</param>
        /// <param name="long1">Longitude of the first point</param>
        /// <param name="lat2">Latitude of the second point</param>
        /// <param name="long2">Longitude of the second point</param>
        /// <returns>Distance between the two points in meters</returns>
        public static int HaversineInM(double lat1, double long1, double lat2, double long2)
        {
            return (int)(1000D * HaversineInKM(lat1, long1, lat2, long2));
        }

        /// <summary>
        /// Calculates the distance between two latitude-longitude points in kilometers
        /// </summary>
        /// <param name="lat1">Latitude of the first point</param>
        /// <param name="long1">Longitude of the first point</param>
        /// <param name="lat2">Latitude of the second point</param>
        /// <param name="long2">Longitude of the second point</param>
        /// <returns>Distance between the two points in kilometers</returns>
        public static double HaversineInKM(double lat1, double long1, double lat2, double long2)
        {
            double dlong = (long2 - long1) * _d2r;
            double dlat = (lat2 - lat1) * _d2r;
            double a = Math.Pow(Math.Sin(dlat / 2D), 2D) + Math.Cos(lat1 * _d2r) * Math.Cos(lat2 * _d2r) * Math.Pow(Math.Sin(dlong / 2D), 2D);
            double c = 2D * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1D - a));
            double d = _eQuatorialEarthRadius * c;

            return d;
        }

    }
}