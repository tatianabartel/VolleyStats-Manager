using VolleyStats_Manager.ViewModels;

namespace VolleyStats_Manager.Views;

public partial class SelectTeamPage : ContentPage
{
    private readonly SelectTeamViewModel _vm;

    public SelectTeamPage(SelectTeamViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.OnAppearing();
    }
}