using MauiApp3.Views;

namespace MauiApp3;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("studentdetail", typeof(StudentDetailPage));
        Routing.RegisterRoute("settings", typeof(SettingsPage));
    }
}