using Windows.UI.Xaml.Controls;
using Windows.Storage;
using System;

namespace RaceTrack.MainScreens
{
    /// <author>Felix Brandt</author>
    /// <summary>
    /// Page for changing various settings
    /// </summary>
    public sealed partial class Settings : Page
    {
        ApplicationDataContainer roamingSettings;

        /// <summary>
        /// MainPage for calling RefreshRaces()
        /// </summary>
        internal MainPage mainPage;

        /// <summary>
        /// Loads the current settings
        /// Sets default settings if no settings are present yet
        /// </summary>
        public Settings()
        {
            this.InitializeComponent();

            roamingSettings = ApplicationData.Current.RoamingSettings;

            /* Load unit selection from settings. Set to metric if no settings are there yet */
            if (roamingSettings.Values["unit"] != null)
            {
                if ((roamingSettings.Values["unit"]).ToString() == "metric")
                {
                    UnitSelection.SelectedIndex = 0; // metric
                } else
                {
                    UnitSelection.SelectedIndex = 1; // imperial
                }
            }
            else
            {
                // No settings entry yet
                roamingSettings.Values["unit"] = "metric";
                UnitSelection.SelectedIndex = 0; // metric
            }

            /* Load speed hud settings. Set to true if no settings are there yet */
            if (roamingSettings.Values["hudSpeed"] != null)
            {
                if ((bool)(roamingSettings.Values["hudSpeed"]))
                {
                    SpeedSwitch.IsOn = true;
                } else
                {
                    SpeedSwitch.IsOn = false;
                }
            } 
            else
            {
                // No settings entry yet
                roamingSettings.Values["hudSpeed"] = true;
                SpeedSwitch.IsOn = true;
            }

            /* Load round time hud settings. Set to true if no settings are there yet */
            if (roamingSettings.Values["hudRoundTime"] != null)
            {
                if ((bool)(roamingSettings.Values["hudRoundTime"]))
                {
                    RoundTimeSwitch.IsOn = true;
                }
                else
                {
                    RoundTimeSwitch.IsOn = false;
                }
            }
            else
            {
                // No settings entry yet
                roamingSettings.Values["hudRoundTime"] = true;
                RoundTimeSwitch.IsOn = true;
            }

            /* Load total time hud settings. Set to true if no settings are there yet */
            if (roamingSettings.Values["hudTotalTime"] != null)
            {
                if ((bool)(roamingSettings.Values["hudTotalTime"]))
                {
                    TotalTimeSwitch.IsOn = true;
                }
                else
                {
                    TotalTimeSwitch.IsOn = false;
                }
            }
            else
            {
                // No settings entry yet
                roamingSettings.Values["hudTotalTime"] = true;
                TotalTimeSwitch.IsOn = true;
            }

            /* Load map hud settings. Set to true if no settings are there yet */
            if (roamingSettings.Values["hudMap"] != null)
            {
                if ((bool)(roamingSettings.Values["hudMap"]))
                {
                    MapSwitch.IsOn = true;
                }
                else
                {
                    MapSwitch.IsOn = false;
                }
            }
            else
            {
                // No settings entry yet
                roamingSettings.Values["hudMap"] = true;
                MapSwitch.IsOn = true;
            }

            /* Load g force hud settings. Set to true if no settings are there yet */
            if (roamingSettings.Values["hudGForce"] != null)
            {
                if ((bool)(roamingSettings.Values["hudGForce"]))
                {
                    GForceSwitch.IsOn = true;
                }
                else
                {
                    GForceSwitch.IsOn = false;
                }
            }
            else
            {
                // No settings entry yet
                roamingSettings.Values["hudGForce"] = true;
                GForceSwitch.IsOn = true;
            }

            /* Load tilt hud settings. Set to true if no settings are there yet */
            if (roamingSettings.Values["hudTilt"] != null)
            {
                if ((bool)(roamingSettings.Values["hudTilt"]))
                {
                    TiltSwitch.IsOn = true;
                }
                else
                {
                    TiltSwitch.IsOn = false;
                }
            }
            else
            {
                // No settings entry yet
                roamingSettings.Values["hudTilt"] = true;
                TiltSwitch.IsOn = true;
            }
        }

        /// <summary>
        /// Called upon measurement unit change
        /// Changes the unit and refreshes all screens where units are displayed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Selection Changed Event Arguments</param>
        private void unitSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBoxItem)e.AddedItems[0]).Content.ToString() == "Imperial")
            {
                roamingSettings.Values["unit"] = "imperial";
            } else
            {
                roamingSettings.Values["unit"] = "metric";
            }

            if (mainPage != null)
            {
                mainPage.RefreshRaces();
            }

        }

        /// <summary>
        /// Called upon switch switching
        /// Switches the selected setting
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Routed Event Args</param>
        private void Switch_Toggled(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ToggleSwitch senderSwitch = (ToggleSwitch)sender;
            string valueKey;
            switch(senderSwitch.Name)
            {
                case "SpeedSwitch":
                    valueKey = "hudSpeed";
                    break;
                case "RoundTimeSwitch":
                    valueKey = "hudRoundTime";
                    break;
                case "TotalTimeSwitch":
                    valueKey = "hudTotalTime";
                    break;
                case "MapSwitch":
                    valueKey = "hudMap";
                    break;
                case "GForceSwitch":
                    valueKey = "hudGForce";
                    break;
                case "TiltSwitch":
                    valueKey = "hudTilt";
                    break;
                default: throw new System.Exception("Whoops.."); // Can't happen!
            }
            roamingSettings.Values[valueKey] = senderSwitch.IsOn;
        }
    }
}