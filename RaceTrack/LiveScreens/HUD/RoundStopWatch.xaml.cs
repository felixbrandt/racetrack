using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace RaceTrack.LiveScreens
{
    /// <author>David Fraas</author>
    /// <summary>
    /// Displays the time for the current lap
    /// </summary>
    public sealed partial class RoundStopWatch : Page
    {
        public RoundStopWatch()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Updates the lap stopwatch view
        /// </summary>
        /// <param name="time">Time to display (in seconds)</param>
        public void UpdateTime(int time)
        {
            var hours = time / 60 / 60;
            var minutes = (time / 60) % 60;
            var seconds = time % 60;

            RoundTimeText.Text = String.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }

        /// <summary>
        /// Invoked when the page is tapped. Starts a new lap
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Tapped Routed Event Args</param>
        private void Page_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // LiveRace.NewRound()
            ((MainScreens.LiveRace)((Grid)((Frame)((Menu.DuringRace)((Grid)((Frame)Parent).Parent).Parent).Parent).Parent).Parent).NewRound();
        }
    }
}