// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// PlantController: This class is responsible for controlling and managing the plant subsystem. It provides methods to set temperature, humidity, water level, and soil moisture readings. It also manages hardware state such as fan and light states.

using SHFT.Models;
using SHFT.Repos;

namespace SHFT.Controllers
{
    /// <summary>
    /// Manages the plant subsystem. Provides methods to set temperature, humidity, water level, and soil moisture readings. Also manages hardware states such as fan and light states.
    /// </summary>
    internal sealed class PlantController
    {
        private static PlantController _instance;
        private const int READINGS_BETWEEN_SAVES = 10;
        private int _temperatureReadingsProcessedInBatch = 0;
        private int _humidityReadingsProcessedInBatch = 0;
        private int _waterLevelReadingsProcessedInBatch = 0;
        private int _soilMoistureReadingsProcessedInBatch = 0;

        public delegate void VoidDelegate();

        /// <summary>
        /// An event which is invoked any time a property is changed.
        /// </summary>
        public event VoidDelegate PropertyUpdated;

        /// <summary>
        /// Initializes a new _instance of the <see cref="PlantController"/> class.
        /// </summary>
        private PlantController()
        {
            Subsystem = new PlantSubsystem(this);
        }

        /// <summary>
        /// Gets the singleton instance of the <see cref="PlantController"/> class.
        /// </summary>
        /// <returns>The singleton instance of the <see cref="PlantController"/> class.</returns>
        public static PlantController GetInstance()
        {
            if (_instance is null)
                _instance = new PlantController();

            return _instance;
        }

        /// <summary>
        /// Gets the plant subsystem.
        /// </summary>
        public PlantSubsystem Subsystem { get; private set; }

        /// <summary>
        /// Sets the temperature value of the subsystem.
        /// </summary>
        /// <param name="reading">The new temperature reading.</param>
        public void SetTemperature(Reading<float> reading)
        {
            Subsystem.Temperature = reading;
            _temperatureReadingsProcessedInBatch++;
            if (_temperatureReadingsProcessedInBatch >= READINGS_BETWEEN_SAVES)
            {
                HistoricalSecurityDataRepo.GetInstance().UploadReading(reading);
                _temperatureReadingsProcessedInBatch = 0;
            }
            PropertyUpdated();
        }

        /// <summary>
        /// Sets the soil moistures of the subsystem.
        /// </summary>
        /// <param name="reading">The new temperature reading.</param>
        public void SetSoilMoistures(List<Reading<float>> readings)
        {
            Subsystem.SoilMoistures = readings;
            foreach (Reading<float> reading in readings)
            {
                _soilMoistureReadingsProcessedInBatch++;
                if (_soilMoistureReadingsProcessedInBatch >= READINGS_BETWEEN_SAVES)
                {
                    HistoricalPlantDataRepo.GetInstance().UploadReading(reading);
                    _soilMoistureReadingsProcessedInBatch = 0;
                }

            }
            PropertyUpdated();
        }

        /// <summary>
        /// Sets the humidity value of the subsystem.
        /// </summary>
        /// <param name="reading">The new humidity reading.</param>
        public void SetHumidity(Reading<float> reading)
        {
            Subsystem.Humidity = reading;
            _humidityReadingsProcessedInBatch++;
            if (_humidityReadingsProcessedInBatch >= READINGS_BETWEEN_SAVES)
            {
                HistoricalSecurityDataRepo.GetInstance().UploadReading(reading);
                _humidityReadingsProcessedInBatch = 0;
            }
            PropertyUpdated();
        }

        /// <summary>
        /// Sets the water level value of the subsystem.
        /// </summary>
        /// <param name="reading">The new water level reading.</param>
        public void SetWaterLevel(Reading<float> reading)
        {
            Subsystem.WaterLevel = reading;
            _waterLevelReadingsProcessedInBatch++;
            if (_waterLevelReadingsProcessedInBatch >= READINGS_BETWEEN_SAVES)
            {
                HistoricalSecurityDataRepo.GetInstance().UploadReading(reading);
                _waterLevelReadingsProcessedInBatch = 0;
            }
            PropertyUpdated();
        }

        /// <summary>
        /// Asynchronously sets the state of the hardware light by making a call to the Telemetry Repository.
        /// </summary>
        /// <param name="state">The new state of the hardware light.</param>
        /// <returns><c>true</c> if the state was successfully set; otherwise, <c>false</c>.</returns>
        public async Task<bool> SetHardwareLightState(bool state)
        {
            return await TelemetryRepo.GetInstance().SetLEDState(state);
        }

        /// <summary>
        /// Sets the state of the hardware fan.
        /// </summary>
        /// <param name="state">The new state of the hardware fan.</param>
        /// <returns><c>true</c> if the state was successfully set; otherwise, <c>false</c>.</returns>
        public async Task<bool> SetHardwareFanState(bool state)
        {
            return await TelemetryRepo.GetInstance().SetFanState(state);
        }

        /// <summary>
        /// Sets the state of the hardware door.
        /// </summary>
        /// <param name="state">The new state of the hardware door.</param>
        /// <returns><c>true</c> if the state was successfully set; otherwise, <c>false</c>.</returns>
        public async Task<bool> SetHardwareLockState(bool state)
        {
            return await TelemetryRepo.GetInstance().SetLockState(state);
        }

    }
}