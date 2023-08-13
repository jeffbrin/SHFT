// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// Container Class
// This class represents a container and manages the relationship with
// the connection manager.

using SHFT.Interfaces;

namespace SHFT.Models
{
    public class Container : IHasUKey
    {
        private string _name;
        private string _description;
        private string _connectionString;

        /// <summary>
        /// Gets or sets the connection string to be used with the IoT Hub.
        /// </summary>
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        /// <summary>
        /// Gets or sets the device ID of the IoT Hub device.
        /// </summary>
        public string DeviceId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the container.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the description of the container.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Gets or sets the key of the container.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Initializes a new instance of the Container class with the specified connection string, container name, description, and device ID.
        /// </summary>
        /// <param name="conString">The connection string to be used with the IoT Hub.</param>
        /// <param name="nameContainer">The name of the container.</param>
        /// <param name="descriptionContainer">The description of the container.</param>
        /// <param name="deviceId">The ID of the IoT Hub device.</param>
        public Container(string conString, string nameContainer, string descriptionContainer, string deviceId)
        {
            _connectionString = conString;
            _name = nameContainer;
            _description = descriptionContainer;
            DeviceId = deviceId;
        }
    }
}
