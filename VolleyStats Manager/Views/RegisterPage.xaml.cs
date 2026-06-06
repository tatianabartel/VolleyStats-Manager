using VolleyStats_Manager.ViewModels;

namespace VolleyStats_Manager.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}