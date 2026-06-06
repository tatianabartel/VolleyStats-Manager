using VolleyStats_Manager.ViewModels;

namespace VolleyStats_Manager.Views;

public partial class MatchesPage : ContentPage
{
    private readonly MatchesViewModel _vm;
    public MatchesPage(MatchesViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadMatches();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _vm.LoadMatches();
    }
}