using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VolleyStats_Manager.Models.DTO;
using VolleyStats_Manager.Models.Services;
using VolleyStatsManager.DAL.Enums;

namespace VolleyStats_Manager.ViewModels;

public partial class UpdatePlayerViewModel(PlayerService playerService) : ObservableObject
{
    private readonly PlayerService _playerService = playerService;

    [ObservableProperty]
    public partial PlayerDTO? Player { get; set; }
    public IEnumerable<PlayerPosition> Positions { get; } = Enum.GetValues<PlayerPosition>();

    public async Task LoadPlayerById(int id)
    {
        var p = await _playerService.GetPlayerByIdAsync(id);
        if (p != null) Player = p;
    }

    [RelayCommand]
    private async Task Save()
    {
        if (Player == null) return;

        Player.FirstName = Player.FirstName?.Trim() ?? string.Empty;
        Player.LastName = Player.LastName?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(Player.FirstName) || string.IsNullOrWhiteSpace(Player.LastName))
        {
            await Shell.Current.DisplayAlert("Error", "First and last name are required.", "OK");
            return;
        }

        if (Player.FirstName.Any(char.IsDigit) || Player.LastName.Any(char.IsDigit))
        {
            await Shell.Current.DisplayAlert("Error", "Name cannot contain digits.", "OK");
            return;
        }

        if (Player.Number < 1 || Player.Number > 99)
        {
            await Shell.Current.DisplayAlert("Error", "Jersey number must be between 1 and 99.", "OK");
            return;
        }

        try
        {
            await _playerService.UpdatePlayerAsync(Player);
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