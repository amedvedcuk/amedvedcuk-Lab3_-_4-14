namespace MauiApp3.Services;

public interface IAppStateService
{
    string CurrentSessionStatus { get; set; }
    string DraftFullName { get; set; }
    string DraftGroup { get; set; }
    double DraftScore { get; set; }
    int? SelectedStudentId { get; set; }

    void SaveState();
    void RestoreState();
    void ApplySavedTheme();
    void SetTheme(AppTheme theme);
}

public class AppStateService : IAppStateService
{
    private const string ThemeKey = "app_theme";
    private const string SessionKey = "session_status";
    private const string DraftFullNameKey = "draft_fullname";
    private const string DraftGroupKey = "draft_group";
    private const string DraftScoreKey = "draft_score";

    public string CurrentSessionStatus { get; set; } = "Active";
    public string DraftFullName { get; set; } = string.Empty;
    public string DraftGroup { get; set; } = string.Empty;
    public double DraftScore { get; set; }
    public int? SelectedStudentId { get; set; }

    public void SaveState()
    {
        Preferences.Set(SessionKey, CurrentSessionStatus);
        Preferences.Set(DraftFullNameKey, DraftFullName ?? string.Empty);
        Preferences.Set(DraftGroupKey, DraftGroup ?? string.Empty);
        Preferences.Set(DraftScoreKey, DraftScore);
    }

    public void RestoreState()
    {
        CurrentSessionStatus = Preferences.Get(SessionKey, "Active");
        DraftFullName = Preferences.Get(DraftFullNameKey, string.Empty);
        DraftGroup = Preferences.Get(DraftGroupKey, string.Empty);
        DraftScore = Preferences.Get(DraftScoreKey, 0.0);
    }

    public void ApplySavedTheme()
    {
        var saved = Preferences.Get(ThemeKey, AppTheme.Unspecified.ToString());
        if (Enum.TryParse<AppTheme>(saved, out var theme) && Application.Current is not null)
        {
            Application.Current.UserAppTheme = theme;
        }
    }

    public void SetTheme(AppTheme theme)
    {
        Preferences.Set(ThemeKey, theme.ToString());
        if (Application.Current is not null)
        {
            Application.Current.UserAppTheme = theme;
        }
    }
}