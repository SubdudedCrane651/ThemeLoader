#if MACCATALYST
using UIKit;
using Foundation;
using UniformTypeIdentifiers;
using ThemeLoader.Services; // shared interface namespace

namespace ThemeLoader.Platforms.MacCatalyst;

public class FilePickerService : IFilePickerService
{
    public Task<string?> PickFileAsync(string[] allowedTypes = null)
    {
        var tcs = new TaskCompletionSource<string?>();

        // Replace UTType.Json.Identifier with the public identifier for JSON files
        var picker = new UIDocumentPickerViewController(
            allowedTypes ?? new string[] { "public.json" },
            UIDocumentPickerMode.Import);

        picker.DidPickDocument += (sender, e) =>
        {
            tcs.TrySetResult(e.Url?.Path);
        };

        picker.WasCancelled += (sender, e) =>
        {
            tcs.TrySetResult(null);
        };

        // Present from the active UIWindowScene
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