using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VolleyStats_Manager.Models.Services;
using VolleyStatsManager.DAL.Enums;

namespace VolleyStats_Manager.ViewModels;

public partial class AddPlayerViewModel(PlayerService playerService, AppSessionService session) : ObservableObject
{
    private readonly PlayerService _playerService = playerService;
    private readonly AppSessionService _session = session;

    [ObservableProperty]
    public partial string FirstName { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string LastName { get; set; } = string.Empty;
    [ObservableProperty]
    public partial int Number { get; set; }
    [ObservableProperty]
    public partial PlayerPosition SelectedPosition { get; set; }
    public List<PlayerPosition> Positions { get; } = [.. Enum.GetValues<PlayerPosition>()];

    [RelayCommand]
    private async Task SavePlayer()
    {
        FirstName = FirstName?.Trim() ?? string.Empty;
        LastName = LastName?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
        {
            await Shell.Current.DisplayAlert("Error", "Please enter first and last name.", "OK");
            return;
        }

        if (FirstName.Any(char.IsDigit) || LastName.Any(char.IsDigit))
        {
            await Shell.Current.DisplayAlert("Error", "Name cannot contain digits.", "OK");
            return;
        }

        if (Number < 1 || Number > 99)
        {
            await Shell.Current.DisplayAlert("Error", "Jersey number must be between 1 and 99.", "OK");
            return;
        }

        if (!_session.HasTeamSelected)
        {
            await Shell.Current.DisplayAlert("Error", "No active team selected.", "OK");
            return;
        }

        try
        {
            await _playerService.CreatePlayerAsync(
                FirstName,
                LastName,
                Number,
                SelectedPosition,
                _session.CurrentTeamId!.Value);

            await Shell.Current.DisplayAlert("Success", "Player added successfully", "OK");

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private static async Task Cancel() => await Shell.Current.GoToAsync("..");
}