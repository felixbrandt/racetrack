using System;
using Windows.UI.Xaml.Controls;

namespace RaceTrack.LiveScreens
{
    /// <author>David Fraas</author>
    /// <summary>
    /// Sideways tilt display
    /// </summary>
    public sealed partial class Tilt : Page
    {
        public Tilt()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Updates the tilt view text
        /// </summary>
        /// <param name="currentTilt">Current tilt in degrees</param>
        /// <param name="maxTilt">Maximum tilt over the whole race in degrees</param>
        public void UpdateTilt(double currentTilt, double maxTilt)
        {
            TiltText.Text = String.Format("{0:0.0}°", currentTilt);
            MaxTilt.Text = String.Format("[{0:0..00}°]", maxTilt);
        }
    }
}