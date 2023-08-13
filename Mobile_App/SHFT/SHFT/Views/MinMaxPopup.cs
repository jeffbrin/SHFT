// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// A static class used to display min and max request popups

namespace SHFT.Views
{
    /// <summary>
    /// A static class which can be used to show min and max prompt sequences.
    /// </summary>
    public static class MinMaxPopup
    {
        /// <summary>
        /// Represents the minimum and maximum values returned from the Show method
        /// </summary>
        public class MinMaxDetails
        {
            /// <summary>
            /// Initializes the MinMaxDetails
            /// </summary>
            /// <param name="min">The minimum value. Can be null.</param>
            /// <param name="max">The maximum value. Can be null.</param>
            public MinMaxDetails(float? min, float? max)
            {
                Min = min;
                Max = max;
            }

            /// <summary>
            /// The minimum value to return.
            /// </summary>
            public float? Min { get; }

            /// <summary>
            /// The maximum value to return.
            /// </summary>
            public float? Max { get; }
        }

        /// <summary>
        /// Shows the prompts to get minimum and maximum values.
        /// </summary>
        /// <param name="contentPage">The content page over which the prompts should be shown.</param>
        /// <param name="propertyName">The name of the property that these prompts should request data for.</param>
        /// <param name="getMin">Indicates whether the method should ask for a minimum value.</param>
        /// <param name="getMax">Indicates whether the method should ask for a maximum value.</param>
        /// <returns>
        /// A <see cref="MinMaxDetails"/> object with the minimum and maximum values from the prompts.
        /// If there is invalid input, null is returned. If minimum or maximum aren't requested, they will be null.
        /// </returns>
        public static async Task<MinMaxDetails> Show(ContentPage contentPage, string propertyName, bool getMin = true, bool getMax = true)
        {

            float? min = null;
            float? max = null;
            const string CANCEL = "Cancel";

            // Get min if desired
            if (getMin)
            {
                string minString = await contentPage.DisplayPromptAsync("Minimum", $"Enter the minimum {propertyName} value.");
                if (minString is null || minString == CANCEL)
                    return null;


                // Get the min value
                try
                {
                    min = float.Parse(minString);
                }
                catch (Exception)
                {
                    await contentPage.DisplayAlert("Failed", "Minimum value was not a number.", "Ok");
                    return null;
                }
            }
            // Get max if desired
            if (getMax)
            {
                string maxString = await contentPage.DisplayPromptAsync("Maximum", $"Enter the maximum {propertyName} value.");
                if (maxString is null || maxString == CANCEL)
                    return null;

                // Get the min value
                try
                {
                    max = float.Parse(maxString);
                    if (min.HasValue && max.Value < min.Value)
                    {
                        await contentPage.DisplayAlert("Failed", "Maximum can not be less than minimum.", "Ok");
                        return null;
                    }
                }
                catch (Exception)
                {
                    await contentPage.DisplayAlert("Failed", "Maximum value was not a number.", "Ok");
                    return null;
                }
            }
            return new MinMaxDetails(min, max);
        }
    }
}
