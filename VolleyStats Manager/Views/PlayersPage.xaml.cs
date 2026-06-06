using VolleyStats_Manager.ViewModels;

namespace VolleyStats_Manager.Views;

public partial class PlayersPage : ContentPage
{
    private readonly PlayersViewModel _vm;
    public PlayersPage(PlayersViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadPlayers();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _vm.LoadPlayers();
    }
}