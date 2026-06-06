using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VolleyStats_Manager.Models.Services;

namespace VolleyStats_Manager.ViewModels;

public partial class LoginViewModel(AuthService authService, AppSessionService session) : ObservableObject
{
    private readonly AuthService _authService = authService;
    private readonly AppSessionService _session = session;

    [ObservableProperty]
    public partial string? UserName { get; set; }
    [ObservableProperty]
    public partial string? Password { get; set; }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsErrorVisible))]
    public partial string? ErrorMessage { get; set; }
    public bool IsErrorVisible => !string.IsNullOrEmpty(ErrorMessage);

    [RelayCommand]
    private async Task Login()
    {
        if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
        {
            ErrorMessage = "Please enter both username and password.";
            return;
        }

        try
        {
            var user = await _authService.LoginAsync(UserName, Password);
            if (user == null)
            {
                ErrorMessage = "Invalid username or password.";
                Password = string.Empty;
                return;
            }

            ErrorMessage = null;
            _session.CurrentUser = user;
            await Shell.Current.GoToAsync("//SelectTeamPage");
        }
        catch (ArgumentException ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task GoToRegisterPage()
    {
        await Shell.Current.GoToAsync("//RegisterPage"); 
    }
}