// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// This class serves as the controller for the Security Subsystem,
// allowing for interaction with and control of various security
// components within the farming container.

using SHFT.Models;
using SHFT.Repos;

namespace SHFT.Controllers
{
    /// <summary>
    /// Controls and sets properties of the Security Subsystem.
    /// Manages the interactions with different sensors and security aspects within the farming container.
    /// </summary>
    public class SecurityController
    {
        private static SecurityController _instance;
        private const int READINGS_BETWEEN_SAVES = 10;
        private int _noiseReadingsProcessedInBatch = 0;
        private int _luminosityReadingsProcessedInBatch = 0;
        private int _vibrationReadingsProcessedInBatch = 0;

        public delegate void VoidDelegate();

        /// <summary>
        /// An event which is invoked any time a property is changed.
        /// </summary>
        public event VoidDelegate PropertyUpdated;

        /// <summary>
        /// Constructor for SecurityController, initializes a new instance of the Security Subsystem.
        /// </summary>
        public SecurityController()
        {
            Subsystem = new SecuritySubsystem(this);
        }

        /// <summary>
        /// Returns a singleton instance of SecurityController. If an instance does not exist, it creates one.
        /// </summary>
        /// <returns>The singleton instance of SecurityController.</returns>
        public static SecurityController GetInstance()
        {
            if (_instance is null)
                _instance = new SecurityController();

            return _instance;
        }

        /// <summary>
        /// Gets the SecuritySubsystem object.
        /// </summary>
        /// <value>The SecuritySubsystem object.</value>
        public SecuritySubsystem Subsystem { get; private set; }

        /// <summary>
        /// Sets a new reading to the noise sensor and saves the data if the number of readings since the last save point has reached a certain limit.
        /// </summary>
        /// <param name="reading">A reading of noise level.</param>
        public void SetNoiseLevel(Reading<float> reading)
        {
            Subsystem.Noise = reading;
            _noiseReadingsProcessedInBatch++;
            if (_noiseReadingsProcessedInBatch >= READINGS_BETWEEN_SAVES)
            {
                HistoricalSecurityDataRepo.GetInstance().UploadReading(reading);
                _noiseReadingsProcessedInBatch = 0;
            }
            PropertyUpdated();
        }

        /// <summary>
        /// Sets a new reading to the luminosity sensor and saves the data if the number of readings since the last save point has reached a certain limit.
        /// </summary>
        /// <param name="reading">A reading of luminosity level.</param>>
        public void SetLuminosityLevels(Reading<int> reading)
        {
            Subsystem.Luminosity = reading;
            _luminosityReadingsProcessedInBatch++;
            if (_luminosityReadingsProcessedInBatch >= READINGS_BETWEEN_SAVES)
            {
                HistoricalSecurityDataRepo.GetInstance().UploadReading(reading);
                _luminosityReadingsProcessedInBatch = 0;
            }
            PropertyUpdated();
        }

        /// <summary>
        /// Sets a new reading to the vibration sensor and saves the data if the number of readings since the last save point has reached a certain limit.
        /// </summary>
        /// <param name="reading">A reading of vibration level.</param>
        public void SetVibrationLevels(Reading<float> reading)
        {
            Subsystem.Vibration = reading;
            _vibrationReadingsProcessedInBatch++;
            if (_vibrationReadingsProcessedInBatch >= READINGS_BETWEEN_SAVES)
            {
                HistoricalSecurityDataRepo.GetInstance().UploadReading(reading);
                _vibrationReadingsProcessedInBatch = 0;
            }
            PropertyUpdated();
        }

        /// <summary>
        /// Updates the motion sensor reading in the security subsystem and uploads the reading to the historical data repository.
        /// </summary>
        /// <param name="reading">Motion sensor reading.</param>
        public void SetMotionSensor(Reading<bool> reading)
        {
            Subsystem.MotionSensor = reading;
            HistoricalSecurityDataRepo.GetInstance().UploadReading(reading);
        }

        /// <summary>
        /// Controls the buzzer state in the security subsystem, updating it in the telemetry repository.
        /// </summary>
        /// <param name="state">Buzzer state.</param>
        /// <returns>The final buzzer state after attempting the change.</returns>
        public async Task<bool> SetHardwareBuzzerState(bool state)
        {
            return await TelemetryRepo.GetInstance().SetBuzzerState(state);
        }

        /// <summary>
        /// Controls the lock state in the security subsystem, updating it in the telemetry repository.
        /// </summary>
        /// <param name="state">Lock state.</param>
        /// <returns>The final lock state after attempting the change.</returns>
        public async Task<bool> SetHardwareLockState(bool state)
        {
            return await TelemetryRepo.GetInstance().SetLockState(state);
        }

    }
}
