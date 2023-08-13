// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// This class provides access to historical security data for display in charts.

using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using SHFT.Config;
using SHFT.Models;
using SHFT.Services;

namespace SHFT.Repos
{
    /// <summary>
    /// A singleton repository to get historical security data.
    /// </summary>
    internal class HistoricalSecurityDataRepo : ReadingRepo
    {
        private static HistoricalSecurityDataRepo _instance;

        /// <summary>
        /// The private constructor to prevent access from other classes.
        /// </summary>
        private HistoricalSecurityDataRepo(Firebase.Auth.User user, string path, string BaseUrl, string key = "") : base(user, path, BaseUrl, key)
        {
        }

        private static readonly int DATA_POINTS = 10;
        private static readonly int MAX_DATA_VALUE = 500;

        /// <summary>
        /// Gets the singleton instance of the repo.
        /// </summary>
        /// <returns>The singleton instance of this repo.</returns>
        public static HistoricalSecurityDataRepo GetInstance()
        {
            if (_instance is null)
                _instance = new HistoricalSecurityDataRepo(AuthService.Client.User, nameof(Reading<Object>), ResourceStrings.FIREBASE_DATABASEURL, nameof(Reading<Object>));
            return _instance;
        }

        /// <summary>
        /// Gets noise data to display in a chart.
        /// </summary>
        /// <returns>A LineSeries of observable points with historical noise data.</returns>
        public async Task<LineSeries<ObservablePoint>> GetNoiseData()
        {
            IEnumerable<Reading<object>> items = await GetItemsAsync();
            var points = GeneratePoints(items, Reading<object>.TypeOptions.NOISE);

            return new LineSeries<ObservablePoint> { Values = points };
        }

        /// <summary>
        /// Gets luminosity data to display in a chart.
        /// </summary>
        /// <returns>A LineSeries of observable points with historical luminosity data.</returns>
        public async Task<LineSeries<ObservablePoint>> GetLuminosityData()
        {
            IEnumerable<Reading<object>> items = await GetItemsAsync();
            var points = GeneratePoints(items, Reading<object>.TypeOptions.LUMINOSITY);

            return new LineSeries<ObservablePoint> { Values = points };
        }
    }
}
