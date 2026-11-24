using MauiApp3.ViewModels;

namespace MauiApp3.Views;

public partial class StudentDetailPage : ContentPage
{
    public StudentDetailPage() : this(new StudentDetailViewModel())
    {
    }

    public StudentDetailPage(StudentDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}