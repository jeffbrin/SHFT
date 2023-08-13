// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// This class represents the page in which users can sign up for SHFT.

namespace SHFT.Views;

using Firebase.Auth;
using SHFT.Models;
using SHFT.Repos;
using SHFT.Services;

public partial class SignupPage : ContentPage
{
    private AccountType accountType = AccountType.FARMING_TECH;

    Color backgroundSelected = Color.FromRgba("#CCC");
    Color backgroundDefault = Color.FromRgba("#FFF");
    Color borderSelected = Color.FromRgba("#FFF");
    Color borderDefault = Color.FromRgba("#FFF");

    /// <summary>
    /// The constructor which initializes the SignupPage and sets the default account type.
    /// </summary>
	public SignupPage()
    {
        InitializeComponent();
        SetSelectionColors();
        SetAccountType(AccountType.FLEET_OWNER);

    }

    /// <summary>
    /// Navigates the user to the login page using the shell.
    /// </summary>
    /// <param name="sender">The pressed button.</param>
    /// <param name="e">The event arguments for this event.</param>
    private void btnToLogin_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(Router.LOGIN);
    }

    /// <summary>
    /// Signs the user up.
    /// </summary>
    /// <param name="sender">The pressed button.</param>
    /// <param name="e">The event arguments for this event.</param>
    private async void btnSignup_Clicked(object sender, EventArgs e)
    {
        string email = entryEmail.Text;
        string password = entryPassword.Text;
        string displayName = entryDisplayName.Text;

        btnSignup.IsEnabled = false;

       LoadingPage loadingPage = new LoadingPage(async () =>
        {
            // Make sure all fields are filled
            if (string.IsNullOrWhiteSpace(entryEmail.Text)
                || string.IsNullOrWhiteSpace(entryPassword.Text)
                || string.IsNullOrWhiteSpace(entryConfirmPassword.Text)
                || string.IsNullOrWhiteSpace(entryDisplayName.Text))
            {
                await DisplayAlert("Signup Failed", "The information provided was incomplete. Please fill out all the fields.", "Ok");
                return;
            }

            // Make sure passwords match
            if (entryPassword.Text != entryConfirmPassword.Text)
            {
                await DisplayAlert("Signup Failed", "The passwords do not match.", "Ok");
                return;
            }

            // Check for wifi
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;
            if (accessType != NetworkAccess.Internet)
            {
                await DisplayAlert("Signup Failed", "SHFT can not access the internet. If you are connected to the internet, " +
                    "please make sure the app has permission to access the internet.", "Ok");
                return;
            }

            Account account;
            try
            {
                // Create the account
                UserCredential credential = await AuthService.Client.CreateUserWithEmailAndPasswordAsync(email, password, displayName);
                account = new(credential.User, credential.AuthCredential, credential.OperationType, accountType);

                // Navigate to next page
                if (accountType == AccountType.FLEET_OWNER)
                    await Shell.Current.GoToAsync(Router.CONTAINER_SELECT);
                else if (accountType == AccountType.FARMING_TECH)
                    await Shell.Current.GoToAsync(Router.FARM_TECH_TABS);
            }
            catch (FirebaseAuthException ex)
            {
                // Display any reason for a failed sign up
                await DisplayAlert("Failed Signup", $"Failed to sign up for the following reason {ex.Reason}.", "OK");
                return;
            }
            catch (Exception ex)
            {
                // Display any random exceptions that slip through. Should not be any
                await DisplayAlert("Failed Signup", ex.ToString(), "Ok");
                return;
            }

            // Add a firebase database with account types by user and add their account type.
            AccountRepo.GetInstance().AddAccount(new DatabaseAccount() { Email = email, AccountType = accountType });
            AuthService.UserAccount = account;
        },
        "Signing Up");
        await Navigation.PushAsync(loadingPage);
        loadingPage.StartTask();

       btnSignup.IsEnabled = true;
        
    }

    /// <summary>
    /// Selects the farming tech account type for the new user.
    /// </summary>
    /// <param name="sender">The pressed button.</param>
    /// <param name="e">Event arguments for this event.</param>
    private void frameFarmingTech_Clicked(object sender, TappedEventArgs e)
    {
        SetAccountType(AccountType.FARMING_TECH);
    }

    /// <summary>
    /// Selects the fleet owner account type for the new user.
    /// </summary>
    /// <param name="sender">The pressed button.</param>
    /// <param name="e">Event arguments for this event.</param>
    private void frameFleetOwner_Clicked(object sender, TappedEventArgs e)
    {
        SetAccountType(AccountType.FLEET_OWNER);
    }

    /// <summary>
    /// Sets the account type for the new user and highlights the appropriate frame.
    /// </summary>
    /// <param name="type">The <see cref="AccountType"/> for this user.</param>
    private void SetAccountType(AccountType type)
    {
        this.accountType = type;

        switch (type)
        {
            case AccountType.FARMING_TECH:
                {
                    frameFarmingTech.BackgroundColor = backgroundSelected;
                    frameFarmingTech.BorderColor = borderSelected;
                    frameFleetOwner.BackgroundColor = backgroundDefault;
                    frameFleetOwner.BorderColor = borderDefault;
                    break;
                }
            case AccountType.FLEET_OWNER:
                {
                    frameFarmingTech.BackgroundColor = backgroundDefault;
                    frameFarmingTech.BorderColor = borderDefault;
                    frameFleetOwner.BackgroundColor = backgroundSelected;
                    frameFleetOwner.BorderColor = borderSelected;
                    break;
                }
        }
    }

    private void OpenPlanLink(object sender, EventArgs e)
    {
        //TODO: Replace with support URL
        string url = "https://github.com/jac-final-project-w23/course-project-shft";
        Launcher.OpenAsync(new Uri(url));
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

    private void TogglePasswordVisibility(object sender, EventArgs e)
    {
        entryPassword.IsPassword = !entryPassword.IsPassword;
        if (entryPassword.IsPassword)
        {
            toggleImage.Source = "closed_eye";
        }
        else
        {
            toggleImage.Source = "open_eye";
        }
    }
    private void ToggleConfirmPasswordVisibility(object sender, EventArgs e)
    {
        entryConfirmPassword.IsPassword = !entryConfirmPassword.IsPassword;
        if (entryConfirmPassword.IsPassword)
        {
            toggleConfirmImage.Source = "closed_eye";
        }
        else
        {
            toggleConfirmImage.Source = "open_eye";
        }
    }
}