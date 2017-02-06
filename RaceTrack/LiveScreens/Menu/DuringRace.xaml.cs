using System.Collections.ObjectModel;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace RaceTrack.LiveScreens.Menu
{
    /// <author>Felix Brandt</author>
    /// <summary>
    /// Live race HUD page
    /// </summary>
    public sealed partial class DuringRace : Page
    {
        // Frames for displaying live information
        Frame MapFrame;
        Frame SpeedFrame;
        Frame GForceFrame;
        Frame TimeFrame;
        Frame RoundTimeFrame;
        Frame TiltFrame;

        // Roaming Settings
        ApplicationDataContainer roamingSettings;

        /// <summary>
        /// Initializes the page, loads HUD tiles from settings
        /// </summary>
        public DuringRace()
        {
            this.InitializeComponent();

            /* Load HUD tiles based on settings */
            roamingSettings = ApplicationData.Current.RoamingSettings;

            // All displayed frames
            var frames = new ObservableCollection<Frame>();

            if ((bool)(roamingSettings.Values["hudMap"]))
            {
                MapFrame = new Frame();
                MapFrame.Navigate(typeof(Map));
                frames.Add(MapFrame);
            }

            if ((bool)(roamingSettings.Values["hudTotalTime"]))
            {
                TimeFrame = new Frame();
                TimeFrame.Navigate(typeof(StopWatch));
                frames.Add(TimeFrame);
            }

            if ((bool)(roamingSettings.Values["hudSpeed"]))
            {
                SpeedFrame = new Frame();
                SpeedFrame.Navigate(typeof(Speed));
                frames.Add(SpeedFrame);
            }

            if ((bool)(roamingSettings.Values["hudRoundTime"]))
            {
                RoundTimeFrame = new Frame();
                RoundTimeFrame.Navigate(typeof(RoundStopWatch));
                frames.Add(RoundTimeFrame);
            }

            if ((bool)(roamingSettings.Values["hudGForce"]))
            {
                GForceFrame = new Frame();
                GForceFrame.Navigate(typeof(GForce));
                frames.Add(GForceFrame);
            }

            if ((bool)(roamingSettings.Values["hudTilt"]))
            {
                TiltFrame = new Frame();
                TiltFrame.Navigate(typeof(Tilt));
                frames.Add(TiltFrame);
            }

            // Row count (without bottom elements) for creating RowDefinitions
            int rowCount = (frames.Count+1)/2;

            for(var i=0; i<rowCount; i++)
            {
                ContentGrid.RowDefinitions.Add(new RowDefinition());
            }

            // Always use 2 columns
            ContentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            ContentGrid.ColumnDefinitions.Add(new ColumnDefinition());

            // Add Frames to Page
            int row = 0;
            int col = 0;

            for (var i=0; i<frames.Count; i++)
            {
                // Add frame to Grid
                ContentGrid.Children.Add(frames[i]);
                frames[i].SetValue(Grid.ColumnProperty, col);
                frames[i].SetValue(Grid.RowProperty, row);

                // If odd and first => Make frame wider
                if (i == 0 && frames.Count % 2 == 1)
                {
                    frames[i].SetValue(Grid.ColumnSpanProperty, 2);
                    col++;
                }

                col++;

                // TODO: Make this a for loop
                if (col == 2)
                {
                    col = 0;
                    row = row + 1;
                }
            }

            // Add Control row at the bottom
            ContentGrid.RowDefinitions.Add(new RowDefinition()
            {
                Height = new Windows.UI.Xaml.GridLength(0, Windows.UI.Xaml.GridUnitType.Auto)
            });

            // Add Control StackPanel
            var bottomControls = new StackPanel()
            {
                Margin = new Windows.UI.Xaml.Thickness(0, 0, 0, 8),
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right,
                Orientation = Orientation.Horizontal,
                Height = double.NaN // Height = Auto
            };

            // Create icon for starting a new round
            var newRoundIcon = new FontIcon()
            {
                Height = 32,
                Glyph = "\uE7C1",
                Foreground = new SolidColorBrush(Colors.Red)
            };
            newRoundIcon.Tapped += NewRoundTapped;

            // Create icon for ending the race
            var saveIcon = new FontIcon()
            {
              Height = 32,
              Glyph = "\uE74E",
              Margin = new Windows.UI.Xaml.Thickness(24,0,16,0),
            };
            saveIcon.Tapped += EndRaceTapped;

            // Add icons to StackPanel
            bottomControls.Children.Add(newRoundIcon);
            bottomControls.Children.Add(saveIcon);

            // Place StackPanel in the Bottom right grid place
            bottomControls.SetValue(Grid.RowProperty, rowCount + 1);
            bottomControls.SetValue(Grid.ColumnProperty, 1);

            // Add StackPanel to Grid
            ContentGrid.Children.Add(bottomControls);
        }

        /// <summary>
        /// Updates the maps position display
        /// </summary>
        /// <param name="args">Position Changed Event Arguments</param>
        internal void UpdateMap(PositionChangedEventArgs args)
        {
            if (MapFrame != null)
            {
                ((Map)MapFrame.Content).UpdatePosition(args);
            }
        }

        /// <summary>
        /// Updates the displayed speed
        /// </summary>
        /// <param name="current">Current speed in m/s</param>
        /// <param name="max">Maximum speed in m/s</param>
        internal void UpdateSpeed(double current, double max)
        {
            if (SpeedFrame != null)
            {
                ((Speed)SpeedFrame.Content).UpdateSpeed(current, max);
            }
        }

        /// <summary>
        /// Updates the displayed G-Force
        /// </summary>
        /// <param name="current">Current G-Force</param>
        /// <param name="max">Maximum G-Force</param>
        internal void UpdateGForce(double current, double max)
        {
            if (GForceFrame != null)
            {
                ((GForce)GForceFrame.Content).UpdateForce(current, max);
            }
        }

        /// <summary>
        /// Updates the displayed total time
        /// </summary>
        /// <param name="time">Total time in seconds</param>
        internal void UpdateTotalTime(int time)
        {
            if (TimeFrame != null)
            {
                ((StopWatch)TimeFrame.Content).UpdateTime(time);
            }
        }

        /// <summary>
        /// Updates the displayed lap time
        /// </summary>
        /// <param name="time">Lap time in seconds</param>
        internal void UpdateRoundTime(int time)
        {
            if (RoundTimeFrame != null)
            {
                ((RoundStopWatch)RoundTimeFrame.Content).UpdateTime(time);
            }
        }

        /// <summary>
        /// Updates the displayed tilt
        /// </summary>
        /// <param name="current">Current tilt in degrees</param>
        /// <param name="max">Maximum tilt in degrees</param>
        internal void UpdateTilt(double current, double max)
        {
            if (TiltFrame != null)
            {
                ((Tilt)TiltFrame.Content).UpdateTilt(current, max);
            }
        }

        private void NewRoundTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            // Start new Round
            ((MainScreens.LiveRace)(((Grid)((Frame)Parent).Parent).Parent)).NewRound();
        }

        private void EndRaceTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            // End race
            ((MainScreens.LiveRace)(((Grid)((Frame)Parent).Parent).Parent)).EndRace();
        }
    }
}