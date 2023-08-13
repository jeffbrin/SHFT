// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// This class contains the functionality for presenting historical data in a Line Series graph

namespace SHFT.Views;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using SHFT.Repos;
using System.Diagnostics;
using System.Text;
using System.Xml.Serialization;

public partial class HistoricalData : ContentPage
{
    private readonly HistoricalSecurityDataRepo _repo;

    /// <summary>
    /// List of ISeries to present the data
    /// </summary>
    public List<ISeries> Series { get; set; }

    /// <summary>
    /// Array to hold all of the X-Axis
    /// </summary>
    public Axis[] XAxis { get; set; }

    /// <summary>
    /// Array to hold all of the Y-Axis
    /// </summary>
    public Axis[] YAxis { get; set; }

    /// <summary>
    /// Line Series graph for which will contain the Noise 
    /// </summary>
    private readonly LineSeries<ObservablePoint> noiseData;

    /// <summary>
    /// Line Series graph for which will contain the Luminosity 
    /// </summary>
    private readonly LineSeries<ObservablePoint> luminosityData;

    /// <summary>
    /// Intilizes and populates the Noise and the Luminosity Line Series with random values
    /// /// </summary>
    public HistoricalData()
    {
        InitializeComponent();

        _repo = HistoricalSecurityDataRepo.GetInstance();
        pickDiffData.SelectedIndex = 0;

        Setup();

    }

    /// <summary>
    /// Sets up the view with initial data.
    /// </summary>
    private async void Setup()
    {
        UpdateChart(await _repo.GetNoiseData());
    }

    /// <summary>
    /// Updates the Series List of ISeries to the appropriate selected option.
    /// </summary>
    /// <param name="series">The updated Series which the user will be presented by.</param>
    private void UpdateChart(LineSeries<ObservablePoint> series)
    {
        Series = new List<ISeries> { series };
        ownerHistorical.Series = Series;
    }

    /// <summary>
    /// Depending on the selected index, the user will be presented with the correct Line Series.
    /// </summary>
    /// <param name="sender">Represents the picker.</param>
    /// <param name="e">Event represented by the picker.</param>
    private async void PickDiffData_SelectedIndexChanged(object sender, EventArgs e)
    {
        Picker pickChart = (Picker)sender;

        try
        {
            if (pickChart.SelectedItem.ToString() == "Noise")
            {
                UpdateChart(await _repo.GetNoiseData());
            }
            if (pickChart.SelectedItem.ToString() == "Luminosity")
            {
                UpdateChart(await _repo.GetLuminosityData());
            }
        }catch(Exception ex)
        {
            Debug.WriteLine($"ERROR UPDATING CHART: {ex.Message}");
        }
    }
    /// <summary>
    /// Depending on the selected index, the user will be presented with the different options
    /// to which he will pick the data wanting to share and data will be sent to converted to the 
    /// apropriate format
    /// </summary>
    /// <param name="sender">Represents the picker</param>
    /// <param name="e">Event represented by the picker</param>
    public async void ShareHistoricalData(object sender, EventArgs e)
    {
        try
        {
            var selectedData = pickDiffData.SelectedItem as string;
            string csvData = null;

            switch (selectedData)
            {
                case "Noise":

                    csvData = ConvertDataToCSV(await _repo.GetNoiseData());
                    break;

                case "Luminosity":

                    csvData = ConvertDataToCSV(await _repo.GetLuminosityData());
                    break;
            }

            if (!string.IsNullOrEmpty(csvData))
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "data.csv");
                File.WriteAllText(filePath, csvData);

                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = $"Share {selectedData} Data",
                    File = new ShareFile(filePath)
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    /// <summary>
    /// This function accepts a line series as an argument and converts it into CSV formatted data.
    /// </summary>
    /// <param name="series">Line series with x and y points</param>
    /// <returns>A string which contains x and y values</returns>
    public string ConvertDataToCSV(LineSeries<ObservablePoint> series)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("X,Y");

        foreach (var point in series.Values)
        {
            sb.AppendLine($"{point.X},{point.Y}");
        }

        return sb.ToString();
    }

}
