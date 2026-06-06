using VolleyStats_Manager.ViewModels;

namespace VolleyStats_Manager.Views;

public partial class AddTeamPage : ContentPage
{
	public AddTeamPage(AddTeamViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}