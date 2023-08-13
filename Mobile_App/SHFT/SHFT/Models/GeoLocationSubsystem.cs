// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// A model which represents the geo location subsystem. Will be used to bind with the view.

using SHFT.Repos;
using System.ComponentModel;

namespace SHFT.Models
{
    /// <summary>
    /// The GeoLocationSubsystem model which contains properties for all values related to geo location.
    /// </summary>
    internal class GeoLocationSubsystem : INotifyPropertyChanged
    {
        private readonly GeoLocationController _controller;

        /// <summary>
        /// Sets the controller which is initializes this subsystem.
        /// </summary>
        /// <param name="controller">The controller which is storing and initializing this subsystem object.</param>
        public GeoLocationSubsystem(GeoLocationController controller)
        {
            _controller = controller;
            SetThresholds();
        }

        /// <summary>
        /// Sets the thresholds saved in the database.
        /// </summary>
        private void SetThresholds()
        {
            Task<Reading<object>> task = ThresholdRepo.GetInstance().GetMinThreshold(Reading<object>.TypeOptions.PITCH);
            task.Wait();
            Reading<object> minPitch = task.Result;
            if (minPitch is not null)
                MinimumPitch = Convert.ToSingle(minPitch.Value);

            task = ThresholdRepo.GetInstance().GetMaxThreshold(Reading<object>.TypeOptions.PITCH);
            task.Wait();
            Reading<object> maxPitch = task.Result;
            if (maxPitch is not null)
                MaximumPitch = Convert.ToSingle(maxPitch.Value);

            task = ThresholdRepo.GetInstance().GetMinThreshold(Reading<object>.TypeOptions.ROLL);
            task.Wait();
            Reading<object> minRoll = task.Result;
            if (minRoll is not null)
                MinimumRoll = Convert.ToSingle(minRoll.Value);

            task = ThresholdRepo.GetInstance().GetMaxThreshold(Reading<object>.TypeOptions.ROLL);
            task.Wait();
            Reading<object> maxRoll = task.Result;
            if (maxRoll is not null)
                MaximumRoll = Convert.ToSingle(maxRoll.Value);

            task = ThresholdRepo.GetInstance().GetMaxThreshold(Reading<object>.TypeOptions.VIBRATION);
            task.Wait();
            Reading<object> maxVibration = task.Result;
            if (maxVibration is not null)
                MaximumVibration = Convert.ToSingle(maxVibration.Value);
        }

        /// <summary>
        /// The minimum permitted longitude for this container subsystem.
        /// </summary>
        public float MinimumLongitude { get; set; }

        /// <summary>
        /// The maximum permitted longitude for this container subsystem.
        /// </summary>
        public float MaximumLongitude { get; set; }

        /// <summary>
        /// The minimum permitted latitude for this container subsystem.
        /// </summary>
        public float MinimumLatitude { get; set; }

        /// <summary>
        /// The maximum permitted latitude for this container subsystem.
        /// </summary>
        public float MaximumLatitude { get; set; }

        /// <summary>
        /// The maximum safe vibration level for this container subsystem.
        /// </summary>
        public float MaximumVibration { get; set; }

        /// <summary>
        /// The minimum permitted pitch angle for this container subsystem.
        /// </summary>
        public float MinimumPitch { get; set; }

        /// <summary>
        /// The maximum permitted pitch angle for this container subsystem.
        /// </summary>
        public float MaximumPitch { get; set; }

        /// <summary>
        /// The minimum permitted roll angle for this container subsystem.
        /// </summary>
        public float MinimumRoll { get; set; }

        /// <summary>
        /// The maximum permitted roll for this container subsystem.
        /// </summary>
        public float MaximumRoll { get; set; }

        /// <summary>
        /// The longitude of the farming container.
        /// </summary>
        public Reading<float> Longitude { get; set; }

        /// <summary>
        /// The latitude of the farming container.
        /// </summary>
        public Reading<float> Latitude { get; set; }

        /// <summary>
        /// The state of the buzzer in the farming container.
        /// </summary>
        public Reading<bool> BuzzerState { get; set; }

        /// <summary>
        /// The vibration level the farming container.
        /// </summary>
        public Reading<float> Vibration { get; set; }

        /// <summary>
        /// The pitch angle of the farming container.
        /// </summary>
        public Reading<float> Pitch { get; set; }

        /// <summary>
        /// The roll angle of the farming container.
        /// </summary>
        public Reading<float> Roll { get; set; }


        /// <summary>
        /// Event triggered when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
