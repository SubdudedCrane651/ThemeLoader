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

        var allowed = (allowedTypes ?? Array.Empty<string>())
                      .Select(e => e.Trim().ToLowerInvariant())
                      .ToHashSet();

        var controller = new UIDocumentPickerViewController(
            new string[] { "public.data" }, // allow general files
            UIDocumentPickerMode.Open);

        controller.AllowsMultipleSelection = false;

        controller.DidPickDocumentAtUrls += (sender, args) =>
        {
            var url = args.Urls?.FirstOrDefault();
            if (url != null && url.IsFileUrl)
            {
                var path = url.Path;
                if (allowed.Count == 0 || allowed.Contains(System.IO.Path.GetExtension(path).ToLowerInvariant()))
                    tcs.TrySetResult(path);
                else
                    tcs.TrySetResult(null);
            }
            else
            {
                tcs.TrySetResult(null);
            }
        };

        controller.WasCancelled += (sender, e) =>
        {
            tcs.TrySetResult(null);
        };

        var vc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
        vc?.PresentViewController(controller, true, null);

        return tcs.Task;
    }
}
#endif