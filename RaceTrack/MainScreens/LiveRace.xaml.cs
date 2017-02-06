using System;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls;

namespace RaceTrack.MainScreens
{
    /// <author>Felix Brandt</author>
    /// <summary>
    /// LiveRace pivot item. Displays different things based on the current racing status.
    /// i)   No race started yet
    /// ii)  During race
    /// iii) After race
    /// </summary>
    public sealed partial class LiveRace : Page
    {
        /// <summary>
        /// Is there an active race?
        /// </summary>
        private bool isRacing;

        /// <summary>
        /// Currently active RaceController
        /// </summary>
        private RaceController RaceController;

        /// <summary>
        /// Initialize page with BeforeRace page
        /// </summary>
        public LiveRace()
        {
            this.InitializeComponent();
            isRacing = false;
            ContentFrame.Navigate(typeof(LiveScreens.Menu.BeforeRace));
        }

        /// <summary>
        /// Starts a new Race and switches to the HUD display
        /// </summary>
        /// <param name="name">Name of the Race</param>
        /// <returns></returns>
        internal RaceController NewRace(string name)
        {
            if (!isRacing)
            {
                // Switch to HUD screen
                ContentFrame.Navigate(typeof(LiveScreens.Menu.DuringRace));

                isRacing = true;

                // New RaceController for this Race
                RaceController = new RaceController(name, this);
                return RaceController;
            }
            else
            {
                // Should not be able to get here
                throw new MethodAccessException("Can't start race while race is still active.");
            }
        }

        /// <summary>
        /// Starts a new lap
        /// </summary>
        internal void NewRound()
        {
            if (isRacing)
            {
                RaceController.NewRound();
            }
        }

        /// <summary>
        /// Start a new race after a race has been finished
        /// </summary>
        internal void startNewRaceAgain()
        {
            ContentFrame.Navigate(typeof(LiveScreens.Menu.BeforeRace));
        }

        /// <summary>
        /// Ends the race if there is an active one
        /// </summary>
        internal void EndRace()
        {
            if (isRacing)
            {
                isRacing = false;

                // End the race in the controller
                RaceController.EndRace();

                // Switch to AfterRace screen
                ContentFrame.Navigate(typeof(LiveScreens.Menu.AfterRace));

                // Display info about the ended race
                ((LiveScreens.Menu.AfterRace)ContentFrame.Content).DisplayInfo(RaceController.GetRace());
            }
            else
            {
                // Should not be able to get here
                throw new MethodAccessException("Can't end race while race is not active.");
            }
        }

        /// <summary>
        /// Update the HUD tiles if a race is active
        /// </summary>
        public async void UpdateView()
        {
            if(isRacing)
            {
                try
                {
                    // Set GForce display
                    var currentForce = RaceController.GetGForce();
                    var maxForce = RaceController.GetGForceMax();
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => ((LiveScreens.Menu.DuringRace)ContentFrame.Content).UpdateGForce(currentForce, maxForce));

                    // Set Tilt display
                    var currentTilt = RaceController.GetTilt();
                    var maxTilt = RaceController.GetTiltMax();
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => ((LiveScreens.Menu.DuringRace)ContentFrame.Content).UpdateTilt(currentTilt, maxTilt));

                    // Set Speed display
                    var currentSpeed = RaceController.GetSpeed();
                    var maxSpeed = RaceController.GetSpeedMax();
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => ((LiveScreens.Menu.DuringRace)ContentFrame.Content).UpdateSpeed(currentSpeed, maxSpeed));

                    // Set round time display
                    var roundTime = RaceController.GetRoundTime();
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => ((LiveScreens.Menu.DuringRace)ContentFrame.Content).UpdateRoundTime(roundTime));

                    // Set total time display
                    var totalTime = RaceController.GetTotalTime();
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => ((LiveScreens.Menu.DuringRace)ContentFrame.Content).UpdateTotalTime(totalTime));

                    // Set map center
                    // Doing this in UpdateMap to get PositionChangedEventArgs
                }
                catch (InvalidCastException e)
                {
                    // TODO: add something here
                }
            }
        }

        /// <summary>
        /// Updates the center of the map on the screen if racing
        /// </summary>
        /// <param name="args">Position Changed Event Arguments</param>
        public async void UpdateMap(PositionChangedEventArgs args)
        {
            if (isRacing)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => ((LiveScreens.Menu.DuringRace)ContentFrame.Content).UpdateMap(args));
            }
        }
    }
}