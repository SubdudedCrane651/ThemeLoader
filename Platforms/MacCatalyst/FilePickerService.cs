#if MACCATALYST
using UIKit;
using Foundation;
using UniformTypeIdentifiers;
using ThemeLoader.Services; // your shared interface namespace

namespace ThemeLoader.Platforms.MacCatalyst;

public class FilePickerService : IFilePickerService
{
    public Task<string?> PickFileAsync(string[] allowedTypes = null)
    {
        var tcs = new TaskCompletionSource<string?>();

        // Use UTType.Content if no specific types are passed
        var picker = new UIDocumentPickerViewController(
            allowedTypes ?? new string[] { UTType.SHSignatureContentType.Identifier },
            UIDocumentPickerMode.Import);

        // Handle selection
        picker.DidPickDocument += (sender, e) =>
        {
            tcs.TrySetResult(e.Url?.Path);
        };

        // Handle cancel
        picker.WasCancelled += (sender, e) =>
        {
            tcs.TrySetResult(null);
        };

        // Present on the active scene’s root controller
        var root = UIApplication.SharedApplication
            .ConnectedScenes
            .OfType<UIWindowScene>()
            .FirstOrDefault()?
            .Windows
            .FirstOrDefault(w => w.IsKeyWindow)?
            .RootViewController;

        root?.PresentViewController(picker, true, null);

        return tcs.Task;
    }
}
#endif