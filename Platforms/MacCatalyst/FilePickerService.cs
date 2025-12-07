#if MACCATALYST
using UIKit;
using Foundation;
using UniformTypeIdentifiers;
using ThemeLoader.Services;

namespace ThemeLoader.Platforms.MacCatalyst;

public class FilePickerService : IFilePickerService
{
    public Task<string?> PickFileAsync(string[] allowedTypes = null)
    {
        var tcs = new TaskCompletionSource<string?>();

        var picker = new UIDocumentPickerViewController(
            allowedTypes ?? new string[] { UTType.SHSignatureContentType.Identifier },
            UIDocumentPickerMode.Import);

        picker.DidPickDocument += (sender, e) =>
        {
            var url = e.Url;
            tcs.TrySetResult(url?.Path);
        };

        picker.WasCancelled += (sender, e) =>
        {
            tcs.TrySetResult(null);
        };

        var root = UIApplication.SharedApplication
            .KeyWindow?
            .RootViewController;

        root?.PresentViewController(picker, true, null);

        return tcs.Task;
    }
}
#endif