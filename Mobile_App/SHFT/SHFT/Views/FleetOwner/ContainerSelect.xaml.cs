// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// This class is used to select container's which the user has available.


using SHFT.Config;
using SHFT.Models;
using SHFT.Repos;
using SHFT.Services;

namespace SHFT.Views;

public partial class ContainerSelect : ContentPage
{
    Color backgroundSelected = Color.FromRgba("#CCC");
    Color borderSelected = Color.FromRgba("#FFF");
    Color backgroundDefault = Color.FromRgba("#FFF");
    Color borderDefault = Color.FromRgba("#CCC");

    private readonly ConnectionManager _connectionManager;

    /// <summary>
    /// Initializes the container select.
    /// </summary>
    public ContainerSelect()
    {
        InitializeComponent();
        SetSelectionColors();
        _connectionManager = ConnectionManager.GetInstance();
        SetItemsSource();
    }

    private void SetSelectionColors()
    {
        bool hasValue = App.Current.Resources.TryGetValue("OrangeAccent", out object color);
        if (hasValue)
            backgroundSelected = (Color)color;

        hasValue = App.Current.Resources.TryGetValue("Primary", out object borderColor);
        if (hasValue)
            borderSelected = (Color)borderColor;
    }

    private async void SetItemsSource()
    {
        collectionViewContainers.ItemsSource = await ContainerRepo.GetInstance().GetContainers();
    }

    /// <summary>
    /// Navigates the user to the Dashboard.
    /// </summary>
    /// <param name="sender">Object to represent the button.</param>
    /// <param name="e">Event related to the button.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    private async void ContainerClicked(object sender, EventArgs e)
    {
        //Light the frame up
        var senderFrame = (Frame)sender;
        senderFrame.BackgroundColor = backgroundSelected;
        senderFrame.BorderColor = borderSelected;


        LoadingPage loadingPage = new LoadingPage(async () =>
        {
            string deviceId = ((sender as Element).BindingContext as Container).DeviceId;
            _connectionManager.Connect(App.connectionsApp.DeviceId);
            await Shell.Current.GoToAsync(Router.FLEET_OWNER_TABS);
            senderFrame.BackgroundColor = backgroundDefault; 
            senderFrame.BorderColor = borderDefault;
        },
        "Connecting...");
        await Navigation.PushAsync(loadingPage);
        loadingPage.StartTask();



    }

    private void FindContainerBtn_Clicked(object sender, EventArgs e)
    {
        //TODO: Replace with support URL
        string url = "https://github.com/jac-final-project-w23/course-project-shft";
        Launcher.OpenAsync(new Uri(url));
    }
}