using System;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using Windows.UI.Xaml.Controls;

namespace RaceTrack
{
    /// <author>David Fraas</author>
    /// <author>Felix Brandt</author>
    /// <summary>
    /// Controls a single race
    /// Contains logic for Geolocator, Accelerometer etc.
    /// </summary>
    class RaceController
    {
        /// <summary>
        /// Accuracy of the Geolocator (in meters)
        /// </summary>
        private const uint LOCATOR_ACCURACY_METERS = 25;

        /// <summary>
        /// Update rate of the Geolocator (in milliseconds)
        /// </summary>
        private const uint LOCATOR_UPDATE_RATE = 100;

        /// <summary>
        /// Update rate of the Accelerometer
        /// </summary>
        private const uint ACCELEROMETER_UPDATE_RATE = 100;

        // Attributes
        private Race MyRace;
        private Geolocator Locator;
        private Accelerometer MyAccelerometer;

        /* Current Race information */
        private double currentSpeed = 0;
        private double currentLongitude = 0;
        private double currentLatitude = 0;
        private double currentTilt = 0;
        private double currentGForce = 0;

        /* Current Race maximums */
        private double maxSpeed = 0;
        private double maxTilt = 0;
        private double maxGForce = 0;

        /* Live Screen where data has to be displayed */
        private MainScreens.LiveRace LiveRaceScreen;

        /// <summary>
        /// Creates a new RaceController
        /// </summary>
        /// <param name="raceName">Name of the Race</param>
        /// <param name="lrs">LiveRace screen where data has to be displayed</param>
        public RaceController(string raceName, MainScreens.LiveRace lrs)
        {
            // Create new Race
            MyRace = new Race();
            MyRace.Name = raceName;
            LiveRaceScreen = lrs;

            // Initiate Geolocator
            Locator = new Geolocator();
            if (Locator != null)
            {
                Locator.PositionChanged += PositionChanged;
                Locator.DesiredAccuracyInMeters = LOCATOR_ACCURACY_METERS;
                Locator.ReportInterval = LOCATOR_UPDATE_RATE;
            }

            // Initiate Accelerometer
            MyAccelerometer = Accelerometer.GetDefault();
            if (MyAccelerometer != null)
            {
                MyAccelerometer.ReportInterval = MyAccelerometer.MinimumReportInterval;
                MyAccelerometer.ReadingChanged += ForceChanged;
            }

        }

        /// <summary>
        /// Starts a new lap
        /// </summary>
        internal void NewRound()
        {
            MyRace.NewRound();
        }

        /// <summary>
        /// Invoked when the position of the devive changes.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Position Changed Event Arguments</param>
        private void PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            var oldLongitude = currentLongitude;
            var oldLatitude = currentLatitude;

            currentLongitude = args.Position.Coordinate.Longitude;
            currentLatitude = args.Position.Coordinate.Latitude;
            ChangeSpeed((double)args.Position.Coordinate.Speed);

            // Update Race data only if position really changed
            // (Don't update when height / heading etc. changes)
            if (currentLongitude != oldLongitude || currentLatitude != oldLatitude)
            {
                MyRace.Update(currentLongitude, currentLatitude, currentSpeed, currentGForce, currentTilt);
                // Update view
                LiveRaceScreen.UpdateView();
                // Other method because we need the heading too but dont really want to save that
                LiveRaceScreen.UpdateMap(args);
            }
        }

        /// <summary>
        /// Invoked when the reading of the accelerometer changes.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Accelerometer Reading Changed Event Arguments</param>
        private void ForceChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            ChangeGForce(args.Reading);
            ChangeTilt(args.Reading);
            // Update view
            LiveRaceScreen.UpdateView();
        }

        /// <summary>
        /// Saves the current speed in m/s
        /// </summary>
        /// <param name="speed">Current speed in m/s</param>
        private void ChangeSpeed(double speed)
        {
            currentSpeed = speed;

            // New max speed?
            if (currentSpeed > maxSpeed)
            {
                maxSpeed = currentSpeed;
            }
        }

        /// <summary>
        /// Saves the current tilt
        /// </summary>
        /// <param name="reading">Accelerometer Reading</param>
        private void ChangeTilt(AccelerometerReading reading)
        {
            // Calculate Tilt
            var tilt = Math.Abs(reading.AccelerationX) * 90;

            currentTilt = tilt;

            // New max tilt?
            if (currentTilt > maxTilt)
            {
                maxTilt = currentTilt;
            }
        }

        /// <summary>
        /// Saves the current G-Force
        /// </summary>
        /// <param name="reading">Accelerometer Reading</param>
        private void ChangeGForce(AccelerometerReading reading)
        {
            // Pythagoras
            var gForce = Math.Sqrt(
                reading.AccelerationX * reading.AccelerationX +
                reading.AccelerationY * reading.AccelerationY +
                reading.AccelerationZ * reading.AccelerationZ);

            currentGForce = gForce;

            // New max force?
            if (currentGForce > maxGForce)
            {
                maxGForce = currentGForce;
            }
        }

        /// <summary>
        /// Max G-Force getter
        /// </summary>
        /// <returns>Maximum G-Force in the race</returns>
        public double GetGForceMax()
        {
            return maxGForce;
        }

        /// <summary>
        /// Max speed getter
        /// </summary>
        /// <returns>Maximum speed in the race</returns>
        public double GetSpeedMax()
        {
            return maxSpeed;
        }

        /// <summary>
        /// Max tilt getter
        /// </summary>
        /// <returns>Maximum tilt in the race</returns>
        public double GetTiltMax()
        {
            return maxTilt;
        }

        /// <summary>
        /// Tilt getter
        /// </summary>
        /// <returns>Current Tilt</returns>
        public double GetTilt()
        {
            return currentTilt;
        }

        /// <summary>
        /// Speed getter
        /// </summary>
        /// <returns>Current Speed</returns>
        public double GetSpeed()
        {
            return currentSpeed;
        }

        /// <summary>
        /// G-Force getter
        /// </summary>
        /// <returns>Current G-Force</returns>
        public double GetGForce()
        {
            return currentGForce;
        }

        /// <summary>
        /// Round Time getter
        /// </summary>
        /// <returns>Current round time</returns>
        public int GetRoundTime()
        {
            return MyRace.GetRoundTime();
        }

        /// <summary>
        /// Total time getter
        /// </summary>
        /// <returns>Current race time</returns>
        public int GetTotalTime()
        {
            return MyRace.GetTotalTime();
        }

        /// <summary>
        /// Race Getter
        /// </summary>
        /// <returns>Current Race</returns>
        public Race GetRace()
        {
            return MyRace;
        }

        /// <summary>
        /// Ends the race
        /// </summary>
        public void EndRace()
        {
            // Unbind event handlers
            Locator.PositionChanged -= PositionChanged;
            MyAccelerometer.ReadingChanged -= ForceChanged;

            MyRace.EndRace();

            // MainPage.RefreshRaces();
            ((MainPage)((Grid)((Pivot)((PivotItem)((Frame)LiveRaceScreen.Parent).Parent).Parent).Parent).Parent).RefreshRaces();
        }
    }
}