using VolleyStats_Manager.ViewModels;

namespace VolleyStats_Manager.Views;

public partial class TeamStatisticsPage : ContentPage
{
	public TeamStatisticsPage(TeamStatisticsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((TeamStatisticsViewModel)BindingContext).LoadStatisticsAsync();
    }
}