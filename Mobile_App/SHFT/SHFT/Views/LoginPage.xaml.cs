// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// A page in which users can login.

namespace SHFT.Views;

using Firebase.Auth;
using Firebase.Auth.Providers;
using Newtonsoft.Json.Linq;
using SHFT.Models;
using SHFT.Repos;
using SHFT.Services;
using System.Text.Json;

public partial class LoginPage : ContentPage
{

    public const string LOGIN_SAVE_FILENAME = "remember_me.json";
    /// <summary>
    /// Initializes the login page.
    /// </summary>
	public LoginPage()
    {
        InitializeComponent();
        LoginWithRememberedInfo();

    }

    /// <summary>
    /// Routes the user to the signup page using the shell.
    /// </summary>
    /// <param name="sender">The pressed button.</param>
    /// <param name="e">Event arguments for this event.</param>
    private void btnToSignup_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(Router.SIGNUP);
    }

    /// <summary>
    /// Logs the user in. If the information provided is invalid or incomplete, an alert is shown.
    /// If the app does not have network access an error is also shown.
    /// </summary>
    /// <param name="sender">The pressed button.</param>
    /// <param name="e">Event arguments for this event.</param>
    private async void btnLogin_Clicked(object sender, EventArgs e)
    {
        string email = entryEmail.Text;
        string password = entryPassword.Text;
        Login(email, password);
    }

    private async void Login(string email, string password)
    {
        btnLogin.IsEnabled = false;
        LoadingPage loadingPage = new LoadingPage(async () =>
        {
            // Check internet access
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;
            if (accessType != NetworkAccess.Internet)
            {
                DisplayAlert("Login Failed", "SHFT can not access the internet. If you are connected to the internet, " +
                    "please make sure the app has permission to access the internet.", "Ok");
                return;
            }

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                DisplayAlert("Login Failed", "The information provided was incomplete. Please fill out all the fields.", "Ok");
                return;
            }

            try
            {
                // Check if the user exists
                FetchUserProvidersResult result = await AuthService.Client.FetchSignInMethodsForEmailAsync(email);
                if (!result.UserExists)
                {
                    DisplayAlert("Failed Login Request", $"The email ({email}) was not found.", "Ok");
                    return;
                }

                // Try to log the user in and store their creds
                UserCredential credential = await AuthService.Client.SignInWithEmailAndPasswordAsync(email, password);

                AccountType accountType = await AccountRepo.GetInstance().GetAccountType(email);
                AuthService.UserAccount = new(credential.User, credential.AuthCredential, credential.OperationType, accountType);

                // Save login info if desired
                if (rememberMeCheckBox.IsChecked)
                {
                    try
                    {
                        RememberLoginInfo(email, password);
                    }
                    catch
                    {
                        DisplayAlert("Oops", "Failed to save your login information. You'll have to manually login next time.", "Continue");
                    }
                }

                // Navigate to next page
                if (accountType == AccountType.FLEET_OWNER)
                {
                    await Shell.Current.GoToAsync(Router.CONTAINER_SELECT);
                }
                else if (accountType == AccountType.FARMING_TECH)
                    await Shell.Current.GoToAsync(Router.FARM_TECH_TABS);
                return;
            }
            catch (FirebaseAuthException ex)
            {
                // Display an alert if the login failed
                DisplayAlert("Failed Login", $"Failed to sign up for the following reason {ex.Reason}.", "OK");
                return;
            }
            catch (Exception ex)
            {
                // Display any exceptions that aren't firebase related. Should not be any.
                DisplayAlert("Failed Login", ex.ToString(), "Ok");
                return;
            }
        },
        "Logging In");
        await Navigation.PushAsync(loadingPage);
        loadingPage.StartTask();
        btnLogin.IsEnabled = true;
    }

    private void PasswordForgotBtn(object sender, EventArgs e)
    {
        //TODO: Replace with support URL
        string url = "https://github.com/jac-final-project-w23/course-project-shft";
        Launcher.OpenAsync(new Uri(url));
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

    class LoginInfo
    {
        public LoginInfo(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; set; }
        public string Password { get; set; }
    }

    private void RememberLoginInfo(string email, string password)
    { 

        string savingDirectory = FileSystem.Current.AppDataDirectory;
        string filePath = Path.Combine(savingDirectory, LOGIN_SAVE_FILENAME); //using System.IO;

        var info = new LoginInfo(email, password);
        string infoString = JsonSerializer.Serialize(info);
        File.WriteAllText(filePath, infoString);
    }

    private void LoginWithRememberedInfo()
    {
        string savingDirectory = FileSystem.Current.AppDataDirectory;
        string filePath = Path.Combine(savingDirectory, LOGIN_SAVE_FILENAME); //using System.IO;

        try
        {
            LoginInfo data = JsonSerializer.Deserialize<LoginInfo>(File.ReadAllText(filePath));
            Login(data.Email, data.Password);
            
        }catch(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }
}