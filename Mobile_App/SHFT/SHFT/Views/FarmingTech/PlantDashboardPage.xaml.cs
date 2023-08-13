// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// This class represents the Plant Dashboard Page in the SHFT application.

namespace SHFT.Views;

using SHFT.Controllers;
using SHFT.Models;
using SHFT.Repos;
using SHFT.Services;
using System.Reflection;

public partial class PlantDashboardPage : ContentPage
{
    private const float DEFAULT_FLOAT = 0.0f;
    private const string TOO_HIGH = "Too High!";
    private const string TOO_LOW = "Too Low!";
    private const string MOISTURE_OK = "check_mark.png";
    private const string MOISTURE_BAD = "x_mark.png";

    PlantSubsystem _subsystem;
    bool _ignoreNextLightSwitch = false;
    bool _ignoreNextFanSwitch = false;
    bool _ignoreNextLockSwitch = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlantDashboardPage"/> class.
    /// </summary>
    public PlantDashboardPage()
    {
        InitializeComponent();
        SetGauges();
        // Sets the binding context of the page to the plant controller subsystem
        plantGauges.BindingContext = PlantController.GetInstance().Subsystem;
        _subsystem = PlantController.GetInstance().Subsystem;
        ConnectionManager.GetInstance().Connect(App.connectionsApp.DeviceId);
        PlantController.GetInstance().PropertyUpdated += SyncWarningsToProperties;
    }

    /// <summary>
    /// Sets the error messages for properties which are past their thresholds.
    /// </summary>
    private void SyncWarningsToProperties()
    {
        _subsystem = PlantController.GetInstance().Subsystem;
        // Temperature
        if (_subsystem.Temperature.Value < _subsystem.MinimumTemperature)
        {
            TemperatureErrorLabel.Text = TOO_LOW;
        }
        else if (_subsystem.Temperature.Value > _subsystem.MaximumTemperature)
        {
            TemperatureErrorLabel.Text = TOO_HIGH;
        }
        else
        {
            TemperatureErrorLabel.Text = string.Empty;
        }

        // Humidity
        if (_subsystem.Humidity.Value < _subsystem.MinimumHumidity)
        {
            HumidityErrorLabel.Text = TOO_LOW;
        }
        else if (_subsystem.Humidity.Value > _subsystem.MaximumHumidity)
        {
            HumidityErrorLabel.Text = TOO_HIGH;
        }
        else
        {
            HumidityErrorLabel.Text = string.Empty;
        }

        // Water level
        if (_subsystem.WaterLevel.Value < _subsystem.MinimumWaterLevel)
        {
            WaterLevelErrorLabel.Text = TOO_LOW;
        }
        else if (_subsystem.WaterLevel.Value > _subsystem.MaximumWaterLevel)
        {
            WaterLevelErrorLabel.Text = TOO_HIGH;
        }
        else
        {
            WaterLevelErrorLabel.Text = string.Empty;
        }

        // Soil Moisture
        if (_subsystem.IsSoilMoistureOkay)
        {
            soilMoistureStateImage.Source = MOISTURE_OK;
        }
        else
        {
            soilMoistureStateImage.Source = MOISTURE_BAD;
        }
    }

    /// <summary>
    /// Event handler for the tap gesture recognizer.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    private void SoilMoistureTapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        // Redirect to the plat soil moisture page
        Navigation.PushAsync(new SoilMoisturePage());
    }

    /// <summary>
    /// Event handler for the fan state switch.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    /// <returns>The new state of the fan switch.</returns>
    private async void FanState_Switch_Toggled(object sender, ToggledEventArgs e)
    {
        bool value = e.Value;
        Switch swc = sender as Switch;
        swc.IsEnabled = false;

        if (_ignoreNextFanSwitch)
        {
            _ignoreNextFanSwitch = false;
            return;
        }

        bool success = false;
        if (ConnectedToWifi())
            success = await PlantController.GetInstance().Subsystem.SetFanState(value);
        if (success)
            swc.IsToggled = value;
        else
        {
            _ignoreNextFanSwitch = true;
            swc.IsToggled = !value;
            FailToToggle();
        }
        swc.IsEnabled = true;
    }

    /// <summary>
    /// Event handler for the light state switch.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    /// <returns>The new state of the light switch.</returns>
    private async void LightState_Switch_Toggled(object sender, ToggledEventArgs e)
    {
        bool value = e.Value;
        Switch swc = sender as Switch;
        swc.IsEnabled = false;

        if (_ignoreNextLightSwitch)
        {
            _ignoreNextLightSwitch = false;
            return;
        }

        bool success = false;
        if (ConnectedToWifi())
            success = await PlantController.GetInstance().Subsystem.SetLightState(value);
        if (success)
            swc.IsToggled = value;
        else
        {
            _ignoreNextLightSwitch = true;
            swc.IsToggled = !value;
            FailToToggle();
        }
        swc.IsEnabled = true;
    }

    /// <summary>
    /// Event handler for the door lock switch.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    /// <returns>The new state of the door lock switch.</returns>
    private async void DoorLock_Switch_Toggled(object sender, ToggledEventArgs e)
    {
        bool value = e.Value;
        Switch swc = sender as Switch;
        swc.IsEnabled = false;

        if (_ignoreNextLockSwitch)
        {
            _ignoreNextLockSwitch = false;
            return;
        }

        bool success = false;
        if (ConnectedToWifi())
            success = await PlantController.GetInstance().Subsystem.SetDoorState(value);
        if (success)
            swc.IsToggled = value;
        else
        {
            _ignoreNextLockSwitch = true;
            swc.IsToggled = !value;
            FailToToggle();
        }
        swc.IsEnabled = true;
    }

    private void FailToToggle()
    {
        DisplayAlert("Oops", "The state couldn't be set. This may be because the farm container is not connected, or because you aren't connected to wifi.", "Ok");
    }

    /// <summary>
    /// Sets the threshold values of a given property in the plant subsystem.
    /// </summary>
    /// <param name="property">The property to set thresholds for.</param>
    /// <param name="readingType">The reading type to set thresholds for.</param>
    /// <param name="min">The minimum value threshold.</param>
    /// <param name="max">The maximum value threshold.</param>
    private void SetThreshold(string property, string readingType, float? min = null, float? max = null)
    {
        PlantSubsystem subsystem = PlantController.GetInstance().Subsystem;

        // Set the min value if it exists
        if (min.HasValue)
        {
            PropertyInfo minPropertyInfo = subsystem.GetType().GetProperty($"Minimum{property}");
            minPropertyInfo.SetValue(subsystem, min.Value, null);
            ThresholdRepo.GetInstance().AddMinThreshold(new Reading<object>() { Type = readingType, Value = min.Value });

        }

        // Set the max value if it exists
        if (max.HasValue)
        {
            PropertyInfo maxPropertyInfo = subsystem.GetType().GetProperty($"Maximum{property}");
            maxPropertyInfo.SetValue(subsystem, max.Value, null);
            ThresholdRepo.GetInstance().AddMaxThreshold(new Reading<object>() { Type = readingType, Value = max.Value });

        }

    }

    /// <summary>
    /// Displays the min max popup and sets the selected values as new temperature thresholds.
    /// </summary>
    /// <param name="sender">The frame which was tapped.</param>
    /// <param name="e">The event object containing details about this event.</param>
    private async void temperature_Tapped(object sender, TappedEventArgs e)
    {
        const string PROPERTY = "Temperature";
        const string READING_TYPE = Reading<object>.TypeOptions.TEMPERATURE;
        MinMaxPopup.MinMaxDetails details = await MinMaxPopup.Show(this, PROPERTY, true, true);
        if (details is null)
            return;
        SetThreshold(PROPERTY, READING_TYPE, details.Min, details.Max);
        SetTemperatureGauge();
    }

    /// <summary>
    /// Displays the min max popup and sets the selected values as new humidity thresholds.
    /// </summary>
    /// <param name="sender">The frame which was tapped.</param>
    /// <param name="e">The event object containing details about this event.</param>
    private async void humidity_Tapped(object sender, TappedEventArgs e)
    {
        const string PROPERTY = "Humidity";
        const string READING_TYPE = Reading<object>.TypeOptions.HUMIDITY;
        MinMaxPopup.MinMaxDetails details = await MinMaxPopup.Show(this, PROPERTY, true, true);
        if (details is null)
            return;
        SetThreshold(PROPERTY, READING_TYPE, details.Min, details.Max);
        SetHumidityGauge();
    }

    /// <summary>
    /// Displays the min max popup and sets the selected values as new water level thresholds.
    /// </summary>
    /// <param name="sender">The frame which was tapped.</param>
    /// <param name="e">The event object containing details about this event.</param>
    private async void waterLevel_Tapped(object sender, TappedEventArgs e)
    {
        const string PROPERTY = "WaterLevel";
        const string READING_TYPE = Reading<object>.TypeOptions.WATER_LEVEL;
        MinMaxPopup.MinMaxDetails details = await MinMaxPopup.Show(this, PROPERTY, true, true);
        if (details is null)
            return;
        SetThreshold(PROPERTY, READING_TYPE, details.Min, details.Max);
        SetWaterLevelGauge();
    }

    /// <summary>
    /// Returns whether the app is connected to wifi.
    /// </summary>
    /// <returns>True if the app is connected, false otherwise.</returns>
    private bool ConnectedToWifi()
    {
        NetworkAccess accessType = Connectivity.Current.NetworkAccess;
        return accessType == NetworkAccess.Internet;
    }

    private void SetGauges()
    {
        try
        {
            SetTemperatureGauge();
            SetHumidityGauge();
            SetWaterLevelGauge();
        }
        catch (Exception ex)
        {
            throw new Exception("An error has occured wehn setting you Gauge parameters. Please check the minimum and maximum value you set for each parameter." +
                $"\nHere are more details: {ex.Message}");
        }
        finally
        {
            SyncWarningsToProperties();
        }
    }

    private void SetTemperatureGauge()
    {
        int numberOfGauges = 11;
        int numberOfIntervals = 10;
        PlantSubsystem subsystem = PlantController.GetInstance().Subsystem;

        float maximumTemperature = subsystem.MaximumTemperature;
        float minimumTemperature = subsystem.MinimumTemperature;
        float rangeIncrement = maximumTemperature < minimumTemperature ? (minimumTemperature - maximumTemperature) / numberOfGauges : (maximumTemperature - minimumTemperature) / numberOfGauges;
        float rangeGap = rangeIncrement / 100;

        TemperatureGauge.Minimum = minimumTemperature;
        TemperatureGauge.Maximum = maximumTemperature;
        TemperatureGauge.Interval = Math.Abs(maximumTemperature - minimumTemperature) / numberOfIntervals;

        TempGauge1.StartValue = minimumTemperature;
        TempGauge1.EndValue = minimumTemperature + rangeIncrement - rangeGap;

        TempGauge2.StartValue = TempGauge1.EndValue + rangeGap;
        TempGauge2.EndValue = TempGauge1.EndValue + rangeIncrement;

        TempGauge3.StartValue = TempGauge2.EndValue + rangeGap;
        TempGauge3.EndValue = TempGauge2.EndValue + rangeIncrement;

        TempGauge4.StartValue = TempGauge3.EndValue + rangeGap;
        TempGauge4.EndValue = TempGauge3.EndValue + rangeIncrement;

        TempGauge5.StartValue = TempGauge4.EndValue + rangeGap;
        TempGauge5.EndValue = TempGauge4.EndValue + rangeIncrement;

        TempGauge6.StartValue = TempGauge5.EndValue + rangeGap;
        TempGauge6.EndValue = TempGauge5.EndValue + rangeIncrement;

        TempGauge7.StartValue = TempGauge6.EndValue + rangeGap;
        TempGauge7.EndValue = TempGauge6.EndValue + rangeIncrement;

        TempGauge8.StartValue = TempGauge7.EndValue + rangeGap;
        TempGauge8.EndValue = TempGauge7.EndValue + rangeIncrement;

        TempGauge9.StartValue = TempGauge8.EndValue + rangeGap;
        TempGauge9.EndValue = TempGauge8.EndValue + rangeIncrement;

        TempGauge10.StartValue = TempGauge9.EndValue + rangeGap;
        TempGauge10.EndValue = TempGauge9.EndValue + rangeIncrement;

        TempGauge11.StartValue = TempGauge10.EndValue + rangeGap;
        TempGauge11.EndValue = maximumTemperature;

        SyncWarningsToProperties();
    }

    private void SetHumidityGauge()
    {
        int numberOfGauges = 11;
        int numberOfIntervals = 10;
        float rangeGap = 0.5f;
        PlantSubsystem subsystem = PlantController.GetInstance().Subsystem;

        float maximumHumidity = subsystem.MaximumHumidity;
        float minimumHumidity = subsystem.MinimumHumidity;
        float rangeIncrement = Math.Abs(maximumHumidity - minimumHumidity) / numberOfGauges;

        HumidityGauge.Minimum = minimumHumidity;
        HumidityGauge.Maximum = maximumHumidity;
        HumidityGauge.Interval = maximumHumidity < minimumHumidity ? (minimumHumidity - maximumHumidity) / numberOfIntervals : (maximumHumidity - minimumHumidity) / numberOfIntervals;

        HumiGauge1.StartValue = minimumHumidity;
        HumiGauge1.EndValue = minimumHumidity + rangeIncrement - rangeGap;

        HumiGauge2.StartValue = HumiGauge1.EndValue + rangeGap;
        HumiGauge2.EndValue = HumiGauge1.EndValue + rangeIncrement;

        HumiGauge3.StartValue = HumiGauge2.EndValue + rangeGap;
        HumiGauge3.EndValue = HumiGauge2.EndValue + rangeIncrement;

        HumiGauge4.StartValue = HumiGauge3.EndValue + rangeGap;
        HumiGauge4.EndValue = HumiGauge3.EndValue + rangeIncrement;

        HumiGauge5.StartValue = HumiGauge4.EndValue + rangeGap;
        HumiGauge5.EndValue = HumiGauge4.EndValue + rangeIncrement;

        HumiGauge6.StartValue = HumiGauge5.EndValue + rangeGap;
        HumiGauge6.EndValue = HumiGauge5.EndValue + rangeIncrement;

        HumiGauge7.StartValue = HumiGauge6.EndValue + rangeGap;
        HumiGauge7.EndValue = HumiGauge6.EndValue + rangeIncrement;

        HumiGauge8.StartValue = HumiGauge7.EndValue + rangeGap;
        HumiGauge8.EndValue = HumiGauge7.EndValue + rangeIncrement;

        HumiGauge9.StartValue = HumiGauge8.EndValue + rangeGap;
        HumiGauge9.EndValue = HumiGauge8.EndValue + rangeIncrement;

        HumiGauge10.StartValue = HumiGauge9.EndValue + rangeGap;
        HumiGauge10.EndValue = HumiGauge9.EndValue + rangeIncrement;

        HumiGauge11.StartValue = HumiGauge10.EndValue + rangeGap;
        HumiGauge11.EndValue = maximumHumidity;

        SyncWarningsToProperties();
    }

    private void SetWaterLevelGauge()
    {
        int numberOfGauges = 5;
        int numberOfIntervals = 10;
        float rangeGap = 0.1f;
        PlantSubsystem subsystem = PlantController.GetInstance().Subsystem;

        float maximumWaterLevel = subsystem.MaximumWaterLevel;
        float minimumWaterLevel = subsystem.MinimumWaterLevel;
        float rangeIncrement = Math.Abs(maximumWaterLevel - minimumWaterLevel) / numberOfGauges;

        WaterLevelGauge.Minimum = minimumWaterLevel;
        WaterLevelGauge.Maximum = maximumWaterLevel;
        WaterLevelGauge.Interval = maximumWaterLevel < minimumWaterLevel ? (minimumWaterLevel - maximumWaterLevel) / numberOfIntervals : (maximumWaterLevel - minimumWaterLevel) / numberOfIntervals;

        WaterLevelGaugeExcellent.StartValue = minimumWaterLevel;
        WaterLevelGaugeExcellent.EndValue = rangeIncrement - rangeGap + minimumWaterLevel;

        WaterLevelGaugeGood.StartValue = WaterLevelGaugeExcellent.EndValue + rangeGap;
        WaterLevelGaugeGood.EndValue = rangeIncrement * 2 - rangeGap + minimumWaterLevel;

        WaterLevelGaugeMedium.StartValue = WaterLevelGaugeGood.EndValue + rangeGap;
        WaterLevelGaugeMedium.EndValue = rangeIncrement * 3 - rangeGap + minimumWaterLevel;

        WaterLevelGaugeBad.StartValue = WaterLevelGaugeMedium.EndValue + rangeGap;
        WaterLevelGaugeBad.EndValue = rangeIncrement * 4 - rangeGap + minimumWaterLevel;

        WaterLevelGaugeTerrible.StartValue = WaterLevelGaugeBad.EndValue + rangeGap;
        WaterLevelGaugeTerrible.EndValue = maximumWaterLevel;

        SyncWarningsToProperties();
    }
}