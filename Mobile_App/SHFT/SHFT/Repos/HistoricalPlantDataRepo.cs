// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// A repository to get historical plant data.
// This class provides methods to get historical temperature,
// water level, and humidity data to display in a chart.

using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using SHFT.Config;
using SHFT.Models;
using SHFT.Services;
using System.Diagnostics;

namespace SHFT.Repos
{
    /// <summary>
    /// A singleton repository to get historical plant data.
    /// </summary>
    internal class HistoricalPlantDataRepo : ReadingRepo
    {
        private static HistoricalPlantDataRepo _instance;
        private static readonly int DATA_POINTS = 10;
        private static readonly int MAX_DATA_VALUE = 500;

        /// <summary>
        /// Private constructor for singleton.
        /// </summary>
        private HistoricalPlantDataRepo(Firebase.Auth.User user, string path, string BaseUrl, string key = "") : base(user, path, BaseUrl, key)
        {
        }

        /// <summary>
        /// Gets the singleton instance of the repo.
        /// </summary>
        /// <returns>The singleton instance of this repo.</returns>
        public static HistoricalPlantDataRepo GetInstance()
        {
            try
            {
                if (_instance is null)
                    _instance = new HistoricalPlantDataRepo(AuthService.Client.User, nameof(Reading<Object>), ResourceStrings.FIREBASE_DATABASEURL, nameof(Reading<Object>));
            }catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return _instance;
        }

        /// <summary>
        /// Gets temperature data to display in a chart.
        /// </summary>
        /// <returns>A LineSeries of observable points with historical temperature data.</returns>
        public async Task<LineSeries<ObservablePoint>> GetTemperatureData()
        {
            IEnumerable<Reading<object>> items = await GetItemsAsync();
            var points = GeneratePoints(items, Reading<object>.TypeOptions.TEMPERATURE);

            return new LineSeries<ObservablePoint> { Values = points };
        }

        /// <summary>
        /// Gets water level data to display in a chart.
        /// </summary>
        /// <returns>A LineSeries of observable points with historical water level data.</returns>
        public async Task<LineSeries<ObservablePoint>> GetWaterLevelData()
        {
            IEnumerable<Reading<object>> items = await GetItemsAsync();
            var points = GeneratePoints(items, Reading<object>.TypeOptions.WATER_LEVEL);

            return new LineSeries<ObservablePoint> { Values = points };
        }

        /// <summary>
        /// Gets humidity data to display in a chart.
        /// </summary>
        /// <returns>A LineSeries of observable points with historical humidity data.</returns>
        public async Task<LineSeries<ObservablePoint>> GetHumidityData()
        {
            IEnumerable<Reading<object>> items = await GetItemsAsync();
            var points = GeneratePoints(items, Reading<object>.TypeOptions.HUMIDITY);

            return new LineSeries<ObservablePoint> { Values = points };
        }

    }
}
