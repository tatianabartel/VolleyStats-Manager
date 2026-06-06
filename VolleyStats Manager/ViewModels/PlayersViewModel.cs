using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VolleyStats_Manager.Models.DTO;
using VolleyStats_Manager.Models.Services;
using VolleyStats_Manager.Views;

namespace VolleyStats_Manager.ViewModels;

public partial class PlayersViewModel(PlayerService playerService, AppSessionService session) : ObservableObject
{
    private readonly PlayerService _playerService = playerService;
    private readonly AppSessionService _session = session;

    public ObservableCollection<PlayerDTO> Players { get; set; } = [];

    [RelayCommand]
    public async Task LoadPlayers()
    {
        if (!_session.HasTeamSelected) return;

        var players = await _playerService
            .GetPlayersByTeamAsync(_session.CurrentTeamId!.Value);

        Players.Clear();
        foreach (var p in players)
            Players.Add(p);
    }

    [RelayCommand]
    private async Task DeletePlayer(PlayerDTO player)
    {
        if (player == null) return;

        bool confirm = await Shell.Current.DisplayAlert("Confirm", $"Delete {player.FullName}?", "Yes", "No");
        if (!confirm) return;

        try
        {
            await _playerService.DeletePlayerAsync(player.Id);
            Players.Remove(player);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private static async Task GoToAddPlayer() => await Shell.Current.GoToAsync(nameof(AddPlayerPage));

    [RelayCommand]
    private static async Task GoToDetail(PlayerDTO player)
    {
        if (player == null) return;
        await Shell.Current.GoToAsync($"{nameof(PlayerDetailsPage)}?PlayerId={player.Id}");
    }

    [RelayCommand]
    private static async Task EditPlayer(PlayerDTO player)
    {
        if (player == null) return;
        await Shell.Current.GoToAsync($"{nameof(UpdatePlayerPage)}?PlayerId={player.Id}");
    }
}