using Windows.UI.Xaml.Controls;

namespace RaceTrack
{
    /// <author>Felix Brandt</author>
    /// <summary>
    /// This page is opened when the app starts
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            // Display correct screens in the PivotItems
            // Settings first to set units on first start
            SettingsFrame.Navigate(typeof(MainScreens.Settings));
            ((MainScreens.Settings)SettingsFrame.Content).mainPage = this;

            HomeFrame.Navigate(typeof(MainScreens.Home));
            LiveRaceFrame.Navigate(typeof(MainScreens.LiveRace));
            HistoryFrame.Navigate(typeof(MainScreens.History));
        }

        /// <summary>
        /// Reloads the pages after data gets added
        /// </summary>
        internal void RefreshRaces()
        {
            HomeFrame.Navigate(typeof(MainScreens.Home));
            HistoryFrame.Navigate(typeof(MainScreens.History));
        }
    }
}