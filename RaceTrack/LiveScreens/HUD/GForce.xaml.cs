using System;
using Windows.UI.Xaml.Controls;

namespace RaceTrack.LiveScreens
{
    /// <author>David Fraas</author>
    /// <summary>
    /// Displays the current G-Force and the maximum G-Force
    /// </summary
    public sealed partial class GForce : Page
    {
        public GForce()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Updates the text display of the view
        /// </summary>
        /// <param name="currentForce">Current G-Force</param>
        /// <param name="maxForce">Maximum G-Force during this race</param>
        public void UpdateForce(double currentForce, double maxForce)
        {
            GForceText.Text = String.Format("{0:0.0}", currentForce);
            MaxGForce.Text = String.Format("[{0:0.00}]", maxForce);
        }
    }
}