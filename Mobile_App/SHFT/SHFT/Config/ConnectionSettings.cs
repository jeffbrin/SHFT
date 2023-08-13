// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// ConnectionSettings: Stores all the necessary connection parameters
// required to set up and manage the connection with various Azure
// services.

namespace SHFT.Config
{
    /// <summary>
    /// Stores all the necessary connection parameters required to set up and manage the connection with various Azure services.
    /// </summary>
    public class ConnectionSettings
    {
        /// <summary>
        /// Gets or sets the connection string for the Azure Event Hub.
        /// </summary>
        public string EventHubConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the Azure Event Hub.
        /// </summary>
        public string EvenHubName { get; set; }

        /// <summary>
        /// Gets or sets the name of the Event Hub consumer group.
        /// </summary>
        public string ConsumerGroup { get; set; }

        /// <summary>
        /// Gets or sets the connection string for Azure Storage Account.
        /// </summary>
        public string StorageConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the Blob Storage container.
        /// </summary>
        public string BlobContainerName { get; set; }

        /// <summary>
        /// Gets or sets the connection string for the IoT Hub.
        /// </summary>
        public string HubConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the unique ID of the IoT device.
        /// </summary>
        public string DeviceId { get; set; }
    }
}
