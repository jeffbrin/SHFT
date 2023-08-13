using Microsoft.Extensions.Configuration;
using SHFT.Config;
using SHFT.Models;

namespace SHFT;

public partial class App : Application
{

    public static ConnectionSettings connectionsApp { get; private set; }
	= MauiProgram.Services.GetService<IConfiguration>()
	.GetRequiredSection(nameof(ConnectionSettings)).Get<ConnectionSettings>();


    public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}
}
