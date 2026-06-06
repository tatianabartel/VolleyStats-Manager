using VolleyStats_Manager.ViewModels;

namespace VolleyStats_Manager.Views;

public partial class PlayerDetailsPage : ContentPage
{
    private readonly PlayerDetailsViewModel _vm;
    public PlayerDetailsPage(PlayerDetailsViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
            
        await _vm.LoadData(_vm.PlayerId);
    }
}