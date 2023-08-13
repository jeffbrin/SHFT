using SHFT.Config;
namespace SHFT;

public partial class AppShell : Shell
{
    public AppShell()
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(ResourceStrings.SYNCFUSION_LICENSE_KEY);
        InitializeComponent();
    }
}
