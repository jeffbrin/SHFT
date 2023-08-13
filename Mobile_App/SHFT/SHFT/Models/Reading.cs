// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// A struct which provides consistent structure to device readings.

using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using SHFT.Interfaces;

namespace SHFT.Models
{
    /// <summary>
    /// Represents a reading from a device.
    /// </summary>
    /// <typeparam name="T">The type of the reading's value.</typeparam>
    public class Reading<T> : IHasUKey
    {
        public class UnitOptions
        {
            public const string DEGREES = "°";
            public const string CELSIUS = "°C";
            public const string PERCENTAGE = "%";
            public const string CENTIMETERS = "cm";
            public const string NONE = "";
            public const string DECIBEL = "dB";
            public const string LUX = "lx";
        }

        public static class TypeOptions
        {
            public const string GEO_LOCATION = "Geo-Location";
            public const string PITCH = "Pitch";
            public const string ROLL = "Roll";
            public const string BUZZER = "Buzzer";
            public const string VIBRATION = "Vibration";
            public const string FAN = "Fan";
            public const string SOIL_MOISTURE = "Soil-Moisture";
            public const string WATER_LEVEL = "Water-Level";
            public const string TEMPERATURE = "Temperature";
            public const string HUMIDITY = "Humidity";
            public const string RGB_LED_STICK = "RGB-LED-Stick";
            public const string LUMINOSITY = "Luminosity";
            public const string DOOR_OPENED = "Door-Opened";
            public const string DOOR_LOCKED = "Door-Locked";
            public const string MOTION = "Motion";
            public const string NOISE = "Noise";
            public const string LATITUDE = "Latitude";
            public const string LONGITUDE = "Longitude";
        }

        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the value of the reading.
        /// </summary>
        /// <value>The value of the reading.</value>
        public T Value { get; set; }

        /// <summary>
        /// Gets or sets the unit of the reading.
        /// </summary>
        /// <value>The unit of the reading.</value>
        public string Unit { get; set; }

        /// <summary>
        /// The time that this reading was created.
        /// </summary>
        public DateTime Timestamp { get; set; }
        public string Key { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Converts the reading to a string representation.
        /// </summary>
        /// <returns>A string representation of the reading.</returns>
        public override string ToString()
        {
            return $"{Value} {Unit}";
        }

        public override bool Equals(object obj)
        {
            Reading<T> other = obj as Reading<T>;
            if (other is null) 
                return false;

            return base.Equals(other) || (DateTime.Compare(other.Timestamp, Timestamp) == 0 && other.Type == Type && other.Unit == Unit && other.Value.Equals(Value));
        }
    }
}
