using Firebase.Auth;
using LiveChartsCore.Defaults;
using SHFT.Models;
using SHFT.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHFT.Repos
{
    public abstract class ReadingRepo : DatabaseService<Reading<object>>
    {

        const int HOURS_TO_STORE_READINGS = 24;

        protected ReadingRepo(User user, string path, string BaseUrl, string key = "") : base(user, path, BaseUrl, key)
        {
            Task cleanupTask = CleaupDatabase();
            cleanupTask.Wait();
        }


        private async Task CleaupDatabase()
        {
            IEnumerable<Reading<object>> allReadings = await GetItemsAsync();
            // Delete items which are too old
            foreach(var reading in allReadings.Where(r => (DateTime.Now - r.Timestamp).TotalHours > HOURS_TO_STORE_READINGS))
            {
                await DeleteItemAsync(reading);
            }
        }

        /// <summary>
        /// Uploads a reading to the firebase database.
        /// </summary>
        /// <param name="reading">The reading to upload.</param>
        public async void UploadReading<T>(Reading<T> reading)
        {
            Reading<object> newReading = new Reading<object>() { Key = reading.Key, Timestamp = reading.Timestamp, Unit = reading.Unit, Value = reading.Value, Type = reading.Type };
            
            // QoS is weird with the event hub so this prevents repeated readings
            if ((await GetItemsAsync()).Any(r => r.Equals(reading)))
                return;

            await AddItemAsync(newReading);
        }

        /// <summary>
        /// Generates a list of points based off a list of readings.
        /// </summary>
        /// <param name="items">All the readings in the database.</param>
        /// <param name="readingType">The reading type to filter on.</param>
        /// <returns>
        /// An IEnumerable of ObservablePoints with the x value equal to the difference in time between now and the creation of the reading,
        /// And the y value equal to the reading value.
        /// </returns>
        protected IEnumerable<ObservablePoint> GeneratePoints(IEnumerable<Reading<object>> items, string readingType)
        {
            var readings = items.Where(r => r.Type == readingType);
            List<double> times = readings.Select(r => (DateTime.Now - r.Timestamp).TotalHours).ToList();
            var points = readings.Select(r => new ObservablePoint(
                    (r.Timestamp - DateTime.Now).TotalHours,
                    double.Parse(r.Value.ToString()))
                    );
            var orderedPoints = points.OrderBy(point => point.X);
            return orderedPoints;
        }

        /// <summary>
        /// Gets the DateTime of the most recent reading processed by the app.
        /// </summary>
        /// <returns>The TimeStamp of the most recent reading processed by the app.</returns>
        public async Task<DateTime> GetMostRecentReading()
        {
           Reading<object> reading = (await GetItemsAsync()).LastOrDefault();

            if (reading is null)
                return DateTime.UnixEpoch;

            return reading.Timestamp;
        }
    }
}
