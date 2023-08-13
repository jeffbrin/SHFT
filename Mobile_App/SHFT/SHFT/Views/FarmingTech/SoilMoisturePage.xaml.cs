// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// The page which shows all the soil moisture values.

namespace SHFT.Views;
using SHFT.Controllers;
using SHFT.Models;
using SHFT.Repos;
using System.Reflection;

public partial class SoilMoisturePage : ContentPage
{
    /// <summary>
    /// Initializes the soil moisture page, binding the soil moistures list.
    /// </summary>
    public SoilMoisturePage()
    {
        InitializeComponent();
        this.BindingContext = PlantController.GetInstance().Subsystem.SoilMoistures;
    }

    private async void btnThresholds_Clicked(object sender, EventArgs e)
    {
        const string PROPERTY = "SoilMoisture";
        const string READING_TYPE = Reading<object>.TypeOptions.SOIL_MOISTURE;
        MinMaxPopup.MinMaxDetails details = await MinMaxPopup.Show(this, PROPERTY, true, true);
        if (details is null)
            return;
        SetThreshold(PROPERTY, READING_TYPE, details.Min, details.Max);
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
}