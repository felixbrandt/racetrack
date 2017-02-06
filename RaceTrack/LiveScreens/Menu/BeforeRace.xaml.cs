using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace RaceTrack.LiveScreens.Menu
{
    /// <author>Manuel Schreiber</author>
    /// <summary>
    /// Displayed right before the race for asking the user for information about the race
    /// </summary>
    public sealed partial class BeforeRace : Page
    {
        public BeforeRace()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Submit button is tapped
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Tapped Routed Event Arguments</param>
        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (NameInput.Text.Length < 3)
            {
                Errors.Text = "Mindestens 3 Buchstaben!";
            } else
            {
                Errors.Text = String.Empty;
                // LiveRace.NewRace()
                ((MainScreens.LiveRace)((Grid)((Frame)Parent).Parent).Parent).NewRace(NameInput.Text);
            }
        }
    }
}