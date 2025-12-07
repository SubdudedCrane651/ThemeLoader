#if WINDOWS
using Microsoft.Maui.Storage;
using ThemeLoader.Services;

namespace ThemeLoader.Platforms.Windows;

public class FilePickerService : IFilePickerService
{
    public async Task<string?> PickFileAsync(string[] allowedTypes = null)
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Select a file",
            FileTypes = allowedTypes != null ? new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, allowedTypes }
            }) : null
        });

        return result?.FullPath;
    }
}
#endif