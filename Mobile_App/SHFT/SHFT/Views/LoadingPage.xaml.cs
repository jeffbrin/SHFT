
namespace SHFT.Views;

    /// <summary>
    /// A page which indicates loading.
    /// </summary>
public partial class LoadingPage : ContentPage
{

    private DateTime lastTime;
    private const int FULL_ROT_SECONDS = 1;
    private bool taskComplete = false;
    private RunInBackground methodToWaitOn;

    public delegate Task RunInBackground();

    /// <summary>
    /// Creates the loading page and starts the task.
    /// </summary>
    /// <param name="methodToWaitOn">The method which should run in the background.</param>
    /// <param name="loadingText">The text to display while loading.</param>
    public LoadingPage(RunInBackground methodToWaitOn, string loadingText = "Loading...")
    {

        InitializeComponent();
        lastTime = DateTime.Now;

        loadingLabel.Text = loadingText;
        StartRotation();
        this.methodToWaitOn = methodToWaitOn;
    }

    public async void StartTask()
    {
        await RunTask(methodToWaitOn);
    }

    /// <summary>
    /// Runs the desired task and ends the looping.
    /// </summary>
    /// <param name="task">A task to run in the background. The loading state.</param>
    private async Task RunTask(RunInBackground task)
    {
        await task();
        taskComplete = true;

        if (Navigation.NavigationStack[Navigation.NavigationStack.Count-1] == this) 
            try
            {
                await Navigation.PopAsync();
                System.Diagnostics.Debug.WriteLine(Navigation.NavigationStack);
            }
            catch(Exception ex) 
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            

        
    }

    /// <summary>
    /// Rotates the smiley face to indicate loading.
    /// </summary>
    private async void Rotate()
    {
        while (!taskComplete)
        {
            DateTime now = DateTime.Now;
            double dt = (lastTime - now).TotalSeconds;
            await smileImage.RelRotateTo(-360 * dt * FULL_ROT_SECONDS);
            lastTime = now;
        }
    }

    /// <summary>
    /// Starts the rotation thread.
    /// </summary>
    private void StartRotation()
    {
        Thread thread = new Thread(new ThreadStart(Rotate));
        thread.Start();
    }
}
