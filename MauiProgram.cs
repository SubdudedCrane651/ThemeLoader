using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using ThemeLoader.Services;

namespace ThemeLoader;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });


#if WINDOWS
        builder.Services.AddSingleton<IFilePickerService, Platforms.Windows.FilePickerService>();
#elif MACCATALYST
        builder.Services.AddSingleton<IFilePickerService, Platforms.MacCatalyst.FilePickerService>();
#endif
        return builder.Build();
    }
}
