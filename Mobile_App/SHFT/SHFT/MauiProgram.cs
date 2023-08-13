using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Syncfusion.Maui.Core.Hosting;
using System.Reflection;

namespace SHFT;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.ConfigureSyncfusionCore();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.UseSkiaSharp();
        var a = Assembly.GetExecutingAssembly();
        using var stream = a.GetManifestResourceStream("SHFT.appsettings.json");

        var config = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();
        builder.Configuration.AddConfiguration(config);
        var app = builder.Build();
        Services = app.Services;
        return app;

    }
    //Service Property need to access the services in the app
    public static IServiceProvider Services { get; private set; }
}
