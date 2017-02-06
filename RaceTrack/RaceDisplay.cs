namespace RaceTrack
{
    /// <author>Felix Brandt</author>
    /// <summary>
    /// Data class for binding Race data to the view
    /// </summary>
    class RaceDisplay
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string AverageSpeed { get; set; }
        public string TotalDistance { get; set; }
        public string Name { get; set; }
        public string RoundCount { get; set; }
    }
}