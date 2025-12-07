using Foundation;
using Microsoft.Maui; // Add this using directive
using Microsoft.Maui.Hosting; // Add this using directive

namespace ThemeLoader
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}