// SHFT - H
// Winter 2023
// April 28th 2023
// Application Development III
// SecuritySubsystem is a class that represents a security subsystem containing all the necessary properties.

using SHFT.Controllers;
using SHFT.Repos;
using System.ComponentModel;

namespace SHFT.Models
{
    /// <summary>
    /// SecuritySubsystem is a class that represents a security subsystem containing all the necessary properties.
    /// </summary>
    public class SecuritySubsystem : INotifyPropertyChanged
    {
        private readonly SecurityController _controller;

        /// <summary>
        /// Initializes a new instance of the SecuritySubsystem class.
        /// </summary>
        /// <param name="repo">An instance of the SecurityController class</param>
        public SecuritySubsystem(SecurityController repo)
        {
            _controller = repo;
            Noise = new Reading<float> { };
            Luminosity = new Reading<int> { };
            MotionSensor = new Reading<bool> { };
            DoorLockState = new Reading<bool> { };
            BuzzerState = new Reading<bool> { };
            DoorState = new Reading<bool> { };
            Vibration = new Reading<float>();
            SetThresholds();
        }

        /// <summary>
        /// Sets the thresholds saved in the database.
        /// </summary>
        private void SetThresholds()
        {
            Task<Reading<object>> task = ThresholdRepo.GetInstance().GetMaxThreshold(Reading<object>.TypeOptions.NOISE);
            task.Wait();
            Reading<object> noiseReading = task.Result;
            if (noiseReading is not null)
                MaximumNoise = Convert.ToSingle(noiseReading.Value);

            task = ThresholdRepo.GetInstance().GetMaxThreshold(Reading<object>.TypeOptions.NOISE);
            task.Wait();
            Reading<object> vibrationReading = task.Result;
            if (vibrationReading is not null)
                MaximumVibration = Convert.ToSingle(vibrationReading.Value);
        }

        /// <summary>
        /// Gets or sets the maximum noise of the security system.
        /// </summary>
        public float MaximumNoise { get; set; }

        public float MinimumNoise { get; set; }

        public float MinimumLuminosity { get; set; }

        public float MaximumLuminosity { get; set; }

        /// <summary>
        /// Gets or sets the maximum vibration of the security system.
        /// </summary>
        public float MaximumVibration { get; set; }
        public float MinimumVibration { get; set; }


        /// <summary>
        /// Gets or sets the noise of the security system.
        /// </summary>
        public Reading<float> Noise { get; set; }

        /// <summary>
        /// Gets or sets the luminosity of the security system.
        /// </summary>
        public Reading<int> Luminosity { get; set; }

        /// <summary>
        /// Gets or sets the motion sensor state of the security system.
        /// </summary>
        public Reading<bool> MotionSensor { get; set; }

        /// <summary>
        /// Gets or sets the door lock state of the security system.
        /// </summary>
        public Reading<bool> DoorLockState { get; set; }

        /// <summary>
        /// Gets or sets the buzzer state of the security system.
        /// </summary>
        public Reading<bool> BuzzerState { get; set; }

        /// <summary>
        /// Gets or sets the door state of the security system.
        /// </summary>
        public Reading<bool> DoorState { get; set; }

        /// <summary>
        /// Gets or sets the vibration level of the security system.
        /// </summary>
        public Reading<float> Vibration { get; set; }

        /// <summary>
        /// Sets the buzzer state of the security system.
        /// </summary>
        /// <param name="state">A boolean value representing the buzzer state</param>
        public async Task<bool> SetBuzzerState(bool state)
        {
            BuzzerState = new Reading<bool> { Value = state, Unit = BuzzerState.Unit };
            return await _controller.SetHardwareBuzzerState(state);
        }

        /// <summary>
        /// Sets the door state of the security system.
        /// </summary>
        /// <param name="state">A boolean value representing the door state</param>
        public async Task<bool> SetDoorState(bool state)
        {
            DoorState = new Reading<bool> { Value = state, Unit = DoorState.Unit };
            return await _controller.SetHardwareLockState(state);
        }

        /// <summary>
        /// Sets the door lock state of the security system.
        /// </summary>
        /// <param name="state">A boolean value representing the door lock state</param>
        public async Task<bool> SetDoorlockState(bool state)
        {
            DoorLockState = new Reading<bool> { Value = state, Unit = DoorLockState.Unit };
            return await _controller.SetHardwareLockState(state);
        }

        /// <summary>
        /// Event triggered when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
