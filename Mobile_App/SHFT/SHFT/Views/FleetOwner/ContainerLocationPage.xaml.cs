// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// The ContainerLocationPage class is responsible for displaying location data and
// allowing the user to open a location in Google Maps.

namespace SHFT.Views;
using SHFT.Models;
using SHFT.Repos;
using System.Text;


public partial class ContainerLocationPage : ContentPage
{
    readonly GeoLocationSubsystem _subsystem;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerLocationPage"/> class with the GeoLocationSubsystem and sets the address text.
    /// </summary>
    public ContainerLocationPage()
    {
        InitializeComponent();
        _subsystem = GeoLocationController.GetInstance().Subsystem;
        BindingContext = _subsystem;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SetAddressText();
    }

    /// <summary>
    /// Gets address info based on the subsystem's current longitude and latitude values and displays it in the label.
    /// </summary>
    private async void SetAddressText()
    {
        lblAddress.Text = await GetAddressInfo((double)_subsystem.Latitude.Value, (double)_subsystem.Longitude.Value);
    }

    /// <summary>
    /// Opens Google Maps with the current subsystem longitude and latitude values.
    /// </summary>
    /// <param name="sender">The button pressed.</param>
    /// <param name="e">Event arguments associated with this event.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown when no map application is available to open.</exception>
    private async void BtnOpen_Clicked(object sender, EventArgs e)
    {
        // Get the location from longitude and latitude
        Location location = new(_subsystem.Latitude.Value, _subsystem.Longitude.Value);

        // Add the container name to the map launch options
        var options = new MapLaunchOptions { Name = "Container" };

        try
        {
            // Open maps
            await Map.OpenAsync(location, options);
        }
        catch (Exception)
        {
            // No map application available to open
            await DisplayAlert("Maps Unavailable", "No maps app is available to open.", "Ok");
        }
    }

    /// <summary>
    /// Gets address info based on the subsystem's current longitude and latitude values.
    /// </summary>
    /// <param name="latitude">The subsystem's current latitude value.</param>
    /// <param name="longitude">The subsystem's current longitude value.</param>
    /// <returns>A task which resolves with a string containing the container's current address as a string.</returns>
    async Task<string> GetAddressInfo(double latitude, double longitude)
    {
        const string NO_DATA = "No Location Data Available";

        IEnumerable<Placemark> placemarks;
        try
        {
            placemarks = await Geocoding.Default.GetPlacemarksAsync(latitude, longitude);
        }catch(Exception)
        {
            return NO_DATA;
        }

        Placemark placemark = placemarks?.FirstOrDefault();

        if (placemark != null)
        {
            // Build the address string.
            StringBuilder sb = new();
            if (!string.IsNullOrEmpty(placemark.SubThoroughfare))
                sb.Append($"{placemark.SubThoroughfare} ");
            if (!string.IsNullOrEmpty(placemark.Thoroughfare))
                sb.Append($"{placemark.Thoroughfare}, ");
            if (!string.IsNullOrEmpty(placemark.Locality))
                sb.Append($"{placemark.Locality}, ");
            if (!string.IsNullOrEmpty(placemark.AdminArea))
                sb.Append($"{placemark.AdminArea}, ");
            if (!string.IsNullOrEmpty(placemark.CountryName))
                sb.Append($"{placemark.CountryName}, ");
            if (!string.IsNullOrEmpty(placemark.PostalCode))
                sb.Append($"({placemark.PostalCode})");

            if (sb.Length != 0)
                return sb.ToString();
        }

        return NO_DATA;
    }
}