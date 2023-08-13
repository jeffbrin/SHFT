// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// ITelemetryRepo Interface
// This interface allows the program to receive telemetry data.

using Newtonsoft.Json.Linq;

namespace SHFT.Interfaces
{
    /// <summary>
    /// An interface which allows the program to receive telemetry data.
    /// </summary>
    public interface ITelemetryRepo
    {

        /// <summary>
        /// Receives data and routes it appropriately.
        /// </summary>
        /// <param name="data">The data to route.</param>
        void PassData(JObject data);

        /// <summary>
        /// Starts the telemetry repository, initiating connections and creating background variables.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task Start();
    }
}
