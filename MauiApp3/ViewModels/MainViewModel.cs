using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MauiApp3.Models;
using MauiApp3.Services;

namespace MauiApp3.ViewModels;

public class MainViewModel : INotifyPropertyChanged, IQueryAttributable
{
    private readonly IAppStateService _appStateService;
    private readonly Command _addStudentCommand;

    private string _fullName = string.Empty;
    private string _group = string.Empty;
    private double _averageScore;
    private string _sessionStatus = "Active";
    private bool _isLoaded;

    public ObservableCollection<Student> Students { get; } = new();

    public MainViewModel(IAppStateService appStateService)
    {
        _appStateService = appStateService;

        LoadCommand = new Command(LoadData);
        _addStudentCommand = new Command(AddStudent, CanAddStudent);
        OpenDetailsCommand = new Command<Student>(async (student) => await OpenDetailsAsync(student));
        OpenSettingsCommand = new Command(async () => await Shell.Current.GoToAsync("settings"));

        RestoreDraftFromState();
    }

    public string FullName
    {
        get => _fullName;
        set
        {
            if (_fullName != value)
            {
                _fullName = value;
                _appStateService.DraftFullName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SummaryText));
                _addStudentCommand.ChangeCanExecute();
            }
        }
    }

    public string Group
    {
        get => _group;
        set
        {
            if (_group != value)
            {
                _group = value;
                _appStateService.DraftGroup = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SummaryText));
            }
        }
    }

    public double AverageScore
    {
        get => _averageScore;
        set
        {
            if (Math.Abs(_averageScore - value) > 0.0001)
            {
                _averageScore = value;
                _appStateService.DraftScore = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ScoreIndicatorColor));
            }
        }
    }

    public string SessionStatus
    {
        get => _sessionStatus;
        set
        {
            if (_sessionStatus != value)
            {
                _sessionStatus = value;
                _appStateService.CurrentSessionStatus = value;
                OnPropertyChanged();
            }
        }
    }

    public string SummaryText =>
        $"Студент: {(string.IsNullOrWhiteSpace(FullName) ? "—" : FullName)}, " +
        $"група {(string.IsNullOrWhiteSpace(Group) ? "—" : Group)}";

    public Color ScoreIndicatorColor =>
        AverageScore >= 4.0 ? Colors.ForestGreen : Colors.IndianRed;

    public ICommand LoadCommand { get; }
    public ICommand AddStudentCommand => _addStudentCommand;
    public ICommand OpenDetailsCommand { get; }
    public ICommand OpenSettingsCommand { get; }

    public void RestoreDraftFromState()
    {
        _fullName = _appStateService.DraftFullName;
        _group = _appStateService.DraftGroup;
        _averageScore = _appStateService.DraftScore;
        _sessionStatus = _appStateService.CurrentSessionStatus;

        OnPropertyChanged(nameof(FullName));
        OnPropertyChanged(nameof(Group));
        OnPropertyChanged(nameof(AverageScore));
        OnPropertyChanged(nameof(SessionStatus));
        OnPropertyChanged(nameof(SummaryText));
        OnPropertyChanged(nameof(ScoreIndicatorColor));
        _addStudentCommand.ChangeCanExecute();
    }

    private void LoadData()
    {
        SessionStatus = _appStateService.CurrentSessionStatus;

        if (_isLoaded)
            return;

        _isLoaded = true;
        Students.Add(new Student { Id = 1, FullName = "Медведчук Андрій", Group = "4-14", AverageScore = 4.6 });
        Students.Add(new Student { Id = 2, FullName = "Коваленко Ірина", Group = "4-14", AverageScore = 3.8 });
    }

    private bool CanAddStudent() => !string.IsNullOrWhiteSpace(FullName);

    private void AddStudent()
    {
        var nextId = Students.Count > 0 ? Students.Max(s => s.Id) + 1 : 1;

        Students.Add(new Student
        {
            Id = nextId,
            FullName = FullName.Trim(),
            Group = Group.Trim(),
            AverageScore = Math.Round(AverageScore, 2)
        });

        FullName = string.Empty;
        Group = string.Empty;
        AverageScore = 0;
        _appStateService.SaveState();
    }

    private async Task OpenDetailsAsync(Student? student)
    {
        if (student is null)
            return;

        _appStateService.SelectedStudentId = student.Id;

        var parameters = new Dictionary<string, object>
        {
            { "SelectedStudent", student }
        };

        await Shell.Current.GoToAsync("studentdetail", parameters);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("UpdatedStudent", out var updatedObj) && updatedObj is Student updated)
        {
            if (query.TryGetValue("OriginalStudent", out var origObj) && origObj is Student original)
            {
                var index = Students.IndexOf(original);
                if (index >= 0)
                {
                    Students[index] = updated;
                    return;
                }
            }

            var byId = Students.FirstOrDefault(s => s.Id == updated.Id);
            if (byId is not null)
            {
                var idx = Students.IndexOf(byId);
                Students[idx] = updated;
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}