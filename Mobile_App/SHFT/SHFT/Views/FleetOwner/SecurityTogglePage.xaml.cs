// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// This class represents the view for the Security Toggle Page.

namespace SHFT.Views;

using Microsoft.Maui.Controls;
using SHFT.Controllers;
using SHFT.Repos;
using SHFT.Services;
using System.Reflection;

public partial class SecurityTogglePage : ContentPage
{

    bool _ignoreNextLockSwitch = false;
    bool _ignoreNextBuzzerSwitch = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecurityTogglePage"/> class.
    /// </summary>
    public SecurityTogglePage()
    {
        InitializeComponent();
        BindingContext = SecurityController.GetInstance().Subsystem;
    }

    /// <summary>
    /// Sets the state of the buzzer.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    /// <returns>Void.</returns>
    private async void ControlBuzzer_Toggle(object sender, ToggledEventArgs e)
    {

        bool buzzControl = e.Value;
        Switch swc = sender as Switch;
        swc.IsEnabled = false;

        if (_ignoreNextBuzzerSwitch)
        {
            _ignoreNextBuzzerSwitch = false;
            return;
        }

        bool success = false;
        if (ConnectedToWifi())
            success = await SecurityController.GetInstance().Subsystem.SetBuzzerState(buzzControl);


        if (success)
            swc.IsToggled = buzzControl;
        else
        {
            _ignoreNextBuzzerSwitch = true;
            swc.IsToggled = !buzzControl;
            FailToToggle();
        }
        swc.IsEnabled = true;

    }

    /// <summary>
    /// Sets the state of the door lock.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    /// <returns>Void.</returns>
    private async void ControlDoorLock_Toggle(object sender, ToggledEventArgs e)
    {
        bool doorlockstate = e.Value;
        Switch swc = sender as Switch;
        swc.IsEnabled = false;

        if (_ignoreNextLockSwitch)
        {
            _ignoreNextLockSwitch = false;
            return;
        }

        bool success = false;
        if (ConnectedToWifi())
        {
            success = await SecurityController.GetInstance().Subsystem.SetDoorlockState(doorlockstate);
        }

        if (success)
            swc.IsToggled = doorlockstate;
        else
        {
            _ignoreNextLockSwitch = true;
            swc.IsToggled = !doorlockstate;
            FailToToggle();
        }
        swc.IsEnabled = true;
    }

    private void FailToToggle()
    {
        DisplayAlert("Oops", "The state couldn't be set. This may be because the farm container is not connected, or because you aren't connected to wifi..", "Ok");
    }

    /// <summary>
    /// Returns whether the app is connected to wifi.
    /// </summary>
    /// <returns>True if the app is connected, false otherwise.</returns>
    private bool ConnectedToWifi()
    {
        NetworkAccess accessType = Connectivity.Current.NetworkAccess;
        return accessType == NetworkAccess.Internet;
    }

    private async void NewTelemetryIntervalButton_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(telemetryIntervalEntry.Text))
        {
            await DisplayAlert("Invalid Input", "Data upload interval must be a number.", "Ok");
            return;
        }

        double val;
        if (!double.TryParse(telemetryIntervalEntry.Text, out val))
        {
            await DisplayAlert("Invalid Input", "Data upload interval must be a number.", "Ok");
            return;
        }

        try
        {
            await ConnectionManager.GetInstance().SetDeviceTwin(TelemetryRepo.TELEMETRY_INTERVAL_TWIN_NAME, val.ToString());
            await DisplayAlert("Success", $"Successfully set data upload interval to {val}.", "Ok");
        }catch(Exception ex)
        {
            await DisplayAlert("Error", "There was a server side error setting the new data upload value. Try again in a moment.", "Ok");
        }
    }
}