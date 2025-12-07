namespace ThemeLoader.Services;

public interface IFilePickerService
{
    Task<string?> PickFileAsync(string[] allowedTypes = null);
}
