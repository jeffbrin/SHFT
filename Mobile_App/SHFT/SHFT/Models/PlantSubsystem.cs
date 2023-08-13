// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// PlantSubsystem Class - This class represents a subsystem that monitors and controls the environmental conditions of a plant.

using SHFT.Controllers;
using SHFT.Repos;
using System.ComponentModel;

namespace SHFT.Models
{
    /// <summary>
    /// Represents a subsystem that monitors and controls the environmental conditions of a plant.
    /// </summary>
    internal class PlantSubsystem : INotifyPropertyChanged
    {
        private readonly PlantController _controller;
        private List<Reading<float>> _soilMoistures;
        private const float DEFAULT_MAXIMUM_TEMPERATURE = 50;
        private const float DEFAULT_MINIMUM_TEMPERATURE = 10;
        private const float DEFAULT_MAXIMUM_HUMIDITY = 50;
        private const float DEFAULT_MINIMUM_HUMIDITY = 0;
        private const float DEFAULT_MAXIMUM_WATER_LEVEL = 10;
        private const float DEFAULT_MINIMUM_WATER_LEVEL = 0;
        private const float DEFAULT_MAXIMUM_SOIL_MOISTURE = 70;
        private const float DEFAULT_MINIMUM_SOIL_MOISTURE = 30;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlantSubsystem"/> class.
        /// </summary>
        /// <param name="controller">The controller for the plant subsystem.</param>
        public PlantSubsystem(PlantController controller)
        {
            _controller = controller;
            Temperature = new Reading<float>();
            Humidity = new Reading<float>();
            WaterLevel = new Reading<float>();
            SoilMoistures = new List<Reading<float>>();
            FanState = new Reading<bool>();
            LightState = new Reading<bool>();
            DoorLockState = new Reading<bool>();
            SetThresholds();
        }

        /// <summary>
        /// Sets the thresholds saved in the database.
        /// </summary>
        private void SetThresholds()
        {
            Task<Reading<object>> task = ThresholdRepo.GetInstance().GetMinThreshold(Reading<object>.TypeOptions.TEMPERATURE);
            task.Wait();
            Reading<object> minTemperatureReading = task.Result;
            if (minTemperatureReading is not null)
                MinimumTemperature = Convert.ToSingle(minTemperatureReading.Value);

            task = ThresholdRepo.GetInstance().GetMaxThreshold(Reading<object>.TypeOptions.TEMPERATURE);
            task.Wait();
            Reading<object> maxTemperatureReading = task.Result;
            if (maxTemperatureReading is not null)
                MaximumTemperature = Convert.ToSingle(maxTemperatureReading.Value);

            task = ThresholdRepo.GetInstance().GetMinThreshold(Reading<object>.TypeOptions.HUMIDITY);
            task.Wait();
            Reading<object> minHumidityReading = task.Result;
            if (minHumidityReading is not null)
                MinimumHumidity = Convert.ToSingle(minHumidityReading.Value);

            task = ThresholdRepo.GetInstance().GetMaxThreshold(Reading<object>.TypeOptions.HUMIDITY);
            task.Wait();
            Reading<object> maxHumidityReading = task.Result;
            if (maxHumidityReading is not null)
                MaximumHumidity= Convert.ToSingle(maxHumidityReading.Value);

            task = ThresholdRepo.GetInstance().GetMinThreshold(Reading<object>.TypeOptions.SOIL_MOISTURE);
            task.Wait();
            Reading<object> minSoilMoisture = task.Result;
            if (minSoilMoisture is not null)
                MinimumSoilMoisture = Convert.ToSingle(minSoilMoisture.Value);

            task = ThresholdRepo.GetInstance().GetMaxThreshold(Reading<object>.TypeOptions.SOIL_MOISTURE);
            task.Wait();
            Reading<object> maxSoilMoistureReading = task.Result;
            if (maxSoilMoistureReading is not null)
                MaximumSoilMoisture = Convert.ToSingle(maxSoilMoistureReading.Value);

            task = ThresholdRepo.GetInstance().GetMinThreshold(Reading<object>.TypeOptions.WATER_LEVEL);
            task.Wait();
            Reading<object> minWaterLevelReading = task.Result;
            if (minWaterLevelReading is not null)
                MinimumWaterLevel = Convert.ToSingle(minWaterLevelReading.Value);

            task = ThresholdRepo.GetInstance().GetMaxThreshold(Reading<object>.TypeOptions.WATER_LEVEL);
            task.Wait();
            Reading<object> maxWaterLevelReading = task.Result;
            if (maxWaterLevelReading is not null)
                MaximumWaterLevel = Convert.ToSingle(maxWaterLevelReading.Value);

        }

        /// <summary>
        /// Gets or sets the minimum temperature value for the plant.
        /// </summary>
        public float MinimumTemperature { get; set; } = DEFAULT_MINIMUM_TEMPERATURE;

        /// <summary>
        /// Gets or sets the maximum temperature value for the plant.
        /// </summary>
        public float MaximumTemperature { get; set; } = DEFAULT_MAXIMUM_TEMPERATURE;

        /// <summary>
        /// Gets or sets the minimum humidity value for the plant.
        /// </summary>
        public float MinimumHumidity { get; set; } = DEFAULT_MINIMUM_HUMIDITY;

        /// <summary>
        /// Gets or sets the maximum humidity value for the plant.
        /// </summary>
        public float MaximumHumidity { get; set; } = DEFAULT_MAXIMUM_HUMIDITY;

        /// <summary>
        /// Gets or sets the minimum water level value for the plant.
        /// </summary>
        public float MinimumWaterLevel { get; set; } = DEFAULT_MINIMUM_WATER_LEVEL;

        /// <summary>
        /// Gets or sets the maximum water level value for the plant.
        /// </summary>
        public float MaximumWaterLevel { get; set; } = DEFAULT_MAXIMUM_WATER_LEVEL;

        /// <summary>
        /// Gets or sets the maximum soil moisture value for the plant.
        /// </summary>
        public float MaximumSoilMoisture { get; set; } = DEFAULT_MAXIMUM_SOIL_MOISTURE;

        /// <summary>
        /// Gets or sets the minimum soil moisture value for the plant.
        /// </summary>
        public float MinimumSoilMoisture { get; set; } = DEFAULT_MINIMUM_SOIL_MOISTURE;

        /// <summary>
        /// Gets or sets the temperature reading of the plant.
        /// </summary>
        public Reading<float> Temperature { get; set; }

        /// <summary>
        /// Gets or sets the humidity reading of the plant.
        /// </summary>
        public Reading<float> Humidity { get; set; }

        /// <summary>
        /// Gets or sets the water level reading of the plant.
        /// </summary>
        public Reading<float> WaterLevel { get; set; }

        /// <summary>
        /// Indicates whether the soil moisture is within the confines of the min and max values.
        /// </summary>
        public bool IsSoilMoistureOkay
        {
            get
            {
                return SoilMoistures.All(moisture => moisture.Value >= MinimumSoilMoisture && moisture.Value <= MaximumSoilMoisture);
            }
        }

        /// <summary>
        /// Gets or sets the soil moisture values for the plant.
        /// </summary>
        public List<Reading<float>> SoilMoistures { get { return _soilMoistures; } set {
                _soilMoistures = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSoilMoistureOkay)));
            } }

        /// <summary>
        /// Gets or sets the fan state of the plant.
        /// </summary>
        public Reading<bool> FanState { get; set; }

        /// <summary>
        /// Gets the light state of the plant.
        /// </summary>
        public Reading<bool> LightState { get; set; }


        /// <summary>
        /// Gets and sets the door lock state of the plant.
        /// </summary>
        /// <remarks>
        /// A <see cref="Reading{T}"/> object is used to store the state value and its associated unit.
        /// </remarks>
        public Reading<bool> DoorLockState { get; set; }

        /// <summary>
        /// Sets the fan state of the plant.
        /// </summary>
        /// <param name="state">The new state of the fan.</param>
        /// <returns>A boolean value indicating whether the fan state was successfully set.</returns>
        public async Task<bool> SetFanState(bool state)
        {
            FanState = new Reading<bool> { Value = state, Unit = FanState.Unit };
            return await _controller.SetHardwareFanState(state);
        }

        /// <summary>
        /// Sets the light state of the plant.
        /// </summary>
        /// <param name="state">The new state of the light.</param>
        /// <returns>A boolean value indicating whether the light state was successfully set.</returns>
        public async Task<bool> SetLightState(bool state)
        {
            LightState = new Reading<bool> { Value = state, Unit = LightState.Unit };
            return await _controller.SetHardwareLightState(state);
        }

        /// <summary>
        /// Sets the door state of the plant.
        /// </summary>
        /// <param name="state">The new state of the door.</param>
        public async Task<bool>SetDoorState(bool state)
        {
            DoorLockState = new Reading<bool> { Value = state, Unit = DoorLockState.Unit };
            return await _controller.SetHardwareLockState(state);
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
