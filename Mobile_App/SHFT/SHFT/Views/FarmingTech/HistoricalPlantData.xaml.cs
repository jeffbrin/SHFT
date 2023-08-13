// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// A page which shows historical plant data.

namespace SHFT.Views;

using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using SHFT.Repos;

public partial class HistoricalPlantData : ContentPage
{
    private readonly HistoricalPlantDataRepo _repo;

    const string TEMPERATURE = "Temperature";
    const string HUMIDITY = "Humidity";
    const string WATER_LEVEL = "Water Level";

    /// <summary>
    /// The default constructor which sets the default value for the chart and gets the repo.
    /// </summary>
    public HistoricalPlantData()
    {
        InitializeComponent();
        dataPicker.ItemsSource = new string[] { TEMPERATURE, HUMIDITY, WATER_LEVEL };

        _repo = HistoricalPlantDataRepo.GetInstance();
        dataPicker.SelectedIndex = 0;
    }

    /// <summary>
    /// Updates the chart with the given series data.
    /// </summary>
    /// <param name="series">A series of <see cref="ObservablePoint"/>s which populate the chart.</param>
    private void UpdateChart(LineSeries<ObservablePoint> series)
    {
        List<ISeries> Series = new() { series };
        historicalPlantDataChart.Series = Series;
    }

    /// <summary>
    /// Changes the chart data to match the data selected by the user.
    /// </summary>
    /// <param name="sender">The picker pressed which selects the data to display.</param>
    /// <param name="e">The event arguments related to this event.</param>
    private async void pickDiffData_SelectedIndexChanged(object sender, EventArgs e)
    {
        Picker pickChart = (Picker)sender;
        try
        {
            switch (pickChart.SelectedItem.ToString())
            {
                case TEMPERATURE:
                    UpdateChart(await _repo.GetTemperatureData());
                    break;
                case HUMIDITY:
                    UpdateChart(await _repo.GetHumidityData());
                    break;
                case WATER_LEVEL:
                    UpdateChart(await _repo.GetWaterLevelData());
                    break;
            }
        }catch(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }
}