using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MauiApp3.Services;

namespace MauiApp3.ViewModels;

public class SettingsViewModel : INotifyPropertyChanged
{
    private readonly IAppStateService _appStateService;
    private bool _isDarkTheme;

    public SettingsViewModel(IAppStateService appStateService)
    {
        _appStateService = appStateService;
        _isDarkTheme = Preferences.Get("app_theme", AppTheme.Unspecified.ToString()) == AppTheme.Dark.ToString();

        SetLightThemeCommand = new Command(() => ApplyTheme(AppTheme.Light));
        SetDarkThemeCommand = new Command(() => ApplyTheme(AppTheme.Dark));
    }

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            if (_isDarkTheme != value)
            {
                _isDarkTheme = value;
                OnPropertyChanged();
                ApplyTheme(value ? AppTheme.Dark : AppTheme.Light);
            }
        }
    }

    public string CurrentThemeLabel => IsDarkTheme ? "Поточна тема: Темна" : "Поточна тема: Світла";

    public ICommand SetLightThemeCommand { get; }
    public ICommand SetDarkThemeCommand { get; }

    private void ApplyTheme(AppTheme theme)
    {
        _isDarkTheme = theme == AppTheme.Dark;
        _appStateService.SetTheme(theme);
        OnPropertyChanged(nameof(IsDarkTheme));
        OnPropertyChanged(nameof(CurrentThemeLabel));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}