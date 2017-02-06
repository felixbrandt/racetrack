using System;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

namespace RaceTrack.LiveScreens
{
    /// <author>David Fraas</author>
    /// <summary>
    /// Map frame for displaying the current position
    /// </summary>
    public sealed partial class Map : Page
    {
        /// <summary>
        /// Map zoom level
        /// </summary>
        private double MAP_ZOOM_LEVEL = 15D;

        public Map()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the gps position of the device changes
        /// Centers the map view to the current position
        /// </summary>
        /// <param name="args">Position changed event arguments</param>
        public async void UpdatePosition(PositionChangedEventArgs args)
        {
            await MyMap.TrySetViewAsync(args.Position.Coordinate.Point, MAP_ZOOM_LEVEL, args.Position.Coordinate.Heading, 0, MapAnimationKind.None);
        }
    }
}