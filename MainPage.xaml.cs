using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ThemeLoader.Services;

namespace ThemeLoader;

public partial class MainPage : ContentPage
{
    ThemeModel? _model;
    string? _baseFolder;
    IDispatcherTimer? _timer;
    int _index = 0;
    readonly IFilePickerService _filePicker;

// MAUI needs this
    public MainPage() : this(
        Microsoft.Maui.Controls.Application.Current?.Handler?.MauiContext?.Services.GetService<IFilePickerService>()
        ?? throw new InvalidOperationException("FilePickerService not registered"))
    {
    }

    public MainPage(IFilePickerService filePicker)
    {
        InitializeComponent();
        _filePicker = filePicker;

        IntervalSlider.ValueChanged += (_, e) =>
        {
            IntervalLabel.Text = $"{(int)e.NewValue}s";
        };
    }

    async void OnBrowseClicked(object sender, EventArgs e)
    {
        var path = await _filePicker.PickFileAsync();
        if (string.IsNullOrEmpty(path)) return;

        _baseFolder = Path.GetDirectoryName(path);

        try
        {
            var json = File.ReadAllText(path);
            _model = JsonSerializer.Deserialize<ThemeModel>(json);

            if (_model != null)
                await DisplayAlert("Theme Loaded",
                    $"Loaded {_model.DisplayName} with {_model.Wallpapers.Count} wallpapers.",
                    "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to read theme.json: {ex.Message}", "OK");
        }
    }

    void OnStartSlideshowClicked(object sender, EventArgs e)
    {
        if (_model == null || string.IsNullOrEmpty(_baseFolder)) return;

        _timer?.Stop();
        _index = 0;

        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds((int)IntervalSlider.Value);
        _timer.Tick += (_, __) =>
        {
            if (_model!.Wallpapers.Count == 0) return;

            var rel = _model.Wallpapers[_index % _model.Wallpapers.Count];
            var full = Path.Combine(_baseFolder!, rel);

            if (File.Exists(full))
                WallpaperImage.Source = ImageSource.FromFile(full);

            _index++;
        };

        _timer.Start();
    }

    void OnStopSlideshowClicked(object sender, EventArgs e)
    {
        _timer?.Stop();
    }
}

public class ThemeModel
{
    public string DisplayName { get; set; } = "Theme";
    public List<string> Wallpapers { get; set; } = new();
    public string AccentColor { get; set; } = "#0078D7";
    public bool IsDarkMode { get; set; }
}