using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MauiApp3.Models;

namespace MauiApp3.ViewModels;

public class StudentDetailViewModel : IQueryAttributable, INotifyPropertyChanged
{
    private Student _originalStudent = new();
    private int _studentId;
    private string _fullName = string.Empty;
    private string _group = string.Empty;
    private double _averageScore;

    public StudentDetailViewModel()
    {
        GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        SaveCommand = new Command(async () => await SaveAsync());
    }

    public int StudentId
    {
        get => _studentId;
        set
        {
            if (_studentId != value)
            {
                _studentId = value;
                OnPropertyChanged();
            }
        }
    }

    public string FullName
    {
        get => _fullName;
        set
        {
            if (_fullName != value)
            {
                _fullName = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }
    }

    public ICommand GoBackCommand { get; }
    public ICommand SaveCommand { get; }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("SelectedStudent", out var value) && value is Student student)
        {
            _originalStudent = student;
            StudentId = student.Id;
            FullName = student.FullName;
            Group = student.Group;
            AverageScore = student.AverageScore;
            return;
        }

        if (query.TryGetValue("id", out var idObj) && int.TryParse(idObj?.ToString(), out var parsedId))
        {
            StudentId = parsedId;
            FullName = $"Студент #{parsedId}";
            Group = "4-14";
            AverageScore = 4.2;
            _originalStudent = new Student
            {
                Id = StudentId,
                FullName = FullName,
                Group = Group,
                AverageScore = AverageScore
            };
        }
    }

    private async Task SaveAsync()
    {
        var updatedStudent = new Student
        {
            Id = StudentId,
            FullName = FullName.Trim(),
            Group = Group.Trim(),
            AverageScore = Math.Round(AverageScore, 2)
        };

        var parameters = new Dictionary<string, object>
        {
            { "OriginalStudent", _originalStudent },
            { "UpdatedStudent", updatedStudent }
        };

        await Shell.Current.GoToAsync("..", parameters);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}