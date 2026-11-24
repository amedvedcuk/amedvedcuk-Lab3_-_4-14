using System.Diagnostics;
using MauiApp3.Services;

namespace MauiApp3;

public partial class App : Application
{
    private readonly IAppStateService _appStateService;

    public App(IAppStateService appStateService)
    {
        InitializeComponent();
        _appStateService = appStateService;
        _appStateService.ApplySavedTheme();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell());

        window.Created += (s, e) =>
            Debug.WriteLine("[Lifecycle] Window Created");

        window.Activated += (s, e) =>
            Debug.WriteLine("[Lifecycle] Window Activated");

        window.Resumed += (s, e) =>
        {
            _appStateService.CurrentSessionStatus = "Resumed";
            _appStateService.RestoreState();
            Debug.WriteLine($"[Lifecycle] Window Resumed | Status: {_appStateService.CurrentSessionStatus}");
        };

        window.Stopped += (s, e) =>
        {
            _appStateService.CurrentSessionStatus = "Stopped";
            _appStateService.SaveState();
            Debug.WriteLine("[Lifecycle] Window Stopped | State saved");
        };

        window.Destroying += (s, e) =>
        {
            _appStateService.CurrentSessionStatus = "Destroying";
            _appStateService.SaveState();
            Debug.WriteLine("[Lifecycle] Window Destroying | Resources/state finalized");
        };

        return window;
    }
}