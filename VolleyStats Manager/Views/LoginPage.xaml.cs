using VolleyStats_Manager.ViewModels;

namespace VolleyStats_Manager.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel vm)
	{
        InitializeComponent();
		BindingContext = vm;
    }
}