using SHFT.Config;
using SHFT.Models;
using SHFT.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHFT.Repos
{
    internal class ThresholdRepo : DatabaseService<Reading<object>>
    {
        private static ThresholdRepo _instance;

        private ThresholdRepo(Firebase.Auth.User user, string path, string BaseUrl, string key = "") : base(user, path, BaseUrl, key)
        {
        }

        /// <summary>
        /// Gets the singleton instance of the repo.
        /// </summary>
        /// <returns>The singleton instance of this repo.</returns>
        public static ThresholdRepo GetInstance()
        {
            if (_instance is null)
                _instance = new ThresholdRepo(AuthService.Client.User, "Threshold", ResourceStrings.FIREBASE_DATABASEURL, "Threshold");
            return _instance;
        }

        /// <summary>
        /// Adds a threshold to the database.
        /// </summary>
        /// <param name="threshold">The new threshold value.</param>
        /// <param name="minMax">"min" if the threshold is a new minimum, max if it's a new max..</param>
        private async void AddThreshold(Reading<object> threshold, string minMax)
        {

            // Update any existing threshold for this reading type
            threshold.Type = $"{minMax}-{threshold.Type}";
            IEnumerable<Reading<object>> thresholds = await GetItemsAsync();
            foreach (Reading<object> t in thresholds)
            {
                if (t.Type == threshold.Type)
                {
                    t.Value = threshold.Value;
                    await UpdateItemAsync(t);
                    return;
                }
            }

            // Add the item if no threshold exists for this reading type.
            await AddItemAsync(threshold);
        }

        /// <summary>
        /// Adds a minimum threshold to the database.
        /// </summary>
        /// <param name="threshold">The new threshold value.</param>
        public void AddMinThreshold(Reading<object> threshold)
        {

            AddThreshold(threshold, "min");
        }

        /// <summary>
        /// Adds a maximum threshold to the database.
        /// </summary>
        /// <param name="threshold">The new threshold value.</param>
        public void AddMaxThreshold(Reading<object> threshold)
        {

            AddThreshold(threshold, "max");
        }

        /// <summary>
        /// Gets the threshold for a specific reading type.
        /// </summary>
        /// <param name="readingType">The desired reading type.</param>
        /// <returns>A reading with the threshold value for the desired reading type.</returns>
        public async Task<Reading<object>> GetMinThreshold(string readingType)
        {
            return (await GetItemsAsync()).Where(t => t.Type == $"min-{readingType}").FirstOrDefault();
        }

        /// <summary>
        /// Gets the threshold for a specific reading type.
        /// </summary>
        /// <param name="readingType">The desired reading type.</param>
        /// <returns>A reading with the threshold value for the desired reading type.</returns>
        public async Task<Reading<object>> GetMaxThreshold(string readingType)
        {
            return (await GetItemsAsync()).Where(t => t.Type == $"max-{readingType}").FirstOrDefault();
        }

    }
}
