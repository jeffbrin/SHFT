// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// GeoLocationController: Singleton class responsible for managing the geo-location subsystem, including 
// setting sensor values and saving historical data.

using SHFT.Models;

namespace SHFT.Repos
{
    /// <summary>
    /// Singleton class responsible for managing the geo-location subsystem, including 
    /// setting sensor values and saving historical data.
    /// </summary>
    internal class GeoLocationController
    {
        private static GeoLocationController _instance;
        private const int READINGS_BETWEEN_SAVES = 10;
        private int _pitchReadingsProcessedInBatch = 0;
        private int _rollReadingsProcessedInBatch = 0;
        private int _vibrationReadingsProcessedInBatch = 0;

        public delegate void VoidDelegate();

        /// <summary>
        /// An event which is invoked any time a property is changed.
        /// </summary>
        public event VoidDelegate PropertyUpdated;

        /// <summary>
        /// Private constructor to ensure single instance creation via GetInstance method. 
        /// It initializes a new GeoLocationSubsystem.
        /// </summary>
        private GeoLocationController()
        {
            Subsystem = new GeoLocationSubsystem(this)
            {
                BuzzerState = new Reading<bool>() { },
                Longitude = new Reading<float>() { },
                Latitude = new Reading<float>() { },
                Pitch = new Reading<float>() { },
                Roll = new Reading<float>() { },
                Vibration = new Reading<float>() { },
            };
        }

        /// <summary>
        /// Provides access to the GeoLocationSubsystem associated with this controller.
        /// </summary>
        public GeoLocationSubsystem Subsystem { get; private set; }

        /// <summary>
        /// Ensures only one instance of GeoLocationController is created and returns it.
        /// </summary>
        /// <returns>The single instance of <see cref="GeoLocationController"/>.</returns>
        public static GeoLocationController GetInstance()
        {
            if (_instance is null)
                _instance = new GeoLocationController();
            return _instance;
        }

        #region Setters
        /// <summary>
        /// Updates the longitude reading of the GeoLocation subsystem.
        /// </summary>
        /// <param name="reading">The most recent longitude reading.</param>     
        public void SetLongitude(Reading<float> reading)
        {
            Subsystem.Longitude = reading;
            PropertyUpdated();
        }

        /// <summary>
        /// Updates the latitude reading of the GeoLocation subsystem.
        /// </summary>
        /// <param name="reading">The most recent latitude reading.</param>   
        public void SetLatitude(Reading<float> reading)
        {
            Subsystem.Latitude = reading;
            PropertyUpdated();

        }

        /// <summary>
        /// Updates the vibration reading of the GeoLocation subsystem and saves it periodically.
        /// </summary>
        /// <param name="reading">The most recent vibration reading.</param> 
        public void SetVibration(Reading<float> reading)
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
        /// Updates the pitch angle reading of the GeoLocation subsystem and saves it periodically. 
        public void SetPitch(Reading<float> reading)
        {
            Subsystem.Pitch = reading;
            _pitchReadingsProcessedInBatch++;
            if (_pitchReadingsProcessedInBatch >= READINGS_BETWEEN_SAVES)
            {
                HistoricalSecurityDataRepo.GetInstance().UploadReading(reading);
                _pitchReadingsProcessedInBatch = 0;
            }
            PropertyUpdated();
        }

        /// <summary>
        /// Updates the roll angle reading of the GeoLocation subsystem and saves it periodically.
        /// </summary>
        /// <param name="reading">The most recent roll angle reading.</param>  
        public void SetRoll(Reading<float> reading)
        {
            Subsystem.Roll = reading;
            _rollReadingsProcessedInBatch++;
            if (_rollReadingsProcessedInBatch >= READINGS_BETWEEN_SAVES)
            {
                HistoricalSecurityDataRepo.GetInstance().UploadReading(reading);
                _rollReadingsProcessedInBatch = 0;
            }
        }

        #endregion
    }
}
