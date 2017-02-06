using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace RaceTrack.LiveScreens
{
    /// <author>David Fraas</author>
    /// <summary>
    /// Displays the time for the current race
    /// </summary>
    public sealed partial class StopWatch : Page
    {
        public StopWatch()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Updates the stopwatch view
        /// </summary>
        /// <param name="time">Time to display (in seconds)</param>
        public void UpdateTime(int time)
        {
            var hours = time / 60 / 60;
            var minutes = (time / 60) % 60;
            var seconds = time % 60;

            TimeText.Text = String.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }

        /// <summary>
        /// Invoked when the page is tapped. Ends the race
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Tapped Routed Event Args</param>
        private void Page_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // LiveRace.EndRace();
            ((MainScreens.LiveRace)((Grid)((Frame)((Menu.DuringRace)((Grid)((Frame)Parent).Parent).Parent).Parent).Parent).Parent).EndRace();
        }
    }
}