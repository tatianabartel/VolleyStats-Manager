using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VolleyStats_Manager.Models.Services;

namespace VolleyStats_Manager.ViewModels;

public partial class RegisterViewModel(AuthService authService, AppSessionService session) : ObservableObject
{
    private readonly AuthService _authService = authService;
    private readonly AppSessionService _session = session;

    [ObservableProperty]
    public partial string? UserName { get; set; }
    [ObservableProperty]
    public partial string? Password { get; set; }
    [ObservableProperty]
    public partial string? ConfirmPassword { get; set; }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsErrorVisible))]
    public partial string? ErrorMessage { get; set; }
    public bool IsErrorVisible => !string.IsNullOrEmpty(ErrorMessage);

    [RelayCommand]
    private async Task Register()
    {
        if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
        {
            ErrorMessage = "Please fill in all fields"; 
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match"; 
            return;
        }

        try
        {
            var user = await _authService.RegisterAsync(UserName, Password);

            ErrorMessage = null;
            _session.CurrentUser = user;

            await Shell.Current.GoToAsync("AddTeamPage");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private static async Task GoToLoginPage()
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}