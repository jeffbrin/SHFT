// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// Overall to set's the binding to the security instance which holds the security subsystem.

namespace SHFT.Views;
using SHFT.Controllers;
using SHFT.Models;
using SHFT.Repos;
using System.Reflection;
using System.Reflection.PortableExecutable;

public partial class DashboardSecurity : ContentPage
{
    private const float DEFAULT_MAXIMUM_NOISE = 60;
    private const float DEFAULT_MINIMUM_NOISE = 0;
    private const float DEFAULT_MAXIMUM_LUMINOSITY = 100;
    private const float DEFAULT_MINIMUM_LUMINOSITY = 40;
    private const float DEFAULT_MAXIMUM_VIBRATION = 50;
    private const float DEFAULT_MINIMUM_VIBRATION = -2;
    private const float DEFAULT_MAXIMUM_ROLL = 100;
    private const float DEFAULT_MINIMUM_ROLL = -100;
    private const float DEFAULT_MAXIMUM_PITCH = 100;
    private const float DEFAULT_MINIMUM_PITCH = -100;
    private const float DEFAULT_FLOAT = 0.0f;
    private const string TOO_HIGH = "Too High!";
    private const string TOO_LOW = "Too Low!";

    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardSecurity"/> class.
    /// </summary>
    public DashboardSecurity()
    {
        InitializeComponent();
        SecurityGauges.BindingContext = SecurityController.GetInstance().Subsystem;
        GeoGauges.BindingContext = GeoLocationController.GetInstance().Subsystem;
        try
        {
            SetGauges();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
        SecurityController.GetInstance().PropertyUpdated += SyncWarningsToProperties;
        GeoLocationController.GetInstance().PropertyUpdated += SyncWarningsToProperties;
    }

    /// <summary>
    /// Sets the error messages for properties which are past their thresholds.
    /// </summary>
    private void SyncWarningsToProperties()
    {
        SecuritySubsystem securitySubsystem = SecurityController.GetInstance().Subsystem;
        GeoLocationSubsystem geoSubsystem = GeoLocationController.GetInstance().Subsystem;


        // Noise
        if (securitySubsystem.Noise.Value < securitySubsystem.MinimumNoise)
        {
            NoiseErrorLabel.Text = TOO_LOW;
        }
        else if (securitySubsystem.Noise.Value > securitySubsystem.MaximumNoise)
        {
            NoiseErrorLabel.Text = TOO_HIGH;
        }
        else
        {
            NoiseErrorLabel.Text = string.Empty;
        }

        // Luminosity
        if (securitySubsystem.Luminosity.Value < securitySubsystem.MinimumLuminosity)
        {
            LuminosityErrorLabel.Text = TOO_LOW;
        }
        else if (securitySubsystem.Luminosity.Value > securitySubsystem.MaximumLuminosity)
        {
            LuminosityErrorLabel.Text = TOO_HIGH;
        }
        else
        {
            LuminosityErrorLabel.Text = string.Empty;
        }

        // Vibration
        if (securitySubsystem.Vibration.Value < securitySubsystem.MinimumVibration)
        {
            VibrationErrorLabel.Text = TOO_LOW;
        }
        else if (securitySubsystem.Vibration.Value > securitySubsystem.MaximumVibration)
        {
            VibrationErrorLabel.Text = TOO_HIGH;
        }
        else
        {
            VibrationErrorLabel.Text = string.Empty;
        }

        // Pitch
        if (geoSubsystem.Pitch.Value < geoSubsystem.MinimumPitch)
        {
            PitchErrorLabel.Text = TOO_LOW;
        }
        else if (geoSubsystem.Pitch.Value > geoSubsystem.MaximumPitch)
        {
            PitchErrorLabel.Text = TOO_HIGH;
        }
        else
        {
            PitchErrorLabel.Text = string.Empty;
        }

        // Roll
        if (geoSubsystem.Roll.Value < geoSubsystem.MinimumRoll)
        {
            RollErrorLabel.Text = TOO_LOW;
        }
        else if (geoSubsystem.Roll.Value > geoSubsystem.MaximumRoll)
        {
            RollErrorLabel.Text = TOO_HIGH;
        }
        else
        {
            RollErrorLabel.Text = string.Empty;
        }
    }

    /// <summary>
    /// Sets the threshold values of a given property in the security subsystem.
    /// </summary>
    /// <param name="property">The property to set thresholds for.</param>
    /// <param name="readingType">The reading type of the new threshold.</param>
    /// <param name="min">The minimum value threshold.</param>
    /// <param name="max">The maximum value threshold.</param>
    private void SetSecurityThreshold(string property, string readingType, float? min = null, float? max = null)
    {
        SecuritySubsystem subsystem = SecurityController.GetInstance().Subsystem;

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
    /// Sets the threshold values of a given property in the security subsystem.
    /// </summary>
    /// <param name="property">The property to set thresholds for.</param>
    /// <param name="readingType">The reading type of the new threshold.</param>
    /// <param name="min">The minimum value threshold.</param>
    /// <param name="max">The maximum value threshold.</param>
    private void SetGeoLocationThreshold(string property, string readingType, float? min = null, float? max = null)
    {
        GeoLocationSubsystem subsystem = GeoLocationController.GetInstance().Subsystem;

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
    /// Displays the min max popup and sets the selected values as new vibration thresholds.
    /// </summary>
    /// <param name="sender">The frame which was tapped.</param>
    /// <param name="e">The event object containing details about this event.</param>
    private async void vibration_Tapped(object sender, TappedEventArgs e)
    {
        const string PROPERTY = "Vibration";
        const string READING_TYPE = Reading<object>.TypeOptions.VIBRATION;
        MinMaxPopup.MinMaxDetails details = await MinMaxPopup.Show(this, PROPERTY, false, true);
        if (details is null)
            return;
        SetSecurityThreshold(PROPERTY, READING_TYPE, details.Min, details.Max);
        SetVibrationGauge();
    }

    /// <summary>
    /// Displays the min max popup and sets the selected values as new pitch thresholds.
    /// </summary>
    /// <param name="sender">The frame which was tapped.</param>
    /// <param name="e">The event object containing details about this event.</param>
    private async void pitch_Tapped(object sender, TappedEventArgs e)
    {
        const string PROPERTY = "Pitch";
        const string READING_TYPE = Reading<object>.TypeOptions.PITCH;
        MinMaxPopup.MinMaxDetails details = await MinMaxPopup.Show(this, PROPERTY, true, true);
        if (details is null)
            return;
        SetGeoLocationThreshold(PROPERTY, READING_TYPE, details.Min, details.Max);
        SetPitchGauge();
    }

    /// <summary>
    /// Displays the min max popup and sets the selected values as new roll thresholds.
    /// </summary>
    /// <param name="sender">The frame which was tapped.</param>
    /// <param name="e">The event object containing details about this event.</param>
    private async void roll_Tapped(object sender, TappedEventArgs e)
    {
        const string PROPERTY = "Roll";
        const string READING_TYPE = Reading<object>.TypeOptions.ROLL;
        MinMaxPopup.MinMaxDetails details = await MinMaxPopup.Show(this, PROPERTY, true, true);
        if (details is null)
            return;
        SetGeoLocationThreshold(PROPERTY, READING_TYPE, details.Min, details.Max);
        SetRollGauge();
    }

    /// <summary>
    /// Displays the min max popup and sets the selected values as new noise thresholds.
    /// </summary>
    /// <param name="sender">The frame which was tapped.</param>
    /// <param name="e">The event object containing details about this event.</param>
    private async void noise_Tapped(object sender, TappedEventArgs e)
    {
        const string PROPERTY = "Noise";
        const string READING_TYPE = Reading<object>.TypeOptions.NOISE;
        MinMaxPopup.MinMaxDetails details = await MinMaxPopup.Show(this, PROPERTY, false, true);
        if (details is null)
            return;
        SetSecurityThreshold(PROPERTY, READING_TYPE, details.Min, details.Max);
        SetNoiseGauge();
    }

    private async void luminosity_Tapped(object sender, TappedEventArgs e)
    {
        const string PROPERTY = "Luminosity";
        const string READING_TYPE = Reading<object>.TypeOptions.LUMINOSITY;
        MinMaxPopup.MinMaxDetails details = await MinMaxPopup.Show(this, PROPERTY, true, true);
        if (details is null)
            return;
        SetSecurityThreshold(PROPERTY, READING_TYPE, details.Min, details.Max);
        SetLuminosityGauge();
    }

    private void SetGauges()
    {
        try
        {
            SetNoiseGauge();
            SetLuminosityGauge();
            SetVibrationGauge();
            SetPitchGauge();
            SetRollGauge();
        }
        catch (Exception ex)
        {
            throw new Exception("An error has occured when setting you Gauge parameters. Please check the minimum and maximum value you set for each parameter." +
                $"\nHere are more details: {ex.Message}");
        }
    }

    private void SetNoiseGauge()
    {
        int numberOfGauges = 5;
        SecuritySubsystem subsystem = SecurityController.GetInstance().Subsystem;

        float maximumNoise = subsystem.MaximumNoise == DEFAULT_FLOAT ? DEFAULT_MAXIMUM_NOISE : subsystem.MaximumNoise;
        float minimumNoise = subsystem.MinimumNoise == DEFAULT_FLOAT ? DEFAULT_MINIMUM_NOISE : subsystem.MinimumNoise;
        float rangeIncrement = maximumNoise < minimumNoise ? (minimumNoise - maximumNoise) / numberOfGauges : (maximumNoise - minimumNoise) / numberOfGauges;

        NoiseGauge.Minimum = minimumNoise;
        NoiseGauge.Maximum = maximumNoise;
        NoiseGauge.Interval = (maximumNoise - minimumNoise) / 10;

        NoiseGaugeExcellent.StartValue = minimumNoise;
        NoiseGaugeExcellent.EndValue = rangeIncrement - 1 + minimumNoise;

        NoiseGaugeGood.StartValue = NoiseGaugeExcellent.EndValue + 1;
        NoiseGaugeGood.EndValue = rangeIncrement * 2 - 1 + minimumNoise;

        NoiseGaugeMedium.StartValue = NoiseGaugeGood.EndValue + 1;
        NoiseGaugeMedium.EndValue = rangeIncrement * 3 - 1 + minimumNoise;

        NoiseGaugeBad.StartValue = NoiseGaugeMedium.EndValue + 1;
        NoiseGaugeBad.EndValue = rangeIncrement * 4 - 1 + minimumNoise;

        NoiseGaugeTerrible.StartValue = NoiseGaugeBad.EndValue + 1;
        NoiseGaugeTerrible.EndValue = maximumNoise;
        SyncWarningsToProperties();
    }

    private void SetLuminosityGauge()
    {
        int numberOfGauges = 11;
        SecuritySubsystem subsystem = SecurityController.GetInstance().Subsystem;

        float maximumLuminosity = subsystem.MaximumLuminosity == DEFAULT_FLOAT ? DEFAULT_MAXIMUM_LUMINOSITY : subsystem.MaximumLuminosity;
        float minimumLuminosity = subsystem.MinimumLuminosity == DEFAULT_FLOAT ? DEFAULT_MINIMUM_LUMINOSITY : subsystem.MinimumLuminosity;
        float rangeIncrement = maximumLuminosity < minimumLuminosity ? (minimumLuminosity - maximumLuminosity) / numberOfGauges : (maximumLuminosity - minimumLuminosity) / numberOfGauges;

        LuminosityGauge.Minimum = minimumLuminosity;
        LuminosityGauge.Maximum = maximumLuminosity;
        LuminosityGauge.Interval = (maximumLuminosity - minimumLuminosity) / 10;

        LumGauge1.StartValue = minimumLuminosity;
        LumGauge1.EndValue = minimumLuminosity + rangeIncrement - 1;

        LumGauge2.StartValue = LumGauge1.EndValue + 1;
        LumGauge2.EndValue = LumGauge1.EndValue + rangeIncrement;

        LumGauge3.StartValue = LumGauge2.EndValue + 1;
        LumGauge3.EndValue = LumGauge2.EndValue + rangeIncrement;

        LumGauge4.StartValue = LumGauge3.EndValue + 1;
        LumGauge4.EndValue = LumGauge3.EndValue + rangeIncrement;

        LumGauge5.StartValue = LumGauge4.EndValue + 1;
        LumGauge5.EndValue = LumGauge4.EndValue + rangeIncrement;

        LumGauge6.StartValue = LumGauge5.EndValue + 1;
        LumGauge6.EndValue = LumGauge5.EndValue + rangeIncrement;

        LumGauge7.StartValue = LumGauge6.EndValue + 1;
        LumGauge7.EndValue = LumGauge6.EndValue + rangeIncrement;

        LumGauge8.StartValue = LumGauge7.EndValue + 1;
        LumGauge8.EndValue = LumGauge7.EndValue + rangeIncrement;

        LumGauge9.StartValue = LumGauge8.EndValue + 1;
        LumGauge9.EndValue = LumGauge8.EndValue + rangeIncrement;

        LumGauge10.StartValue = LumGauge9.EndValue + 1;
        LumGauge10.EndValue = LumGauge9.EndValue + rangeIncrement;

        LumGauge11.StartValue = LumGauge10.EndValue + 1;
        LumGauge11.EndValue = maximumLuminosity;
        SyncWarningsToProperties();
    }

    private void SetVibrationGauge()
    {
        int numberOfGauges = 5;
        SecuritySubsystem subsystem = SecurityController.GetInstance().Subsystem;

        float maximumVibration = subsystem.MaximumVibration == DEFAULT_FLOAT ? DEFAULT_MAXIMUM_VIBRATION : subsystem.MaximumVibration;
        float minimumVibration = subsystem.MinimumVibration == DEFAULT_FLOAT ? DEFAULT_MINIMUM_VIBRATION : subsystem.MinimumVibration;
        float rangeIncrement = Math.Abs(maximumVibration - minimumVibration) / numberOfGauges;

        VibrationGauge.Minimum = -2;
        VibrationGauge.Maximum = maximumVibration;
        VibrationGauge.Interval = (int)Math.Round((maximumVibration - minimumVibration) / 10);

        VibGaugeExcellent.StartValue = minimumVibration;
        VibGaugeGood.StartValue = rangeIncrement + minimumVibration;
        VibGaugeMedium.StartValue = rangeIncrement * 2 + minimumVibration;
        VibGaugeBad.StartValue = rangeIncrement * 3 + minimumVibration;
        VibGaugeTerrible.StartValue = rangeIncrement * 4 + minimumVibration;

        VibGaugeExcellent.EndValue = VibGaugeGood.StartValue - 1;
        VibGaugeGood.EndValue = VibGaugeMedium.StartValue - 1;
        VibGaugeMedium.EndValue = VibGaugeBad.StartValue - 1;
        VibGaugeBad.EndValue = VibGaugeTerrible.StartValue - 1;
        VibGaugeTerrible.EndValue = maximumVibration;
        SyncWarningsToProperties();
    }

    private void SetPitchGauge()
    {
        int numberOfGauges = 11;
        GeoLocationSubsystem subsystem = GeoLocationController.GetInstance().Subsystem;

        float maximumPitch = subsystem.MaximumPitch == DEFAULT_FLOAT ? DEFAULT_MAXIMUM_PITCH : subsystem.MaximumPitch;
        float minimumPitch = subsystem.MinimumPitch == DEFAULT_FLOAT ? DEFAULT_MINIMUM_PITCH : subsystem.MinimumPitch;
        float rangeIncrement = Math.Abs(maximumPitch - minimumPitch) / numberOfGauges;

        PitchGauge.Minimum = minimumPitch;
        PitchGauge.Maximum = maximumPitch;
        PitchGauge.Interval = (maximumPitch - minimumPitch) / 10;

        PitchGauge1.StartValue = minimumPitch;
        PitchGauge1.EndValue = minimumPitch + rangeIncrement - 1;

        PitchGauge2.StartValue = PitchGauge1.EndValue + 1;
        PitchGauge2.EndValue = PitchGauge1.EndValue + rangeIncrement;

        PitchGauge3.StartValue = PitchGauge2.EndValue + 1;
        PitchGauge3.EndValue = PitchGauge2.EndValue + rangeIncrement;

        PitchGauge4.StartValue = PitchGauge3.EndValue + 1;
        PitchGauge4.EndValue = PitchGauge3.EndValue + rangeIncrement;

        PitchGauge5.StartValue = PitchGauge4.EndValue + 1;
        PitchGauge5.EndValue = PitchGauge4.EndValue + rangeIncrement;

        PitchGauge6.StartValue = PitchGauge5.EndValue + 1;
        PitchGauge6.EndValue = PitchGauge5.EndValue + rangeIncrement;

        PitchGauge7.StartValue = PitchGauge6.EndValue + 1;
        PitchGauge7.EndValue = PitchGauge6.EndValue + rangeIncrement;

        PitchGauge8.StartValue = PitchGauge7.EndValue + 1;
        PitchGauge8.EndValue = PitchGauge7.EndValue + rangeIncrement;

        PitchGauge9.StartValue = PitchGauge8.EndValue + 1;
        PitchGauge9.EndValue = PitchGauge8.EndValue + rangeIncrement;

        PitchGauge10.StartValue = PitchGauge9.EndValue + 1;
        PitchGauge10.EndValue = PitchGauge9.EndValue + rangeIncrement;

        PitchGauge11.StartValue = PitchGauge10.EndValue + 1;
        PitchGauge11.EndValue = maximumPitch;
        SyncWarningsToProperties();
    }

    private void SetRollGauge()
    {
        int numberOfGauges = 11;
        GeoLocationSubsystem subsystem = GeoLocationController.GetInstance().Subsystem;

        float maximumRoll = subsystem.MaximumRoll == DEFAULT_FLOAT ? DEFAULT_MAXIMUM_ROLL : subsystem.MaximumRoll;
        float minimumRoll = subsystem.MinimumRoll == DEFAULT_FLOAT ? DEFAULT_MINIMUM_ROLL : subsystem.MinimumRoll;
        float rangeIncrement = Math.Abs(maximumRoll - minimumRoll) / numberOfGauges;

        RollGauge.Minimum = minimumRoll;
        RollGauge.Maximum = maximumRoll;
        RollGauge.Interval = (maximumRoll - minimumRoll) / 10;

        RollGauge1.StartValue = minimumRoll;
        RollGauge1.EndValue = minimumRoll + rangeIncrement - 1;

        RollGauge2.StartValue = RollGauge1.EndValue + 1;
        RollGauge2.EndValue = RollGauge1.EndValue + rangeIncrement;

        RollGauge3.StartValue = RollGauge2.EndValue + 1;
        RollGauge3.EndValue = RollGauge2.EndValue + rangeIncrement;

        RollGauge4.StartValue = RollGauge3.EndValue + 1;
        RollGauge4.EndValue = RollGauge3.EndValue + rangeIncrement;

        RollGauge5.StartValue = RollGauge4.EndValue + 1;
        RollGauge5.EndValue = RollGauge4.EndValue + rangeIncrement;

        RollGauge6.StartValue = RollGauge5.EndValue + 1;
        RollGauge6.EndValue = RollGauge5.EndValue + rangeIncrement;

        RollGauge7.StartValue = RollGauge6.EndValue + 1;
        RollGauge7.EndValue = RollGauge6.EndValue + rangeIncrement;

        RollGauge8.StartValue = RollGauge7.EndValue + 1;
        RollGauge8.EndValue = RollGauge7.EndValue + rangeIncrement;

        RollGauge9.StartValue = RollGauge8.EndValue + 1;
        RollGauge9.EndValue = RollGauge8.EndValue + rangeIncrement;

        RollGauge10.StartValue = RollGauge9.EndValue + 1;
        RollGauge10.EndValue = RollGauge9.EndValue + rangeIncrement;

        RollGauge11.StartValue = RollGauge10.EndValue + 1;
        RollGauge11.EndValue = maximumRoll;
        SyncWarningsToProperties();
    }
}
