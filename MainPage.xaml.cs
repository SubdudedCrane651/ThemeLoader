using System.Text.Json;
#if MACCATALYST
using AppKit;
using Foundation;
#endif

namespace ThemeLoader;

public partial class MainPage : ContentPage
{
    ThemeModel? _model;
    string? _baseFolder;
    IDispatcherTimer? _timer;

    public MainPage()
    {
        InitializeComponent();
        IntervalSlider.ValueChanged += (_, e) => IntervalLabel.Text = ((int)e.NewValue).ToString();
    }

    async void OnBrowseClicked(object sender, EventArgs e)
    {
        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Select theme.json",
            FileTypes = FilePickerFileType.Json
        });

        if (result == null) return;

        JsonPathEntry.Text = result.FullPath;
        _baseFolder = Path.GetDirectoryName(result.FullPath);
        WallpapersFolderEntry.Text = Path.Combine(_baseFolder!, "wallpapers");

        try
        {
            var json = File.ReadAllText(result.FullPath);
            _model = JsonSerializer.Deserialize<ThemeModel>(json);

            StatusLabel.Text = _model == null
                ? "Failed to parse theme.json"
                : $"Loaded theme: {_model.DisplayName} ({_model.Wallpapers?.Count ?? 0} wallpapers)";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Error reading theme.json: {ex.Message}";
        }
    }

    void OnApplyOnceClicked(object sender, EventArgs e)
    {
#if MACCATALYST
        if (!EnsureModel()) return;

        var last = _model!.Wallpapers.LastOrDefault();
        if (last == null) { StatusLabel.Text = "No wallpapers found."; return; }

        var full = Path.Combine(_baseFolder!, last);
        if (!File.Exists(full)) { StatusLabel.Text = "Wallpaper file not found."; return; }

        ApplyDesktopImage(full);
        StatusLabel.Text = $"Applied: {Path.GetFileName(full)}";
#else
        StatusLabel.Text = "This action is only supported on MacCatalyst.";
#endif
    }

    void OnStartSlideshowClicked(object sender, EventArgs e)
    {
#if MACCATALYST
        if (!EnsureModel()) return;

        var seconds = (int)IntervalSlider.Value;
        _timer?.Stop();

        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(seconds);

        int index = 0;
        _timer.Tick += (_, __) =>
        {
            if (_model!.Wallpapers.Count == 0) return;

            var rel = _model.Wallpapers[index % _model.Wallpapers.Count];
            var full = Path.Combine(_baseFolder!, rel);

            if (File.Exists(full))
            {
                ApplyDesktopImage(full);
                StatusLabel.Text = $"Slideshow: {Path.GetFileName(full)}";
            }

            index++;
        };

        _timer.Start();
        StatusLabel.Text = $"Slideshow started (every {seconds}s)";
#else
        StatusLabel.Text = "This action is only supported on MacCatalyst.";
#endif
    }

    void OnStopSlideshowClicked(object sender, EventArgs e)
    {
        _timer?.Stop();
        StatusLabel.Text = "Slideshow stopped.";
    }

    bool EnsureModel()
    {
        if (_model == null || string.IsNullOrEmpty(_baseFolder))
        {
            StatusLabel.Text = "Load theme.json first.";
            return false;
        }
        return true;
    }

#if MACCATALYST
    void ApplyDesktopImage(string fullPath)
    {
        var ws = NSWorkspace.SharedWorkspace;
        var screen = NSScreen.MainScreen;
        var url = NSUrl.FromFilename(fullPath);
        ws.SetDesktopImageUrl(url, screen, new NSDictionary());
    }
#endif
}

// Simple file type constraint for FilePicker
public static class FilePickerFileType
{
    public static FilePickerFileType Json => new(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        { DevicePlatform.MacCatalyst, new[] { "public.json" } },
        { DevicePlatform.iOS, new[] { "public.json" } },
        { DevicePlatform.WinUI, new[] { ".json" } },
        { DevicePlatform.Android, new[] { "application/json" } },
        { DevicePlatform.TvOS, new[] { "public.json" } }
    });
}

// Shared model
public class ThemeModel
{
    public string DisplayName { get; set; } = "Theme";
    public List<string> Wallpapers { get; set; } = new();
    public string AccentColor { get; set; } = "#0078D7";
    public bool IsDarkMode { get; set; }
}