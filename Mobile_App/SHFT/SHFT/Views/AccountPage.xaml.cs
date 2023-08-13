using SHFT.Services;

namespace SHFT.Views;

public partial class AccountPage : ContentPage
{
	public AccountPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BindingContext = AuthService.UserAccount;
    }

    private void logoutButton_Clicked(object sender, EventArgs e)
	{
		AuthService.UserAccount = null;

        string savingDirectory = FileSystem.Current.AppDataDirectory;
        string filePath = Path.Combine(savingDirectory, LoginPage.LOGIN_SAVE_FILENAME); //using System.IO;

		// File may not exist
		try
		{
			File.Delete(filePath);
		}
		catch
		{

		}

        Shell.Current.GoToAsync(Router.LOGIN);
	}
}