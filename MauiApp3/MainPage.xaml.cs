using System.Diagnostics;
using MauiApp3.ViewModels;

namespace MauiApp3;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Debug.WriteLine("[PageLifecycle] MainPage OnAppearing");

        if (BindingContext is MainViewModel vm)
        {
            vm.RestoreDraftFromState();
            vm.LoadCommand.Execute(null);
        }
    }

    protected override void OnDisappearing()
    {
        Debug.WriteLine("[PageLifecycle] MainPage OnDisappearing");
        base.OnDisappearing();
    }

    private async void OnOpenDeepLinkClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("studentdetail?id=1");
    }
}