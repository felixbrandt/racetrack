using System;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace RaceTrack.LiveScreens
{
    /// <author>David Fraas</author>
    /// <summary>
    /// Speed frame for displaying the current speed in mph or km/h
    /// </summary>
    public sealed partial class Speed : Page
    {
        ApplicationDataContainer roamingSettings;

        public Speed()
        {
            this.InitializeComponent();

            roamingSettings = ApplicationData.Current.RoamingSettings;
        }

        /// <summary>
        /// Update the current speed view. Output changes based on SPEED_UNIT
        /// </summary>
        /// <param name="currentSpeed">Current speed in m/s</param>
        /// <param name="maxSpeed">Maximum speed in this race in m/s</param>
        public void UpdateSpeed(double currentSpeed, double maxSpeed)
        {
            // Speed with an actually readable unit
            var realSpeed = 0D;
            string unit;

            if ((roamingSettings.Values["unit"]).ToString() == "metric")
            {
                // 1 km/h = 3.6 m/s
                realSpeed = currentSpeed * 3.6;
                maxSpeed = maxSpeed * 3.6;
                unit = "km/h";
            }
            else
            {
                // 1 mph ~= 2.23694 m/s
                realSpeed = currentSpeed * 2.23694;
                maxSpeed = maxSpeed * 2.23694;
                unit = "mph";
            }

            SpeedText.Text = String.Format("{0:0} {1}", realSpeed, unit);
            MaxSpeed.Text = String.Format("[{0:0}]", maxSpeed);            
        }
    }
}