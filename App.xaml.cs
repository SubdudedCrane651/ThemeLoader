namespace ThemeLoader;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell(); // or new MainPage()
    }
}